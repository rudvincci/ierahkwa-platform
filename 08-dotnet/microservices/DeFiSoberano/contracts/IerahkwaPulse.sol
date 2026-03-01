// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "@openzeppelin/contracts/access/Ownable.sol";

/**
 * @title IerahkwaPulse — Human Heartbeat Protocol
 * @dev Dead-man's switch ensuring human oversight of the sovereign ecosystem.
 *      Requires minimum 3 of 5 Lead Guardians to send a pulse within 30 days.
 *      If the heartbeat flatlines, linked contracts can be paused.
 *      Gobierno Soberano de Ierahkwa Ne Kanienke
 */

interface IPausableContract {
    function pause() external;
    function unpause() external;
}

contract IerahkwaPulse is Ownable {

    // ─── Constants ───────────────────────────────────────────────────────

    uint256 public constant TIMEOUT = 30 days;
    uint256 public constant MIN_PULSES_REQUIRED = 3;
    uint256 public constant MAX_LEAD_GUARDIANS = 5;

    // ─── State ───────────────────────────────────────────────────────────

    address[5] public leadGuardians;
    mapping(address => bool) public isLeadGuardian;
    mapping(address => uint256) public lastPulseTimestamp;

    uint256 public lastSystemPulse;
    uint256 public pulsesThisCycle;
    bool public flatlined;

    address[] public linkedContracts; // IerahkwaTreasury, IerahkwaReputation, etc.

    // Track which guardians pulsed in current cycle
    mapping(address => bool) public pulsedThisCycle;

    // ─── Events ──────────────────────────────────────────────────────────

    event HeartbeatSent(
        address indexed guardian,
        uint256 pulsesThisCycle,
        uint256 pulsesRequired,
        uint256 timestamp
    );

    event FlatlineDetected(
        uint256 lastPulseTime,
        uint256 detectedAt,
        uint256 contractsPaused
    );

    event SystemRecovered(
        uint256 recoveredAt,
        uint256 pulsesReceived
    );

    event LeadGuardianUpdated(
        uint256 indexed slot,
        address indexed oldGuardian,
        address indexed newGuardian,
        uint256 timestamp
    );

    event LinkedContractAdded(address indexed contractAddress);
    event LinkedContractRemoved(address indexed contractAddress);

    // ─── Modifiers ───────────────────────────────────────────────────────

    modifier onlyLeadGuardian() {
        require(isLeadGuardian[msg.sender], "Pulse: caller is not a Lead Guardian");
        _;
    }

    // ─── Constructor ─────────────────────────────────────────────────────

    /**
     * @param _leadGuardians Array of 5 lead guardian addresses.
     */
    constructor(address[5] memory _leadGuardians) Ownable(msg.sender) {
        for (uint256 i = 0; i < MAX_LEAD_GUARDIANS; i++) {
            require(
                _leadGuardians[i] != address(0),
                "Pulse: lead guardian cannot be zero address"
            );

            // Check for duplicates
            for (uint256 j = 0; j < i; j++) {
                require(
                    _leadGuardians[i] != _leadGuardians[j],
                    "Pulse: duplicate lead guardian"
                );
            }

            leadGuardians[i] = _leadGuardians[i];
            isLeadGuardian[_leadGuardians[i]] = true;
        }

        lastSystemPulse = block.timestamp;
    }

    // ─── Core: Send Pulse ────────────────────────────────────────────────

    /**
     * @dev Lead Guardian sends a heartbeat pulse.
     *      Requires minimum 3 of 5 within each 30-day cycle.
     *      If the system was flatlined and enough pulses arrive, it recovers.
     */
    function sendPulse() external onlyLeadGuardian {
        // Check if a new cycle should start
        if (block.timestamp > lastSystemPulse + TIMEOUT) {
            // Cycle expired — check if flatline
            if (!flatlined && pulsesThisCycle < MIN_PULSES_REQUIRED) {
                _triggerFlatline();
            }
            // Start new cycle regardless
            _resetCycle();
        }

        require(!pulsedThisCycle[msg.sender], "Pulse: already pulsed this cycle");

        pulsedThisCycle[msg.sender] = true;
        lastPulseTimestamp[msg.sender] = block.timestamp;
        pulsesThisCycle++;

        emit HeartbeatSent(
            msg.sender,
            pulsesThisCycle,
            MIN_PULSES_REQUIRED,
            block.timestamp
        );

        // Check if minimum threshold met — keep system alive
        if (pulsesThisCycle >= MIN_PULSES_REQUIRED) {
            lastSystemPulse = block.timestamp;

            // If previously flatlined, recover
            if (flatlined) {
                flatlined = false;
                _recoverLinkedContracts();

                emit SystemRecovered(block.timestamp, pulsesThisCycle);
            }
        }
    }

    // ─── View Functions ──────────────────────────────────────────────────

    /**
     * @dev Returns true if the system is alive (heartbeat within TIMEOUT).
     */
    function isSystemAlive() external view returns (bool) {
        if (flatlined) return false;
        return block.timestamp <= lastSystemPulse + TIMEOUT;
    }

    /**
     * @dev Returns seconds remaining before flatline. 0 if already flatlined or expired.
     */
    function getTimeUntilFlatline() external view returns (uint256) {
        if (flatlined) return 0;
        uint256 deadline = lastSystemPulse + TIMEOUT;
        if (block.timestamp >= deadline) return 0;
        return deadline - block.timestamp;
    }

    /**
     * @dev Returns the pulse status for all lead guardians.
     */
    function getPulseStatus() external view returns (
        address[5] memory guardians,
        uint256[5] memory lastPulses,
        bool[5] memory pulsedCurrent,
        uint256 currentPulses,
        uint256 required,
        bool alive,
        bool isFlatlined,
        uint256 cycleDeadline
    ) {
        for (uint256 i = 0; i < MAX_LEAD_GUARDIANS; i++) {
            guardians[i] = leadGuardians[i];
            lastPulses[i] = lastPulseTimestamp[leadGuardians[i]];
            pulsedCurrent[i] = pulsedThisCycle[leadGuardians[i]];
        }

        bool systemAlive = !flatlined && block.timestamp <= lastSystemPulse + TIMEOUT;

        return (
            guardians,
            lastPulses,
            pulsedCurrent,
            pulsesThisCycle,
            MIN_PULSES_REQUIRED,
            systemAlive,
            flatlined,
            lastSystemPulse + TIMEOUT
        );
    }

    // ─── Check Flatline (callable by anyone) ─────────────────────────────

    /**
     * @dev Anyone can call this to check and trigger flatline if timeout expired.
     *      Incentivizes external monitoring.
     */
    function checkFlatline() external {
        require(!flatlined, "Pulse: already flatlined");
        require(
            block.timestamp > lastSystemPulse + TIMEOUT,
            "Pulse: system still alive"
        );
        require(
            pulsesThisCycle < MIN_PULSES_REQUIRED,
            "Pulse: sufficient pulses received"
        );

        _triggerFlatline();
    }

    // ─── Governance: Update Lead Guardians ───────────────────────────────

    /**
     * @dev Replace a lead guardian at a specific slot (0-4).
     *      Governance-only operation.
     */
    function updateLeadGuardian(uint256 slot, address newGuardian) external onlyOwner {
        require(slot < MAX_LEAD_GUARDIANS, "Pulse: invalid slot (0-4)");
        require(newGuardian != address(0), "Pulse: zero address");
        require(!isLeadGuardian[newGuardian], "Pulse: already a lead guardian");

        address oldGuardian = leadGuardians[slot];

        isLeadGuardian[oldGuardian] = false;
        pulsedThisCycle[oldGuardian] = false;

        leadGuardians[slot] = newGuardian;
        isLeadGuardian[newGuardian] = true;

        emit LeadGuardianUpdated(slot, oldGuardian, newGuardian, block.timestamp);
    }

    /**
     * @dev Batch update all 5 lead guardians. Governance-only.
     */
    function updateLeadGuardians(address[5] calldata newGuardians) external onlyOwner {
        // Validate no duplicates and no zero addresses
        for (uint256 i = 0; i < MAX_LEAD_GUARDIANS; i++) {
            require(newGuardians[i] != address(0), "Pulse: zero address");
            for (uint256 j = 0; j < i; j++) {
                require(newGuardians[i] != newGuardians[j], "Pulse: duplicate guardian");
            }
        }

        // Remove old guardians
        for (uint256 i = 0; i < MAX_LEAD_GUARDIANS; i++) {
            address old = leadGuardians[i];
            isLeadGuardian[old] = false;
            pulsedThisCycle[old] = false;
        }

        // Set new guardians
        for (uint256 i = 0; i < MAX_LEAD_GUARDIANS; i++) {
            leadGuardians[i] = newGuardians[i];
            isLeadGuardian[newGuardians[i]] = true;

            emit LeadGuardianUpdated(i, address(0), newGuardians[i], block.timestamp);
        }

        // Reset cycle with new guardians
        _resetCycle();
        lastSystemPulse = block.timestamp;
    }

    // ─── Linked Contract Management ──────────────────────────────────────

    /**
     * @dev Add a contract to pause/unpause on flatline/recovery.
     *      Intended for IerahkwaTreasury, IerahkwaReputation, etc.
     */
    function addLinkedContract(address _contract) external onlyOwner {
        require(_contract != address(0), "Pulse: zero address");
        linkedContracts.push(_contract);
        emit LinkedContractAdded(_contract);
    }

    /**
     * @dev Remove a linked contract by index.
     */
    function removeLinkedContract(uint256 index) external onlyOwner {
        require(index < linkedContracts.length, "Pulse: index out of bounds");
        address removed = linkedContracts[index];
        linkedContracts[index] = linkedContracts[linkedContracts.length - 1];
        linkedContracts.pop();
        emit LinkedContractRemoved(removed);
    }

    /**
     * @dev Returns all linked contract addresses.
     */
    function getLinkedContracts() external view returns (address[] memory) {
        return linkedContracts;
    }

    // ─── Internal ────────────────────────────────────────────────────────

    /**
     * @dev Trigger flatline: pause all linked contracts.
     */
    function _triggerFlatline() internal {
        flatlined = true;
        uint256 paused = 0;

        for (uint256 i = 0; i < linkedContracts.length; i++) {
            (bool success, ) = linkedContracts[i].call(
                abi.encodeWithSignature("pause()")
            );
            if (success) paused++;
        }

        emit FlatlineDetected(lastSystemPulse, block.timestamp, paused);
    }

    /**
     * @dev Recover from flatline: unpause all linked contracts.
     */
    function _recoverLinkedContracts() internal {
        for (uint256 i = 0; i < linkedContracts.length; i++) {
            (bool success, ) = linkedContracts[i].call(
                abi.encodeWithSignature("unpause()")
            );
            // Silently continue if a contract fails to unpause
        }
    }

    /**
     * @dev Reset cycle tracking for a new 30-day period.
     */
    function _resetCycle() internal {
        for (uint256 i = 0; i < MAX_LEAD_GUARDIANS; i++) {
            pulsedThisCycle[leadGuardians[i]] = false;
        }
        pulsesThisCycle = 0;
    }
}
