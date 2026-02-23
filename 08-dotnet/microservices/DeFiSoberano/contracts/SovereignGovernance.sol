// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "@openzeppelin/contracts/governance/Governor.sol";
import "@openzeppelin/contracts/governance/extensions/GovernorSettings.sol";
import "@openzeppelin/contracts/governance/extensions/GovernorCountingSimple.sol";
import "@openzeppelin/contracts/governance/extensions/GovernorVotes.sol";
import "@openzeppelin/contracts/governance/extensions/GovernorVotesQuorumFraction.sol";
import "@openzeppelin/contracts/governance/extensions/GovernorTimelockControl.sol";

/**
 * @title SovereignGovernance
 * @dev DAO governance for IERAHKWA Nation decisions
 * Based on consensus-building principles of sovereign nations
 */
contract SovereignGovernance is 
    Governor, 
    GovernorSettings, 
    GovernorCountingSimple, 
    GovernorVotes, 
    GovernorVotesQuorumFraction, 
    GovernorTimelockControl 
{
    // Proposal categories
    enum ProposalCategory {
        Treasury,       // Treasury management
        Protocol,       // Protocol upgrades
        Community,      // Community initiatives
        Emergency,      // Emergency actions
        Cultural        // Cultural preservation
    }

    // Extended proposal info
    struct ProposalInfo {
        ProposalCategory category;
        string ipfsHash;        // IPFS hash for detailed proposal
        address proposer;
        uint256 createdAt;
    }

    mapping(uint256 => ProposalInfo) public proposalInfo;

    // Events
    event ProposalCreatedWithCategory(
        uint256 indexed proposalId,
        ProposalCategory category,
        string ipfsHash
    );

    constructor(
        IVotes _token,
        TimelockController _timelock
    )
        Governor("IERAHKWA Sovereign Governance")
        GovernorSettings(
            1 days,     // Voting delay (1 day)
            7 days,     // Voting period (7 days)
            100000e18   // Proposal threshold (100,000 tokens)
        )
        GovernorVotes(_token)
        GovernorVotesQuorumFraction(4) // 4% quorum
        GovernorTimelockControl(_timelock)
    {}

    /**
     * @dev Create proposal with category and IPFS metadata
     */
    function proposeWithCategory(
        address[] memory targets,
        uint256[] memory values,
        bytes[] memory calldatas,
        string memory description,
        ProposalCategory category,
        string memory ipfsHash
    ) public returns (uint256) {
        uint256 proposalId = propose(targets, values, calldatas, description);
        
        proposalInfo[proposalId] = ProposalInfo({
            category: category,
            ipfsHash: ipfsHash,
            proposer: msg.sender,
            createdAt: block.timestamp
        });

        emit ProposalCreatedWithCategory(proposalId, category, ipfsHash);
        
        return proposalId;
    }

    /**
     * @dev Get proposal category
     */
    function getProposalCategory(uint256 proposalId) public view returns (ProposalCategory) {
        return proposalInfo[proposalId].category;
    }

    // Required overrides

    function votingDelay()
        public
        view
        override(Governor, GovernorSettings)
        returns (uint256)
    {
        return super.votingDelay();
    }

    function votingPeriod()
        public
        view
        override(Governor, GovernorSettings)
        returns (uint256)
    {
        return super.votingPeriod();
    }

    function quorum(uint256 blockNumber)
        public
        view
        override(Governor, GovernorVotesQuorumFraction)
        returns (uint256)
    {
        return super.quorum(blockNumber);
    }

    function state(uint256 proposalId)
        public
        view
        override(Governor, GovernorTimelockControl)
        returns (ProposalState)
    {
        return super.state(proposalId);
    }

    function proposalNeedsQueuing(uint256 proposalId)
        public
        view
        override(Governor, GovernorTimelockControl)
        returns (bool)
    {
        return super.proposalNeedsQueuing(proposalId);
    }

    function proposalThreshold()
        public
        view
        override(Governor, GovernorSettings)
        returns (uint256)
    {
        return super.proposalThreshold();
    }

    function _queueOperations(
        uint256 proposalId,
        address[] memory targets,
        uint256[] memory values,
        bytes[] memory calldatas,
        bytes32 descriptionHash
    ) internal override(Governor, GovernorTimelockControl) returns (uint48) {
        return super._queueOperations(proposalId, targets, values, calldatas, descriptionHash);
    }

    function _executeOperations(
        uint256 proposalId,
        address[] memory targets,
        uint256[] memory values,
        bytes[] memory calldatas,
        bytes32 descriptionHash
    ) internal override(Governor, GovernorTimelockControl) {
        super._executeOperations(proposalId, targets, values, calldatas, descriptionHash);
    }

    function _cancel(
        address[] memory targets,
        uint256[] memory values,
        bytes[] memory calldatas,
        bytes32 descriptionHash
    ) internal override(Governor, GovernorTimelockControl) returns (uint256) {
        return super._cancel(targets, values, calldatas, descriptionHash);
    }

    function _executor()
        internal
        view
        override(Governor, GovernorTimelockControl)
        returns (address)
    {
        return super._executor();
    }
}
