// SPDX-License-Identifier: MIT
// ============================================================================
// SOVEREIGN JUSTICE -- Decentralized Arbitration Court
// On-chain dispute resolution for 1B+ citizens of Ierahkwa Ne Kanienke.
// Guardians with Genesis Badge or high $MATTR reputation serve as jurors.
// Integrates with IerahkwaReputation and IerahkwaToken for juror selection
// and WMP-based filing fees and reward distribution.
// ============================================================================

pragma solidity ^0.8.26;

import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/utils/Pausable.sol";
import "@openzeppelin/contracts/utils/ReentrancyGuard.sol";
import "@openzeppelin/contracts/token/ERC20/IERC20.sol";

/// @notice Minimal interface for IerahkwaReputation ($MATTR soulbound token).
interface IMATTRToken {
    function balanceOf(address account) external view returns (uint256);
    function totalSupply() external view returns (uint256);
}

/// @notice Minimal interface for Genesis Badge SBT (ERC-721).
interface IGenesisNFT {
    function balanceOf(address owner) external view returns (uint256);
}

/**
 * @title SovereignJustice
 * @notice Decentralized arbitration court for the Ierahkwa sovereign ecosystem.
 *
 * When disputes arise among citizens across the Americas, Guardians with high
 * $MATTR reputation (>500) or a Genesis Badge serve as jurors. Standard disputes
 * require 5 jurors; appeals escalate to 7.
 *
 * Key flows:
 *   1. Plaintiff files a dispute by paying a 100 WMP filing fee.
 *   2. System assigns jurors pseudo-randomly from eligible Guardians.
 *   3. Jurors cast verdicts (plaintiff or defendant).
 *   4. After all votes, majority wins and the reward is distributed.
 *   5. Loser may appeal (200 WMP, new panel of 7 jurors).
 *   6. Guardian Council can fast-track emergency disputes (24h deadline).
 */
