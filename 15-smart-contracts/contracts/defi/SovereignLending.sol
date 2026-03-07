// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import "@openzeppelin/contracts/token/ERC20/IERC20.sol";
import "@openzeppelin/contracts/token/ERC20/utils/SafeERC20.sol";
import "@openzeppelin/contracts/utils/ReentrancyGuard.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/Pausable.sol";

/**
 * @title ISovereignDEX
 * @dev Minimal interface used by SovereignLending to read DEX reserves for
 *      on-chain price discovery.
 */
interface ISovereignDEX {
    function getPair(address tokenA, address tokenB) external view returns (uint256);
    function getReserves(uint256 pairId) external view returns (uint256, uint256);
    function pairs(uint256 pairId) external view returns (
        address tokenA,
        address tokenB,
        uint256 reserveA,
        uint256 reserveB,
        uint256 totalLiquidity,
        bool active
    );
}

/**
 * @title SovereignLending
 * @author Ierahkwa Ne Kanienke — Sovereign Digital Nation
 * @notice Over-collateralized lending protocol for the sovereign token
 *         ecosystem.  Borrows are backed by at least 133 % collateral
 *         (max LTV 75 %).  Positions whose collateral ratio drops below
 *         110 % can be liquidated by any address.
 *
 * @dev Design overview:
 *
 *      **Deposits** — Any supported token can be deposited as collateral.
 *        The deposit balance is tracked per-user per-token.
 *
 *      **Borrows** — A borrower locks collateral and receives a different
 *        supported token.  The loan records the base interest rate at
 *        origination, collateral amounts, and the start timestamp.
 *
 *      **Interest** — A base rate of 500 bp (5 % APR) that increases
 *        linearly with pool utilization (up to 2000 bp at 100 % utilization).
 *
 *      **Liquidation** — Anyone may repay a loan whose health factor
 *        (collateral value / borrow value) falls below 110 %.  The
 *        liquidator receives the collateral.
 *
 *      **Oracle** — On-chain price comes from SovereignDEX reserves.
 *        `getPrice(tokenA, tokenB)` returns (reserveB * 1e18) / reserveA.
 *        If the DEX pair does not exist, the price call reverts — this is
 *        intentional so that unsupported pairs cannot be borrowed against.
 *
 *      Supported tokens: WMP, BDET, IGT-GOV and any other token with a
 *      DEX pair for price discovery.
 */
