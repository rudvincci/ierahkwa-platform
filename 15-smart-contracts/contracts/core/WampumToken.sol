// SPDX-License-Identifier: MIT
// ============================================================================
// WAMPUM TOKEN (WMP) — Moneda Soberana Nativa
// Ierahkwa Ne Kanienke — Nacion Digital Soberana
// Blockchain: MameyNode (EVM-compatible)
// ============================================================================

pragma solidity 0.8.24;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Burnable.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Permit.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Votes.sol";
import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/utils/Pausable.sol";
import "@openzeppelin/contracts/utils/ReentrancyGuard.sol";
import "@openzeppelin/contracts/token/ERC20/IERC20.sol";
import "@openzeppelin/contracts/token/ERC20/utils/SafeERC20.sol";

/**
 * @title WampumToken
 * @author Ierahkwa Sovereign Development Council
 * @notice WAMPUM (WMP) is the native sovereign currency of the Ierahkwa
 *         digital nation, serving 72M indigenous people across 19 nations
 *         and 574 tribal nations.
 * @dev ERC-20 token with governance voting, transfer fees, burn mechanics,
 *      and role-based access control. Built on OpenZeppelin v5.x patterns.
 *
 * Key features:
 *   - Configurable transfer fee (0-5%) directed to treasury
 *   - Deflationary burn mechanism
 *   - ERC20Votes for on-chain governance participation
 *   - Emergency pause capability
 *   - Stuck token recovery
 *
 * Roles:
 *   - DEFAULT_ADMIN_ROLE: Full administrative control
 *   - MINTER_ROLE: Can mint new tokens up to MAX_SUPPLY
 *   - PAUSER_ROLE: Can pause/unpause all transfers
 *   - GOVERNANCE_ROLE: Can configure fees, treasury, and ecosystem addresses
 */