contract SovereignJustice is AccessControl, Pausable, ReentrancyGuard {

    // =========================================================================
    // ROLES
    // =========================================================================

    bytes32 public constant GUARDIAN_COUNCIL_ROLE = keccak256("GUARDIAN_COUNCIL_ROLE");
    bytes32 public constant PAUSER_ROLE = keccak256("PAUSER_ROLE");

    // =========================================================================
    // ENUMS
    // =========================================================================

    enum Category {
        TRADE,      // 0 -- Commercial disputes
        LAND,       // 1 -- Territory and land rights
        IP,         // 2 -- Intellectual property
        FRAUD,      // 3 -- Fraud and theft
        OTHER       // 4 -- General disputes
    }

    enum DisputeStatus {
        Filed,              // 0 -- Awaiting juror assignment
        JurorsAssigned,     // 1 -- Jurors selected, voting open
        Resolved,           // 2 -- Verdict reached
        Appealed,           // 3 -- Under appeal with larger jury
        AppealResolved,     // 4 -- Appeal verdict reached
        Cancelled           // 5 -- Cancelled by admin or mutual agreement
    }

    // =========================================================================
    // STRUCTS
    // =========================================================================

    struct Dispute {
        address plaintiff;
        address defendant;
        bytes32 evidenceIPFS;
        uint256 rewardWMP;
        Category category;
        DisputeStatus status;
        address[] jurors;
        mapping(address => bool) hasVoted;
        mapping(address => bool) votedForPlaintiff;
        uint256 votesForPlaintiff;
        uint256 votesForDefendant;
        uint256 filedAt;
        uint256 deadline;
        bool isEmergency;
        bool isAppeal;
        uint256 originalDisputeId;
    }

    // =========================================================================
    // CONSTANTS
    // =========================================================================

    /// @notice Standard filing fee in WMP tokens (100 * 10^18).
    uint256 public constant FILING_FEE = 100 * 10 ** 18;

    /// @notice Appeal filing fee in WMP tokens (200 * 10^18).
    uint256 public constant APPEAL_FEE = 200 * 10 ** 18;

    /// @notice Minimum $MATTR balance required to serve as juror.
    uint256 public constant MIN_MATTR_FOR_JUROR = 500 * 10 ** 18;

    /// @notice Number of jurors for standard disputes.
    uint256 public constant STANDARD_JURY_SIZE = 5;

    /// @notice Number of jurors for appeal disputes.
    uint256 public constant APPEAL_JURY_SIZE = 7;

    /// @notice Standard dispute voting period (7 days).
    uint256 public constant STANDARD_DEADLINE = 7 days;

    /// @notice Emergency dispute voting period (24 hours).
    uint256 public constant EMERGENCY_DEADLINE = 1 days;

    // =========================================================================
    // STATE
    // =========================================================================

    /// @notice Reference to the WMP (IerahkwaToken) contract for fees and rewards.
    IERC20 public wmpToken;

    /// @notice Reference to the $MATTR (IerahkwaReputation) soulbound contract.
    IMATTRToken public mattrToken;

    /// @notice Reference to the Genesis Badge NFT contract (optional).
    IGenesisNFT public genesisBadge;

    /// @notice All disputes indexed by ID.
    mapping(uint256 => Dispute) private _disputes;

    /// @notice Auto-incrementing dispute counter.
    uint256 public disputeCount;

    /// @notice Registry of eligible jurors (Guardians who opted in).
    address[] public jurorRegistry;

    /// @notice Quick lookup for juror registry membership.
    mapping(address => bool) public isRegisteredJuror;

    /// @notice Total WMP collected in filing fees.
    uint256 public totalFeesCollected;

    /// @notice Total WMP distributed as rewards.
    uint256 public totalRewardsDistributed;

    // =========================================================================
    // EVENTS
    // =========================================================================

    event DisputeCreated(
        uint256 indexed disputeId,
        address indexed plaintiff,
        address indexed defendant,
        Category category,
        uint256 rewardWMP,
        bytes32 evidenceIPFS,
        uint256 deadline
    );

    event JurorsAssigned(
        uint256 indexed disputeId,
        address[] jurors,
        uint256 jurySize
    );

    event VerdictCast(
        uint256 indexed disputeId,
        address indexed juror,
        bool votedForPlaintiff,
        uint256 currentPlaintiffVotes,
        uint256 currentDefendantVotes
    );

    event DisputeResolved(
        uint256 indexed disputeId,
        bool plaintiffWon,
        uint256 votesFor,
        uint256 votesAgainst,
        uint256 rewardDistributed
    );

    event DisputeAppealed(
        uint256 indexed originalDisputeId,
        uint256 indexed appealDisputeId,
        address indexed appellant,
        uint256 appealFee
    );

    event EmergencyDisputeCreated(
        uint256 indexed disputeId,
        address indexed activator,
        uint256 deadline
    );

    event JurorRegistered(address indexed juror);
    event JurorRemoved(address indexed juror);
    event DisputeCancelled(uint256 indexed disputeId, address indexed cancelledBy);

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    /**
     * @param _admin         Address receiving DEFAULT_ADMIN_ROLE.
     * @param _wmpToken      Address of the WMP (IerahkwaToken) ERC-20 contract.
     * @param _mattrToken    Address of the IerahkwaReputation ($MATTR) contract.
     * @param _genesisBadge  Address of the Genesis Badge SBT (address(0) to skip).
     */
    constructor(
        address _admin,
        address _wmpToken,
        address _mattrToken,
        address _genesisBadge
    ) {
        require(_admin != address(0), "Justice: admin is zero address");
        require(_wmpToken != address(0), "Justice: WMP token is zero address");
        require(_mattrToken != address(0), "Justice: MATTR token is zero address");

        wmpToken = IERC20(_wmpToken);
        mattrToken = IMATTRToken(_mattrToken);

        if (_genesisBadge != address(0)) {
            genesisBadge = IGenesisNFT(_genesisBadge);
        }

        _grantRole(DEFAULT_ADMIN_ROLE, _admin);
        _grantRole(GUARDIAN_COUNCIL_ROLE, _admin);
        _grantRole(PAUSER_ROLE, _admin);
    }

    // =========================================================================
    // JUROR REGISTRATION
    // =========================================================================

    /**
     * @notice Register as an eligible juror. Requires Genesis Badge or >500 $MATTR.
     */
    function registerAsJuror() external whenNotPaused {
        require(!isRegisteredJuror[msg.sender], "Justice: already registered");
        require(_isEligibleJuror(msg.sender), "Justice: insufficient reputation or no Genesis Badge");

        jurorRegistry.push(msg.sender);
        isRegisteredJuror[msg.sender] = true;

        emit JurorRegistered(msg.sender);
    }

    /**
     * @notice Remove self from the juror registry.
     */
    function unregisterAsJuror() external {
        require(isRegisteredJuror[msg.sender], "Justice: not registered");

        isRegisteredJuror[msg.sender] = false;

        // Remove from array (swap with last)
        for (uint256 i = 0; i < jurorRegistry.length; i++) {
            if (jurorRegistry[i] == msg.sender) {
                jurorRegistry[i] = jurorRegistry[jurorRegistry.length - 1];
                jurorRegistry.pop();
                break;
            }
        }

        emit JurorRemoved(msg.sender);
    }

    // =========================================================================
    // DISPUTE CREATION
    // =========================================================================

    /**
     * @notice File a new dispute. Caller must approve FILING_FEE WMP tokens first.
     * @param _defendant     The party being accused.
     * @param _evidenceHash  IPFS hash of evidence documents.
     * @param _category      Category of the dispute.
     * @param _rewardWMP     Additional WMP reward for jurors (from plaintiff).
     * @return disputeId     The newly created dispute ID.
     */
    function createDispute(
        address _defendant,
        bytes32 _evidenceHash,
        Category _category,
        uint256 _rewardWMP
    )
        external
        whenNotPaused
        nonReentrant
        returns (uint256 disputeId)
    {
        require(_defendant != address(0), "Justice: defendant is zero address");
        require(_defendant != msg.sender, "Justice: cannot dispute yourself");
        require(_evidenceHash != bytes32(0), "Justice: evidence hash required");

        // Collect filing fee
        require(
            wmpToken.transferFrom(msg.sender, address(this), FILING_FEE),
            "Justice: filing fee transfer failed"
        );
        totalFeesCollected += FILING_FEE;

        // Collect juror reward pool (optional extra from plaintiff)
        if (_rewardWMP > 0) {
            require(
                wmpToken.transferFrom(msg.sender, address(this), _rewardWMP),
                "Justice: reward transfer failed"
            );
        }

        disputeId = disputeCount++;
        Dispute storage d = _disputes[disputeId];
        d.plaintiff = msg.sender;
        d.defendant = _defendant;
        d.evidenceIPFS = _evidenceHash;
        d.rewardWMP = _rewardWMP;
        d.category = _category;
        d.status = DisputeStatus.Filed;
        d.filedAt = block.timestamp;
        d.deadline = block.timestamp + STANDARD_DEADLINE;
        d.isEmergency = false;
        d.isAppeal = false;
        d.originalDisputeId = disputeId;

        emit DisputeCreated(
            disputeId,
            msg.sender,
            _defendant,
            _category,
            _rewardWMP,
            _evidenceHash,
            d.deadline
        );
    }

    /**
     * @notice Guardian Council creates an emergency dispute with a 24h deadline.
     */
    function createEmergencyDispute(
        address _plaintiff,
        address _defendant,
        bytes32 _evidenceHash,
        Category _category,
        uint256 _rewardWMP
    )
        external
        onlyRole(GUARDIAN_COUNCIL_ROLE)
        whenNotPaused
        nonReentrant
        returns (uint256 disputeId)
    {
        require(_plaintiff != address(0) && _defendant != address(0), "Justice: zero address");
        require(_plaintiff != _defendant, "Justice: same party");
        require(_evidenceHash != bytes32(0), "Justice: evidence required");

        disputeId = disputeCount++;
        Dispute storage d = _disputes[disputeId];
        d.plaintiff = _plaintiff;
        d.defendant = _defendant;
        d.evidenceIPFS = _evidenceHash;
        d.rewardWMP = _rewardWMP;
        d.category = _category;
        d.status = DisputeStatus.Filed;
        d.filedAt = block.timestamp;
        d.deadline = block.timestamp + EMERGENCY_DEADLINE;
        d.isEmergency = true;
        d.isAppeal = false;
        d.originalDisputeId = disputeId;

        emit EmergencyDisputeCreated(disputeId, msg.sender, d.deadline);
        emit DisputeCreated(
            disputeId,
            _plaintiff,
            _defendant,
            _category,
            _rewardWMP,
            _evidenceHash,
            d.deadline
        );
    }

    // =========================================================================
    // JUROR ASSIGNMENT
    // =========================================================================

    /**
     * @notice Assign jurors to a dispute using pseudo-random selection.
     * @dev Uses block.prevrandao + block.timestamp for on-chain randomness.
     *      Only eligible Guardians (Genesis Badge or >500 $MATTR) who are
     *      registered and not parties to the dispute can be selected.
     * @param _disputeId  The dispute to assign jurors to.
     */
    function assignJurors(uint256 _disputeId)
        external
        whenNotPaused
        nonReentrant
    {
        require(_disputeId < disputeCount, "Justice: invalid dispute ID");
        Dispute storage d = _disputes[_disputeId];
        require(d.status == DisputeStatus.Filed, "Justice: dispute not in Filed state");
        require(block.timestamp < d.deadline, "Justice: deadline passed");

        uint256 jurySize = d.isAppeal ? APPEAL_JURY_SIZE : STANDARD_JURY_SIZE;

        // Build candidate pool (eligible jurors not party to dispute)
        address[] memory candidates = _buildCandidatePool(d.plaintiff, d.defendant);
        require(candidates.length >= jurySize, "Justice: not enough eligible jurors");

        // Pseudo-random selection using Fisher-Yates partial shuffle
        uint256 seed = uint256(keccak256(abi.encodePacked(
            block.prevrandao,
            block.timestamp,
            _disputeId,
            msg.sender,
            candidates.length
        )));

        address[] memory selected = new address[](jurySize);
        uint256 remaining = candidates.length;

        for (uint256 i = 0; i < jurySize; i++) {
            seed = uint256(keccak256(abi.encodePacked(seed, i)));
            uint256 idx = seed % remaining;
            selected[i] = candidates[idx];

            // Swap selected with last unselected
            candidates[idx] = candidates[remaining - 1];
            remaining--;
        }

        d.jurors = selected;
        d.status = DisputeStatus.JurorsAssigned;

        emit JurorsAssigned(_disputeId, selected, jurySize);
    }

    // =========================================================================
    // VOTING
    // =========================================================================

    /**
     * @notice Cast a verdict on a dispute. Only assigned jurors may vote.
     * @param _disputeId          The dispute to vote on.
     * @param _voteForPlaintiff   True = plaintiff wins, false = defendant wins.
     */
    function castVerdict(uint256 _disputeId, bool _voteForPlaintiff)
        external
        whenNotPaused
        nonReentrant
    {
        require(_disputeId < disputeCount, "Justice: invalid dispute ID");
        Dispute storage d = _disputes[_disputeId];

        require(
            d.status == DisputeStatus.JurorsAssigned ||
            d.status == DisputeStatus.Appealed,
            "Justice: voting not open"
        );
        require(block.timestamp < d.deadline, "Justice: deadline passed");
        require(!d.hasVoted[msg.sender], "Justice: already voted");
        require(_isAssignedJuror(d, msg.sender), "Justice: not an assigned juror");

        d.hasVoted[msg.sender] = true;
        d.votedForPlaintiff[msg.sender] = _voteForPlaintiff;

        if (_voteForPlaintiff) {
            d.votesForPlaintiff++;
        } else {
            d.votesForDefendant++;
        }

        emit VerdictCast(
            _disputeId,
            msg.sender,
            _voteForPlaintiff,
            d.votesForPlaintiff,
            d.votesForDefendant
        );
    }

    // =========================================================================
    // RESOLUTION
    // =========================================================================

    /**
     * @notice Resolve a dispute after all jurors have voted (or deadline passed).
     *         Majority wins. Reward is split among jurors who voted with majority.
     * @param _disputeId  The dispute to resolve.
     */
    function resolveDispute(uint256 _disputeId)
        external
        whenNotPaused
        nonReentrant
    {
        require(_disputeId < disputeCount, "Justice: invalid dispute ID");
        Dispute storage d = _disputes[_disputeId];

        require(
            d.status == DisputeStatus.JurorsAssigned ||
            d.status == DisputeStatus.Appealed,
            "Justice: not resolvable"
        );

        uint256 totalVotes = d.votesForPlaintiff + d.votesForDefendant;
        uint256 jurySize = d.jurors.length;

        // All jurors voted OR deadline passed
        require(
            totalVotes == jurySize || block.timestamp >= d.deadline,
            "Justice: voting still in progress"
        );

        // Must have at least one vote to resolve
        require(totalVotes > 0, "Justice: no votes cast");

        bool plaintiffWon = d.votesForPlaintiff > d.votesForDefendant;

        // Update status
        if (d.isAppeal) {
            d.status = DisputeStatus.AppealResolved;
        } else {
            d.status = DisputeStatus.Resolved;
        }

        // Distribute rewards to majority jurors
        uint256 rewardDistributed = 0;
        if (d.rewardWMP > 0) {
            uint256 majorityCount = plaintiffWon ? d.votesForPlaintiff : d.votesForDefendant;
            if (majorityCount > 0) {
                uint256 perJuror = d.rewardWMP / majorityCount;
                for (uint256 i = 0; i < d.jurors.length; i++) {
                    address juror = d.jurors[i];
                    if (d.hasVoted[juror]) {
                        bool votedWithMajority = d.votedForPlaintiff[juror] == plaintiffWon;
                        if (votedWithMajority) {
                            wmpToken.transfer(juror, perJuror);
                            rewardDistributed += perJuror;
                        }
                    }
                }
            }
        }

        totalRewardsDistributed += rewardDistributed;

        emit DisputeResolved(
            _disputeId,
            plaintiffWon,
            d.votesForPlaintiff,
            d.votesForDefendant,
            rewardDistributed
        );
    }

    // =========================================================================
    // APPEAL
    // =========================================================================

    /**
     * @notice Appeal a resolved dispute. Creates a new dispute with a larger jury (7).
     *         Costs APPEAL_FEE (200 WMP). Only the losing party can appeal.
     * @param _disputeId  The original dispute to appeal.
     * @return appealId   The new appeal dispute ID.
     */
    function appealDispute(uint256 _disputeId)
        external
        whenNotPaused
        nonReentrant
        returns (uint256 appealId)
    {
        require(_disputeId < disputeCount, "Justice: invalid dispute ID");
        Dispute storage original = _disputes[_disputeId];

        require(
            original.status == DisputeStatus.Resolved,
            "Justice: can only appeal resolved disputes"
        );
        require(!original.isAppeal, "Justice: cannot appeal an appeal");

        // Only the losing party can appeal
        bool plaintiffWon = original.votesForPlaintiff > original.votesForDefendant;
        address loser = plaintiffWon ? original.defendant : original.plaintiff;
        require(msg.sender == loser, "Justice: only the losing party can appeal");

        // Collect appeal fee
        require(
            wmpToken.transferFrom(msg.sender, address(this), APPEAL_FEE),
            "Justice: appeal fee transfer failed"
        );
        totalFeesCollected += APPEAL_FEE;

        // Create appeal dispute
        appealId = disputeCount++;
        Dispute storage appeal = _disputes[appealId];
        appeal.plaintiff = original.plaintiff;
        appeal.defendant = original.defendant;
        appeal.evidenceIPFS = original.evidenceIPFS;
        appeal.rewardWMP = original.rewardWMP;
        appeal.category = original.category;
        appeal.status = DisputeStatus.Appealed;
        appeal.filedAt = block.timestamp;
        appeal.deadline = block.timestamp + STANDARD_DEADLINE;
        appeal.isEmergency = false;
        appeal.isAppeal = true;
        appeal.originalDisputeId = _disputeId;

        // Mark original as appealed
        original.status = DisputeStatus.Appealed;

        emit DisputeAppealed(_disputeId, appealId, msg.sender, APPEAL_FEE);
    }

    // =========================================================================
    // CANCELLATION
    // =========================================================================

    /**
     * @notice Cancel a dispute. Only admin or both parties agreeing can cancel.
     */
    function cancelDispute(uint256 _disputeId)
        external
        whenNotPaused
    {
        require(_disputeId < disputeCount, "Justice: invalid dispute ID");
        Dispute storage d = _disputes[_disputeId];

        require(
            d.status == DisputeStatus.Filed ||
            d.status == DisputeStatus.JurorsAssigned,
            "Justice: cannot cancel in current state"
        );

        require(
            msg.sender == d.plaintiff || hasRole(DEFAULT_ADMIN_ROLE, msg.sender),
            "Justice: not authorized to cancel"
        );

        d.status = DisputeStatus.Cancelled;

        // Refund reward pool to plaintiff if not yet distributed
        if (d.rewardWMP > 0) {
            wmpToken.transfer(d.plaintiff, d.rewardWMP);
        }

        emit DisputeCancelled(_disputeId, msg.sender);
    }

    // =========================================================================
    // VIEW FUNCTIONS
    // =========================================================================

    /**
     * @notice Get basic dispute information.
     */
    function getDispute(uint256 _disputeId)
        external
        view
        returns (
            address plaintiff,
            address defendant,
            bytes32 evidenceIPFS,
            uint256 rewardWMP,
            Category category,
            DisputeStatus status,
            uint256 votesForPlaintiff,
            uint256 votesForDefendant,
            uint256 filedAt,
            uint256 deadline,
            bool isEmergency,
            bool isAppeal
        )
    {
        require(_disputeId < disputeCount, "Justice: invalid dispute ID");
        Dispute storage d = _disputes[_disputeId];
        return (
            d.plaintiff,
            d.defendant,
            d.evidenceIPFS,
            d.rewardWMP,
            d.category,
            d.status,
            d.votesForPlaintiff,
            d.votesForDefendant,
            d.filedAt,
            d.deadline,
            d.isEmergency,
            d.isAppeal
        );
    }

    /**
     * @notice Get the jurors assigned to a dispute.
     */
    function getJurors(uint256 _disputeId) external view returns (address[] memory) {
        require(_disputeId < disputeCount, "Justice: invalid dispute ID");
        return _disputes[_disputeId].jurors;
    }

    /**
     * @notice Check if a juror has voted on a dispute.
     */
    function hasJurorVoted(uint256 _disputeId, address _juror) external view returns (bool) {
        require(_disputeId < disputeCount, "Justice: invalid dispute ID");
        return _disputes[_disputeId].hasVoted[_juror];
    }

    /**
     * @notice Get the total number of registered jurors.
     */
    function getJurorRegistrySize() external view returns (uint256) {
        return jurorRegistry.length;
    }

    /**
     * @notice Check if an address is eligible to serve as juror.
     */
    function isEligibleJuror(address _account) external view returns (bool) {
        return _isEligibleJuror(_account);
    }

    // =========================================================================
    // ADMIN
    // =========================================================================

    function pause() external onlyRole(PAUSER_ROLE) {
        _pause();
    }

    function unpause() external onlyRole(PAUSER_ROLE) {
        _unpause();
    }

    /**
     * @notice Withdraw accumulated filing fees to treasury.
     */
    function withdrawFees(address _treasury, uint256 _amount)
        external
        onlyRole(DEFAULT_ADMIN_ROLE)
        nonReentrant
    {
        require(_treasury != address(0), "Justice: zero address");
        require(_amount > 0, "Justice: zero amount");
        require(wmpToken.transfer(_treasury, _amount), "Justice: transfer failed");
    }

    /**
     * @notice Update the Genesis Badge contract reference.
     */
    function setGenesisBadge(address _genesisBadge) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (_genesisBadge != address(0)) {
            genesisBadge = IGenesisNFT(_genesisBadge);
        }
    }

    // =========================================================================
    // INTERNAL FUNCTIONS
    // =========================================================================

    /**
     * @dev Check if an address qualifies as a juror.
     *      Requires Genesis Badge ownership OR >500 $MATTR balance.
     */
    function _isEligibleJuror(address _account) internal view returns (bool) {
        // Check Genesis Badge
        if (address(genesisBadge) != address(0)) {
            if (genesisBadge.balanceOf(_account) > 0) {
                return true;
            }
        }

        // Check $MATTR balance
        if (mattrToken.balanceOf(_account) >= MIN_MATTR_FOR_JUROR) {
            return true;
        }

        return false;
    }

    /**
     * @dev Build a pool of eligible juror candidates, excluding dispute parties.
     */
    function _buildCandidatePool(address _plaintiff, address _defendant)
        internal
        view
        returns (address[] memory)
    {
        uint256 registryLen = jurorRegistry.length;
        address[] memory temp = new address[](registryLen);
        uint256 count = 0;

        for (uint256 i = 0; i < registryLen; i++) {
            address candidate = jurorRegistry[i];
            if (
                candidate != _plaintiff &&
                candidate != _defendant &&
                isRegisteredJuror[candidate] &&
                _isEligibleJuror(candidate)
            ) {
                temp[count] = candidate;
                count++;
            }
        }

        // Trim the array
        address[] memory pool = new address[](count);
        for (uint256 i = 0; i < count; i++) {
            pool[i] = temp[i];
        }

        return pool;
    }

    /**
     * @dev Check if an address is an assigned juror for a specific dispute.
     */
    function _isAssignedJuror(Dispute storage d, address _juror) internal view returns (bool) {
        for (uint256 i = 0; i < d.jurors.length; i++) {
            if (d.jurors[i] == _juror) {
                return true;
            }
        }
        return false;
    }
}