contract SovereignLending is ReentrancyGuard, Ownable, Pausable {
    using SafeERC20 for IERC20;

    // -----------------------------------------------------------------------
    //  Types
    // -----------------------------------------------------------------------

    /// @dev On-chain representation of an active loan.
    struct Loan {
        address borrower;
        address collateralToken;
        address borrowToken;
        uint256 collateralAmount;
        uint256 borrowAmount;
        uint256 interestRate;       // basis points, snapshot at origination
        uint256 startTime;
        bool    active;
    }

    // -----------------------------------------------------------------------
    //  State
    // -----------------------------------------------------------------------

    /// @notice Loan storage, indexed by loanId (sequential, starting at 1).
    mapping(uint256 => Loan) public loans;

    /// @notice Running loan counter (0 means "not found").
    uint256 public loanCount;

    /// @notice User deposit balances:  user => token => amount.
    mapping(address => mapping(address => uint256)) public deposits;

    /// @notice Total deposited per token (used for utilization calculation).
    mapping(address => uint256) public totalDeposited;

    /// @notice Total borrowed per token.
    mapping(address => uint256) public totalBorrowed;

    /// @notice Set of tokens the protocol accepts as collateral or borrow.
    mapping(address => bool) public supportedTokens;

    /// @notice Reference to the SovereignDEX used for price discovery.
    ISovereignDEX public dexContract;

    // -----------------------------------------------------------------------
    //  Constants
    // -----------------------------------------------------------------------

    /// @dev Maximum loan-to-value ratio: 75 % (7500 / 10000).
    uint256 public constant MAX_LTV_BP = 7500;

    /// @dev Liquidation threshold: 110 % collateral ratio (11000 / 10000).
    uint256 public constant LIQUIDATION_THRESHOLD_BP = 11000;

    /// @dev Base interest rate: 5 % APR (500 bp).
    uint256 public constant BASE_RATE_BP = 500;

    /// @dev Maximum interest rate at full utilization: 20 % APR (2000 bp).
    uint256 public constant MAX_RATE_BP = 2000;

    /// @dev Basis-point denominator.
    uint256 private constant BP = 10_000;

    /// @dev Seconds in a 365-day year, used for interest accrual.
    uint256 private constant SECONDS_PER_YEAR = 365 days;

    /// @dev 1e18 precision scalar.
    uint256 private constant PRECISION = 1e18;

    // -----------------------------------------------------------------------
    //  Events
    // -----------------------------------------------------------------------

    event Deposited(
        address indexed user,
        address indexed token,
        uint256 amount
    );

    event Withdrawn(
        address indexed user,
        address indexed token,
        uint256 amount
    );

    event Borrowed(
        uint256 indexed loanId,
        address indexed borrower,
        address collateralToken,
        address borrowToken,
        uint256 collateralAmount,
        uint256 borrowAmount,
        uint256 interestRate
    );

    event Repaid(
        uint256 indexed loanId,
        address indexed borrower,
        uint256 amountRepaid,
        uint256 interestPaid
    );

    event Liquidated(
        uint256 indexed loanId,
        address indexed liquidator,
        uint256 collateralSeized,
        uint256 debtRepaid
    );

    event TokenSupported(address indexed token, bool supported);

    // -----------------------------------------------------------------------
    //  Errors
    // -----------------------------------------------------------------------

    error UnsupportedToken(address token);
    error ZeroAmount();
    error InsufficientDeposit(uint256 available, uint256 requested);
    error ExceedsMaxLTV(uint256 ltvBp);
    error LoanNotActive(uint256 loanId);
    error NotBorrower(uint256 loanId);
    error LoanNotLiquidatable(uint256 healthBp);
    error InsufficientPoolLiquidity(uint256 available, uint256 requested);
    error ZeroAddress();
    error SameToken();
    error NoDEXPair(address tokenA, address tokenB);

    // -----------------------------------------------------------------------
    //  Constructor
    // -----------------------------------------------------------------------

    /**
     * @param _owner       Consejo Soberano multisig.
     * @param _dexContract Address of the deployed SovereignDEX for price feeds.
     */
    constructor(
        address _owner,
        address _dexContract
    ) Ownable(_owner) {
        if (_dexContract == address(0)) revert ZeroAddress();
        dexContract = ISovereignDEX(_dexContract);
    }

    // -----------------------------------------------------------------------
    //  Admin
    // -----------------------------------------------------------------------

    /**
     * @notice Add or remove a token from the supported set.
     * @param token     ERC-20 address.
     * @param supported Whether the token is accepted.
     */
    function setSupportedToken(
        address token,
        bool supported
    ) external onlyOwner {
        if (token == address(0)) revert ZeroAddress();
        supportedTokens[token] = supported;
        emit TokenSupported(token, supported);
    }

    /**
     * @notice Update the DEX contract reference.
     * @param _dexContract New SovereignDEX address.
     */
    function setDexContract(address _dexContract) external onlyOwner {
        if (_dexContract == address(0)) revert ZeroAddress();
        dexContract = ISovereignDEX(_dexContract);
    }

    /// @notice Pause the protocol (blocks deposits, borrows, repays).
    function pause() external onlyOwner { _pause(); }

    /// @notice Unpause the protocol.
    function unpause() external onlyOwner { _unpause(); }

    // -----------------------------------------------------------------------
    //  Deposit / Withdraw
    // -----------------------------------------------------------------------

    /**
     * @notice Deposit collateral tokens into the lending pool.
     * @param token  ERC-20 address of the token to deposit.
     * @param amount Number of tokens (in wei) to deposit.
     */
    function deposit(
        address token,
        uint256 amount
    ) external nonReentrant whenNotPaused {
        if (!supportedTokens[token]) revert UnsupportedToken(token);
        if (amount == 0) revert ZeroAmount();

        deposits[msg.sender][token] += amount;
        totalDeposited[token] += amount;

        IERC20(token).safeTransferFrom(msg.sender, address(this), amount);

        emit Deposited(msg.sender, token, amount);
    }

    /**
     * @notice Withdraw previously deposited collateral.
     * @dev Does NOT check whether the user has outstanding loans using this
     *      collateral — that is managed through the Loan struct.
     * @param token  ERC-20 address.
     * @param amount Number of tokens to withdraw.
     */
    function withdraw(
        address token,
        uint256 amount
    ) external nonReentrant {
        if (amount == 0) revert ZeroAmount();
        uint256 available = deposits[msg.sender][token];
        if (available < amount) revert InsufficientDeposit(available, amount);

        deposits[msg.sender][token] -= amount;
        totalDeposited[token] -= amount;

        IERC20(token).safeTransfer(msg.sender, amount);

        emit Withdrawn(msg.sender, token, amount);
    }

    // -----------------------------------------------------------------------
    //  Borrow
    // -----------------------------------------------------------------------

    /**
     * @notice Open a collateralized loan.
     * @dev The borrower must have already deposited sufficient collateral via
     *      `deposit`.  The loan's LTV is checked against `MAX_LTV_BP` (75 %).
     *      The interest rate is snapshotted at origination based on current
     *      utilization of the borrow token pool.
     * @param collateralToken Token used as collateral.
     * @param borrowToken     Token being borrowed.
     * @param collateralAmount Amount of collateral to lock.
     * @param borrowAmount    Amount of borrowToken requested.
     */
    function borrow(
        address collateralToken,
        address borrowToken,
        uint256 collateralAmount,
        uint256 borrowAmount
    ) external nonReentrant whenNotPaused {
        if (collateralToken == borrowToken) revert SameToken();
        if (!supportedTokens[collateralToken]) revert UnsupportedToken(collateralToken);
        if (!supportedTokens[borrowToken]) revert UnsupportedToken(borrowToken);
        if (collateralAmount == 0 || borrowAmount == 0) revert ZeroAmount();

        // Verify the borrower has enough deposited collateral.
        uint256 available = deposits[msg.sender][collateralToken];
        if (available < collateralAmount) {
            revert InsufficientDeposit(available, collateralAmount);
        }

        // Verify pool has enough liquidity to lend.
        uint256 poolLiquidity = totalDeposited[borrowToken] - totalBorrowed[borrowToken];
        if (poolLiquidity < borrowAmount) {
            revert InsufficientPoolLiquidity(poolLiquidity, borrowAmount);
        }

        // LTV check:  (borrowValue / collateralValue) <= MAX_LTV_BP / BP.
        uint256 collateralValue = _getValueInBase(collateralToken, borrowToken, collateralAmount);
        uint256 ltvBp = (borrowAmount * BP) / collateralValue;
        if (ltvBp > MAX_LTV_BP) revert ExceedsMaxLTV(ltvBp);

        // Compute interest rate based on utilization.
        uint256 rate = _calculateRate(borrowToken, borrowAmount);

        // Lock collateral.
        deposits[msg.sender][collateralToken] -= collateralAmount;

        // Track borrowed amount globally.
        totalBorrowed[borrowToken] += borrowAmount;

        // Create loan record.
        uint256 loanId = ++loanCount;
        loans[loanId] = Loan({
            borrower:         msg.sender,
            collateralToken:  collateralToken,
            borrowToken:      borrowToken,
            collateralAmount: collateralAmount,
            borrowAmount:     borrowAmount,
            interestRate:     rate,
            startTime:        block.timestamp,
            active:           true
        });

        // Transfer borrowed tokens to the borrower.
        IERC20(borrowToken).safeTransfer(msg.sender, borrowAmount);

        emit Borrowed(
            loanId,
            msg.sender,
            collateralToken,
            borrowToken,
            collateralAmount,
            borrowAmount,
            rate
        );
    }

    // -----------------------------------------------------------------------
    //  Repay
    // -----------------------------------------------------------------------

    /**
     * @notice Repay an active loan (fully or partially).
     * @dev `amount` covers principal first; any excess covers accrued interest.
     *      When the full debt (principal + interest) is repaid the loan is
     *      closed and collateral is returned to the borrower.
     * @param loanId ID of the loan.
     * @param amount Amount of borrowToken to repay.
     */
    function repay(
        uint256 loanId,
        uint256 amount
    ) external nonReentrant whenNotPaused {
        Loan storage loan = loans[loanId];
        if (!loan.active) revert LoanNotActive(loanId);
        if (amount == 0) revert ZeroAmount();

        uint256 interest = calculateInterest(loanId);
        uint256 totalDebt = loan.borrowAmount + interest;

        // Cap repayment at total debt.
        uint256 repayAmount = amount > totalDebt ? totalDebt : amount;

        // Pull borrow tokens from the caller.
        IERC20(loan.borrowToken).safeTransferFrom(msg.sender, address(this), repayAmount);

        uint256 principalRepaid;
        uint256 interestPaid;

        if (repayAmount >= totalDebt) {
            // Full repayment — close the loan.
            principalRepaid = loan.borrowAmount;
            interestPaid = interest;

            loan.active = false;
            totalBorrowed[loan.borrowToken] -= loan.borrowAmount;

            // Return collateral to borrower.
            deposits[loan.borrower][loan.collateralToken] += loan.collateralAmount;
        } else {
            // Partial repayment — reduce principal.
            if (repayAmount <= interest) {
                interestPaid = repayAmount;
            } else {
                interestPaid = interest;
                principalRepaid = repayAmount - interest;
                loan.borrowAmount -= principalRepaid;
                totalBorrowed[loan.borrowToken] -= principalRepaid;
            }
            // Reset the interest clock.
            loan.startTime = block.timestamp;
        }

        emit Repaid(loanId, loan.borrower, principalRepaid, interestPaid);
    }

    // -----------------------------------------------------------------------
    //  Liquidation
    // -----------------------------------------------------------------------

    /**
     * @notice Liquidate an under-collateralized loan.
     * @dev Anyone may call this if the loan's collateral ratio drops below
     *      110 % (`LIQUIDATION_THRESHOLD_BP`).  The liquidator repays the
     *      full debt (principal + interest) and receives all the collateral.
     * @param loanId ID of the loan to liquidate.
     */
    function liquidate(uint256 loanId) external nonReentrant {
        Loan storage loan = loans[loanId];
        if (!loan.active) revert LoanNotActive(loanId);

        // Check health: collateralValue / debtValue must be < 110 %.
        uint256 collateralValue = _getValueInBase(
            loan.collateralToken,
            loan.borrowToken,
            loan.collateralAmount
        );
        uint256 interest = calculateInterest(loanId);
        uint256 totalDebt = loan.borrowAmount + interest;

        uint256 healthBp = (collateralValue * BP) / totalDebt;
        if (healthBp >= LIQUIDATION_THRESHOLD_BP) {
            revert LoanNotLiquidatable(healthBp);
        }

        // Close the loan.
        loan.active = false;
        totalBorrowed[loan.borrowToken] -= loan.borrowAmount;

        // Liquidator pays the debt.
        IERC20(loan.borrowToken).safeTransferFrom(msg.sender, address(this), totalDebt);

        // Liquidator receives the collateral.
        IERC20(loan.collateralToken).safeTransfer(msg.sender, loan.collateralAmount);

        emit Liquidated(loanId, msg.sender, loan.collateralAmount, totalDebt);
    }

    // -----------------------------------------------------------------------
    //  Interest calculation
    // -----------------------------------------------------------------------

    /**
     * @notice Calculate accrued interest on a loan since its start time.
     * @dev interest = (principal * rate * elapsed) / (SECONDS_PER_YEAR * BP)
     * @param loanId Loan identifier.
     * @return Accrued interest in borrowToken units.
     */
    function calculateInterest(uint256 loanId) public view returns (uint256) {
        Loan memory loan = loans[loanId];
        if (!loan.active) return 0;

        uint256 elapsed = block.timestamp - loan.startTime;
        return (loan.borrowAmount * loan.interestRate * elapsed) / (SECONDS_PER_YEAR * BP);
    }

    // -----------------------------------------------------------------------
    //  Internal helpers
    // -----------------------------------------------------------------------

    /**
     * @dev Get the value of `amount` of `tokenA` denominated in `tokenB`
     *      using SovereignDEX reserves as the price oracle.
     *      price = (reserveB * PRECISION) / reserveA
     *      value = (amount * price) / PRECISION
     */
    function _getValueInBase(
        address tokenA,
        address tokenB,
        uint256 amount
    ) internal view returns (uint256) {
        uint256 pairId = dexContract.getPair(tokenA, tokenB);
        if (pairId == 0) revert NoDEXPair(tokenA, tokenB);

        (
            address pairTokenA,
            ,
            uint256 reserveA,
            uint256 reserveB,
            ,
        ) = dexContract.pairs(pairId);

        // The DEX stores tokens with the lower address as tokenA.
        uint256 reserveIn;
        uint256 reserveOut;
        if (tokenA < tokenB) {
            // tokenA is the DEX's tokenA.
            reserveIn  = reserveA;
            reserveOut = reserveB;
        } else {
            reserveIn  = reserveB;
            reserveOut = reserveA;
        }

        if (reserveIn == 0) revert NoDEXPair(tokenA, tokenB);

        // Suppress unused variable warning — pairTokenA used for ordering logic above.
        pairTokenA;

        return (amount * reserveOut * PRECISION) / (reserveIn * PRECISION);
    }

    /**
     * @dev Calculate the interest rate for a new borrow based on utilization.
     *      utilization = totalBorrowed[token] / totalDeposited[token]
     *      rate = BASE_RATE + (MAX_RATE - BASE_RATE) * utilization
     *      The `additionalBorrow` is included in the numerator to reflect
     *      the state *after* this borrow.
     */
    function _calculateRate(
        address token,
        uint256 additionalBorrow
    ) internal view returns (uint256) {
        uint256 totalDep = totalDeposited[token];
        if (totalDep == 0) return BASE_RATE_BP;

        uint256 totalBorr = totalBorrowed[token] + additionalBorrow;
        // utilization scaled to BP (10_000 = 100 %).
        uint256 utilization = (totalBorr * BP) / totalDep;
        if (utilization > BP) utilization = BP;

        return BASE_RATE_BP + ((MAX_RATE_BP - BASE_RATE_BP) * utilization) / BP;
    }

    // -----------------------------------------------------------------------
    //  View helpers
    // -----------------------------------------------------------------------

    /**
     * @notice Get the current health factor of a loan in basis points.
     * @dev health = (collateralValue * BP) / totalDebt.
     *      A value below `LIQUIDATION_THRESHOLD_BP` (11000) means the loan
     *      is eligible for liquidation.
     * @param loanId Loan identifier.
     * @return healthBp Health factor in basis points.
     */
    function getHealthFactor(uint256 loanId) external view returns (uint256 healthBp) {
        Loan memory loan = loans[loanId];
        if (!loan.active) return 0;

        uint256 collateralValue = _getValueInBase(
            loan.collateralToken,
            loan.borrowToken,
            loan.collateralAmount
        );
        uint256 totalDebt = loan.borrowAmount + calculateInterest(loanId);
        if (totalDebt == 0) return type(uint256).max;

        healthBp = (collateralValue * BP) / totalDebt;
    }

    /**
     * @notice Retrieve the current utilization rate for a token pool.
     * @param token ERC-20 address.
     * @return utilizationBp Utilization in basis points (10000 = 100 %).
     */
    function getUtilization(address token) external view returns (uint256 utilizationBp) {
        uint256 totalDep = totalDeposited[token];
        if (totalDep == 0) return 0;
        utilizationBp = (totalBorrowed[token] * BP) / totalDep;
    }

    /**
     * @notice Retrieve the current interest rate that would apply to a new
     *         borrow of `amount` of `token`.
     * @param token  ERC-20 address of the borrow token.
     * @param amount Hypothetical borrow amount.
     * @return rateBp Interest rate in basis points.
     */
    function getCurrentRate(
        address token,
        uint256 amount
    ) external view returns (uint256 rateBp) {
        return _calculateRate(token, amount);
    }
}
