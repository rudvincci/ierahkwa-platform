// SPDX-License-Identifier: MIT
// ============================================================================
// BDET BANK TOKEN — Banco Digital de Economia Tribal
// Ierahkwa Ne Kanienke — Nacion Digital Soberana
// Blockchain: MameyNode (EVM-compatible)
// ============================================================================

pragma solidity 0.8.24;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Burnable.sol";
import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/utils/Pausable.sol";
import "@openzeppelin/contracts/token/ERC20/IERC20.sol";
import "@openzeppelin/contracts/token/ERC20/utils/SafeERC20.sol";

/**
 * @title BDETToken
 * @author Ierahkwa Sovereign Development Council
 * @notice BDET (Banco Digital de Economia Tribal) is the central bank token
 *         for the Ierahkwa sovereign financial system. It serves as the
 *         institutional backbone for tribal banking operations, reserve
 *         management, and AML/KYC compliance enforcement.
 * @dev ERC-20 with account freeze capability, interest rate tracking,
 *      and reserve ratio management. Designed for central-bank-level
 *      monetary policy within the sovereign digital economy.
 *
 * Key features:
 *   - Account freeze/unfreeze for AML/KYC compliance
 *   - Configurable interest rate mechanism (basis points)
 *   - Reserve ratio tracking and enforcement
 *   - Banker role for institutional operations
 *
 * Roles:
 *   - DEFAULT_ADMIN_ROLE: Full administrative control
 *   - MINTER_ROLE: Can mint new tokens up to MAX_SUPPLY
 *   - PAUSER_ROLE: Can pause/unpause all transfers
 *   - BANKER_ROLE: Can freeze/unfreeze accounts, manage interest & reserves
 */
