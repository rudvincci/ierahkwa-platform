// SPDX-License-Identifier: MIT
// ============================================================================
// IERAHKWA ORACLE -- Sovereign Decentralized Oracle (Guardian Consensus)
// Replaces external oracle dependencies (Chainlink, Band, etc.) with a
// Guardian-consensus model. 100 trusted Guardians submit data points; when
// 70+ agree on a value within a tolerance band, it becomes canonical truth
// on-chain for the IERAHKWA ecosystem.
// ============================================================================

pragma solidity ^0.8.20;

import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/security/Pausable.sol";
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";

/**
 * @title IerahkwaOracle
 * @author Ierahkwa Ne Kanienke -- Sovereign Digital Nation
 * @notice Guardian-consensus oracle for sovereign data feeds.
 *
 * Architecture:
 *   - Data is organized into feeds identified by `bytes32 feedId`.
 *   - Each feed operates in sequential rounds. A new round begins when
 *     the previous one has reached consensus or timed out.
 *   - In each round, Guardians submit a uint256 value via `submitData`.
 *   - Values are bucketed: submissions within TOLERANCE_BPS of each other
 *     are counted as agreeing. Once CONSENSUS_THRESHOLD Guardians agree
 *     on a bucket, that value becomes the canonical data point.
 *   - If a round does not reach consensus before ROUND_TIMEOUT elapses,
 *     the round is marked as failed and a new round can begin.
 *
 * Consumers query `getLatestData(feedId)` to retrieve the most recent
 * consensus value and its metadata.
 *
 * @dev The maximum number of Guardians is capped at MAX_GUARDIANS (100).
 *      Tolerance is expressed in basis points (default 200 = 2%).
 */
