// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/security/Pausable.sol";

/**
 * @title IerahkwaDestruct — Red Button Protocol
 * @dev Kill-switch contract requiring 51% Guardian consensus with 72-hour cooldown.
 *      Protects the sovereign ecosystem with collective decision-making.
 *      Gobierno Soberano de Ierahkwa Ne Kanienke
 */

interface IIerahkwaManifesto {
    function hasSigned(address signer) external view returns (bool);
    function totalSigners() external view returns (uint256);
}

interface IPausable {
    function pause() external;
}

contract IerahkwaDestruct is Ownable {

    // ─── State ───────────────────────────────────────────────────────────

    IIerahkwaManifesto public manifesto;

    address[] public linkedContracts;

    uint256 public votesForDestruction;
    uint256 public thresholdReachedAt;
    bool public destructionPending;
    bool public destructionExecuted;

    mapping(address => bool) public hasVoted;
    address[] public voters;

    uint256 public lastResetTimestamp;

    // ─── Constants ───────────────────────────────────────────────────────

    uint256 public constant COOLDOWN_PERIOD = 72 hours;
    uint256 public constant RESET_PERIOD = 30 days;
    uint256 public constant CONSENSUS_BPS = 5100; // 51% in basis points

    // ─── Events ──────────────────────────────────────────────────────────

    event DestructionVoteCast(
        address indexed guardian,
        uint256 currentVotes,
        uint256 requiredVotes,
        uint256 timestamp
    );

    event DestructionThresholdReached(
        uint256 votes,
        uint256 totalGuardians,
        uint256 executionAvailableAt,
        uint256 timestamp
    );

    event DestructionExecuted(
        uint256 totalVotes,
        uint256 contractsPaused,
        uint256 timestamp
    );

    event DestructionCancelled(
        address indexed cancelledBy,
        uint256 votesAtCancellation,
        uint256 timestamp
    );

    event LinkedContractAdded(address indexed contractAddress);
    event LinkedContractRemoved(address indexed contractAddress);
    event VotesReset(address indexed resetBy, uint256 timestamp);

    // ─── Modifiers ───────────────────────────────────────────────────────

    modifier onlyGuardian() {
        require(
            manifesto.hasSigned(msg.sender),
            "Destruct: caller is not a Guardian (must sign Manifesto)"
        );
        _;
    }

    modifier notExecuted() {
        require(!destructionExecuted, "Destruct: destruction already executed");
        _;
    }

    // ─── Constructor ─────────────────────────────────────────────────────

    /**
     * @param _manifesto Address of the IerahkwaManifesto contract
     * @param _linkedContracts Initial array of contracts to pause on execution
     */
    constructor(
        address _manifesto,
        address[] memory _linkedContracts
    ) Ownable(msg.sender) {
        require(_manifesto != address(0), "Destruct: manifesto is zero address");
        manifesto = IIerahkwaManifesto(_manifesto);

        for (uint256 i = 0; i < _linkedContracts.length; i++) {
            require(_linkedContracts[i] != address(0), "Destruct: linked contract is zero address");
            linkedContracts.push(_linkedContracts[i]);
            emit LinkedContractAdded(_linkedContracts[i]);
        }

        lastResetTimestamp = block.timestamp;
    }

    // ─── Core: Trigger Red Button ────────────────────────────────────────

    /**
     * @dev Cast a vote for ecosystem destruction/pause.
     *      Only Guardians (verified via Manifesto SBT) can vote.
     *      When 51% threshold is reached, a 72-hour cooldown begins.
     */
    function triggerRedButton() external onlyGuardian notExecuted {
        require(!hasVoted[msg.sender], "Destruct: already voted");
        require(!destructionPending, "Destruct: threshold already reached, awaiting cooldown");

        hasVoted[msg.sender] = true;
        voters.push(msg.sender);
        votesForDestruction++;

        uint256 totalGuardians = manifesto.totalSigners();
        uint256 requiredVotes = _calculateRequired(totalGuardians);

        emit DestructionVoteCast(
            msg.sender,
            votesForDestruction,
            requiredVotes,
            block.timestamp
        );

        // Check if threshold reached
        if (votesForDestruction >= requiredVotes) {
            destructionPending = true;
            thresholdReachedAt = block.timestamp;

            emit DestructionThresholdReached(
                votesForDestruction,
                totalGuardians,
                block.timestamp + COOLDOWN_PERIOD,
                block.timestamp
            );
        }
    }

    // ─── Cancel Destruction ──────────────────────────────────────────────

    /**
     * @dev Any Guardian can cancel the destruction during the 72-hour cooldown.
     *      Resets all votes and the pending state.
     */
    function cancelDestruction() external onlyGuardian notExecuted {
        require(destructionPending, "Destruct: no pending destruction to cancel");

        uint256 votesAtCancel = votesForDestruction;

        _resetVotingState();

        emit DestructionCancelled(msg.sender, votesAtCancel, block.timestamp);
    }

    // ─── Execute Destruction ─────────────────────────────────────────────

    /**
     * @dev Execute the destruction after the 72-hour cooldown has elapsed.
     *      Pauses all linked contracts.
     */
    function executeDestruction() external onlyGuardian notExecuted {
        require(destructionPending, "Destruct: threshold not reached");
        require(
            block.timestamp >= thresholdReachedAt + COOLDOWN_PERIOD,
            "Destruct: 72-hour cooldown not elapsed"
        );

        destructionExecuted = true;
        uint256 contractsPaused = 0;

        for (uint256 i = 0; i < linkedContracts.length; i++) {
            // Attempt to pause each linked contract; do not revert on failure
            (bool success, ) = linkedContracts[i].call(
                abi.encodeWithSignature("pause()")
            );
            if (success) {
                contractsPaused++;
            }
        }

        emit DestructionExecuted(
            votesForDestruction,
            contractsPaused,
            block.timestamp
        );
    }

    // ─── Governance: Reset Votes ─────────────────────────────────────────

    /**
     * @dev Governance can reset votes after 30 days if destruction was not executed.
     *      Allows the system to recover from a stalled vote.
     */
    function resetVotes() external onlyOwner notExecuted {
        require(
            block.timestamp >= lastResetTimestamp + RESET_PERIOD,
            "Destruct: must wait 30 days between resets"
        );

        _resetVotingState();
        lastResetTimestamp = block.timestamp;

        emit VotesReset(msg.sender, block.timestamp);
    }

    // ─── Linked Contract Management ──────────────────────────────────────

    /**
     * @dev Add a contract to the list of contracts paused on destruction.
     */
    function addLinkedContract(address _contract) external onlyOwner notExecuted {
        require(_contract != address(0), "Destruct: zero address");
        linkedContracts.push(_contract);
        emit LinkedContractAdded(_contract);
    }

    /**
     * @dev Remove a contract from the linked list by index.
     */
    function removeLinkedContract(uint256 index) external onlyOwner notExecuted {
        require(index < linkedContracts.length, "Destruct: index out of bounds");
        address removed = linkedContracts[index];
        linkedContracts[index] = linkedContracts[linkedContracts.length - 1];
        linkedContracts.pop();
        emit LinkedContractRemoved(removed);
    }

    // ─── View Functions ──────────────────────────────────────────────────

    /**
     * @dev Returns the number of votes required for 51% consensus.
     */
    function requiredVotes() external view returns (uint256) {
        return _calculateRequired(manifesto.totalSigners());
    }

    /**
     * @dev Returns the current voting status.
     */
    function getDestructionStatus() external view returns (
        uint256 currentVotes,
        uint256 required,
        bool pending,
        bool executed,
        uint256 cooldownEndsAt,
        uint256 totalLinkedContracts
    ) {
        uint256 req = _calculateRequired(manifesto.totalSigners());
        uint256 cooldownEnd = destructionPending
            ? thresholdReachedAt + COOLDOWN_PERIOD
            : 0;

        return (
            votesForDestruction,
            req,
            destructionPending,
            destructionExecuted,
            cooldownEnd,
            linkedContracts.length
        );
    }

    /**
     * @dev Returns the list of addresses that have voted.
     */
    function getVoters() external view returns (address[] memory) {
        return voters;
    }

    /**
     * @dev Returns the list of linked contracts.
     */
    function getLinkedContracts() external view returns (address[] memory) {
        return linkedContracts;
    }

    /**
     * @dev Returns seconds remaining in cooldown. 0 if not pending or already elapsed.
     */
    function getCooldownRemaining() external view returns (uint256) {
        if (!destructionPending) return 0;
        uint256 endTime = thresholdReachedAt + COOLDOWN_PERIOD;
        if (block.timestamp >= endTime) return 0;
        return endTime - block.timestamp;
    }

    // ─── Internal ────────────────────────────────────────────────────────

    /**
     * @dev Calculate 51% of total guardians (rounded up).
     */
    function _calculateRequired(uint256 total) internal pure returns (uint256) {
        if (total == 0) return 1;
        return (total * CONSENSUS_BPS + 9999) / 10000;
    }

    /**
     * @dev Reset all voting state (votes, mappings, pending flag).
     */
    function _resetVotingState() internal {
        for (uint256 i = 0; i < voters.length; i++) {
            hasVoted[voters[i]] = false;
        }
        delete voters;
        votesForDestruction = 0;
        destructionPending = false;
        thresholdReachedAt = 0;
    }
}
