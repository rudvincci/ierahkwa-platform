// SPDX-License-Identifier: MIT
// ============================================================================
// SOVEREIGN TREASURY — Tesoro Soberano con Timelock
// Ierahkwa Ne Kanienke — Nacion Digital Soberana
// Blockchain: MameyNode (EVM-compatible)
// ============================================================================

pragma solidity 0.8.24;

import "@openzeppelin/contracts/governance/TimelockController.sol";
import "@openzeppelin/contracts/token/ERC20/IERC20.sol";
import "@openzeppelin/contracts/token/ERC20/utils/SafeERC20.sol";

/**
 * @title Treasury
 * @author Ierahkwa Sovereign Development Council
 * @notice Sovereign Treasury with dual-timelock architecture. Manages the
 *         sovereign nation's funds with a standard 48-hour delay for normal
 *         operations and a 6-hour emergency delay for critical actions.
 * @dev Extends OpenZeppelin v5.x TimelockController as the primary treasury.
 *      The emergency timelock is a separate TimelockController instance with
 *      shorter delay, requiring EMERGENCY_ROLE for scheduling.
 *
 * Architecture:
 *   - Primary treasury: TimelockController with 48h minimum delay
 *   - Emergency treasury: Separate TimelockController with 6h delay
 *   - Governance contract holds PROPOSER_ROLE on the primary treasury
 *   - Multisig holds EXECUTOR_ROLE on both treasuries
 *   - Emergency council holds PROPOSER_ROLE on emergency treasury
 *
 * Roles (inherited from TimelockController):
 *   - PROPOSER_ROLE: Can schedule operations (governance contract)
 *   - EXECUTOR_ROLE: Can execute ready operations (multisig)
 *   - CANCELLER_ROLE: Can cancel pending operations
 *
 * Custom Roles:
 *   - EMERGENCY_ROLE: Can schedule on the emergency timelock
 */