contract IerahkwaOracle is AccessControl, Pausable, ReentrancyGuard {

    // =========================================================================
    // ROLES
    // =========================================================================

    /// @notice Role for Guardians who can submit data observations.
    bytes32 public constant GUARDIAN_ROLE = keccak256("GUARDIAN_ROLE");

    /// @notice Role for managing feeds and oracle configuration.
    bytes32 public constant ORACLE_ADMIN_ROLE = keccak256("ORACLE_ADMIN_ROLE");

    /// @notice Role for pause/unpause operations.
    bytes32 public constant PAUSER_ROLE = keccak256("PAUSER_ROLE");

    // =========================================================================
    // CONSTANTS
    // =========================================================================

    /// @notice Maximum number of Guardians that can participate.
    uint256 public constant MAX_GUARDIANS = 100;

    /// @notice Number of Guardians that must agree for consensus.
    uint256 public constant CONSENSUS_THRESHOLD = 70;

    /// @notice Tolerance band in basis points for value agreement.
    ///         Two values v1 and v2 agree if |v1 - v2| <= max(v1,v2) * TOLERANCE_BPS / 10000.
    uint256 public constant TOLERANCE_BPS = 200; // 2%

    /// @notice Duration of a single voting round before timeout.
    uint256 public constant ROUND_TIMEOUT = 1 hours;

    /// @notice Denominator for basis point calculations.
    uint256 public constant BPS_DENOMINATOR = 10000;

    // =========================================================================
    // STRUCTS
    // =========================================================================

    /// @notice A finalized consensus data point.
    /// @param value          The consensus value.
    /// @param timestamp      Block timestamp when consensus was reached.
    /// @param confirmations  Number of Guardians that confirmed this value.
    struct DataPoint {
        uint256 value;
        uint256 timestamp;
        uint256 confirmations;
    }

    /// @notice Internal representation of a submission round.
    /// @param roundId       Sequential round number for this feed.
    /// @param startTime     Timestamp when the round opened.
    /// @param finalized     Whether consensus has been reached or round timed out.
    /// @param consensusValue The value that reached consensus (0 if none yet).
    /// @param submissionCount Number of Guardians that submitted in this round.
    struct Round {
        uint256 roundId;
        uint256 startTime;
        bool finalized;
        uint256 consensusValue;
        uint256 submissionCount;
    }

    /// @notice Metadata for a data feed.
    /// @param description   Human-readable feed description.
    /// @param active        Whether the feed accepts submissions.
    /// @param currentRound  Current active round ID.
    /// @param totalRounds   Total rounds completed (including failed).
    struct FeedInfo {
        string description;
        bool active;
        uint256 currentRound;
        uint256 totalRounds;
    }

    // =========================================================================
    // STATE
    // =========================================================================

    /// @notice Feed metadata, indexed by feed ID.
    mapping(bytes32 => FeedInfo) public feeds;

    /// @notice Latest consensus data point per feed.
    mapping(bytes32 => DataPoint) private _latestData;

    /// @notice Round data: feedId => roundId => Round.
    mapping(bytes32 => mapping(uint256 => Round)) public rounds;

    /// @notice Individual submissions: feedId => roundId => guardian => value.
    mapping(bytes32 => mapping(uint256 => mapping(address => uint256))) public submissions;

    /// @notice Whether a guardian has submitted in a given round:
    ///         feedId => roundId => guardian => bool.
    mapping(bytes32 => mapping(uint256 => mapping(address => bool))) public hasSubmitted;

    /// @notice Collected values in the current round for consensus checking:
    ///         feedId => roundId => array of submitted values.
    mapping(bytes32 => mapping(uint256 => uint256[])) private _roundValues;

    /// @notice Array of all registered feed IDs for enumeration.
    bytes32[] public feedIds;

    /// @notice Count of active Guardians.
    uint256 public guardianCount;

    // =========================================================================
    // EVENTS
    // =========================================================================

    /// @notice Emitted when a Guardian submits a data observation.
    event DataSubmitted(
        bytes32 indexed feedId,
        uint256 indexed roundId,
        address indexed guardian,
        uint256 value,
        uint256 timestamp
    );

    /// @notice Emitted when a round reaches Guardian consensus.
    event ConsensusReached(
        bytes32 indexed feedId,
        uint256 indexed roundId,
        uint256 consensusValue,
        uint256 confirmations,
        uint256 timestamp
    );

    /// @notice Emitted when a round times out without consensus.
    event RoundTimedOut(
        bytes32 indexed feedId,
        uint256 indexed roundId,
        uint256 submissionCount
    );

    /// @notice Emitted when a new data feed is registered.
    event FeedRegistered(bytes32 indexed feedId, string description);

    /// @notice Emitted when a feed is activated or deactivated.
    event FeedStatusChanged(bytes32 indexed feedId, bool active);

    /// @notice Emitted when a new round begins for a feed.
    event RoundStarted(bytes32 indexed feedId, uint256 indexed roundId, uint256 startTime);

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    /**
     * @notice Deploy the sovereign oracle.
     * @param _admin Address that receives admin, oracle admin, and pauser roles.
     */
    constructor(address _admin) {
        require(_admin != address(0), "Oracle: admin is zero address");

        _grantRole(DEFAULT_ADMIN_ROLE, _admin);
        _grantRole(ORACLE_ADMIN_ROLE, _admin);
        _grantRole(PAUSER_ROLE, _admin);
        _grantRole(GUARDIAN_ROLE, _admin);

        guardianCount = 1;
    }

    // =========================================================================
    // FEED MANAGEMENT
    // =========================================================================

    /**
     * @notice Register a new data feed.
     * @param _feedId      Unique identifier for the feed (e.g., keccak256("ETH/USD")).
     * @param _description Human-readable description of the feed.
     */
    function registerFeed(bytes32 _feedId, string calldata _description)
        external
        onlyRole(ORACLE_ADMIN_ROLE)
    {
        require(!feeds[_feedId].active && feeds[_feedId].totalRounds == 0, "Oracle: feed already exists");
        require(bytes(_description).length > 0, "Oracle: empty description");

        feeds[_feedId] = FeedInfo({
            description: _description,
            active: true,
            currentRound: 0,
            totalRounds: 0
        });

        feedIds.push(_feedId);

        // Start the first round
        _startNewRound(_feedId);

        emit FeedRegistered(_feedId, _description);
    }

    /**
     * @notice Activate or deactivate a feed.
     * @param _feedId The feed to update.
     * @param _active New active status.
     */
    function setFeedActive(bytes32 _feedId, bool _active)
        external
        onlyRole(ORACLE_ADMIN_ROLE)
    {
        require(feeds[_feedId].totalRounds > 0 || feeds[_feedId].active, "Oracle: feed does not exist");
        feeds[_feedId].active = _active;
        emit FeedStatusChanged(_feedId, _active);
    }

    // =========================================================================
    // DATA SUBMISSION
    // =========================================================================

    /**
     * @notice Submit a data observation for a feed's current round.
     * @dev Only Guardians may submit. Each Guardian can submit once per round.
     *      After submission, the contract checks whether consensus has been
     *      reached. If so, the round is finalized and the data point is stored.
     *
     * @param _feedId The feed to submit data for.
     * @param _value  The observed value.
     */
    function submitData(bytes32 _feedId, uint256 _value)
        external
        onlyRole(GUARDIAN_ROLE)
        whenNotPaused
        nonReentrant
    {
        FeedInfo storage feed = feeds[_feedId];
        require(feed.active, "Oracle: feed not active");

        uint256 currentRoundId = feed.currentRound;
        Round storage round = rounds[_feedId][currentRoundId];

        // If the current round has timed out, finalize it and start a new one
        if (!round.finalized && block.timestamp > round.startTime + ROUND_TIMEOUT) {
            round.finalized = true;
            emit RoundTimedOut(_feedId, currentRoundId, round.submissionCount);

            _startNewRound(_feedId);
            currentRoundId = feed.currentRound;
            round = rounds[_feedId][currentRoundId];
        }

        require(!round.finalized, "Oracle: round already finalized");
        require(!hasSubmitted[_feedId][currentRoundId][msg.sender], "Oracle: already submitted this round");

        // Record the submission
        submissions[_feedId][currentRoundId][msg.sender] = _value;
        hasSubmitted[_feedId][currentRoundId][msg.sender] = true;
        _roundValues[_feedId][currentRoundId].push(_value);
        round.submissionCount++;

        emit DataSubmitted(_feedId, currentRoundId, msg.sender, _value, block.timestamp);

        // Check for consensus
        _checkConsensus(_feedId, currentRoundId);
    }

    // =========================================================================
    // CONSENSUS LOGIC
    // =========================================================================

    /**
     * @dev Check whether any value in the current round has reached the
     *      consensus threshold. Uses a simple O(n^2) comparison which is
     *      acceptable given MAX_GUARDIANS = 100.
     *
     *      Two values agree if: |v1 - v2| <= max(v1, v2) * TOLERANCE_BPS / BPS_DENOMINATOR
     *
     * @param _feedId  The feed being checked.
     * @param _roundId The round being checked.
     */
    function _checkConsensus(bytes32 _feedId, uint256 _roundId) internal {
        uint256[] storage values = _roundValues[_feedId][_roundId];
        uint256 count = values.length;

        if (count < CONSENSUS_THRESHOLD) {
            return; // Not enough submissions yet
        }

        // For each value, count how many others agree within tolerance
        for (uint256 i = 0; i < count; i++) {
            uint256 agreements = 0;
            uint256 candidateValue = values[i];
            uint256 sumAgreed = 0;

            for (uint256 j = 0; j < count; j++) {
                if (_valuesAgree(candidateValue, values[j])) {
                    agreements++;
                    sumAgreed += values[j];
                }
            }

            if (agreements >= CONSENSUS_THRESHOLD) {
                // Consensus reached -- use the average of agreeing values
                uint256 consensusValue = sumAgreed / agreements;

                Round storage round = rounds[_feedId][_roundId];
                round.finalized = true;
                round.consensusValue = consensusValue;

                _latestData[_feedId] = DataPoint({
                    value: consensusValue,
                    timestamp: block.timestamp,
                    confirmations: agreements
                });

                emit ConsensusReached(
                    _feedId,
                    _roundId,
                    consensusValue,
                    agreements,
                    block.timestamp
                );

                // Start the next round
                _startNewRound(_feedId);

                return; // Exit after first consensus found
            }
        }
    }

    /**
     * @dev Determine whether two values are within the tolerance band.
     * @param _v1 First value.
     * @param _v2 Second value.
     * @return True if the values agree within TOLERANCE_BPS.
     */
    function _valuesAgree(uint256 _v1, uint256 _v2) internal pure returns (bool) {
        if (_v1 == _v2) return true;
        if (_v1 == 0 || _v2 == 0) return false;

        uint256 larger = _v1 > _v2 ? _v1 : _v2;
        uint256 diff = _v1 > _v2 ? _v1 - _v2 : _v2 - _v1;
        uint256 tolerance = (larger * TOLERANCE_BPS) / BPS_DENOMINATOR;

        return diff <= tolerance;
    }

    /**
     * @dev Start a new submission round for a feed.
     * @param _feedId The feed to advance.
     */
    function _startNewRound(bytes32 _feedId) internal {
        FeedInfo storage feed = feeds[_feedId];
        feed.totalRounds++;
        uint256 newRoundId = feed.totalRounds;
        feed.currentRound = newRoundId;

        rounds[_feedId][newRoundId] = Round({
            roundId: newRoundId,
            startTime: block.timestamp,
            finalized: false,
            consensusValue: 0,
            submissionCount: 0
        });

        emit RoundStarted(_feedId, newRoundId, block.timestamp);
    }

    // =========================================================================
    // DATA QUERIES
    // =========================================================================

    /**
     * @notice Retrieve the latest consensus data for a feed.
     * @param _feedId The feed to query.
     * @return The DataPoint struct with value, timestamp, and confirmation count.
     */
    function getLatestData(bytes32 _feedId)
        external
        view
        returns (DataPoint memory)
    {
        require(_latestData[_feedId].timestamp > 0, "Oracle: no data available for feed");
        return _latestData[_feedId];
    }

    /**
     * @notice Check if a feed has any finalized data.
     * @param _feedId The feed to check.
     * @return True if at least one consensus round has completed.
     */
    function hasData(bytes32 _feedId) external view returns (bool) {
        return _latestData[_feedId].timestamp > 0;
    }

    /**
     * @notice Get the current round info for a feed.
     * @param _feedId The feed to query.
     * @return The current Round struct.
     */
    function getCurrentRound(bytes32 _feedId)
        external
        view
        returns (Round memory)
    {
        uint256 currentRoundId = feeds[_feedId].currentRound;
        return rounds[_feedId][currentRoundId];
    }

    /**
     * @notice Get the number of submissions in the current round.
     * @param _feedId The feed to query.
     * @return Submission count for the active round.
     */
    function getCurrentRoundSubmissions(bytes32 _feedId)
        external
        view
        returns (uint256)
    {
        uint256 currentRoundId = feeds[_feedId].currentRound;
        return rounds[_feedId][currentRoundId].submissionCount;
    }

    /**
     * @notice Get the total number of registered feeds.
     * @return The length of the feedIds array.
     */
    function getFeedCount() external view returns (uint256) {
        return feedIds.length;
    }

    /**
     * @notice Compute a feed ID from a human-readable name.
     * @dev Utility for off-chain tooling: computeFeedId("ETH/USD") => bytes32.
     * @param _name The human-readable feed name.
     * @return The keccak256 hash of the name.
     */
    function computeFeedId(string calldata _name) external pure returns (bytes32) {
        return keccak256(abi.encodePacked(_name));
    }

    // =========================================================================
    // GUARDIAN MANAGEMENT
    // =========================================================================

    /**
     * @notice Add a new Guardian oracle participant.
     * @param _guardian Address to grant GUARDIAN_ROLE.
     */
    function addGuardian(address _guardian)
        external
        onlyRole(DEFAULT_ADMIN_ROLE)
    {
        require(_guardian != address(0), "Oracle: guardian is zero address");
        require(!hasRole(GUARDIAN_ROLE, _guardian), "Oracle: already a guardian");
        require(guardianCount < MAX_GUARDIANS, "Oracle: max guardians reached");

        _grantRole(GUARDIAN_ROLE, _guardian);
        guardianCount++;
    }

    /**
     * @notice Remove a Guardian oracle participant.
     * @param _guardian Address to revoke GUARDIAN_ROLE from.
     */
    function removeGuardian(address _guardian)
        external
        onlyRole(DEFAULT_ADMIN_ROLE)
    {
        require(hasRole(GUARDIAN_ROLE, _guardian), "Oracle: not a guardian");
        require(guardianCount > 1, "Oracle: cannot remove last guardian");

        _revokeRole(GUARDIAN_ROLE, _guardian);
        guardianCount--;
    }

    // =========================================================================
    // ROUND MANAGEMENT
    // =========================================================================

    /**
     * @notice Manually finalize a timed-out round and start a new one.
     * @dev Anyone can call this to advance a stale feed.
     * @param _feedId The feed with a timed-out round.
     */
    function finalizeTimedOutRound(bytes32 _feedId) external whenNotPaused {
        FeedInfo storage feed = feeds[_feedId];
        require(feed.active, "Oracle: feed not active");

        uint256 currentRoundId = feed.currentRound;
        Round storage round = rounds[_feedId][currentRoundId];

        require(!round.finalized, "Oracle: round already finalized");
        require(
            block.timestamp > round.startTime + ROUND_TIMEOUT,
            "Oracle: round not yet timed out"
        );

        round.finalized = true;
        emit RoundTimedOut(_feedId, currentRoundId, round.submissionCount);

        _startNewRound(_feedId);
    }

    // =========================================================================
    // PAUSE
    // =========================================================================

    /// @notice Pause all oracle operations.
    function pause() external onlyRole(PAUSER_ROLE) {
        _pause();
    }

    /// @notice Resume operations.
    function unpause() external onlyRole(PAUSER_ROLE) {
        _unpause();
    }
}
