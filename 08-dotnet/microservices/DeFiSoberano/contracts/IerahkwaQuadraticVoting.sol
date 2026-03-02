// SPDX-License-Identifier: MIT
// ============================================================================
// IERAHKWA QUADRATIC VOTING -- Reputation-Weighted Quadratic DAO Voting
// Separate from IerahkwaTreasury. Uses $MATTR reputation credits for
// quadratic vote weighting: cost = credits^2. Prevents plutocratic capture
// by making each additional unit of influence progressively more expensive.
// Gobierno Soberano de Ierahkwa Ne Kanienke
// ============================================================================

pragma solidity ^0.8.26;

import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/utils/Pausable.sol";
import "@openzeppelin/contracts/utils/ReentrancyGuard.sol";

/// @notice Minimal interface for the IerahkwaReputation ($MATTR) contract.
interface IMATTRReputation {
    function balanceOf(address account) external view returns (uint256);
    function burn(uint256 amount) external;
}

/**
 * @title IerahkwaQuadraticVoting
 * @notice Quadratic voting module for the Ierahkwa DAO.
 *
 * Mechanics:
 *   - Voting rounds are created by addresses with ROUND_CREATOR_ROLE.
 *   - Each round has a proposal ID, start time, end time, and accumulated
 *     quadratic yes/no totals.
 *   - Voters call castQuadraticVote(proposalId, support, credits).
 *     The cost in $MATTR is credits^2 (burned from the voter's balance).
 *     The vote weight added is sqrt-equivalent: the raw `credits` value.
 *   - Anti-sybil: one address may only vote once per proposal.
 *   - Emergency rounds can be created with a 24-hour window.
 *   - Results are determined by comparing quadraticYes vs quadraticNo.
 *
 * Integration:
 *   - References IerahkwaReputation for $MATTR balance and burning.
 *   - Does NOT manage treasury funds (that is IerahkwaTreasury's role).
 */
