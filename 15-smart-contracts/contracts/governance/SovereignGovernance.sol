// SPDX-License-Identifier: MIT
// ============================================================================
// SOVEREIGN GOVERNANCE — Gobernanza Soberana DAO
// Ierahkwa Ne Kanienke — Nacion Digital Soberana
// Blockchain: MameyNode (EVM-compatible)
// ============================================================================

pragma solidity 0.8.24;

import "@openzeppelin/contracts/governance/Governor.sol";
import "@openzeppelin/contracts/governance/extensions/GovernorSettings.sol";
import "@openzeppelin/contracts/governance/extensions/GovernorCountingSimple.sol";
import "@openzeppelin/contracts/governance/extensions/GovernorVotes.sol";
import "@openzeppelin/contracts/governance/extensions/GovernorVotesQuorumFraction.sol";
import "@openzeppelin/contracts/governance/extensions/GovernorTimelockControl.sol";

/**
 * @title SovereignGovernance
 * @author Ierahkwa Sovereign Development Council
 * @notice On-chain DAO governance for the Ierahkwa sovereign digital nation.
 *         Based on consensus-building principles of indigenous sovereign nations,
 *         enabling 72M indigenous people to participate in collective decision-making.
 * @dev Governor contract with timelock, quorum fraction, and categorized proposals.
 *      Built on OpenZeppelin v5.x Governor framework.
 *
 * Governance Parameters:
 *   - Voting delay:       1 day (time between proposal creation and voting start)
 *   - Voting period:      7 days (duration of active voting)
 *   - Proposal threshold: 100,000 WMP (minimum tokens to create a proposal)
 *   - Quorum:             4% of total voting power
 *
 * Proposal Categories:
 *   0 - Treasury:    Allocation and management of sovereign funds
 *   1 - Protocol:    Technical upgrades and protocol changes
 *   2 - Community:   Community initiatives and social programs
 *   3 - Emergency:   Urgent actions requiring expedited processing
 *   4 - Cultural:    Cultural preservation and heritage programs
 */
