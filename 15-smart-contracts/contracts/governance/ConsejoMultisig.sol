// SPDX-License-Identifier: MIT
// ============================================================================
// CONSEJO MULTISIG — Billetera Multi-firma del Consejo Soberano
// Ierahkwa Ne Kanienke — Nacion Digital Soberana
// Blockchain: MameyNode (EVM-compatible)
// ============================================================================

pragma solidity 0.8.24;

import "@openzeppelin/contracts/utils/ReentrancyGuard.sol";
import "@openzeppelin/contracts/token/ERC20/IERC20.sol";

/**
 * @title ConsejoMultisig
 * @author Ierahkwa Sovereign Development Council
 * @notice Custom N-of-M multi-signature wallet for the Ierahkwa Sovereign
 *         Council (Consejo Soberano). Provides collective custody of sovereign
 *         assets with configurable threshold and timelock protection for
 *         high-value transactions.
 * @dev Standalone multisig wallet without external dependencies beyond
 *      OpenZeppelin's ReentrancyGuard. Default configuration: 5-of-9.
 *
 * Key features:
 *   - N-of-M signature threshold (configurable, default 5-of-9)
 *   - Transaction submission, confirmation, execution, and revocation
 *   - 48-hour timelock for transactions exceeding 1M WMP value
 *   - Owner management (add/remove) only via the multisig itself
 *   - Threshold changes only via the multisig itself
 *   - Native ETH/WMP and ERC-20 token support
 *
 * Transaction Lifecycle:
 *   1. Owner calls submitTransaction() -> returns txId
 *   2. Other owners call confirmTransaction(txId)
 *   3. When threshold reached, any owner calls executeTransaction(txId)
 *   4. For high-value tx: 48h must elapse after threshold is reached
 *   5. Owners can revoke with revokeConfirmation(txId) before execution
 */