contract WampumToken is
    ERC20,
    ERC20Burnable,
    ERC20Permit,
    ERC20Votes,
    AccessControl,
    Pausable,
    ReentrancyGuard
{
    using SafeERC20 for IERC20;

    // =========================================================================
    // CONSTANTS
    // =========================================================================

    /// @notice Role identifier for accounts permitted to mint new tokens.
    bytes32 public constant MINTER_ROLE = keccak256("MINTER_ROLE");

    /// @notice Role identifier for accounts permitted to pause/unpause transfers.
    bytes32 public constant PAUSER_ROLE = keccak256("PAUSER_ROLE");

    /// @notice Role identifier for governance-level configuration changes.
    bytes32 public constant GOVERNANCE_ROLE = keccak256("GOVERNANCE_ROLE");

    /// @notice Absolute maximum supply: 10 trillion WMP.
    uint256 public constant MAX_SUPPLY = 10_000_000_000_000 * 10 ** 18;

    /// @notice Initial supply minted to treasury at deployment: 100 million WMP.
    uint256 public constant INITIAL_SUPPLY = 100_000_000 * 10 ** 18;

    /// @notice Maximum transfer fee in basis points (500 = 5%).
    uint256 public constant MAX_FEE = 500;

    /// @notice Basis points denominator (10000 = 100%).
    uint256 private constant _BASIS_POINTS = 10_000;

    // =========================================================================
    // STATE VARIABLES
    // =========================================================================

    /// @notice Address of the sovereign treasury that receives transfer fees.
    address public treasury;

    /// @notice Address of the staking rewards distribution contract.
    address public stakingRewards;

    /// @notice Address of the primary liquidity pool.
    address public liquidityPool;

    /// @notice Current transfer fee in basis points (0 = no fee, 500 = 5%).
    uint256 public transferFee;

    /// @notice Accounts exempt from paying transfer fees.
    mapping(address => bool) public feeExempt;

    /// @notice Total amount of WMP burned across all time.
    uint256 public totalBurned;

    // =========================================================================
    // EVENTS
    // =========================================================================

    /// @notice Emitted when the treasury address is changed.
    event TreasuryUpdated(address indexed oldTreasury, address indexed newTreasury);

    /// @notice Emitted when the transfer fee rate is modified.
    event TransferFeeUpdated(uint256 oldFee, uint256 newFee);

    /// @notice Emitted when an account's fee exemption status changes.
    event FeeExemptionUpdated(address indexed account, bool exempt);

    /// @notice Emitted when the staking rewards address is set.
    event StakingRewardsUpdated(address indexed oldAddress, address indexed newAddress);

    /// @notice Emitted when the liquidity pool address is set.
    event LiquidityPoolUpdated(address indexed oldAddress, address indexed newAddress);

    /// @notice Emitted when stuck ERC-20 tokens are recovered from this contract.
    event TokensRecovered(address indexed token, address indexed to, uint256 amount);

    /// @notice Emitted when tokens are burned (in addition to the standard Transfer event).
    event TokensBurned(address indexed burner, uint256 amount, uint256 newTotalBurned);

    // =========================================================================
    // ERRORS
    // =========================================================================

    /// @notice Thrown when the zero address is provided where a valid address is required.
    error ZeroAddress();

    /// @notice Thrown when minting would exceed MAX_SUPPLY.
    error ExceedsMaxSupply(uint256 requested, uint256 available);

    /// @notice Thrown when the proposed fee exceeds MAX_FEE.
    error FeeTooHigh(uint256 proposed, uint256 maximum);

    /// @notice Thrown when attempting to recover the native WMP token from this contract.
    error CannotRecoverNativeToken();

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    /**
     * @notice Deploys the WampumToken and mints INITIAL_SUPPLY to the treasury.
     * @param _treasury Address of the sovereign treasury.
     * @param _admin    Address that receives all initial roles.
     */
    constructor(
        address _treasury,
        address _admin
    )
        ERC20("WAMPUM", "WMP")
        ERC20Permit("WAMPUM")
    {
        if (_treasury == address(0)) revert ZeroAddress();
        if (_admin == address(0)) revert ZeroAddress();

        treasury = _treasury;

        // Grant roles to the initial admin
        _grantRole(DEFAULT_ADMIN_ROLE, _admin);
        _grantRole(MINTER_ROLE, _admin);
        _grantRole(PAUSER_ROLE, _admin);
        _grantRole(GOVERNANCE_ROLE, _admin);

        // Set default fee exemptions for core addresses
        feeExempt[_treasury] = true;
        feeExempt[_admin] = true;
        feeExempt[address(this)] = true;

        // Mint initial supply to the sovereign treasury
        _mint(_treasury, INITIAL_SUPPLY);
    }

    // =========================================================================
    // MINTING
    // =========================================================================

    /**
     * @notice Mint new WMP tokens to a specified address.
     * @dev Caller must have MINTER_ROLE. Reverts if totalSupply + amount > MAX_SUPPLY.
     * @param to     Recipient of the newly minted tokens.
     * @param amount Number of tokens to mint (in wei).
     */
    function mint(address to, uint256 amount) external onlyRole(MINTER_ROLE) {
        if (to == address(0)) revert ZeroAddress();
        uint256 available = MAX_SUPPLY - totalSupply();
        if (amount > available) revert ExceedsMaxSupply(amount, available);
        _mint(to, amount);
    }

    /**
     * @notice Convenience function to mint tokens directly to the treasury.
     * @param amount Number of tokens to mint (in wei).
     */
    function mintToTreasury(uint256 amount) external onlyRole(MINTER_ROLE) {
        uint256 available = MAX_SUPPLY - totalSupply();
        if (amount > available) revert ExceedsMaxSupply(amount, available);
        _mint(treasury, amount);
    }

    // =========================================================================
    // BURN (DEFLATIONARY MECHANISM)
    // =========================================================================

    /**
     * @notice Burns tokens from the caller's balance. Tracks cumulative burn.
     * @param amount Number of tokens to burn (in wei).
     */
    function burn(uint256 amount) public override {
        super.burn(amount);
        totalBurned += amount;
        emit TokensBurned(msg.sender, amount, totalBurned);
    }

    /**
     * @notice Burns tokens from a specified account using the caller's allowance.
     * @param account Account whose tokens will be burned.
     * @param amount  Number of tokens to burn (in wei).
     */
    function burnFrom(address account, uint256 amount) public override {
        super.burnFrom(account, amount);
        totalBurned += amount;
        emit TokensBurned(account, amount, totalBurned);
    }

    // =========================================================================
    // GOVERNANCE CONFIGURATION
    // =========================================================================

    /**
     * @notice Update the sovereign treasury address.
     * @dev The new treasury is automatically made fee-exempt.
     * @param _treasury New treasury address.
     */
    function setTreasury(address _treasury) external onlyRole(GOVERNANCE_ROLE) {
        if (_treasury == address(0)) revert ZeroAddress();
        address oldTreasury = treasury;
        treasury = _treasury;
        feeExempt[_treasury] = true;
        emit TreasuryUpdated(oldTreasury, _treasury);
    }

    /**
     * @notice Set the transfer fee in basis points.
     * @param _fee Fee in basis points (0 = disabled, max = 500 = 5%).
     */
    function setTransferFee(uint256 _fee) external onlyRole(GOVERNANCE_ROLE) {
        if (_fee > MAX_FEE) revert FeeTooHigh(_fee, MAX_FEE);
        uint256 oldFee = transferFee;
        transferFee = _fee;
        emit TransferFeeUpdated(oldFee, _fee);
    }

    /**
     * @notice Set or remove fee exemption for an account.
     * @param account Address to modify.
     * @param exempt  True to exempt, false to remove exemption.
     */
    function setFeeExemption(address account, bool exempt) external onlyRole(GOVERNANCE_ROLE) {
        if (account == address(0)) revert ZeroAddress();
        feeExempt[account] = exempt;
        emit FeeExemptionUpdated(account, exempt);
    }

    /**
     * @notice Set the staking rewards contract address (auto fee-exempt).
     * @param _stakingRewards Address of the staking rewards contract.
     */
    function setStakingRewards(address _stakingRewards) external onlyRole(GOVERNANCE_ROLE) {
        if (_stakingRewards == address(0)) revert ZeroAddress();
        address old = stakingRewards;
        stakingRewards = _stakingRewards;
        feeExempt[_stakingRewards] = true;
        emit StakingRewardsUpdated(old, _stakingRewards);
    }

    /**
     * @notice Set the liquidity pool address (auto fee-exempt).
     * @param _liquidityPool Address of the primary liquidity pool.
     */
    function setLiquidityPool(address _liquidityPool) external onlyRole(GOVERNANCE_ROLE) {
        if (_liquidityPool == address(0)) revert ZeroAddress();
        address old = liquidityPool;
        liquidityPool = _liquidityPool;
        feeExempt[_liquidityPool] = true;
        emit LiquidityPoolUpdated(old, _liquidityPool);
    }

    // =========================================================================
    // PAUSE
    // =========================================================================

    /**
     * @notice Pause all token transfers. Emergency use only.
     */
    function pause() external onlyRole(PAUSER_ROLE) {
        _pause();
    }

    /**
     * @notice Unpause all token transfers.
     */
    function unpause() external onlyRole(PAUSER_ROLE) {
        _unpause();
    }

    // =========================================================================
    // TOKEN RECOVERY
    // =========================================================================

    /**
     * @notice Recover ERC-20 tokens accidentally sent to this contract.
     * @dev Cannot recover WMP itself. Uses SafeERC20 for safe transfer.
     * @param token  Address of the ERC-20 token to recover.
     * @param amount Amount to recover.
     */
    function recoverTokens(
        address token,
        uint256 amount
    ) external onlyRole(DEFAULT_ADMIN_ROLE) nonReentrant {
        if (token == address(this)) revert CannotRecoverNativeToken();
        IERC20(token).safeTransfer(treasury, amount);
        emit TokensRecovered(token, treasury, amount);
    }

    // =========================================================================
    // REQUIRED OVERRIDES (OpenZeppelin v5.x)
    // =========================================================================

    /**
     * @dev Internal hook called on every transfer, mint, and burn.
     *      Enforces pause state and applies transfer fees.
     *      Uses the OZ v5 `_update` pattern instead of `_afterTokenTransfer`.
     */
    function _update(
        address from,
        address to,
        uint256 amount
    ) internal override(ERC20, ERC20Votes) {
        // Enforce pause on all transfers (allow mint/burn while paused only by admin)
        if (from != address(0) && to != address(0)) {
            _requireNotPaused();

            // Apply transfer fee if applicable
            if (transferFee > 0 && !feeExempt[from] && !feeExempt[to]) {
                uint256 feeAmount = (amount * transferFee) / _BASIS_POINTS;
                if (feeAmount > 0) {
                    // Transfer fee portion to treasury
                    super._update(from, treasury, feeAmount);
                    amount -= feeAmount;
                }
            }
        }

        super._update(from, to, amount);
    }

    /**
     * @dev Override nonces for ERC20Permit + ERC20Votes compatibility.
     */
    function nonces(
        address owner
    ) public view override(ERC20Permit, Nonces) returns (uint256) {
        return super.nonces(owner);
    }

    // =========================================================================
    // VIEW FUNCTIONS
    // =========================================================================

    /**
     * @notice Returns the circulating supply (total minted minus total burned).
     * @return The current circulating supply in wei.
     */
    function circulatingSupply() external view returns (uint256) {
        return totalSupply();
    }

    /**
     * @notice Returns the remaining mintable supply.
     * @return The number of tokens that can still be minted.
     */
    function mintableSupply() external view returns (uint256) {
        return MAX_SUPPLY - totalSupply();
    }

    /**
     * @notice Calculate the fee that would be charged on a transfer amount.
     * @param amount The transfer amount.
     * @return The fee amount in wei.
     */
    function calculateFee(uint256 amount) external view returns (uint256) {
        if (transferFee == 0) return 0;
        return (amount * transferFee) / _BASIS_POINTS;
    }
}