contract BDETToken is
    ERC20,
    ERC20Burnable,
    AccessControl,
    Pausable
{
    using SafeERC20 for IERC20;

    // =========================================================================
    // CONSTANTS
    // =========================================================================

    /// @notice Role identifier for accounts permitted to mint new tokens.
    bytes32 public constant MINTER_ROLE = keccak256("MINTER_ROLE");

    /// @notice Role identifier for accounts permitted to pause/unpause.
    bytes32 public constant PAUSER_ROLE = keccak256("PAUSER_ROLE");

    /// @notice Role identifier for banking operations (freeze, interest, reserves).
    bytes32 public constant BANKER_ROLE = keccak256("BANKER_ROLE");

    /// @notice Maximum supply: 1 billion BDET.
    uint256 public constant MAX_SUPPLY = 1_000_000_000 * 10 ** 18;

    /// @notice Initial supply minted to treasury: 500 million BDET.
    uint256 public constant INITIAL_SUPPLY = 500_000_000 * 10 ** 18;

    /// @notice Maximum interest rate: 50% (5000 basis points).
    uint256 public constant MAX_INTEREST_RATE = 5000;

    /// @notice Maximum reserve ratio: 100% (10000 basis points).
    uint256 public constant MAX_RESERVE_RATIO = 10_000;

    /// @notice Basis points denominator.
    uint256 private constant _BASIS_POINTS = 10_000;

    // =========================================================================
    // STATE VARIABLES
    // =========================================================================

    /// @notice Address of the sovereign treasury.
    address public treasury;

    /// @notice Mapping of frozen accounts (AML/KYC compliance).
    mapping(address => bool) public frozen;

    /// @notice Timestamp of when each account was frozen.
    mapping(address => uint256) public frozenAt;

    /// @notice Reason hash for each freeze (keccak256 of reason string).
    mapping(address => bytes32) public freezeReason;

    /// @notice Annual interest rate in basis points (e.g., 250 = 2.5%).
    uint256 public annualInterestRate;

    /// @notice Required reserve ratio in basis points (e.g., 2000 = 20%).
    uint256 public reserveRatio;

    /// @notice Current reserve balance tracked by the banking system.
    uint256 public reserveBalance;

    /// @notice Timestamp of the last interest rate change.
    uint256 public lastInterestUpdate;

    /// @notice Total number of currently frozen accounts.
    uint256 public frozenAccountCount;

    // =========================================================================
    // EVENTS
    // =========================================================================

    /// @notice Emitted when an account is frozen for compliance.
    event AccountFrozen(
        address indexed account,
        bytes32 indexed reason,
        address indexed frozenBy,
        uint256 timestamp
    );

    /// @notice Emitted when a previously frozen account is unfrozen.
    event AccountUnfrozen(
        address indexed account,
        address indexed unfrozenBy,
        uint256 timestamp
    );

    /// @notice Emitted when the annual interest rate is changed.
    event InterestRateUpdated(
        uint256 oldRate,
        uint256 newRate,
        address indexed updatedBy,
        uint256 timestamp
    );

    /// @notice Emitted when the reserve ratio is changed.
    event ReserveRatioUpdated(
        uint256 oldRatio,
        uint256 newRatio,
        address indexed updatedBy
    );

    /// @notice Emitted when the reserve balance is updated.
    event ReserveBalanceUpdated(
        uint256 oldBalance,
        uint256 newBalance,
        address indexed updatedBy
    );

    /// @notice Emitted when the treasury address is changed.
    event TreasuryUpdated(address indexed oldTreasury, address indexed newTreasury);

    /// @notice Emitted when stuck tokens are recovered.
    event TokensRecovered(address indexed token, uint256 amount);

    // =========================================================================
    // ERRORS
    // =========================================================================

    /// @notice Thrown when the zero address is provided.
    error ZeroAddress();

    /// @notice Thrown when minting would exceed MAX_SUPPLY.
    error ExceedsMaxSupply(uint256 requested, uint256 available);

    /// @notice Thrown when a transfer involves a frozen account.
    error AccountIsFrozen(address account);

    /// @notice Thrown when trying to freeze an already frozen account.
    error AlreadyFrozen(address account);

    /// @notice Thrown when trying to unfreeze an account that is not frozen.
    error NotFrozen(address account);

    /// @notice Thrown when the interest rate exceeds the maximum.
    error InterestRateTooHigh(uint256 proposed, uint256 maximum);

    /// @notice Thrown when the reserve ratio exceeds the maximum.
    error ReserveRatioTooHigh(uint256 proposed, uint256 maximum);

    /// @notice Thrown when attempting to recover the native BDET token.
    error CannotRecoverNativeToken();

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    /**
     * @notice Deploys the BDET token and mints INITIAL_SUPPLY to the treasury.
     * @param _treasury Address of the sovereign treasury.
     * @param _admin    Address that receives all initial roles.
     */
    constructor(
        address _treasury,
        address _admin
    ) ERC20("BDET Bank Token", "BDET") {
        if (_treasury == address(0)) revert ZeroAddress();
        if (_admin == address(0)) revert ZeroAddress();

        treasury = _treasury;
        lastInterestUpdate = block.timestamp;

        // Default monetary policy
        annualInterestRate = 250; // 2.5% annual
        reserveRatio = 2000;     // 20% reserve requirement

        // Grant roles
        _grantRole(DEFAULT_ADMIN_ROLE, _admin);
        _grantRole(MINTER_ROLE, _admin);
        _grantRole(PAUSER_ROLE, _admin);
        _grantRole(BANKER_ROLE, _admin);

        // Mint initial supply to treasury
        _mint(_treasury, INITIAL_SUPPLY);
    }

    // =========================================================================
    // MINTING
    // =========================================================================

    /**
     * @notice Mint new BDET tokens to a specified address.
     * @dev Caller must have MINTER_ROLE. Cannot mint to frozen accounts.
     * @param to     Recipient of the newly minted tokens.
     * @param amount Number of tokens to mint (in wei).
     */
    function mint(address to, uint256 amount) external onlyRole(MINTER_ROLE) {
        if (to == address(0)) revert ZeroAddress();
        if (frozen[to]) revert AccountIsFrozen(to);
        uint256 available = MAX_SUPPLY - totalSupply();
        if (amount > available) revert ExceedsMaxSupply(amount, available);
        _mint(to, amount);
    }

    /**
     * @notice Mint tokens directly to the treasury.
     * @param amount Number of tokens to mint (in wei).
     */
    function mintToTreasury(uint256 amount) external onlyRole(MINTER_ROLE) {
        uint256 available = MAX_SUPPLY - totalSupply();
        if (amount > available) revert ExceedsMaxSupply(amount, available);
        _mint(treasury, amount);
    }

    // =========================================================================
    // ACCOUNT FREEZE / UNFREEZE (AML/KYC COMPLIANCE)
    // =========================================================================

    /**
     * @notice Freeze an account, preventing all transfers to and from it.
     * @dev Only callable by BANKER_ROLE. Used for AML/KYC compliance.
     * @param account Account to freeze.
     * @param reason  keccak256 hash of the reason for freezing.
     */
    function freezeAccount(
        address account,
        bytes32 reason
    ) external onlyRole(BANKER_ROLE) {
        if (account == address(0)) revert ZeroAddress();
        if (frozen[account]) revert AlreadyFrozen(account);

        frozen[account] = true;
        frozenAt[account] = block.timestamp;
        freezeReason[account] = reason;
        frozenAccountCount++;

        emit AccountFrozen(account, reason, msg.sender, block.timestamp);
    }

    /**
     * @notice Unfreeze a previously frozen account.
     * @dev Only callable by BANKER_ROLE.
     * @param account Account to unfreeze.
     */
    function unfreezeAccount(address account) external onlyRole(BANKER_ROLE) {
        if (!frozen[account]) revert NotFrozen(account);

        frozen[account] = false;
        frozenAt[account] = 0;
        freezeReason[account] = bytes32(0);
        frozenAccountCount--;

        emit AccountUnfrozen(account, msg.sender, block.timestamp);
    }

    /**
     * @notice Check if an account is currently frozen.
     * @param account Address to check.
     * @return True if the account is frozen.
     */
    function isAccountFrozen(address account) external view returns (bool) {
        return frozen[account];
    }

    // =========================================================================
    // INTEREST RATE MECHANISM
    // =========================================================================

    /**
     * @notice Set the annual interest rate for the BDET banking system.
     * @dev Rate is expressed in basis points (e.g., 250 = 2.5%).
     *      Maximum rate is 50% (5000 bp).
     * @param _rate New annual interest rate in basis points.
     */
    function setInterestRate(uint256 _rate) external onlyRole(BANKER_ROLE) {
        if (_rate > MAX_INTEREST_RATE) {
            revert InterestRateTooHigh(_rate, MAX_INTEREST_RATE);
        }

        uint256 oldRate = annualInterestRate;
        annualInterestRate = _rate;
        lastInterestUpdate = block.timestamp;

        emit InterestRateUpdated(oldRate, _rate, msg.sender, block.timestamp);
    }

    /**
     * @notice Calculate accrued interest for a given principal over elapsed seconds.
     * @dev Uses simple interest formula: principal * rate * time / (365 days * 10000).
     * @param principal       The principal amount in wei.
     * @param elapsedSeconds  Duration in seconds over which to calculate interest.
     * @return The accrued interest amount in wei.
     */
    function calculateInterest(
        uint256 principal,
        uint256 elapsedSeconds
    ) external view returns (uint256) {
        if (annualInterestRate == 0 || principal == 0 || elapsedSeconds == 0) {
            return 0;
        }
        return (principal * annualInterestRate * elapsedSeconds) / (365 days * _BASIS_POINTS);
    }

    // =========================================================================
    // RESERVE MANAGEMENT
    // =========================================================================

    /**
     * @notice Set the required reserve ratio.
     * @dev Ratio is in basis points (e.g., 2000 = 20%). Max is 100%.
     * @param _ratio New reserve ratio in basis points.
     */
    function setReserveRatio(uint256 _ratio) external onlyRole(BANKER_ROLE) {
        if (_ratio > MAX_RESERVE_RATIO) {
            revert ReserveRatioTooHigh(_ratio, MAX_RESERVE_RATIO);
        }

        uint256 oldRatio = reserveRatio;
        reserveRatio = _ratio;

        emit ReserveRatioUpdated(oldRatio, _ratio, msg.sender);
    }

    /**
     * @notice Update the current reserve balance.
     * @dev This is a tracking mechanism; actual reserves are held externally.
     * @param _balance New reserve balance in wei.
     */
    function updateReserveBalance(uint256 _balance) external onlyRole(BANKER_ROLE) {
        uint256 oldBalance = reserveBalance;
        reserveBalance = _balance;

        emit ReserveBalanceUpdated(oldBalance, _balance, msg.sender);
    }

    /**
     * @notice Calculate the required reserve for the current total supply.
     * @return Required reserve amount in wei.
     */
    function requiredReserve() external view returns (uint256) {
        return (totalSupply() * reserveRatio) / _BASIS_POINTS;
    }

    /**
     * @notice Check if the current reserves meet the required ratio.
     * @return True if reserves are adequate.
     */
    function isReserveAdequate() external view returns (bool) {
        uint256 required = (totalSupply() * reserveRatio) / _BASIS_POINTS;
        return reserveBalance >= required;
    }

    /**
     * @notice Get the reserve surplus or deficit.
     * @return surplus True if surplus, false if deficit.
     * @return amount  Absolute value of surplus or deficit in wei.
     */
    function reserveStatus() external view returns (bool surplus, uint256 amount) {
        uint256 required = (totalSupply() * reserveRatio) / _BASIS_POINTS;
        if (reserveBalance >= required) {
            return (true, reserveBalance - required);
        } else {
            return (false, required - reserveBalance);
        }
    }

    // =========================================================================
    // GOVERNANCE
    // =========================================================================

    /**
     * @notice Update the treasury address.
     * @param _treasury New treasury address.
     */
    function setTreasury(address _treasury) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (_treasury == address(0)) revert ZeroAddress();
        address old = treasury;
        treasury = _treasury;
        emit TreasuryUpdated(old, _treasury);
    }

    // =========================================================================
    // PAUSE
    // =========================================================================

    /**
     * @notice Pause all token transfers.
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
     * @param token  Address of the ERC-20 token to recover.
     * @param amount Amount to recover.
     */
    function recoverTokens(
        address token,
        uint256 amount
    ) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (token == address(this)) revert CannotRecoverNativeToken();
        IERC20(token).safeTransfer(treasury, amount);
        emit TokensRecovered(token, amount);
    }

    // =========================================================================
    // INTERNAL OVERRIDES
    // =========================================================================

    /**
     * @dev Override _update to enforce pause and freeze checks on transfers.
     *      Minting (from == address(0)) and burning (to == address(0)) bypass
     *      the freeze check on the zero address but still check the real account.
     */
    function _update(
        address from,
        address to,
        uint256 amount
    ) internal override {
        // Enforce pause on transfers (allow mint/burn by authorized roles)
        if (from != address(0) && to != address(0)) {
            _requireNotPaused();
        }

        // Check freeze status for both sender and receiver
        if (from != address(0) && frozen[from]) {
            revert AccountIsFrozen(from);
        }
        if (to != address(0) && frozen[to]) {
            revert AccountIsFrozen(to);
        }

        super._update(from, to, amount);
    }

    // =========================================================================
    // VIEW FUNCTIONS
    // =========================================================================

    /**
     * @notice Get the remaining mintable supply.
     * @return Amount of BDET that can still be minted.
     */
    function mintableSupply() external view returns (uint256) {
        return MAX_SUPPLY - totalSupply();
    }

    /**
     * @notice Get a summary of the current monetary policy.
     * @return interestRate     Current annual interest rate in basis points.
     * @return currentRatio     Current reserve ratio in basis points.
     * @return currentReserve   Current reserve balance.
     * @return requiredReserveAmt Required reserve for current supply.
     * @return adequate         Whether reserves are adequate.
     */
    function monetaryPolicySummary()
        external
        view
        returns (
            uint256 interestRate,
            uint256 currentRatio,
            uint256 currentReserve,
            uint256 requiredReserveAmt,
            bool adequate
        )
    {
        uint256 required = (totalSupply() * reserveRatio) / _BASIS_POINTS;
        return (
            annualInterestRate,
            reserveRatio,
            reserveBalance,
            required,
            reserveBalance >= required
        );
    }
}