contract ConsejoMultisig is ReentrancyGuard {
    // =========================================================================
    // CONSTANTS
    // =========================================================================

    /// @notice Maximum number of council owners.
    uint256 public constant MAX_OWNERS = 20;

    /// @notice Minimum number of council owners.
    uint256 public constant MIN_OWNERS = 3;

    /// @notice Minimum confirmation threshold.
    uint256 public constant MIN_THRESHOLD = 2;

    /// @notice Timelock duration for high-value transactions: 48 hours.
    uint256 public constant HIGH_VALUE_TIMELOCK = 48 hours;

    /// @notice Value threshold that triggers the 48h timelock (1 million WMP).
    uint256 public constant HIGH_VALUE_THRESHOLD = 1_000_000 * 10 ** 18;

    // =========================================================================
    // DATA STRUCTURES
    // =========================================================================

    /**
     * @notice Represents a pending multisig transaction.
     * @param to              Target address for the transaction.
     * @param value           Native currency value to send (in wei).
     * @param data            Encoded function call data (empty for simple transfers).
     * @param executed        True if the transaction has been executed.
     * @param confirmations   Number of confirmations received.
     * @param submittedAt     Block timestamp when the transaction was submitted.
     * @param thresholdMetAt  Block timestamp when the threshold was first reached (0 if not yet met).
     * @param submitter       Address of the owner who submitted the transaction.
     */
    struct Transaction {
        address to;
        uint256 value;
        bytes data;
        bool executed;
        uint256 confirmations;
        uint256 submittedAt;
        uint256 thresholdMetAt;
        address submitter;
    }

    // =========================================================================
    // STATE VARIABLES
    // =========================================================================

    /// @notice Array of all council owner addresses.
    address[] public owners;

    /// @notice Mapping to check if an address is a council owner.
    mapping(address => bool) public isOwner;

    /// @notice Required number of confirmations to execute a transaction.
    uint256 public threshold;

    /// @notice All transactions submitted to the multisig.
    Transaction[] public transactions;

    /// @notice Mapping: txId => owner => has confirmed.
    mapping(uint256 => mapping(address => bool)) public confirmations;

    /// @notice WMP token address (for high-value threshold checks on ERC-20 transfers).
    address public wmpToken;

    // =========================================================================
    // EVENTS
    // =========================================================================

    /// @notice Emitted when a new transaction is submitted.
    event SubmitTransaction(
        uint256 indexed txId,
        address indexed submitter,
        address indexed to,
        uint256 value,
        bytes data,
        uint256 timestamp
    );

    /// @notice Emitted when an owner confirms a transaction.
    event ConfirmTransaction(
        uint256 indexed txId,
        address indexed owner,
        uint256 confirmationCount,
        uint256 timestamp
    );

    /// @notice Emitted when a confirmed transaction is executed.
    event ExecuteTransaction(
        uint256 indexed txId,
        address indexed executor,
        address indexed to,
        uint256 value,
        uint256 timestamp
    );

    /// @notice Emitted when an owner revokes their confirmation.
    event RevokeConfirmation(
        uint256 indexed txId,
        address indexed owner,
        uint256 confirmationCount,
        uint256 timestamp
    );

    /// @notice Emitted when the confirmation threshold changes.
    event ThresholdChanged(
        uint256 oldThreshold,
        uint256 newThreshold,
        uint256 timestamp
    );

    /// @notice Emitted when a new owner is added to the council.
    event OwnerAdded(
        address indexed owner,
        uint256 ownerCount,
        uint256 timestamp
    );

    /// @notice Emitted when an owner is removed from the council.
    event OwnerRemoved(
        address indexed owner,
        uint256 ownerCount,
        uint256 timestamp
    );

    /// @notice Emitted when the WMP token address is configured.
    event WMPTokenSet(address indexed token);

    /// @notice Emitted when the confirmation threshold is met and timelock starts.
    event ThresholdMet(
        uint256 indexed txId,
        uint256 timestamp,
        bool requiresTimelock
    );

    /// @notice Emitted when native currency is received.
    event Deposit(address indexed sender, uint256 amount, uint256 timestamp);

    // =========================================================================
    // ERRORS
    // =========================================================================

    error NotOwner();
    error NotSelf();
    error ZeroAddress();
    error DuplicateOwner(address owner);
    error OwnerNotFound(address owner);
    error InvalidThreshold(uint256 threshold, uint256 ownerCount);
    error TooManyOwners(uint256 count, uint256 maximum);
    error TooFewOwners(uint256 count, uint256 minimum);
    error TransactionDoesNotExist(uint256 txId);
    error TransactionAlreadyExecuted(uint256 txId);
    error TransactionAlreadyConfirmed(uint256 txId, address owner);
    error TransactionNotConfirmed(uint256 txId, address owner);
    error InsufficientConfirmations(uint256 current, uint256 required);
    error TimelockNotExpired(uint256 txId, uint256 unlockTime);
    error TransactionFailed(uint256 txId);

    // =========================================================================
    // MODIFIERS
    // =========================================================================

    /// @dev Restricts function to council owners only.
    modifier onlyOwner() {
        if (!isOwner[msg.sender]) revert NotOwner();
        _;
    }

    /// @dev Restricts function to calls from the multisig itself.
    modifier onlySelf() {
        if (msg.sender != address(this)) revert NotSelf();
        _;
    }

    /// @dev Validates that a transaction ID exists.
    modifier txExists(uint256 _txId) {
        if (_txId >= transactions.length) revert TransactionDoesNotExist(_txId);
        _;
    }

    /// @dev Validates that a transaction has not been executed.
    modifier notExecuted(uint256 _txId) {
        if (transactions[_txId].executed) revert TransactionAlreadyExecuted(_txId);
        _;
    }

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    /**
     * @notice Deploys the ConsejoMultisig with initial owners and threshold.
     * @param _owners    Array of initial council owner addresses.
     * @param _threshold Required number of confirmations (default: 5).
     * @param _wmpToken  Address of the WMP token (for high-value detection).
     */
    constructor(
        address[] memory _owners,
        uint256 _threshold,
        address _wmpToken
    ) {
        uint256 ownerCount = _owners.length;
        if (ownerCount < MIN_OWNERS) revert TooFewOwners(ownerCount, MIN_OWNERS);
        if (ownerCount > MAX_OWNERS) revert TooManyOwners(ownerCount, MAX_OWNERS);
        if (_threshold < MIN_THRESHOLD || _threshold > ownerCount) {
            revert InvalidThreshold(_threshold, ownerCount);
        }

        for (uint256 i = 0; i < ownerCount; i++) {
            address owner = _owners[i];
            if (owner == address(0)) revert ZeroAddress();
            if (isOwner[owner]) revert DuplicateOwner(owner);

            isOwner[owner] = true;
            owners.push(owner);
        }

        threshold = _threshold;
        wmpToken = _wmpToken;
    }

    // =========================================================================
    // TRANSACTION LIFECYCLE
    // =========================================================================

    /**
     * @notice Submit a new transaction for council approval.
     * @dev The submitter's confirmation is automatically recorded.
     * @param _to    Target address.
     * @param _value Native currency value to send (in wei).
     * @param _data  Encoded function call data.
     * @return txId  The transaction identifier.
     */
    function submitTransaction(
        address _to,
        uint256 _value,
        bytes calldata _data
    ) external onlyOwner returns (uint256 txId) {
        if (_to == address(0)) revert ZeroAddress();

        txId = transactions.length;

        transactions.push(Transaction({
            to: _to,
            value: _value,
            data: _data,
            executed: false,
            confirmations: 0,
            submittedAt: block.timestamp,
            thresholdMetAt: 0,
            submitter: msg.sender
        }));

        emit SubmitTransaction(txId, msg.sender, _to, _value, _data, block.timestamp);

        // Auto-confirm for submitter
        _confirm(txId);
    }

    /**
     * @notice Confirm a pending transaction.
     * @param _txId Transaction identifier to confirm.
     */
    function confirmTransaction(
        uint256 _txId
    ) external onlyOwner txExists(_txId) notExecuted(_txId) {
        if (confirmations[_txId][msg.sender]) {
            revert TransactionAlreadyConfirmed(_txId, msg.sender);
        }
        _confirm(_txId);
    }

    /**
     * @dev Internal confirmation logic.
     */
    function _confirm(uint256 _txId) private {
        confirmations[_txId][msg.sender] = true;
        transactions[_txId].confirmations++;

        emit ConfirmTransaction(
            _txId,
            msg.sender,
            transactions[_txId].confirmations,
            block.timestamp
        );

        // Record when threshold is first met
        Transaction storage txn = transactions[_txId];
        if (txn.confirmations >= threshold && txn.thresholdMetAt == 0) {
            txn.thresholdMetAt = block.timestamp;
            bool needsTimelock = _isHighValue(_txId);
            emit ThresholdMet(_txId, block.timestamp, needsTimelock);
        }
    }

    /**
     * @notice Execute a transaction that has reached the confirmation threshold.
     * @dev High-value transactions require a 48h timelock after threshold is met.
     * @param _txId Transaction identifier to execute.
     */
    function executeTransaction(
        uint256 _txId
    ) external onlyOwner txExists(_txId) notExecuted(_txId) nonReentrant {
        Transaction storage txn = transactions[_txId];

        if (txn.confirmations < threshold) {
            revert InsufficientConfirmations(txn.confirmations, threshold);
        }

        // Enforce timelock for high-value transactions
        if (_isHighValue(_txId)) {
            uint256 unlockTime = txn.thresholdMetAt + HIGH_VALUE_TIMELOCK;
            if (block.timestamp < unlockTime) {
                revert TimelockNotExpired(_txId, unlockTime);
            }
        }

        txn.executed = true;

        (bool success, ) = txn.to.call{value: txn.value}(txn.data);
        if (!success) revert TransactionFailed(_txId);

        emit ExecuteTransaction(
            _txId,
            msg.sender,
            txn.to,
            txn.value,
            block.timestamp
        );
    }

    /**
     * @notice Revoke a previously given confirmation.
     * @dev Cannot revoke after execution. Resets thresholdMetAt if count drops below threshold.
     * @param _txId Transaction identifier.
     */
    function revokeConfirmation(
        uint256 _txId
    ) external onlyOwner txExists(_txId) notExecuted(_txId) {
        if (!confirmations[_txId][msg.sender]) {
            revert TransactionNotConfirmed(_txId, msg.sender);
        }

        confirmations[_txId][msg.sender] = false;
        transactions[_txId].confirmations--;

        // Reset threshold met timestamp if we drop below threshold
        if (transactions[_txId].confirmations < threshold) {
            transactions[_txId].thresholdMetAt = 0;
        }

        emit RevokeConfirmation(
            _txId,
            msg.sender,
            transactions[_txId].confirmations,
            block.timestamp
        );
    }

    // =========================================================================
    // OWNER MANAGEMENT (SELF-GOVERNED)
    // =========================================================================

    /**
     * @notice Add a new owner to the council.
     * @dev Can only be called by the multisig itself (via an executed transaction).
     * @param _owner New owner address.
     */
    function addOwner(address _owner) external onlySelf {
        if (_owner == address(0)) revert ZeroAddress();
        if (isOwner[_owner]) revert DuplicateOwner(_owner);
        if (owners.length + 1 > MAX_OWNERS) {
            revert TooManyOwners(owners.length + 1, MAX_OWNERS);
        }

        isOwner[_owner] = true;
        owners.push(_owner);

        emit OwnerAdded(_owner, owners.length, block.timestamp);
    }

    /**
     * @notice Remove an owner from the council.
     * @dev Can only be called by the multisig itself. Automatically adjusts
     *      threshold downward if it would exceed new owner count.
     * @param _owner Owner address to remove.
     */
    function removeOwner(address _owner) external onlySelf {
        if (!isOwner[_owner]) revert OwnerNotFound(_owner);
        if (owners.length - 1 < MIN_OWNERS) {
            revert TooFewOwners(owners.length - 1, MIN_OWNERS);
        }

        isOwner[_owner] = false;

        // Find and remove from array (swap with last element)
        uint256 length = owners.length;
        for (uint256 i = 0; i < length; i++) {
            if (owners[i] == _owner) {
                owners[i] = owners[length - 1];
                owners.pop();
                break;
            }
        }

        // Adjust threshold if necessary
        if (threshold > owners.length) {
            uint256 oldThreshold = threshold;
            threshold = owners.length;
            emit ThresholdChanged(oldThreshold, threshold, block.timestamp);
        }

        emit OwnerRemoved(_owner, owners.length, block.timestamp);
    }

    /**
     * @notice Change the confirmation threshold.
     * @dev Can only be called by the multisig itself.
     * @param _threshold New threshold value.
     */
    function changeThreshold(uint256 _threshold) external onlySelf {
        if (_threshold < MIN_THRESHOLD || _threshold > owners.length) {
            revert InvalidThreshold(_threshold, owners.length);
        }

        uint256 oldThreshold = threshold;
        threshold = _threshold;

        emit ThresholdChanged(oldThreshold, _threshold, block.timestamp);
    }

    /**
     * @notice Set the WMP token address (for high-value detection on ERC-20 transfers).
     * @dev Can only be called by the multisig itself.
     * @param _wmpToken New WMP token address.
     */
    function setWMPToken(address _wmpToken) external onlySelf {
        wmpToken = _wmpToken;
        emit WMPTokenSet(_wmpToken);
    }

    // =========================================================================
    // INTERNAL FUNCTIONS
    // =========================================================================

    /**
     * @dev Determines if a transaction is high-value and requires the 48h timelock.
     *      A transaction is high-value if:
     *        - Its native value exceeds HIGH_VALUE_THRESHOLD, OR
     *        - It's an ERC-20 transfer/approve of WMP tokens exceeding the threshold
     * @param _txId Transaction identifier.
     * @return True if the transaction is high-value.
     */
    function _isHighValue(uint256 _txId) internal view returns (bool) {
        Transaction storage txn = transactions[_txId];

        // Check native value
        if (txn.value >= HIGH_VALUE_THRESHOLD) {
            return true;
        }

        // Check for WMP ERC-20 transfer or approve
        if (wmpToken != address(0) && txn.to == wmpToken && txn.data.length >= 68) {
            bytes4 selector = bytes4(txn.data[:4]);

            // transfer(address,uint256) = 0xa9059cbb
            // approve(address,uint256)  = 0x095ea7b3
            if (
                selector == IERC20.transfer.selector ||
                selector == IERC20.approve.selector
            ) {
                // Decode amount from the second parameter (offset 36-68)
                uint256 amount = abi.decode(txn.data[36:68], (uint256));
                if (amount >= HIGH_VALUE_THRESHOLD) {
                    return true;
                }
            }
        }

        return false;
    }

    // =========================================================================
    // VIEW FUNCTIONS
    // =========================================================================

    /**
     * @notice Get the total number of transactions submitted.
     * @return The transaction count.
     */
    function getTransactionCount() external view returns (uint256) {
        return transactions.length;
    }

    /**
     * @notice Get all council owners.
     * @return Array of owner addresses.
     */
    function getOwners() external view returns (address[] memory) {
        return owners;
    }

    /**
     * @notice Get the number of council owners.
     * @return The owner count.
     */
    function getOwnerCount() external view returns (uint256) {
        return owners.length;
    }

    /**
     * @notice Get full transaction details.
     * @param _txId Transaction identifier.
     * @return to             Target address.
     * @return value          Native value.
     * @return data           Call data.
     * @return executed       Whether executed.
     * @return numConfirmations Number of confirmations.
     * @return submittedAt    Submission timestamp.
     * @return thresholdMetAt Threshold met timestamp (0 if not yet met).
     * @return submitter      Submitter address.
     */
    function getTransaction(uint256 _txId)
        external
        view
        txExists(_txId)
        returns (
            address to,
            uint256 value,
            bytes memory data,
            bool executed,
            uint256 numConfirmations,
            uint256 submittedAt,
            uint256 thresholdMetAt,
            address submitter
        )
    {
        Transaction storage txn = transactions[_txId];
        return (
            txn.to,
            txn.value,
            txn.data,
            txn.executed,
            txn.confirmations,
            txn.submittedAt,
            txn.thresholdMetAt,
            txn.submitter
        );
    }

    /**
     * @notice Check if a specific owner has confirmed a transaction.
     * @param _txId  Transaction identifier.
     * @param _owner Owner address to check.
     * @return True if the owner has confirmed.
     */
    function isConfirmed(
        uint256 _txId,
        address _owner
    ) external view txExists(_txId) returns (bool) {
        return confirmations[_txId][_owner];
    }

    /**
     * @notice Check if a transaction has reached the confirmation threshold.
     * @param _txId Transaction identifier.
     * @return True if confirmations >= threshold.
     */
    function isThresholdMet(uint256 _txId) external view txExists(_txId) returns (bool) {
        return transactions[_txId].confirmations >= threshold;
    }

    /**
     * @notice Check if a high-value transaction's timelock has expired.
     * @param _txId Transaction identifier.
     * @return ready     True if the transaction can be executed.
     * @return unlockTime Unix timestamp when the timelock expires (0 if no timelock).
     */
    function timelockStatus(uint256 _txId)
        external
        view
        txExists(_txId)
        returns (bool ready, uint256 unlockTime)
    {
        Transaction storage txn = transactions[_txId];

        if (txn.executed) return (false, 0);
        if (txn.confirmations < threshold) return (false, 0);

        if (_isHighValue(_txId)) {
            unlockTime = txn.thresholdMetAt + HIGH_VALUE_TIMELOCK;
            ready = block.timestamp >= unlockTime;
        } else {
            ready = true;
            unlockTime = 0;
        }
    }

    /**
     * @notice Get pending (unexecuted) transaction IDs.
     * @return ids Array of pending transaction IDs.
     */
    function getPendingTransactions() external view returns (uint256[] memory) {
        uint256 total = transactions.length;
        uint256 count = 0;

        // Count pending
        for (uint256 i = 0; i < total; i++) {
            if (!transactions[i].executed) count++;
        }

        // Collect pending IDs
        uint256[] memory ids = new uint256[](count);
        uint256 idx = 0;
        for (uint256 i = 0; i < total; i++) {
            if (!transactions[i].executed) {
                ids[idx] = i;
                idx++;
            }
        }

        return ids;
    }

    // =========================================================================
    // RECEIVE
    // =========================================================================

    /// @dev Accept native currency deposits.
    receive() external payable {
        emit Deposit(msg.sender, msg.value, block.timestamp);
    }

    /// @dev Fallback for calls with data that don't match any function.
    fallback() external payable {
        if (msg.value > 0) {
            emit Deposit(msg.sender, msg.value, block.timestamp);
        }
    }
}
