// SPDX-License-Identifier: Sovereign-1.0
pragma solidity 0.8.24;

import {ReentrancyGuard} from "@openzeppelin/contracts/utils/ReentrancyGuard.sol";
import {AccessControl} from "@openzeppelin/contracts/access/AccessControl.sol";
import {Pausable} from "@openzeppelin/contracts/utils/Pausable.sol";
import {IERC20} from "@openzeppelin/contracts/token/ERC20/IERC20.sol";
import {SafeERC20} from "@openzeppelin/contracts/token/ERC20/utils/SafeERC20.sol";
import {ECDSA} from "@openzeppelin/contracts/utils/cryptography/ECDSA.sol";
import {MessageHashUtils} from "@openzeppelin/contracts/utils/cryptography/MessageHashUtils.sol";

/**
 * @title SovereignBridge
 * @author Ierahkwa Ne Kanienke — Sovereign Digital Nation
 * @notice Cross-chain bridge using lock-and-mint pattern with multi-relayer verification.
 *         Enables interoperability between MameyNode and external EVM chains (Ethereum,
 *         Polygon, BSC, Arbitrum, Avalanche) for the sovereign token ecosystem.
 * @dev Implements 3-of-5 multi-signature verification by authorized relayers.
 *      Includes daily transfer limits, fee collection, and emergency withdrawal capabilities.
 *      All locked tokens are held in this contract until unlocked via verified cross-chain proofs.
 */