contract SovereignGovernance is
    Governor,
    GovernorSettings,
    GovernorCountingSimple,
    GovernorVotes,
    GovernorVotesQuorumFraction,
    GovernorTimelockControl
{
    // =========================================================================
    // DATA STRUCTURES
    // =========================================================================

    /**
     * @notice Categories for governance proposals.
     */
    enum ProposalCategory {
        Treasury,       // Allocation and management of sovereign funds
        Protocol,       // Technical upgrades and protocol changes
        Community,      // Community initiatives and social programs
        Emergency,      // Urgent actions requiring expedited processing
        Cultural        // Cultural preservation and heritage programs
    }

    /**
     * @notice Extended metadata for each proposal.
     * @param category  The category of the proposal.
     * @param ipfsHash  IPFS content identifier for the full proposal document.
     * @param proposer  Address of the original proposer.
     * @param createdAt Block timestamp when the proposal was created.
     */
    struct ProposalInfo {
        ProposalCategory category;
        string ipfsHash;
        address proposer;
        uint256 createdAt;
    }

    // =========================================================================
    // STATE VARIABLES
    // =========================================================================

    /// @notice Extended metadata for each proposal, keyed by proposal ID.
    mapping(uint256 => ProposalInfo) public proposalInfo;

    /// @notice Count of proposals per category.
    mapping(ProposalCategory => uint256) public categoryProposalCount;

    /// @notice Total number of proposals created through this governance.
    uint256 public totalProposals;

    // =========================================================================
    // EVENTS
    // =========================================================================

    /// @notice Emitted when a proposal is created with category metadata.
    event ProposalCreatedWithCategory(
        uint256 indexed proposalId,
        ProposalCategory indexed category,
        address indexed proposer,
        string ipfsHash,
        uint256 timestamp
    );

    // =========================================================================
    // ERRORS
    // =========================================================================

    /// @notice Thrown when an empty IPFS hash is provided.
    error EmptyIPFSHash();

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    /**
     * @notice Deploys the SovereignGovernance contract.
     * @param _token    The ERC20Votes token used for voting power (WMP).
     * @param _timelock The TimelockController for proposal execution delay.
     */
    constructor(
        IVotes _token,
        TimelockController _timelock
    )
        Governor("Ierahkwa Sovereign Governance")
        GovernorSettings(
            1 days,       // Voting delay: 1 day
            7 days,       // Voting period: 7 days
            100_000e18    // Proposal threshold: 100,000 WMP
        )
        GovernorVotes(_token)
        GovernorVotesQuorumFraction(4) // 4% quorum
        GovernorTimelockControl(_timelock)
    {}

    // =========================================================================
    // PROPOSAL CREATION WITH CATEGORY
    // =========================================================================

    /**
     * @notice Create a governance proposal with category and IPFS metadata.
     * @dev Wraps the standard `propose()` call with additional metadata storage.
     * @param targets     Target contract addresses for the proposal actions.
     * @param values      ETH values for each action.
     * @param calldatas   Encoded function call data for each action.
     * @param description Human-readable description of the proposal.
     * @param category    Category classification for the proposal.
     * @param ipfsHash    IPFS content identifier for the full proposal document.
     * @return proposalId The unique identifier for the created proposal.
     */
    function proposeWithCategory(
        address[] memory targets,
        uint256[] memory values,
        bytes[] memory calldatas,
        string memory description,
        ProposalCategory category,
        string memory ipfsHash
    ) public returns (uint256) {
        if (bytes(ipfsHash).length == 0) revert EmptyIPFSHash();

        uint256 proposalId = propose(targets, values, calldatas, description);

        proposalInfo[proposalId] = ProposalInfo({
            category: category,
            ipfsHash: ipfsHash,
            proposer: msg.sender,
            createdAt: block.timestamp
        });

        categoryProposalCount[category]++;
        totalProposals++;

        emit ProposalCreatedWithCategory(
            proposalId,
            category,
            msg.sender,
            ipfsHash,
            block.timestamp
        );

        return proposalId;
    }

    // =========================================================================
    // VIEW FUNCTIONS
    // =========================================================================

    /**
     * @notice Get the category of a proposal.
     * @param proposalId The proposal identifier.
     * @return The ProposalCategory enum value.
     */
    function getProposalCategory(
        uint256 proposalId
    ) external view returns (ProposalCategory) {
        return proposalInfo[proposalId].category;
    }

    /**
     * @notice Get the IPFS hash of a proposal's full document.
     * @param proposalId The proposal identifier.
     * @return The IPFS content identifier string.
     */
    function getProposalIPFS(
        uint256 proposalId
    ) external view returns (string memory) {
        return proposalInfo[proposalId].ipfsHash;
    }

    /**
     * @notice Get the full metadata for a proposal.
     * @param proposalId The proposal identifier.
     * @return category   The proposal category.
     * @return ipfsHash   The IPFS content identifier.
     * @return proposer   The address that created the proposal.
     * @return createdAt  The timestamp of creation.
     */
    function getProposalInfo(
        uint256 proposalId
    )
        external
        view
        returns (
            ProposalCategory category,
            string memory ipfsHash,
            address proposer,
            uint256 createdAt
        )
    {
        ProposalInfo storage info = proposalInfo[proposalId];
        return (info.category, info.ipfsHash, info.proposer, info.createdAt);
    }

    // =========================================================================
    // REQUIRED OVERRIDES (OpenZeppelin v5.x)
    // =========================================================================

    /**
     * @dev Override votingDelay from Governor and GovernorSettings.
     */
    function votingDelay()
        public
        view
        override(Governor, GovernorSettings)
        returns (uint256)
    {
        return super.votingDelay();
    }

    /**
     * @dev Override votingPeriod from Governor and GovernorSettings.
     */
    function votingPeriod()
        public
        view
        override(Governor, GovernorSettings)
        returns (uint256)
    {
        return super.votingPeriod();
    }

    /**
     * @dev Override quorum from Governor and GovernorVotesQuorumFraction.
     */
    function quorum(
        uint256 blockNumber
    )
        public
        view
        override(Governor, GovernorVotesQuorumFraction)
        returns (uint256)
    {
        return super.quorum(blockNumber);
    }

    /**
     * @dev Override state from Governor and GovernorTimelockControl.
     */
    function state(
        uint256 proposalId
    )
        public
        view
        override(Governor, GovernorTimelockControl)
        returns (ProposalState)
    {
        return super.state(proposalId);
    }

    /**
     * @dev Override proposalNeedsQueuing from Governor and GovernorTimelockControl.
     */
    function proposalNeedsQueuing(
        uint256 proposalId
    )
        public
        view
        override(Governor, GovernorTimelockControl)
        returns (bool)
    {
        return super.proposalNeedsQueuing(proposalId);
    }

    /**
     * @dev Override proposalThreshold from Governor and GovernorSettings.
     */
    function proposalThreshold()
        public
        view
        override(Governor, GovernorSettings)
        returns (uint256)
    {
        return super.proposalThreshold();
    }

    /**
     * @dev Override _queueOperations from Governor and GovernorTimelockControl.
     */
    function _queueOperations(
        uint256 proposalId,
        address[] memory targets,
        uint256[] memory values,
        bytes[] memory calldatas,
        bytes32 descriptionHash
    ) internal override(Governor, GovernorTimelockControl) returns (uint48) {
        return super._queueOperations(
            proposalId, targets, values, calldatas, descriptionHash
        );
    }

    /**
     * @dev Override _executeOperations from Governor and GovernorTimelockControl.
     */
    function _executeOperations(
        uint256 proposalId,
        address[] memory targets,
        uint256[] memory values,
        bytes[] memory calldatas,
        bytes32 descriptionHash
    ) internal override(Governor, GovernorTimelockControl) {
        super._executeOperations(
            proposalId, targets, values, calldatas, descriptionHash
        );
    }

    /**
     * @dev Override _cancel from Governor and GovernorTimelockControl.
     */
    function _cancel(
        address[] memory targets,
        uint256[] memory values,
        bytes[] memory calldatas,
        bytes32 descriptionHash
    ) internal override(Governor, GovernorTimelockControl) returns (uint256) {
        return super._cancel(targets, values, calldatas, descriptionHash);
    }

    /**
     * @dev Override _executor from Governor and GovernorTimelockControl.
     */
    function _executor()
        internal
        view
        override(Governor, GovernorTimelockControl)
        returns (address)
    {
        return super._executor();
    }
}
