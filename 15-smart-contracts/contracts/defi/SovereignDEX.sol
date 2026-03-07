// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import "@openzeppelin/contracts/token/ERC20/IERC20.sol";
import "@openzeppelin/contracts/token/ERC20/utils/SafeERC20.sol";
import "@openzeppelin/contracts/utils/ReentrancyGuard.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/Pausable.sol";

/**
 * @title SovereignDEX
 * @author Ierahkwa Ne Kanienke — Sovereign Digital Nation
 * @notice Automated Market Maker (AMM) for the sovereign token ecosystem,
 *         inspired by the Uniswap V2 constant-product model (x * y = k).
 * @dev Core mechanics:
 *
 *      1. **Pair creation** — any whitelisted pair of ERC-20 tokens can be
 *         registered.  The first `addLiquidity` call bootstraps reserves.
 *
 *      2. **Liquidity provision** — LPs deposit both tokens in proportion to
 *         current reserves and receive an internal liquidity balance tracked
 *         per pair per address (no separate LP token contract for gas savings).
 *
 *      3. **Swaps** — the constant-product invariant `reserveIn * reserveOut`
 *         is maintained after every swap, minus the fee.
 *
 *      4. **Fees** — 30 basis points (0.30 %) total:
 *           - 25 bp accrue to liquidity providers via reserve growth.
 *           - 5 bp are transferred to `feeRecipient` (sovereign treasury).
 *
 *      5. **Price oracle** — `getPrice(pairId, tokenIn)` returns the
 *         instantaneous spot price based on current reserves (no TWAP).
 *
 *      Security: ReentrancyGuard on every mutating function, Pausable for
 *      emergency halts, Ownable for admin operations.
 */
