// SPDX-License-Identifier: MIT
// ============================================================================
// IERAHKWA TREASURY -- Sovereign DAO Treasury & Proposal System
// Manages collective funds through reputation-weighted governance proposals.
// Integrates with IerahkwaReputation ($MATTR) for voting power and with
// IerahkwaToken ($IRHK) for balance-gated proposal creation.
// ============================================================================

pragma solidity ^0.8.20;

import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/security/Pausable.sol";
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";
import "@openzeppelin/contracts/token/ERC20/IERC20.sol";

/**
 * @title IerahkwaTreasury
 * @author Ierahkwa Ne Kanienke -- Sovereign Digital Nation
 * @notice DAO-managed treasury that allocates funds through community proposals.
 *
 * Key mechanics:
 *   - Proposal creation requires the proposer to hold a non-zero $MATTR balance,
 *     ensuring only reputable community members can initiate spending.
 *   - Voting power is denominated in $MATTR (soulbound reputation tokens).
 *     Higher reputation = stronger voice.
 *   - Proposals have typed categories (Humanitarian, Environmental,
 *     Infrastructure, Education, Emergency) for transparent allocation tracking.
 *   - Emergency proposals have a shorter voting window (24 hours vs 7 days).
 *   - Execution requires: deadline passed, votesFor > votesAgainst, and
 *     total participation >= QUORUM_PERCENTAGE of circulating $MATTR supply.
 *   - The contract can receive native ETH through `receive()`.
 *
 * @dev This contract references IerahkwaReputation through the IMATTRToken
 *      interface for voting weight queries. Ensure the reputation contract
 *      address is set correctly at deployment.
 */

/// @notice Minimal interface for the IerahkwaReputation ($MATTR) contract.
interface IMATTRToken {
    function balanceOf(address account) external view returns (uint256);
    function totalSupply() external view returns (uint256);
}