contract SovereignBridge is ReentrancyGuard, AccessControl, Pausable {
    using SafeERC20 for IERC20;
    using ECDSA for bytes32;
    using MessageHashUtils for bytes32;

    // ──────────────────────────────────────────────────────────────
    //  Constants & Roles
    // ──────────────────────────────────────────────────────────────

    /// @notice Role identifier for bridge relayer operators.
    bytes32 public constant RELAYER_ROLE = keccak256("RELAYER_ROLE");

    /// @notice Contract version for upgrade tracking.
    string public constant VERSION = "1.0.0";

    /// @notice Bridge fee in basis points (0.1% = 10 bps).
    uint256 public constant BRIDGE_FEE_BPS = 10;

    /// @notice Basis points denominator (100% = 10,000).
    uint256 public constant BPS_DENOMINATOR = 10_000;

    /// @notice Duration of the daily limit window (24 hours).
    uint256 public constant DAILY_LIMIT_WINDOW = 1 days;

    // ──────────────────────────────────────────────────────────────
    //  State Variables
    // ──────────────────────────────────────────────────────────────

    /// @notice Number of relayer signatures required to authorize an unlock.
    uint256 public relayerThreshold;

    /// @notice Monotonically increasing nonce for lock operations, preventing replay.
    uint256 public lockNonce;

    /// @notice Address that receives accumulated bridge fees.
    address public feeRecipient;

    /// @notice Set of chain IDs that this bridge supports for cross-chain transfers.
    mapping(uint256 => bool) public supportedChains;

    /// @notice Tracks processed source-chain transaction hashes to prevent double-spending.
    mapping(bytes32 => bool) public processedTxs;

    /// @notice Daily transfer limit per token (in token's smallest unit).
    /// @dev token address => max daily amount
    mapping(address => uint256) public dailyLimit;

    /// @notice Tracks cumulative daily volume per token within the current window.
    /// @dev token address => DailyVolume struct
    mapping(address => DailyVolume) private _dailyVolume;

    /// @notice Accumulated fees per token available for withdrawal.
    /// @dev token address => fee amount
    mapping(address => uint256) public accumulatedFees;

    // ──────────────────────────────────────────────────────────────
    //  Structs
    // ──────────────────────────────────────────────────────────────

    /// @dev Tracks cumulative transfer volume within a rolling daily window.
    struct DailyVolume {
        uint256 amount;
        uint256 windowStart;
    }

    // ──────────────────────────────────────────────────────────────
    //  Events
    // ──────────────────────────────────────────────────────────────

    /// @notice Emitted when tokens are locked in the bridge for cross-chain transfer.
    /// @param token Address of the ERC-20 token locked.
    /// @param sender Address that initiated the lock.
    /// @param amount Amount of tokens locked (before fees).
    /// @param destChainId Target chain ID for the transfer.
    /// @param destAddress Recipient address on the destination chain (encoded as bytes32).
    /// @param nonce Unique lock nonce for this operation.
    /// @param fee Fee deducted from the locked amount.
    event TokensLocked(
        address indexed token,
        address indexed sender,
        uint256 amount,
        uint256 destChainId,
        bytes32 destAddress,
        uint256 nonce,
        uint256 fee
    );

    /// @notice Emitted when tokens are unlocked after cross-chain verification.
    /// @param token Address of the ERC-20 token released.
    /// @param to Recipient address on this chain.
    /// @param amount Amount of tokens released.
    /// @param srcChainId Source chain ID from which the tokens were bridged.
    /// @param srcTxHash Transaction hash on the source chain (used for dedup).
    event TokensUnlocked(
        address indexed token,
        address indexed to,
        uint256 amount,
        uint256 srcChainId,
        bytes32 srcTxHash
    );

    /// @notice Emitted when a new relayer is added to the bridge.
    /// @param relayer Address of the newly added relayer.
    event RelayerAdded(address indexed relayer);

    /// @notice Emitted when a relayer is removed from the bridge.
    /// @param relayer Address of the removed relayer.
    event RelayerRemoved(address indexed relayer);

    /// @notice Emitted when the relayer threshold is updated.
    /// @param oldThreshold Previous threshold value.
    /// @param newThreshold New threshold value.
    event ThresholdUpdated(uint256 oldThreshold, uint256 newThreshold);

    /// @notice Emitted when a supported chain is added.
    /// @param chainId The chain ID that was added.
    event ChainAdded(uint256 indexed chainId);

    /// @notice Emitted when a supported chain is removed.
    /// @param chainId The chain ID that was removed.
    event ChainRemoved(uint256 indexed chainId);

    /// @notice Emitted when the daily limit for a token is updated.
    /// @param token Address of the token.
    /// @param newLimit New daily transfer limit.
    event DailyLimitUpdated(address indexed token, uint256 newLimit);

    /// @notice Emitted during an emergency withdrawal by admin.
    /// @param token Address of the token withdrawn.
    /// @param to Recipient of the withdrawn tokens.
    /// @param amount Amount withdrawn.
    event EmergencyWithdraw(address indexed token, address indexed to, uint256 amount);

    /// @notice Emitted when accumulated fees are withdrawn.
    /// @param token Address of the token.
    /// @param to Recipient of the fees.
    /// @param amount Amount of fees withdrawn.
    event FeesWithdrawn(address indexed token, address indexed to, uint256 amount);

    // ──────────────────────────────────────────────────────────────
    //  Errors
    // ──────────────────────────────────────────────────────────────

    error UnsupportedChain(uint256 chainId);
    error ZeroAmount();
    error ZeroAddress();
    error TransactionAlreadyProcessed(bytes32 txHash);
    error InsufficientSignatures(uint256 provided, uint256 required);
    error DuplicateSignature(address signer);
    error InvalidSignature(address recovered);
    error DailyLimitExceeded(uint256 requested, uint256 remaining);
    error ChainAlreadySupported(uint256 chainId);
    error ChainNotSupported(uint256 chainId);
    error InvalidThreshold(uint256 threshold);

    // ──────────────────────────────────────────────────────────────
    //  Constructor
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Initializes the SovereignBridge with default relayers and supported chains.
     * @param admin Address granted DEFAULT_ADMIN_ROLE (controls all roles).
     * @param initialRelayers Array of 5 initial relayer addresses.
     * @param _feeRecipient Address that will receive bridge fees.
     */
    constructor(
        address admin,
        address[] memory initialRelayers,
        address _feeRecipient
    ) {
        if (admin == address(0)) revert ZeroAddress();
        if (_feeRecipient == address(0)) revert ZeroAddress();
        require(initialRelayers.length == 5, "Must provide exactly 5 initial relayers");

        _grantRole(DEFAULT_ADMIN_ROLE, admin);

        for (uint256 i = 0; i < initialRelayers.length; i++) {
            if (initialRelayers[i] == address(0)) revert ZeroAddress();
            _grantRole(RELAYER_ROLE, initialRelayers[i]);
            emit RelayerAdded(initialRelayers[i]);
        }

        relayerThreshold = 3;
        feeRecipient = _feeRecipient;

        // Default supported chains: Ethereum, Polygon, BSC, Arbitrum, Avalanche
        supportedChains[1] = true;       // Ethereum Mainnet
        supportedChains[137] = true;     // Polygon
        supportedChains[56] = true;      // BSC
        supportedChains[42161] = true;   // Arbitrum One
        supportedChains[43114] = true;   // Avalanche C-Chain

        emit ChainAdded(1);
        emit ChainAdded(137);
        emit ChainAdded(56);
        emit ChainAdded(42161);
        emit ChainAdded(43114);
    }

    // ──────────────────────────────────────────────────────────────
    //  Core Bridge Functions
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Locks ERC-20 tokens in this contract to initiate a cross-chain transfer.
     * @dev Deducts a 0.1% fee from the locked amount. The fee is accumulated for later withdrawal.
     *      Caller must have approved this contract to spend at least `amount` of `token`.
     * @param token Address of the ERC-20 token to lock.
     * @param amount Amount of tokens to lock (fee will be deducted from this).
     * @param destChainId Target chain ID (must be in supportedChains).
     * @param destAddress Recipient address on the destination chain (encoded as bytes32).
     */
    function lock(
        address token,
        uint256 amount,
        uint256 destChainId,
        bytes32 destAddress
    ) external nonReentrant whenNotPaused {
        if (amount == 0) revert ZeroAmount();
        if (token == address(0)) revert ZeroAddress();
        if (!supportedChains[destChainId]) revert UnsupportedChain(destChainId);

        // Check daily limit
        _checkAndUpdateDailyVolume(token, amount);

        // Calculate fee
        uint256 fee = (amount * BRIDGE_FEE_BPS) / BPS_DENOMINATOR;
        uint256 netAmount = amount - fee;

        // Transfer tokens to this contract
        IERC20(token).safeTransferFrom(msg.sender, address(this), amount);

        // Accumulate fee
        accumulatedFees[token] += fee;

        // Increment nonce
        uint256 currentNonce = lockNonce;
        lockNonce = currentNonce + 1;

        emit TokensLocked(
            token,
            msg.sender,
            netAmount,
            destChainId,
            destAddress,
            currentNonce,
            fee
        );
    }

    /**
     * @notice Unlocks tokens after verifying multi-relayer signatures from the source chain.
     * @dev Requires at least `relayerThreshold` valid signatures from distinct relayers.
     *      Each source transaction hash can only be processed once (prevents double-spending).
     * @param token Address of the ERC-20 token to release.
     * @param to Recipient address on this chain.
     * @param amount Amount of tokens to release.
     * @param srcChainId Chain ID of the source chain.
     * @param srcTxHash Transaction hash on the source chain (for dedup).
     * @param signatures Array of ECDSA signatures from relayers authorizing the unlock.
     */
    function unlock(
        address token,
        address to,
        uint256 amount,
        uint256 srcChainId,
        bytes32 srcTxHash,
        bytes[] calldata signatures
    ) external nonReentrant whenNotPaused {
        if (amount == 0) revert ZeroAmount();
        if (to == address(0)) revert ZeroAddress();
        if (token == address(0)) revert ZeroAddress();
        if (processedTxs[srcTxHash]) revert TransactionAlreadyProcessed(srcTxHash);
        if (signatures.length < relayerThreshold) {
            revert InsufficientSignatures(signatures.length, relayerThreshold);
        }

        // Construct the message hash that relayers signed
        bytes32 messageHash = keccak256(
            abi.encodePacked(
                token,
                to,
                amount,
                srcChainId,
                srcTxHash,
                block.chainid
            )
        );

        bytes32 ethSignedHash = messageHash.toEthSignedMessageHash();

        // Verify signatures — must be from distinct relayers
        address[] memory signers = new address[](signatures.length);

        for (uint256 i = 0; i < signatures.length; i++) {
            address recovered = ethSignedHash.recover(signatures[i]);

            if (!hasRole(RELAYER_ROLE, recovered)) {
                revert InvalidSignature(recovered);
            }

            // Check for duplicate signers
            for (uint256 j = 0; j < i; j++) {
                if (signers[j] == recovered) {
                    revert DuplicateSignature(recovered);
                }
            }

            signers[i] = recovered;
        }

        // Mark as processed
        processedTxs[srcTxHash] = true;

        // Release tokens
        IERC20(token).safeTransfer(to, amount);

        emit TokensUnlocked(token, to, amount, srcChainId, srcTxHash);
    }

    // ──────────────────────────────────────────────────────────────
    //  Daily Limit Management
    // ──────────────────────────────────────────────────────────────

    /**
     * @dev Checks whether the requested amount exceeds the daily limit for a token
     *      and updates the rolling volume tracker.
     * @param token Address of the token being transferred.
     * @param amount Amount requested for transfer.
     */
    function _checkAndUpdateDailyVolume(address token, uint256 amount) internal {
        uint256 limit = dailyLimit[token];

        // If no limit is set, skip check
        if (limit == 0) return;

        DailyVolume storage vol = _dailyVolume[token];

        // Reset window if expired
        if (block.timestamp >= vol.windowStart + DAILY_LIMIT_WINDOW) {
            vol.amount = 0;
            vol.windowStart = block.timestamp;
        }

        uint256 remaining = limit - vol.amount;
        if (amount > remaining) {
            revert DailyLimitExceeded(amount, remaining);
        }

        vol.amount += amount;
    }

    /**
     * @notice Returns the remaining daily transfer allowance for a token.
     * @param token Address of the token to query.
     * @return remaining Amount still available within the current daily window.
     */
    function getDailyRemaining(address token) external view returns (uint256 remaining) {
        uint256 limit = dailyLimit[token];
        if (limit == 0) return type(uint256).max;

        DailyVolume storage vol = _dailyVolume[token];

        if (block.timestamp >= vol.windowStart + DAILY_LIMIT_WINDOW) {
            return limit;
        }

        return limit - vol.amount;
    }

    // ──────────────────────────────────────────────────────────────
    //  Admin Functions
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Updates the number of relayer signatures required to authorize an unlock.
     * @dev Must be at least 1 and no more than the total number of relayers conceptually
     *      (enforced loosely; admin is trusted to set reasonable values).
     * @param _threshold New signature threshold.
     */
    function setRelayerThreshold(uint256 _threshold) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (_threshold == 0) revert InvalidThreshold(_threshold);
        uint256 oldThreshold = relayerThreshold;
        relayerThreshold = _threshold;
        emit ThresholdUpdated(oldThreshold, _threshold);
    }

    /**
     * @notice Adds a chain ID to the set of supported destination chains.
     * @param chainId The chain ID to add.
     */
    function addSupportedChain(uint256 chainId) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (supportedChains[chainId]) revert ChainAlreadySupported(chainId);
        supportedChains[chainId] = true;
        emit ChainAdded(chainId);
    }

    /**
     * @notice Removes a chain ID from the set of supported destination chains.
     * @param chainId The chain ID to remove.
     */
    function removeSupportedChain(uint256 chainId) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (!supportedChains[chainId]) revert ChainNotSupported(chainId);
        supportedChains[chainId] = false;
        emit ChainRemoved(chainId);
    }

    /**
     * @notice Sets the daily transfer limit for a specific token.
     * @dev Set to 0 to disable the daily limit for the token.
     * @param token Address of the ERC-20 token.
     * @param limit Maximum amount that can be bridged in a 24-hour window.
     */
    function setDailyLimit(address token, uint256 limit) external onlyRole(DEFAULT_ADMIN_ROLE) {
        dailyLimit[token] = limit;
        emit DailyLimitUpdated(token, limit);
    }

    /**
     * @notice Updates the fee recipient address.
     * @param _feeRecipient New address to receive bridge fees.
     */
    function setFeeRecipient(address _feeRecipient) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (_feeRecipient == address(0)) revert ZeroAddress();
        feeRecipient = _feeRecipient;
    }

    /**
     * @notice Adds a new relayer to the bridge.
     * @param relayer Address of the relayer to add.
     */
    function addRelayer(address relayer) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (relayer == address(0)) revert ZeroAddress();
        _grantRole(RELAYER_ROLE, relayer);
        emit RelayerAdded(relayer);
    }

    /**
     * @notice Removes a relayer from the bridge.
     * @param relayer Address of the relayer to remove.
     */
    function removeRelayer(address relayer) external onlyRole(DEFAULT_ADMIN_ROLE) {
        _revokeRole(RELAYER_ROLE, relayer);
        emit RelayerRemoved(relayer);
    }

    /**
     * @notice Withdraws accumulated bridge fees for a specific token.
     * @param token Address of the ERC-20 token.
     */
    function withdrawFees(address token) external onlyRole(DEFAULT_ADMIN_ROLE) {
        uint256 fees = accumulatedFees[token];
        if (fees == 0) revert ZeroAmount();

        accumulatedFees[token] = 0;
        IERC20(token).safeTransfer(feeRecipient, fees);

        emit FeesWithdrawn(token, feeRecipient, fees);
    }

    // ──────────────────────────────────────────────────────────────
    //  Emergency Functions
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Emergency withdrawal of tokens from the bridge contract.
     * @dev Only callable by DEFAULT_ADMIN_ROLE. Intended for disaster recovery.
     *      Automatically pauses the bridge after withdrawal.
     * @param token Address of the ERC-20 token to withdraw.
     * @param amount Amount of tokens to withdraw.
     */
    function emergencyWithdraw(
        address token,
        uint256 amount
    ) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (amount == 0) revert ZeroAmount();
        if (token == address(0)) revert ZeroAddress();

        IERC20(token).safeTransfer(msg.sender, amount);

        emit EmergencyWithdraw(token, msg.sender, amount);

        // Auto-pause after emergency withdrawal
        if (!paused()) {
            _pause();
        }
    }

    /**
     * @notice Pauses all bridge operations (lock and unlock).
     */
    function pause() external onlyRole(DEFAULT_ADMIN_ROLE) {
        _pause();
    }

    /**
     * @notice Unpauses bridge operations.
     */
    function unpause() external onlyRole(DEFAULT_ADMIN_ROLE) {
        _unpause();
    }
}
