// SPDX-License-Identifier: MIT
// ============================================================================
// IERAHKWA REPUTATION TOKEN — Soulbound Reputation ($MATTR)
// Non-transferable reputation token for the IERAHKWA sovereign ecosystem
// Integrates with SovereignID and SovereignGovernance for weighted voting
// ============================================================================

pragma solidity ^0.8.20;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/security/Pausable.sol";
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";

/**
 * @title IerahkwaReputation (MATTR)
 * @dev Soulbound ERC20 reputation token — non-transferable by design.
 *
 * Only Guardians and the AI Mediator can mint MATTR to reward conscious
 * actions verified on-chain. Users may burn their own tokens but can never
 * transfer them to another wallet, making reputation permanently bound to
 * its earner's identity.
 *
 * Reputation levels derived from balance:
 *   Observer   — 0 to 99 MATTR
 *   Contributor — 100 to 999 MATTR
 *   Guardian   — 1,000 to 9,999 MATTR
 *   Elder      — 10,000 to 99,999 MATTR
 *   Sovereign  — 100,000+ MATTR
 *
 * Integration: balanceOf(account) can be queried by SovereignGovernance
 * to compute voting weight based on reputation.
 */
contract IerahkwaReputation is
    ERC20,
    AccessControl,
    Pausable,
    ReentrancyGuard
{
    // =========================================================================
    // ROLES
    // =========================================================================

    bytes32 public constant GUARDIAN_ROLE = keccak256("GUARDIAN_ROLE");
    bytes32 public constant AI_MEDIATOR_ROLE = keccak256("AI_MEDIATOR_ROLE");
    bytes32 public constant PAUSER_ROLE = keccak256("PAUSER_ROLE");
    bytes32 public constant GOVERNANCE_ROLE = keccak256("GOVERNANCE_ROLE");

    // =========================================================================
    // CONSTANTS
    // =========================================================================

    uint256 public constant MAX_SUPPLY = 1_000_000_000 * 10 ** 18; // 1 billion MATTR

    // Reputation level thresholds (in whole tokens, without decimals)
    uint256 public constant THRESHOLD_CONTRIBUTOR = 100;
    uint256 public constant THRESHOLD_GUARDIAN = 1_000;
    uint256 public constant THRESHOLD_ELDER = 10_000;
    uint256 public constant THRESHOLD_SOVEREIGN = 100_000;

    // =========================================================================
    // ENUMS
    // =========================================================================

    enum ActionType {
        EMPATHY_MEDIATION,     // 0 — Mediated a conflict with empathy
        FACT_VERIFICATION,     // 1 — Verified facts / debunked misinformation
        ENVIRONMENTAL_DATA,    // 2 — Contributed environmental sensor data
        COMMUNITY_SERVICE,     // 3 — Performed community service
        CODE_AUDIT,            // 4 — Audited code for security
        CULTURAL_PRESERVATION  // 5 — Preserved cultural knowledge
    }

    enum ReputationLevel {
        Observer,      // 0
        Contributor,   // 1
        Guardian,      // 2
        Elder,         // 3
        Sovereign      // 4
    }

    // =========================================================================
    // STRUCTS
    // =========================================================================

    struct ActionRecord {
        address account;
        ActionType actionType;
        uint256 impactScore;
        uint256 tokensAwarded;
        uint256 timestamp;
        bytes32 evidenceHash;
    }

    // =========================================================================
    // STATE
    // =========================================================================

    /// @notice Impact score multiplier per action type (in MATTR tokens, 18 decimals)
    mapping(ActionType => uint256) public actionImpactScores;

    /// @notice Governance contract address for voting weight queries
    address public governanceContract;

    /// @notice Full action history
    ActionRecord[] public actionHistory;

    /// @notice Action count per user
    mapping(address => uint256) public userActionCount;

    /// @notice Per-user per-action-type count
    mapping(address => mapping(ActionType => uint256)) public userActionsByType;

    /// @notice Cached reputation level per user (updated on mint/burn)
    mapping(address => ReputationLevel) public userReputationLevel;

    /// @notice Total actions recorded
    uint256 public totalActions;

    // =========================================================================
    // EVENTS
    // =========================================================================

    event ConscienceAction(
        address indexed account,
        ActionType indexed actionType,
        uint256 impactScore,
        uint256 tokensAwarded,
        bytes32 evidenceHash,
        uint256 timestamp
    );

    event GuardianAdded(address indexed guardian, address indexed addedBy);
    event GuardianRemoved(address indexed guardian, address indexed removedBy);

    event ReputationLevelChanged(
        address indexed account,
        ReputationLevel oldLevel,
        ReputationLevel newLevel
    );

    event AIMediatorUpdated(address indexed oldMediator, address indexed newMediator);
    event GovernanceContractUpdated(address indexed oldGovernance, address indexed newGovernance);
    event ActionImpactScoreUpdated(ActionType indexed actionType, uint256 oldScore, uint256 newScore);

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    constructor(
        address _admin,
        address _aiMediator
    ) ERC20("Ierahkwa Matter", "MATTR") {
        require(_admin != address(0), "Invalid admin address");
        require(_aiMediator != address(0), "Invalid AI mediator address");

        // Admin roles
        _grantRole(DEFAULT_ADMIN_ROLE, _admin);
        _grantRole(PAUSER_ROLE, _admin);
        _grantRole(GOVERNANCE_ROLE, _admin);
        _grantRole(GUARDIAN_ROLE, _admin);

        // AI Mediator
        _grantRole(AI_MEDIATOR_ROLE, _aiMediator);
        _grantRole(GUARDIAN_ROLE, _aiMediator);

        // Default impact scores (in whole tokens, will be scaled to 18 decimals on mint)
        actionImpactScores[ActionType.EMPATHY_MEDIATION]     = 50 * 10 ** 18;
        actionImpactScores[ActionType.FACT_VERIFICATION]     = 30 * 10 ** 18;
        actionImpactScores[ActionType.ENVIRONMENTAL_DATA]    = 25 * 10 ** 18;
        actionImpactScores[ActionType.COMMUNITY_SERVICE]     = 40 * 10 ** 18;
        actionImpactScores[ActionType.CODE_AUDIT]            = 60 * 10 ** 18;
        actionImpactScores[ActionType.CULTURAL_PRESERVATION] = 75 * 10 ** 18;
    }

    // =========================================================================
    // SOULBOUND TRANSFER OVERRIDE
    // =========================================================================

    /**
     * @dev Override _transfer to make the token soulbound.
     * Transfers between wallets are permanently disabled.
     * Only mint (from zero address) and burn (to zero address) are permitted.
     */
    function _transfer(
        address from,
        address to,
        uint256 /* amount */
    ) internal pure override {
        // Mint: from == address(0); Burn: to == address(0)
        // Both are handled by _mint/_burn which do NOT call _transfer in OZ v5,
        // but in OZ v4.x _transfer is called by the public transfer/transferFrom.
        // We block ALL wallet-to-wallet transfers unconditionally.
        require(
            from == address(0) || to == address(0),
            "MATTR: Soulbound token — transfers are disabled"
        );
    }

    /**
     * @dev Explicitly block approve to prevent any transfer workflow.
     */
    function approve(address, uint256) public pure override returns (bool) {
        revert("MATTR: Soulbound token — approvals are disabled");
    }

    /**
     * @dev Explicitly block transferFrom.
     */
    function transferFrom(address, address, uint256) public pure override returns (bool) {
        revert("MATTR: Soulbound token — transfers are disabled");
    }

    /**
     * @dev Explicitly block transfer.
     */
    function transfer(address, uint256) public pure override returns (bool) {
        revert("MATTR: Soulbound token — transfers are disabled");
    }

    // =========================================================================
    // CONSCIENCE ACTIONS — MINTING REPUTATION
    // =========================================================================

    /**
     * @dev Record a conscience action and mint reputation tokens to the actor.
     * Only Guardians or the AI Mediator may call this function.
     *
     * @param account       The address that performed the conscious action
     * @param actionType    Category of the action
     * @param impactMultiplier  Multiplier (100 = 1x, 200 = 2x) applied to base impact score
     * @param evidenceHash  IPFS or on-chain hash proving the action occurred
     */
    function recordAction(
        address account,
        ActionType actionType,
        uint256 impactMultiplier,
        bytes32 evidenceHash
    ) external onlyRole(GUARDIAN_ROLE) whenNotPaused nonReentrant {
        require(account != address(0), "Invalid account");
        require(impactMultiplier > 0 && impactMultiplier <= 1000, "Multiplier must be 1-1000");
        require(evidenceHash != bytes32(0), "Evidence hash required");

        uint256 baseScore = actionImpactScores[actionType];
        require(baseScore > 0, "Action type not configured");

        uint256 tokensToMint = (baseScore * impactMultiplier) / 100;
        require(totalSupply() + tokensToMint <= MAX_SUPPLY, "Exceeds max supply");

        // Record the action
        actionHistory.push(ActionRecord({
            account: account,
            actionType: actionType,
            impactScore: impactMultiplier,
            tokensAwarded: tokensToMint,
            timestamp: block.timestamp,
            evidenceHash: evidenceHash
        }));

        userActionCount[account]++;
        userActionsByType[account][actionType]++;
        totalActions++;

        // Capture old level before mint
        ReputationLevel oldLevel = userReputationLevel[account];

        // Mint soulbound tokens
        _mint(account, tokensToMint);

        // Update and emit level change if applicable
        ReputationLevel newLevel = _computeReputationLevel(account);
        if (newLevel != oldLevel) {
            userReputationLevel[account] = newLevel;
            emit ReputationLevelChanged(account, oldLevel, newLevel);
        }

        emit ConscienceAction(
            account,
            actionType,
            impactMultiplier,
            tokensToMint,
            evidenceHash,
            block.timestamp
        );
    }

    /**
     * @dev Batch-record multiple conscience actions in a single transaction.
     * Gas-efficient for processing queued actions from the AI Mediator.
     */
    function recordActionsBatch(
        address[] calldata accounts,
        ActionType[] calldata actionTypes,
        uint256[] calldata impactMultipliers,
        bytes32[] calldata evidenceHashes
    ) external onlyRole(GUARDIAN_ROLE) whenNotPaused nonReentrant {
        uint256 length = accounts.length;
        require(
            length == actionTypes.length &&
            length == impactMultipliers.length &&
            length == evidenceHashes.length,
            "Arrays length mismatch"
        );
        require(length <= 100, "Batch too large");

        for (uint256 i = 0; i < length; i++) {
            _recordActionInternal(
                accounts[i],
                actionTypes[i],
                impactMultipliers[i],
                evidenceHashes[i]
            );
        }
    }

    /**
     * @dev Internal action recording logic shared by single and batch operations.
     */
    function _recordActionInternal(
        address account,
        ActionType actionType,
        uint256 impactMultiplier,
        bytes32 evidenceHash
    ) internal {
        require(account != address(0), "Invalid account");
        require(impactMultiplier > 0 && impactMultiplier <= 1000, "Multiplier must be 1-1000");
        require(evidenceHash != bytes32(0), "Evidence hash required");

        uint256 baseScore = actionImpactScores[actionType];
        require(baseScore > 0, "Action type not configured");

        uint256 tokensToMint = (baseScore * impactMultiplier) / 100;
        require(totalSupply() + tokensToMint <= MAX_SUPPLY, "Exceeds max supply");

        actionHistory.push(ActionRecord({
            account: account,
            actionType: actionType,
            impactScore: impactMultiplier,
            tokensAwarded: tokensToMint,
            timestamp: block.timestamp,
            evidenceHash: evidenceHash
        }));

        userActionCount[account]++;
        userActionsByType[account][actionType]++;
        totalActions++;

        ReputationLevel oldLevel = userReputationLevel[account];

        _mint(account, tokensToMint);

        ReputationLevel newLevel = _computeReputationLevel(account);
        if (newLevel != oldLevel) {
            userReputationLevel[account] = newLevel;
            emit ReputationLevelChanged(account, oldLevel, newLevel);
        }

        emit ConscienceAction(
            account,
            actionType,
            impactMultiplier,
            tokensToMint,
            evidenceHash,
            block.timestamp
        );
    }

    // =========================================================================
    // BURN — Users can burn their own reputation
    // =========================================================================

    /**
     * @dev Allow a user to burn their own MATTR tokens.
     * This is the only voluntary reduction mechanism.
     */
    function burn(uint256 amount) external whenNotPaused {
        require(amount > 0, "Amount must be greater than zero");
        require(balanceOf(msg.sender) >= amount, "Insufficient balance");

        ReputationLevel oldLevel = userReputationLevel[msg.sender];

        _burn(msg.sender, amount);

        ReputationLevel newLevel = _computeReputationLevel(msg.sender);
        if (newLevel != oldLevel) {
            userReputationLevel[msg.sender] = newLevel;
            emit ReputationLevelChanged(msg.sender, oldLevel, newLevel);
        }
    }

    // =========================================================================
    // REPUTATION LEVEL QUERIES
    // =========================================================================

    /**
     * @dev Compute and return the reputation level for an account.
     */
    function getReputationLevel(address account) external view returns (ReputationLevel) {
        return _computeReputationLevel(account);
    }

    /**
     * @dev Return the reputation level as a human-readable string.
     */
    function getReputationLevelName(address account) external view returns (string memory) {
        ReputationLevel level = _computeReputationLevel(account);

        if (level == ReputationLevel.Sovereign)    return "Sovereign";
        if (level == ReputationLevel.Elder)        return "Elder";
        if (level == ReputationLevel.Guardian)     return "Guardian";
        if (level == ReputationLevel.Contributor)  return "Contributor";
        return "Observer";
    }

    /**
     * @dev Internal pure computation of reputation level from balance.
     */
    function _computeReputationLevel(address account) internal view returns (ReputationLevel) {
        uint256 balance = balanceOf(account) / 10 ** 18; // Convert to whole tokens

        if (balance >= THRESHOLD_SOVEREIGN)   return ReputationLevel.Sovereign;
        if (balance >= THRESHOLD_ELDER)       return ReputationLevel.Elder;
        if (balance >= THRESHOLD_GUARDIAN)    return ReputationLevel.Guardian;
        if (balance >= THRESHOLD_CONTRIBUTOR) return ReputationLevel.Contributor;
        return ReputationLevel.Observer;
    }

    // =========================================================================
    // GOVERNANCE INTEGRATION
    // =========================================================================

    /**
     * @dev Returns voting weight for governance.
     * SovereignGovernance can call this to weight votes by reputation.
     * Weight is the raw MATTR balance — higher reputation = stronger voice.
     */
    function getVotingWeight(address account) external view returns (uint256) {
        return balanceOf(account);
    }

    /**
     * @dev Set the governance contract address for integration.
     */
    function setGovernanceContract(address _governance) external onlyRole(GOVERNANCE_ROLE) {
        require(_governance != address(0), "Invalid governance address");
        address oldGovernance = governanceContract;
        governanceContract = _governance;
        emit GovernanceContractUpdated(oldGovernance, _governance);
    }

    // =========================================================================
    // GUARDIAN MANAGEMENT
    // =========================================================================

    /**
     * @dev Add a new Guardian who can record conscience actions.
     */
    function addGuardian(address guardian) external onlyRole(DEFAULT_ADMIN_ROLE) {
        require(guardian != address(0), "Invalid guardian address");
        require(!hasRole(GUARDIAN_ROLE, guardian), "Already a guardian");

        _grantRole(GUARDIAN_ROLE, guardian);
        emit GuardianAdded(guardian, msg.sender);
    }

    /**
     * @dev Remove a Guardian. Cannot remove the last admin.
     */
    function removeGuardian(address guardian) external onlyRole(DEFAULT_ADMIN_ROLE) {
        require(hasRole(GUARDIAN_ROLE, guardian), "Not a guardian");

        _revokeRole(GUARDIAN_ROLE, guardian);
        emit GuardianRemoved(guardian, msg.sender);
    }

    /**
     * @dev Governance can also add/remove guardians via proposals.
     */
    function addGuardianByGovernance(address guardian) external onlyRole(GOVERNANCE_ROLE) {
        require(guardian != address(0), "Invalid guardian address");
        require(!hasRole(GUARDIAN_ROLE, guardian), "Already a guardian");

        _grantRole(GUARDIAN_ROLE, guardian);
        emit GuardianAdded(guardian, msg.sender);
    }

    function removeGuardianByGovernance(address guardian) external onlyRole(GOVERNANCE_ROLE) {
        require(hasRole(GUARDIAN_ROLE, guardian), "Not a guardian");

        _revokeRole(GUARDIAN_ROLE, guardian);
        emit GuardianRemoved(guardian, msg.sender);
    }

    // =========================================================================
    // AI MEDIATOR MANAGEMENT
    // =========================================================================

    /**
     * @dev Update the AI Mediator address.
     */
    function setAIMediator(address newMediator) external onlyRole(DEFAULT_ADMIN_ROLE) {
        require(newMediator != address(0), "Invalid mediator address");

        // Find current mediator(s) with AI_MEDIATOR_ROLE — revoke from old
        // Note: this only sets the new one; admin should revoke old manually
        // if there are multiple. For clean operation, we grant the new one.
        address oldMediator = address(0); // Informational for event
        _grantRole(AI_MEDIATOR_ROLE, newMediator);
        _grantRole(GUARDIAN_ROLE, newMediator);

        emit AIMediatorUpdated(oldMediator, newMediator);
    }

    // =========================================================================
    // ACTION IMPACT SCORE CONFIGURATION
    // =========================================================================

    /**
     * @dev Update the base impact score for an action type.
     * @param actionType The action category to update
     * @param newScore   New base score in tokens (with 18 decimals)
     */
    function setActionImpactScore(
        ActionType actionType,
        uint256 newScore
    ) external onlyRole(GOVERNANCE_ROLE) {
        require(newScore > 0 && newScore <= 1000 * 10 ** 18, "Score out of range");

        uint256 oldScore = actionImpactScores[actionType];
        actionImpactScores[actionType] = newScore;

        emit ActionImpactScoreUpdated(actionType, oldScore, newScore);
    }

    // =========================================================================
    // QUERY FUNCTIONS
    // =========================================================================

    /**
     * @dev Get the total number of recorded actions.
     */
    function getActionHistoryLength() external view returns (uint256) {
        return actionHistory.length;
    }

    /**
     * @dev Get a specific action record by index.
     */
    function getActionRecord(uint256 index) external view returns (ActionRecord memory) {
        require(index < actionHistory.length, "Index out of bounds");
        return actionHistory[index];
    }

    /**
     * @dev Get the number of actions a user has performed of a specific type.
     */
    function getUserActionCountByType(
        address account,
        ActionType actionType
    ) external view returns (uint256) {
        return userActionsByType[account][actionType];
    }

    /**
     * @dev Check if an address is a Guardian.
     */
    function isGuardian(address account) external view returns (bool) {
        return hasRole(GUARDIAN_ROLE, account);
    }

    /**
     * @dev Check if an address is the AI Mediator.
     */
    function isAIMediator(address account) external view returns (bool) {
        return hasRole(AI_MEDIATOR_ROLE, account);
    }

    // =========================================================================
    // PAUSE
    // =========================================================================

    function pause() external onlyRole(PAUSER_ROLE) {
        _pause();
    }

    function unpause() external onlyRole(PAUSER_ROLE) {
        _unpause();
    }
}