contract IerahkwaTreasury is AccessControl, Pausable, ReentrancyGuard {

    // =========================================================================
    // ROLES
    // =========================================================================

    /// @notice Role for emergency actions (pause, emergency proposals).
    bytes32 public constant EMERGENCY_ROLE = keccak256("EMERGENCY_ROLE");

    /// @notice Role for pause/unpause operations.
    bytes32 public constant PAUSER_ROLE = keccak256("PAUSER_ROLE");

    // =========================================================================
    // CONSTANTS & CONFIGURATION
    // =========================================================================

    /// @notice Standard voting period for normal proposals (7 days).
    uint256 public constant STANDARD_VOTING_PERIOD = 7 days;

    /// @notice Shortened voting period for emergency proposals (24 hours).
    uint256 public constant EMERGENCY_VOTING_PERIOD = 1 days;

    /// @notice Minimum percentage of total $MATTR supply that must participate
    ///         for a proposal to reach quorum (denominator = 10000, so 500 = 5%).
    uint256 public constant QUORUM_PERCENTAGE = 500;

    /// @notice Denominator for percentage calculations.
    uint256 public constant PERCENTAGE_DENOMINATOR = 10000;

    // =========================================================================
    // ENUMS
    // =========================================================================

    /// @notice Categories of treasury proposals.
    enum ProposalType {
        Humanitarian,    // 0 -- Aid, disaster relief, community welfare
        Environmental,   // 1 -- Reforestation, water, conservation
        Infrastructure,  // 2 -- Digital and physical infrastructure
        Education,       // 3 -- Schools, training, scholarships
        Emergency        // 4 -- Crisis response (shorter deadline)
    }

    /// @notice Lifecycle states of a proposal.
    enum ProposalState {
        Active,      // 0 -- Voting is open
        Passed,      // 1 -- Deadline reached, quorum met, votesFor > votesAgainst
        Rejected,    // 2 -- Deadline reached but conditions not met
        Executed,    // 3 -- Funds disbursed
        Cancelled    // 4 -- Cancelled by admin or proposer
    }

    // =========================================================================
    // STRUCTS
    // =========================================================================

    /// @notice Represents a single treasury spending proposal.
    /// @param description   Human-readable description of the proposal.
    /// @param amount        Amount of native ETH (in wei) to disburse.
    /// @param recipient     Address that will receive the funds if executed.
    /// @param votesFor      Total $MATTR-weighted votes in favor.
    /// @param votesAgainst  Total $MATTR-weighted votes against.
    /// @param executed      Whether the proposal has been executed.
    /// @param deadline      Timestamp after which voting closes.
    /// @param proposalType  Category of the proposal.
    /// @param proposer      Address that created the proposal.
    /// @param createdAt     Timestamp of proposal creation.
    struct Proposal {
        string description;
        uint256 amount;
        address payable recipient;
        uint256 votesFor;
        uint256 votesAgainst;
        bool executed;
        uint256 deadline;
        ProposalType proposalType;
        address proposer;
        uint256 createdAt;
    }

    // =========================================================================
    // STATE
    // =========================================================================

    /// @notice Reference to the $MATTR reputation token contract.
    IMATTRToken public mattrToken;

    /// @notice All proposals indexed by ID.
    mapping(uint256 => Proposal) public proposals;

    /// @notice Tracks whether an address has voted on a specific proposal.
    /// @dev proposalId => voter => bool
    mapping(uint256 => mapping(address => bool)) public hasVoted;

    /// @notice Records the vote weight each voter committed per proposal.
    /// @dev proposalId => voter => weight
    mapping(uint256 => mapping(address => uint256)) public voteWeight;

    /// @notice Auto-incrementing proposal counter.
    uint256 public proposalCount;

    /// @notice Running total of funds disbursed through executed proposals.
    uint256 public totalDisbursed;

    /// @notice Count of proposals by type for analytics.
    mapping(ProposalType => uint256) public proposalsByType;

    // =========================================================================
    // EVENTS
    // =========================================================================

    /// @notice Emitted when a new proposal is created.
    event ProposalCreated(
        uint256 indexed proposalId,
        address indexed proposer,
        string description,
        uint256 amount,
        address recipient,
        ProposalType proposalType,
        uint256 deadline
    );

    /// @notice Emitted when a vote is cast on a proposal.
    event Voted(
        uint256 indexed proposalId,
        address indexed voter,
        bool support,
        uint256 weight
    );

    /// @notice Emitted when a passed proposal is executed and funds are transferred.
    event ProposalExecuted(
        uint256 indexed proposalId,
        address indexed recipient,
        uint256 amount
    );

    /// @notice Emitted when an emergency proposal is created (shorter deadline).
    event EmergencyActivated(
        uint256 indexed proposalId,
        address indexed activator,
        string description,
        uint256 amount
    );

    /// @notice Emitted when a proposal is cancelled.
    event ProposalCancelled(uint256 indexed proposalId, address indexed cancelledBy);

    /// @notice Emitted when the treasury receives native ETH.
    event TreasuryFunded(address indexed sender, uint256 amount);

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    /**
     * @notice Deploy the treasury contract.
     * @param _admin     Address that receives DEFAULT_ADMIN_ROLE and EMERGENCY_ROLE.
     * @param _mattrToken Address of the deployed IerahkwaReputation ($MATTR) contract.
     */
    constructor(address _admin, address _mattrToken) {
        require(_admin != address(0), "Treasury: admin is zero address");
        require(_mattrToken != address(0), "Treasury: MATTR token is zero address");

        mattrToken = IMATTRToken(_mattrToken);

        _grantRole(DEFAULT_ADMIN_ROLE, _admin);
        _grantRole(EMERGENCY_ROLE, _admin);
        _grantRole(PAUSER_ROLE, _admin);
    }

    // =========================================================================
    // RECEIVE
    // =========================================================================

    /// @notice Accept native ETH deposits into the treasury.
    receive() external payable {
        require(msg.value > 0, "Treasury: zero value");
        emit TreasuryFunded(msg.sender, msg.value);
    }

    // =========================================================================
    // PROPOSAL CREATION
    // =========================================================================

    /**
     * @notice Create a standard treasury proposal.
     * @dev Caller must hold a non-zero $MATTR balance (reputation requirement).
     *
     * @param _description  Human-readable description of the spending proposal.
     * @param _amount       Amount of native ETH (wei) requested.
     * @param _recipient    Address that will receive funds if the proposal passes.
     * @param _proposalType Category of the proposal.
     * @return proposalId   The newly created proposal's ID.
     */
    function createProposal(
        string calldata _description,
        uint256 _amount,
        address payable _recipient,
        ProposalType _proposalType
    )
        external
        whenNotPaused
        nonReentrant
        returns (uint256 proposalId)
    {
        require(mattrToken.balanceOf(msg.sender) > 0, "Treasury: must hold MATTR to propose");
        require(bytes(_description).length > 0, "Treasury: empty description");
        require(_amount > 0, "Treasury: zero amount");
        require(_recipient != address(0), "Treasury: recipient is zero address");
        require(_proposalType != ProposalType.Emergency, "Treasury: use emergencyProposal()");

        proposalId = proposalCount++;

        proposals[proposalId] = Proposal({
            description: _description,
            amount: _amount,
            recipient: _recipient,
            votesFor: 0,
            votesAgainst: 0,
            executed: false,
            deadline: block.timestamp + STANDARD_VOTING_PERIOD,
            proposalType: _proposalType,
            proposer: msg.sender,
            createdAt: block.timestamp
        });

        proposalsByType[_proposalType]++;

        emit ProposalCreated(
            proposalId,
            msg.sender,
            _description,
            _amount,
            _recipient,
            _proposalType,
            block.timestamp + STANDARD_VOTING_PERIOD
        );
    }

    /**
     * @notice Create an emergency proposal with a shortened voting period.
     * @dev Only callable by addresses with EMERGENCY_ROLE. Emergency proposals
     *      have a 24-hour window instead of the standard 7 days.
     *
     * @param _description Human-readable description of the emergency.
     * @param _amount      Amount of native ETH (wei) requested.
     * @param _recipient   Address that will receive the funds.
     * @return proposalId  The newly created proposal's ID.
     */
    function emergencyProposal(
        string calldata _description,
        uint256 _amount,
        address payable _recipient
    )
        external
        onlyRole(EMERGENCY_ROLE)
        whenNotPaused
        nonReentrant
        returns (uint256 proposalId)
    {
        require(bytes(_description).length > 0, "Treasury: empty description");
        require(_amount > 0, "Treasury: zero amount");
        require(_recipient != address(0), "Treasury: recipient is zero address");

        proposalId = proposalCount++;

        proposals[proposalId] = Proposal({
            description: _description,
            amount: _amount,
            recipient: _recipient,
            votesFor: 0,
            votesAgainst: 0,
            executed: false,
            deadline: block.timestamp + EMERGENCY_VOTING_PERIOD,
            proposalType: ProposalType.Emergency,
            proposer: msg.sender,
            createdAt: block.timestamp
        });

        proposalsByType[ProposalType.Emergency]++;

        emit EmergencyActivated(proposalId, msg.sender, _description, _amount);

        emit ProposalCreated(
            proposalId,
            msg.sender,
            _description,
            _amount,
            _recipient,
            ProposalType.Emergency,
            block.timestamp + EMERGENCY_VOTING_PERIOD
        );
    }

    // =========================================================================
    // VOTING
    // =========================================================================

    /**
     * @notice Cast a vote on an active proposal.
     * @dev Voting power equals the caller's $MATTR balance at the time of voting.
     *      Each address may only vote once per proposal.
     *
     * @param _proposalId The ID of the proposal to vote on.
     * @param _support    True to vote in favor, false to vote against.
     */
    function vote(uint256 _proposalId, bool _support)
        external
        whenNotPaused
        nonReentrant
    {
        require(_proposalId < proposalCount, "Treasury: invalid proposal ID");
        Proposal storage proposal = proposals[_proposalId];

        require(block.timestamp < proposal.deadline, "Treasury: voting period ended");
        require(!proposal.executed, "Treasury: proposal already executed");
        require(!hasVoted[_proposalId][msg.sender], "Treasury: already voted");

        uint256 weight = mattrToken.balanceOf(msg.sender);
        require(weight > 0, "Treasury: no MATTR balance (no voting power)");

        hasVoted[_proposalId][msg.sender] = true;
        voteWeight[_proposalId][msg.sender] = weight;

        if (_support) {
            proposal.votesFor += weight;
        } else {
            proposal.votesAgainst += weight;
        }

        emit Voted(_proposalId, msg.sender, _support, weight);
    }

    // =========================================================================
    // EXECUTION
    // =========================================================================

    /**
     * @notice Execute a proposal that has passed voting.
     * @dev Requirements for execution:
     *      1. Voting deadline has passed.
     *      2. votesFor > votesAgainst.
     *      3. Total participation (votesFor + votesAgainst) >= quorum.
     *      4. Treasury has sufficient balance.
     *      5. Proposal has not already been executed.
     *
     * @param _proposalId The ID of the proposal to execute.
     */
    function executeProposal(uint256 _proposalId)
        external
        whenNotPaused
        nonReentrant
    {
        require(_proposalId < proposalCount, "Treasury: invalid proposal ID");
        Proposal storage proposal = proposals[_proposalId];

        require(block.timestamp >= proposal.deadline, "Treasury: voting still active");
        require(!proposal.executed, "Treasury: already executed");
        require(proposal.votesFor > proposal.votesAgainst, "Treasury: proposal did not pass");

        // Quorum check: total votes must be >= QUORUM_PERCENTAGE of MATTR supply
        uint256 totalVotes = proposal.votesFor + proposal.votesAgainst;
        uint256 mattrSupply = mattrToken.totalSupply();
        uint256 quorumRequired = (mattrSupply * QUORUM_PERCENTAGE) / PERCENTAGE_DENOMINATOR;
        require(totalVotes >= quorumRequired, "Treasury: quorum not reached");

        require(address(this).balance >= proposal.amount, "Treasury: insufficient balance");

        proposal.executed = true;
        totalDisbursed += proposal.amount;

        (bool success, ) = proposal.recipient.call{value: proposal.amount}("");
        require(success, "Treasury: ETH transfer failed");

        emit ProposalExecuted(_proposalId, proposal.recipient, proposal.amount);
    }

    // =========================================================================
    // PROPOSAL CANCELLATION
    // =========================================================================

    /**
     * @notice Cancel a proposal before its deadline.
     * @dev Only the original proposer or an admin can cancel.
     *
     * @param _proposalId The ID of the proposal to cancel.
     */
    function cancelProposal(uint256 _proposalId)
        external
        whenNotPaused
    {
        require(_proposalId < proposalCount, "Treasury: invalid proposal ID");
        Proposal storage proposal = proposals[_proposalId];

        require(!proposal.executed, "Treasury: already executed");
        require(
            msg.sender == proposal.proposer || hasRole(DEFAULT_ADMIN_ROLE, msg.sender),
            "Treasury: not authorized to cancel"
        );

        // Set deadline to now to effectively close voting
        proposal.deadline = block.timestamp;

        emit ProposalCancelled(_proposalId, msg.sender);
    }

    // =========================================================================
    // VIEW FUNCTIONS
    // =========================================================================

    /**
     * @notice Get the current state of a proposal.
     * @param _proposalId The ID of the proposal.
     * @return The ProposalState enum value.
     */
    function getProposalState(uint256 _proposalId)
        external
        view
        returns (ProposalState)
    {
        require(_proposalId < proposalCount, "Treasury: invalid proposal ID");
        Proposal storage proposal = proposals[_proposalId];

        if (proposal.executed) {
            return ProposalState.Executed;
        }

        if (block.timestamp < proposal.deadline) {
            return ProposalState.Active;
        }

        // Deadline has passed -- check results
        uint256 totalVotes = proposal.votesFor + proposal.votesAgainst;
        uint256 mattrSupply = mattrToken.totalSupply();
        uint256 quorumRequired = (mattrSupply * QUORUM_PERCENTAGE) / PERCENTAGE_DENOMINATOR;

        if (
            proposal.votesFor > proposal.votesAgainst &&
            totalVotes >= quorumRequired
        ) {
            return ProposalState.Passed;
        }

        return ProposalState.Rejected;
    }

    /**
     * @notice Get the full details of a proposal.
     * @param _proposalId The ID of the proposal.
     * @return The Proposal struct.
     */
    function getProposal(uint256 _proposalId)
        external
        view
        returns (Proposal memory)
    {
        require(_proposalId < proposalCount, "Treasury: invalid proposal ID");
        return proposals[_proposalId];
    }

    /**
     * @notice Get the current ETH balance of the treasury.
     * @return The balance in wei.
     */
    function treasuryBalance() external view returns (uint256) {
        return address(this).balance;
    }

    /**
     * @notice Compute the current quorum requirement based on MATTR supply.
     * @return The minimum total vote weight needed.
     */
    function currentQuorum() external view returns (uint256) {
        return (mattrToken.totalSupply() * QUORUM_PERCENTAGE) / PERCENTAGE_DENOMINATOR;
    }

    // =========================================================================
    // PAUSE
    // =========================================================================

    /// @notice Pause all treasury operations.
    function pause() external onlyRole(PAUSER_ROLE) {
        _pause();
    }

    /// @notice Resume operations.
    function unpause() external onlyRole(PAUSER_ROLE) {
        _unpause();
    }
}