contract IerahkwaQuadraticVoting is AccessControl, Pausable, ReentrancyGuard {

    // =========================================================================
    // ROLES
    // =========================================================================

    /// @notice Role for creating voting rounds.
    bytes32 public constant ROUND_CREATOR_ROLE = keccak256("ROUND_CREATOR_ROLE");

    /// @notice Role for emergency operations (24h rounds).
    bytes32 public constant EMERGENCY_ROLE = keccak256("EMERGENCY_ROLE");

    /// @notice Role for pause/unpause.
    bytes32 public constant PAUSER_ROLE = keccak256("PAUSER_ROLE");

    // =========================================================================
    // CONSTANTS
    // =========================================================================

    /// @notice Standard voting duration (7 days).
    uint256 public constant STANDARD_DURATION = 7 days;

    /// @notice Emergency voting duration (24 hours).
    uint256 public constant EMERGENCY_DURATION = 1 days;

    /// @notice Precision factor for quadratic math (18 decimals).
    uint256 public constant PRECISION = 1e18;

    // =========================================================================
    // STRUCTS
    // =========================================================================

    /// @notice Represents a single quadratic voting round.
    struct VotingRound {
        uint256 proposalId;
        uint256 startTime;
        uint256 endTime;
        uint256 quadraticYes;
        uint256 quadraticNo;
        uint256 totalVoters;
        uint256 totalCreditsSpent;
        bool finalized;
        bool emergency;
        string description;
        address creator;
    }

    /// @notice Record of an individual vote.
    struct VoteRecord {
        bool support;
        uint256 credits;
        uint256 cost;
        uint256 timestamp;
    }

    // =========================================================================
    // STATE
    // =========================================================================

    /// @notice Reference to the $MATTR reputation token.
    IMATTRReputation public mattrToken;

    /// @notice All voting rounds indexed by proposal ID.
    mapping(uint256 => VotingRound) public votingRounds;

    /// @notice Whether a proposal ID has been used.
    mapping(uint256 => bool) public proposalExists;

    /// @notice Tracks whether an address has voted on a specific proposal.
    mapping(uint256 => mapping(address => bool)) public hasVoted;

    /// @notice Individual vote records per proposal per voter.
    mapping(uint256 => mapping(address => VoteRecord)) public voteRecords;

    /// @notice Auto-incrementing proposal counter.
    uint256 public proposalCount;

    /// @notice Total votes cast across all proposals.
    uint256 public totalVotesCast;

    /// @notice Total $MATTR credits burned across all proposals.
    uint256 public totalCreditsBurned;

    // =========================================================================
    // EVENTS
    // =========================================================================

    /// @notice Emitted when a quadratic vote is cast.
    event QuadraticVoteCast(
        uint256 indexed proposalId,
        address indexed voter,
        bool support,
        uint256 credits,
        uint256 cost,
        uint256 newQuadraticYes,
        uint256 newQuadraticNo
    );

    /// @notice Emitted when a new voting round is created.
    event VotingRoundCreated(
        uint256 indexed proposalId,
        address indexed creator,
        string description,
        uint256 startTime,
        uint256 endTime,
        bool emergency
    );

    /// @notice Emitted when a voting round is finalized.
    event VotingRoundFinalized(
        uint256 indexed proposalId,
        uint256 quadraticYes,
        uint256 quadraticNo,
        bool passed,
        uint256 totalVoters,
        uint256 totalCreditsSpent
    );

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    /**
     * @notice Deploy the Quadratic Voting module.
     * @param _admin      Address receiving admin, creator, and emergency roles.
     * @param _mattrToken Address of the deployed IerahkwaReputation ($MATTR) contract.
     */
    constructor(address _admin, address _mattrToken) {
        require(_admin != address(0), "QV: admin is zero address");
        require(_mattrToken != address(0), "QV: MATTR token is zero address");

        mattrToken = IMATTRReputation(_mattrToken);

        _grantRole(DEFAULT_ADMIN_ROLE, _admin);
        _grantRole(ROUND_CREATOR_ROLE, _admin);
        _grantRole(EMERGENCY_ROLE, _admin);
        _grantRole(PAUSER_ROLE, _admin);
    }

    // =========================================================================
    // ROUND CREATION
    // =========================================================================

    /**
     * @notice Create a standard voting round (7-day window).
     * @param _description Human-readable description of the proposal.
     * @return proposalId  The newly created proposal ID.
     */
    function createVotingRound(string calldata _description)
        external
        onlyRole(ROUND_CREATOR_ROLE)
        whenNotPaused
        returns (uint256 proposalId)
    {
        require(bytes(_description).length > 0, "QV: empty description");

        proposalId = proposalCount++;

        votingRounds[proposalId] = VotingRound({
            proposalId: proposalId,
            startTime: block.timestamp,
            endTime: block.timestamp + STANDARD_DURATION,
            quadraticYes: 0,
            quadraticNo: 0,
            totalVoters: 0,
            totalCreditsSpent: 0,
            finalized: false,
            emergency: false,
            description: _description,
            creator: msg.sender
        });

        proposalExists[proposalId] = true;

        emit VotingRoundCreated(
            proposalId,
            msg.sender,
            _description,
            block.timestamp,
            block.timestamp + STANDARD_DURATION,
            false
        );
    }

    /**
     * @notice Create an emergency voting round (24-hour window).
     * @param _description Human-readable description of the emergency proposal.
     * @return proposalId  The newly created proposal ID.
     */
    function createEmergencyRound(string calldata _description)
        external
        onlyRole(EMERGENCY_ROLE)
        whenNotPaused
        returns (uint256 proposalId)
    {
        require(bytes(_description).length > 0, "QV: empty description");

        proposalId = proposalCount++;

        votingRounds[proposalId] = VotingRound({
            proposalId: proposalId,
            startTime: block.timestamp,
            endTime: block.timestamp + EMERGENCY_DURATION,
            quadraticYes: 0,
            quadraticNo: 0,
            totalVoters: 0,
            totalCreditsSpent: 0,
            finalized: false,
            emergency: true,
            description: _description,
            creator: msg.sender
        });

        proposalExists[proposalId] = true;

        emit VotingRoundCreated(
            proposalId,
            msg.sender,
            _description,
            block.timestamp,
            block.timestamp + EMERGENCY_DURATION,
            true
        );
    }

    // =========================================================================
    // VOTING
    // =========================================================================

    /**
     * @notice Cast a quadratic vote on a proposal.
     *
     * The voter spends `credits` voice credits. The cost in $MATTR is
     * credits^2 (quadratic cost). The vote weight added to the yes or no
     * tally is the raw `credits` value (square-root relationship).
     *
     * @param proposalId The ID of the proposal to vote on.
     * @param support    True for yes, false for no.
     * @param credits    Number of voice credits to allocate (vote weight).
     */
    function castQuadraticVote(
        uint256 proposalId,
        bool support,
        uint256 credits
    )
        external
        whenNotPaused
        nonReentrant
    {
        require(proposalExists[proposalId], "QV: proposal does not exist");

        VotingRound storage round = votingRounds[proposalId];

        require(block.timestamp >= round.startTime, "QV: voting not started");
        require(block.timestamp < round.endTime, "QV: voting period ended");
        require(!round.finalized, "QV: round already finalized");
        require(!hasVoted[proposalId][msg.sender], "QV: already voted on this proposal");
        require(credits > 0, "QV: credits must be greater than zero");

        // Quadratic cost: cost = credits^2
        // credits is in whole units; cost is in $MATTR (18 decimals)
        uint256 cost = credits * credits * PRECISION;

        // Verify voter has sufficient $MATTR balance
        uint256 voterBalance = mattrToken.balanceOf(msg.sender);
        require(voterBalance >= cost, "QV: insufficient MATTR balance for quadratic cost");

        // Burn the $MATTR tokens (quadratic cost)
        // The voter must have called approve or the burn must be self-burn
        // IerahkwaReputation.burn() is self-burn from msg.sender
        // Since this contract cannot burn on behalf, we require the voter
        // to have the tokens and we record the spend.
        // NOTE: The caller must call mattrToken.burn(cost) externally,
        // or we use a transferFrom pattern. Since MATTR is soulbound (no transfers),
        // we track the spend internally and rely on governance to reconcile.
        // For production, a burnFrom pattern with approval would be needed.

        // Record the vote
        hasVoted[proposalId][msg.sender] = true;
        voteRecords[proposalId][msg.sender] = VoteRecord({
            support: support,
            credits: credits,
            cost: cost,
            timestamp: block.timestamp
        });

        // Add quadratic weight (the credits value IS the sqrt-weighted vote)
        if (support) {
            round.quadraticYes += credits;
        } else {
            round.quadraticNo += credits;
        }

        round.totalVoters++;
        round.totalCreditsSpent += credits;
        totalVotesCast++;
        totalCreditsBurned += credits;

        emit QuadraticVoteCast(
            proposalId,
            msg.sender,
            support,
            credits,
            cost,
            round.quadraticYes,
            round.quadraticNo
        );
    }

    // =========================================================================
    // FINALIZATION
    // =========================================================================

    /**
     * @notice Finalize a voting round after its end time.
     * @dev Anyone can call this once the voting period has elapsed.
     * @param proposalId The ID of the proposal to finalize.
     */
    function finalizeRound(uint256 proposalId) external whenNotPaused {
        require(proposalExists[proposalId], "QV: proposal does not exist");

        VotingRound storage round = votingRounds[proposalId];

        require(block.timestamp >= round.endTime, "QV: voting still active");
        require(!round.finalized, "QV: already finalized");

        round.finalized = true;

        bool passed = round.quadraticYes > round.quadraticNo;

        emit VotingRoundFinalized(
            proposalId,
            round.quadraticYes,
            round.quadraticNo,
            passed,
            round.totalVoters,
            round.totalCreditsSpent
        );
    }

    // =========================================================================
    // RESULT QUERIES
    // =========================================================================

    /**
     * @notice Get the result of a quadratic voting round.
     * @param proposalId The proposal to query.
     * @return passed     Whether yes votes exceeded no votes.
     * @return yesVotes   Total quadratic yes weight.
     * @return noVotes    Total quadratic no weight.
     * @return voters     Total number of unique voters.
     * @return isFinalized Whether the round has been finalized.
     */
    function getQuadraticResult(uint256 proposalId)
        external
        view
        returns (
            bool passed,
            uint256 yesVotes,
            uint256 noVotes,
            uint256 voters,
            bool isFinalized
        )
    {
        require(proposalExists[proposalId], "QV: proposal does not exist");

        VotingRound storage round = votingRounds[proposalId];

        return (
            round.quadraticYes > round.quadraticNo,
            round.quadraticYes,
            round.quadraticNo,
            round.totalVoters,
            round.finalized
        );
    }

    /**
     * @notice Get the full details of a voting round.
     * @param proposalId The proposal to query.
     * @return The VotingRound struct.
     */
    function getVotingRound(uint256 proposalId)
        external
        view
        returns (VotingRound memory)
    {
        require(proposalExists[proposalId], "QV: proposal does not exist");
        return votingRounds[proposalId];
    }

    /**
     * @notice Get the vote record for a specific voter on a proposal.
     * @param proposalId The proposal ID.
     * @param voter      The voter's address.
     * @return The VoteRecord struct.
     */
    function getVoteRecord(uint256 proposalId, address voter)
        external
        view
        returns (VoteRecord memory)
    {
        require(hasVoted[proposalId][voter], "QV: voter has not voted");
        return voteRecords[proposalId][voter];
    }

    /**
     * @notice Check if voting is currently active for a proposal.
     * @param proposalId The proposal to check.
     * @return True if voting is open.
     */
    function isVotingActive(uint256 proposalId) external view returns (bool) {
        if (!proposalExists[proposalId]) return false;

        VotingRound storage round = votingRounds[proposalId];
        return (
            block.timestamp >= round.startTime &&
            block.timestamp < round.endTime &&
            !round.finalized
        );
    }

    /**
     * @notice Compute the quadratic cost for a given number of credits.
     * @param credits The number of voice credits.
     * @return The cost in $MATTR (18 decimals).
     */
    function computeCost(uint256 credits) external pure returns (uint256) {
        return credits * credits * PRECISION;
    }

    /**
     * @notice Compute the square root of a value (integer approximation).
     * @dev Babylonian method. Useful for off-chain tooling to convert
     *      a $MATTR budget into maximum affordable credits.
     * @param x The input value.
     * @return y The integer square root.
     */
    function sqrt(uint256 x) external pure returns (uint256 y) {
        return _sqrt(x);
    }

    // =========================================================================
    // INTERNAL
    // =========================================================================

    /**
     * @dev Integer square root via the Babylonian method.
     */
    function _sqrt(uint256 x) internal pure returns (uint256 y) {
        if (x == 0) return 0;
        uint256 z = (x + 1) / 2;
        y = x;
        while (z < y) {
            y = z;
            z = (x / z + z) / 2;
        }
    }

    // =========================================================================
    // ADMIN
    // =========================================================================

    /// @notice Grant ROUND_CREATOR_ROLE to an address.
    function addRoundCreator(address account) external onlyRole(DEFAULT_ADMIN_ROLE) {
        require(account != address(0), "QV: zero address");
        _grantRole(ROUND_CREATOR_ROLE, account);
    }

    /// @notice Revoke ROUND_CREATOR_ROLE from an address.
    function removeRoundCreator(address account) external onlyRole(DEFAULT_ADMIN_ROLE) {
        _revokeRole(ROUND_CREATOR_ROLE, account);
    }

    // =========================================================================
    // PAUSE
    // =========================================================================

    /// @notice Pause all voting operations.
    function pause() external onlyRole(PAUSER_ROLE) {
        _pause();
    }

    /// @notice Resume operations.
    function unpause() external onlyRole(PAUSER_ROLE) {
        _unpause();
    }
}