contract Treasury is TimelockController {
    using SafeERC20 for IERC20;

    // =========================================================================
    // CONSTANTS
    // =========================================================================

    /// @notice Normal operation minimum delay: 48 hours.
    uint256 public constant NORMAL_DELAY = 48 hours;

    /// @notice Emergency operation minimum delay: 6 hours.
    uint256 public constant EMERGENCY_DELAY = 6 hours;

    /// @notice Role for emergency operations on the emergency timelock.
    bytes32 public constant EMERGENCY_ROLE = keccak256("EMERGENCY_ROLE");

    // =========================================================================
    // STATE VARIABLES
    // =========================================================================

    /// @notice Reference to the emergency timelock controller.
    TimelockController public emergencyTimelock;

    /// @notice Total native currency (WMP) received by the treasury.
    uint256 public totalReceived;

    /// @notice Total native currency disbursed from the treasury.
    uint256 public totalDisbursed;

    /// @notice Mapping of approved ERC-20 tokens for treasury management.
    mapping(address => bool) public approvedTokens;

    /// @notice Count of operations scheduled on this treasury.
    uint256 public operationCount;

    // =========================================================================
    // EVENTS
    // =========================================================================

    /// @notice Emitted when native currency is received.
    event FundsReceived(address indexed sender, uint256 amount, uint256 timestamp);

    /// @notice Emitted when the emergency timelock reference is set.
    event EmergencyTimelockSet(address indexed emergencyTimelock);

    /// @notice Emitted when a token is approved or unapproved for treasury use.
    event TokenApprovalUpdated(address indexed token, bool approved);

    /// @notice Emitted when an emergency operation is scheduled.
    event EmergencyOperationScheduled(
        bytes32 indexed operationId,
        address indexed scheduler,
        uint256 timestamp
    );

    // =========================================================================
    // ERRORS
    // =========================================================================

    /// @notice Thrown when the zero address is provided.
    error ZeroAddress();

    /// @notice Thrown when emergency timelock is not configured.
    error EmergencyTimelockNotSet();

    /// @notice Thrown when caller lacks the emergency role.
    error NotEmergencyRole();

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    /**
     * @notice Deploys the Treasury with a 48-hour minimum delay.
     * @param proposers  Addresses with PROPOSER_ROLE (typically governance contract).
     * @param executors  Addresses with EXECUTOR_ROLE (typically multisig).
     * @param admin      Address that receives DEFAULT_ADMIN_ROLE for initial setup.
     */
    constructor(
        address[] memory proposers,
        address[] memory executors,
        address admin
    )
        TimelockController(NORMAL_DELAY, proposers, executors, admin)
    {
        // Grant EMERGENCY_ROLE to admin for initial setup
        // Admin should later grant this to the emergency council and renounce
        _grantRole(EMERGENCY_ROLE, admin);
    }

    // =========================================================================
    // EMERGENCY TIMELOCK CONFIGURATION
    // =========================================================================

    /**
     * @notice Set the emergency timelock controller reference.
     * @dev Only callable by admin. Should be called once after deploying
     *      the emergency TimelockController separately.
     * @param _emergencyTimelock Address of the emergency TimelockController.
     */
    function setEmergencyTimelock(
        address payable _emergencyTimelock
    ) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (_emergencyTimelock == address(0)) revert ZeroAddress();
        emergencyTimelock = TimelockController(_emergencyTimelock);
        emit EmergencyTimelockSet(_emergencyTimelock);
    }

    // =========================================================================
    // EMERGENCY SCHEDULING
    // =========================================================================

    /**
     * @notice Schedule an emergency operation on the emergency timelock.
     * @dev Requires EMERGENCY_ROLE. The operation will execute after the
     *      emergency timelock's minimum delay (6 hours).
     * @param target       Target contract address.
     * @param value        ETH value to send.
     * @param data         Encoded function call data.
     * @param predecessor  Required predecessor operation (bytes32(0) for none).
     * @param salt         Salt for unique operation ID generation.
     */
    function scheduleEmergency(
        address target,
        uint256 value,
        bytes calldata data,
        bytes32 predecessor,
        bytes32 salt
    ) external onlyRole(EMERGENCY_ROLE) {
        if (address(emergencyTimelock) == address(0)) {
            revert EmergencyTimelockNotSet();
        }

        bytes32 operationId = emergencyTimelock.hashOperation(
            target, value, data, predecessor, salt
        );

        emergencyTimelock.schedule(
            target,
            value,
            data,
            predecessor,
            salt,
            EMERGENCY_DELAY
        );

        operationCount++;

        emit EmergencyOperationScheduled(operationId, msg.sender, block.timestamp);
    }

    /**
     * @notice Schedule a batch of emergency operations.
     * @param targets      Target contract addresses.
     * @param values       ETH values for each call.
     * @param payloads     Encoded function call data for each call.
     * @param predecessor  Required predecessor operation.
     * @param salt         Salt for unique operation ID generation.
     */
    function scheduleEmergencyBatch(
        address[] calldata targets,
        uint256[] calldata values,
        bytes[] calldata payloads,
        bytes32 predecessor,
        bytes32 salt
    ) external onlyRole(EMERGENCY_ROLE) {
        if (address(emergencyTimelock) == address(0)) {
            revert EmergencyTimelockNotSet();
        }

        bytes32 operationId = emergencyTimelock.hashOperationBatch(
            targets, values, payloads, predecessor, salt
        );

        emergencyTimelock.scheduleBatch(
            targets,
            values,
            payloads,
            predecessor,
            salt,
            EMERGENCY_DELAY
        );

        operationCount++;

        emit EmergencyOperationScheduled(operationId, msg.sender, block.timestamp);
    }

    // =========================================================================
    // TOKEN MANAGEMENT
    // =========================================================================

    /**
     * @notice Approve or unapprove an ERC-20 token for treasury tracking.
     * @param token    Address of the ERC-20 token.
     * @param approved True to approve, false to unapprove.
     */
    function setTokenApproval(
        address token,
        bool approved
    ) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (token == address(0)) revert ZeroAddress();
        approvedTokens[token] = approved;
        emit TokenApprovalUpdated(token, approved);
    }

    // =========================================================================
    // VIEW FUNCTIONS
    // =========================================================================

    /**
     * @notice Get the native currency balance of the treasury.
     * @return The balance in wei.
     */
    function balance() external view returns (uint256) {
        return address(this).balance;
    }

    /**
     * @notice Get the ERC-20 token balance of the treasury.
     * @param token Address of the ERC-20 token.
     * @return The token balance.
     */
    function tokenBalance(address token) external view returns (uint256) {
        return IERC20(token).balanceOf(address(this));
    }

    /**
     * @notice Get the minimum delay for the emergency timelock.
     * @return The emergency minimum delay in seconds.
     */
    function emergencyMinDelay() external view returns (uint256) {
        if (address(emergencyTimelock) == address(0)) return 0;
        return emergencyTimelock.getMinDelay();
    }

    /**
     * @notice Check if an emergency operation is pending.
     * @param operationId The operation hash.
     * @return True if the operation is pending on the emergency timelock.
     */
    function isEmergencyOperationPending(
        bytes32 operationId
    ) external view returns (bool) {
        if (address(emergencyTimelock) == address(0)) return false;
        return emergencyTimelock.isOperationPending(operationId);
    }

    /**
     * @notice Check if an emergency operation is ready for execution.
     * @param operationId The operation hash.
     * @return True if the operation is ready on the emergency timelock.
     */
    function isEmergencyOperationReady(
        bytes32 operationId
    ) external view returns (bool) {
        if (address(emergencyTimelock) == address(0)) return false;
        return emergencyTimelock.isOperationReady(operationId);
    }

    // =========================================================================
    // RECEIVE
    // =========================================================================

    /**
     * @dev Log all native currency received by the treasury.
     *      The parent TimelockController already has a receive() function,
     *      so we track via the fallback behavior.
     */
    receive() external payable override {
        totalReceived += msg.value;
        emit FundsReceived(msg.sender, msg.value, block.timestamp);
    }
}