contract SovereignDEX is ReentrancyGuard, Ownable, Pausable {
    using SafeERC20 for IERC20;

    // -----------------------------------------------------------------------
    //  Types
    // -----------------------------------------------------------------------

    /// @dev On-chain representation of a trading pair.
    struct Pair {
        address tokenA;
        address tokenB;
        uint256 reserveA;
        uint256 reserveB;
        uint256 totalLiquidity;
        bool    active;
    }

    // -----------------------------------------------------------------------
    //  State
    // -----------------------------------------------------------------------

    /// @notice Pair storage indexed by pairId (sequential, starting at 1).
    mapping(uint256 => Pair) public pairs;

    /// @notice LP balances:  pairId => provider => liquidity units.
    mapping(uint256 => mapping(address => uint256)) public liquidity;

    /// @notice Reverse lookup:  keccak256(tokenA, tokenB) => pairId.
    ///         Always stored with the lower address first.
    mapping(bytes32 => uint256) public pairIds;

    /// @notice Running counter for pair IDs (starts at 1; 0 means "not found").
    uint256 public pairCount;

    /// @notice Address that receives the protocol fee (5 bp per swap).
    address public feeRecipient;

    // -----------------------------------------------------------------------
    //  Constants
    // -----------------------------------------------------------------------

    /// @dev Total fee in basis points (30 bp = 0.30 %).
    uint256 private constant TOTAL_FEE_BP = 30;

    /// @dev Protocol share in basis points (5 bp of the total 30 bp).
    uint256 private constant PROTOCOL_FEE_BP = 5;

    /// @dev Basis point denominator.
    uint256 private constant BP = 10_000;

    /// @dev Minimum liquidity burned on first deposit to prevent manipulation.
    uint256 private constant MINIMUM_LIQUIDITY = 1000;

    // -----------------------------------------------------------------------
    //  Events
    // -----------------------------------------------------------------------

    event PairCreated(
        uint256 indexed pairId,
        address indexed tokenA,
        address indexed tokenB
    );

    event LiquidityAdded(
        uint256 indexed pairId,
        address indexed provider,
        uint256 amountA,
        uint256 amountB,
        uint256 liquidityMinted
    );

    event LiquidityRemoved(
        uint256 indexed pairId,
        address indexed provider,
        uint256 amountA,
        uint256 amountB,
        uint256 liquidityBurned
    );

    event Swap(
        uint256 indexed pairId,
        address indexed sender,
        address tokenIn,
        uint256 amountIn,
        address tokenOut,
        uint256 amountOut
    );

    // -----------------------------------------------------------------------
    //  Errors
    // -----------------------------------------------------------------------

    error IdenticalTokens();
    error ZeroAddress();
    error PairAlreadyExists();
    error PairNotActive(uint256 pairId);
    error InvalidPair(uint256 pairId);
    error ZeroAmount();
    error InsufficientLiquidity();
    error InsufficientOutputAmount(uint256 received, uint256 minimum);
    error InsufficientLiquidityMinted(uint256 minted, uint256 minimum);
    error InvalidTokenForPair();
    error InsufficientReserves();

    // -----------------------------------------------------------------------
    //  Constructor
    // -----------------------------------------------------------------------

    /**
     * @param _owner        Consejo Soberano multisig.
     * @param _feeRecipient Treasury address for protocol fees.
     */
    constructor(
        address _owner,
        address _feeRecipient
    ) Ownable(_owner) {
        if (_feeRecipient == address(0)) revert ZeroAddress();
        feeRecipient = _feeRecipient;
    }

    // -----------------------------------------------------------------------
    //  Pair management
    // -----------------------------------------------------------------------

    /**
     * @notice Register a new trading pair.
     * @dev Tokens are sorted internally so (A,B) and (B,A) map to the same
     *      pair. Reverts if the pair already exists.
     * @param tokenA First ERC-20 address.
     * @param tokenB Second ERC-20 address.
     * @return pairId Unique identifier for the new pair.
     */
    function createPair(
        address tokenA,
        address tokenB
    ) external onlyOwner returns (uint256 pairId) {
        if (tokenA == tokenB) revert IdenticalTokens();
        if (tokenA == address(0) || tokenB == address(0)) revert ZeroAddress();

        // Canonical order: lower address first.
        (address t0, address t1) = tokenA < tokenB
            ? (tokenA, tokenB)
            : (tokenB, tokenA);

        bytes32 key = keccak256(abi.encodePacked(t0, t1));
        if (pairIds[key] != 0) revert PairAlreadyExists();

        pairId = ++pairCount;
        pairs[pairId] = Pair({
            tokenA:         t0,
            tokenB:         t1,
            reserveA:       0,
            reserveB:       0,
            totalLiquidity: 0,
            active:         true
        });
        pairIds[key] = pairId;

        emit PairCreated(pairId, t0, t1);
    }

    // -----------------------------------------------------------------------
    //  Liquidity
    // -----------------------------------------------------------------------

    /**
     * @notice Add liquidity to an existing pair.
     * @dev On the very first deposit, `sqrt(amountA * amountB) - MINIMUM_LIQUIDITY`
     *      units are minted (MINIMUM_LIQUIDITY is permanently locked to prevent
     *      the total-liquidity-equals-zero edge case).  Subsequent deposits
     *      mint proportionally to the smaller ratio of amounts-to-reserves.
     * @param pairId            ID of the pair.
     * @param amountA           Desired amount of tokenA to deposit.
     * @param amountB           Desired amount of tokenB to deposit.
     * @param minLiquidityTokens Minimum liquidity units the caller accepts.
     * @return liquidityMinted  Number of liquidity units credited to the caller.
     */
    function addLiquidity(
        uint256 pairId,
        uint256 amountA,
        uint256 amountB,
        uint256 minLiquidityTokens
    ) external nonReentrant whenNotPaused returns (uint256 liquidityMinted) {
        Pair storage pair = pairs[pairId];
        if (!pair.active) revert PairNotActive(pairId);
        if (amountA == 0 || amountB == 0) revert ZeroAmount();

        if (pair.totalLiquidity == 0) {
            // First deposit — bootstrap the pair.
            liquidityMinted = _sqrt(amountA * amountB);
            if (liquidityMinted <= MINIMUM_LIQUIDITY) revert InsufficientLiquidity();
            liquidityMinted -= MINIMUM_LIQUIDITY;
            // MINIMUM_LIQUIDITY is burned (assigned to address(0) conceptually).
            pair.totalLiquidity = MINIMUM_LIQUIDITY;
        } else {
            // Proportional deposit.
            uint256 liqA = (amountA * pair.totalLiquidity) / pair.reserveA;
            uint256 liqB = (amountB * pair.totalLiquidity) / pair.reserveB;
            liquidityMinted = liqA < liqB ? liqA : liqB;
        }

        if (liquidityMinted < minLiquidityTokens) {
            revert InsufficientLiquidityMinted(liquidityMinted, minLiquidityTokens);
        }

        pair.reserveA += amountA;
        pair.reserveB += amountB;
        pair.totalLiquidity += liquidityMinted;
        liquidity[pairId][msg.sender] += liquidityMinted;

        IERC20(pair.tokenA).safeTransferFrom(msg.sender, address(this), amountA);
        IERC20(pair.tokenB).safeTransferFrom(msg.sender, address(this), amountB);

        emit LiquidityAdded(pairId, msg.sender, amountA, amountB, liquidityMinted);
    }

    /**
     * @notice Remove liquidity from a pair.
     * @param pairId          ID of the pair.
     * @param liquidityAmount Units of liquidity to burn.
     * @param minAmountA      Minimum tokenA the caller accepts.
     * @param minAmountB      Minimum tokenB the caller accepts.
     */
    function removeLiquidity(
        uint256 pairId,
        uint256 liquidityAmount,
        uint256 minAmountA,
        uint256 minAmountB
    ) external nonReentrant {
        Pair storage pair = pairs[pairId];
        if (pair.tokenA == address(0)) revert InvalidPair(pairId);
        if (liquidityAmount == 0) revert ZeroAmount();
        if (liquidity[pairId][msg.sender] < liquidityAmount) {
            revert InsufficientLiquidity();
        }

        uint256 amountA = (liquidityAmount * pair.reserveA) / pair.totalLiquidity;
        uint256 amountB = (liquidityAmount * pair.reserveB) / pair.totalLiquidity;

        if (amountA < minAmountA) {
            revert InsufficientOutputAmount(amountA, minAmountA);
        }
        if (amountB < minAmountB) {
            revert InsufficientOutputAmount(amountB, minAmountB);
        }

        pair.reserveA -= amountA;
        pair.reserveB -= amountB;
        pair.totalLiquidity -= liquidityAmount;
        liquidity[pairId][msg.sender] -= liquidityAmount;

        IERC20(pair.tokenA).safeTransfer(msg.sender, amountA);
        IERC20(pair.tokenB).safeTransfer(msg.sender, amountB);

        emit LiquidityRemoved(pairId, msg.sender, amountA, amountB, liquidityAmount);
    }

    // -----------------------------------------------------------------------
    //  Swap
    // -----------------------------------------------------------------------

    /**
     * @notice Swap `amountIn` of `tokenIn` for the other token in the pair.
     * @dev Fee: 30 bp total.  25 bp remain in reserves (accrue to LPs).
     *      5 bp are transferred to `feeRecipient`.
     *      The constant-product invariant is enforced post-fee.
     * @param pairId     ID of the pair.
     * @param tokenIn    Address of the token being sold.
     * @param amountIn   Amount of tokenIn to sell.
     * @param minAmountOut Minimum output the caller will accept (slippage guard).
     * @return amountOut Tokens received by the caller.
     */
    function swap(
        uint256 pairId,
        address tokenIn,
        uint256 amountIn,
        uint256 minAmountOut
    ) external nonReentrant whenNotPaused returns (uint256 amountOut) {
        Pair storage pair = pairs[pairId];
        if (!pair.active) revert PairNotActive(pairId);
        if (amountIn == 0) revert ZeroAmount();

        bool isTokenA = (tokenIn == pair.tokenA);
        if (!isTokenA && tokenIn != pair.tokenB) revert InvalidTokenForPair();

        (uint256 reserveIn, uint256 reserveOut) = isTokenA
            ? (pair.reserveA, pair.reserveB)
            : (pair.reserveB, pair.reserveA);

        // Protocol fee (5 bp) taken from amountIn before the swap math.
        uint256 protocolFee = (amountIn * PROTOCOL_FEE_BP) / BP;
        uint256 amountInAfterProtocolFee = amountIn - protocolFee;

        // LP fee (25 bp of original amountIn) stays inside reserves.
        uint256 lpFee = (amountIn * (TOTAL_FEE_BP - PROTOCOL_FEE_BP)) / BP;
        uint256 amountInAfterAllFees = amountInAfterProtocolFee - lpFee;

        // Constant product: amountOut = reserveOut - (reserveIn * reserveOut) / (reserveIn + amountInAfterAllFees)
        // Simplified: amountOut = (amountInAfterAllFees * reserveOut) / (reserveIn + amountInAfterAllFees)
        amountOut = (amountInAfterAllFees * reserveOut) / (reserveIn + amountInAfterAllFees);

        if (amountOut == 0) revert InsufficientReserves();
        if (amountOut < minAmountOut) {
            revert InsufficientOutputAmount(amountOut, minAmountOut);
        }

        // Update reserves:  amountIn minus protocolFee enters reserves (LP fee included).
        if (isTokenA) {
            pair.reserveA += amountInAfterProtocolFee;
            pair.reserveB -= amountOut;
        } else {
            pair.reserveB += amountInAfterProtocolFee;
            pair.reserveA -= amountOut;
        }

        address tokenOut = isTokenA ? pair.tokenB : pair.tokenA;

        // Transfers
        IERC20(tokenIn).safeTransferFrom(msg.sender, address(this), amountIn);
        IERC20(tokenIn).safeTransfer(feeRecipient, protocolFee);
        IERC20(tokenOut).safeTransfer(msg.sender, amountOut);

        emit Swap(pairId, msg.sender, tokenIn, amountIn, tokenOut, amountOut);
    }

    // -----------------------------------------------------------------------
    //  View helpers
    // -----------------------------------------------------------------------

    /**
     * @notice Look up the pairId for two tokens (order-agnostic).
     * @return pairId 0 if the pair does not exist.
     */
    function getPair(
        address tokenA,
        address tokenB
    ) external view returns (uint256) {
        (address t0, address t1) = tokenA < tokenB
            ? (tokenA, tokenB)
            : (tokenB, tokenA);
        return pairIds[keccak256(abi.encodePacked(t0, t1))];
    }

    /**
     * @notice Retrieve current reserves for a pair.
     * @param pairId Pair identifier.
     * @return reserveA Current reserve of tokenA.
     * @return reserveB Current reserve of tokenB.
     */
    function getReserves(uint256 pairId)
        external
        view
        returns (uint256 reserveA, uint256 reserveB)
    {
        Pair storage pair = pairs[pairId];
        return (pair.reserveA, pair.reserveB);
    }

    /**
     * @notice Instantaneous spot price of `tokenIn` denominated in the other
     *         token of the pair, scaled to 1e18 precision.
     * @dev price = (reserveOut * 1e18) / reserveIn.
     *      This is a raw reserve ratio — NOT a TWAP oracle.
     * @param pairId  Pair identifier.
     * @param tokenIn Token whose price is being queried.
     * @return price  Price scaled to 18 decimals.
     */
    function getPrice(
        uint256 pairId,
        address tokenIn
    ) external view returns (uint256 price) {
        Pair storage pair = pairs[pairId];
        if (pair.tokenA == address(0)) revert InvalidPair(pairId);

        bool isTokenA = (tokenIn == pair.tokenA);
        if (!isTokenA && tokenIn != pair.tokenB) revert InvalidTokenForPair();

        (uint256 reserveIn, uint256 reserveOut) = isTokenA
            ? (pair.reserveA, pair.reserveB)
            : (pair.reserveB, pair.reserveA);

        if (reserveIn == 0) revert InsufficientReserves();
        price = (reserveOut * 1e18) / reserveIn;
    }

    /**
     * @notice Compute the expected output for a given input, accounting for fees.
     * @param amountIn   Input amount.
     * @param reserveIn  Reserve of the input token.
     * @param reserveOut Reserve of the output token.
     * @return amountOut Expected output.
     */
    function getAmountOut(
        uint256 amountIn,
        uint256 reserveIn,
        uint256 reserveOut
    ) external pure returns (uint256 amountOut) {
        return _getAmountOut(amountIn, reserveIn, reserveOut);
    }

    // -----------------------------------------------------------------------
    //  Admin
    // -----------------------------------------------------------------------

    /**
     * @notice Update the protocol fee recipient address.
     * @param _feeRecipient New treasury address.
     */
    function setFeeRecipient(address _feeRecipient) external onlyOwner {
        if (_feeRecipient == address(0)) revert ZeroAddress();
        feeRecipient = _feeRecipient;
    }

    /**
     * @notice Deactivate a pair (prevents new swaps and liquidity additions).
     *         Existing LPs can still withdraw.
     * @param pairId Pair identifier.
     */
    function deactivatePair(uint256 pairId) external onlyOwner {
        pairs[pairId].active = false;
    }

    /**
     * @notice Reactivate a previously deactivated pair.
     * @param pairId Pair identifier.
     */
    function activatePair(uint256 pairId) external onlyOwner {
        pairs[pairId].active = true;
    }

    /// @notice Pause all swaps and liquidity additions.
    function pause() external onlyOwner { _pause(); }

    /// @notice Unpause the DEX.
    function unpause() external onlyOwner { _unpause(); }

    // -----------------------------------------------------------------------
    //  Internal helpers
    // -----------------------------------------------------------------------

    /**
     * @dev Compute output amount using the constant-product formula with
     *      total fee deducted.
     *      amountOut = (amountInWithFee * reserveOut) / (reserveIn * BP + amountInWithFee)
     *      where amountInWithFee = amountIn * (BP - TOTAL_FEE_BP)
     */
    function _getAmountOut(
        uint256 amountIn,
        uint256 reserveIn,
        uint256 reserveOut
    ) internal pure returns (uint256) {
        if (amountIn == 0 || reserveIn == 0 || reserveOut == 0) return 0;
        uint256 amountInWithFee = amountIn * (BP - TOTAL_FEE_BP);
        uint256 numerator = amountInWithFee * reserveOut;
        uint256 denominator = (reserveIn * BP) + amountInWithFee;
        return numerator / denominator;
    }

    /**
     * @dev Integer square root (Babylonian method).
     */
    function _sqrt(uint256 y) internal pure returns (uint256 z) {
        if (y > 3) {
            z = y;
            uint256 x = y / 2 + 1;
            while (x < z) {
                z = x;
                x = (y / x + x) / 2;
            }
        } else if (y != 0) {
            z = 1;
        }
    }
}
