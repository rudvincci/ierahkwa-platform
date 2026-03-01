// SPDX-License-Identifier: MIT
// ============================================================================
// IERAHKWA VERITAS PROTOCOL -- On-Chain Fact-Checking & Evidence Certification
// Decentralized truth verification layer for the IERAHKWA sovereign ecosystem.
// Guardians and AI agents certify evidence on-chain; community members verify
// it through a threshold-based consensus mechanism.
// ============================================================================

pragma solidity ^0.8.20;

import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/security/Pausable.sol";
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";

/**
 * @title IerahkwaVeritas
 * @author Ierahkwa Ne Kanienke -- Sovereign Digital Nation
 * @notice Decentralized evidence certification and fact-checking protocol.
 *
 * Evidence lifecycle:
 *   1. A Guardian (or AI agent) calls `certifyEvidence` to register an IPFS
 *      hash along with metadata (content type, AI verification flag).
 *   2. Community members call `verifyEvidence` to add their confirmation.
 *   3. Once `verificationCount >= VERIFICATION_THRESHOLD`, the evidence is
 *      considered community-verified and `isVerified` returns true.
 *
 * The contract uses AccessControl to manage Guardian privileges. Only
 * addresses with `GUARDIAN_ROLE` may certify new evidence. Any address
 * may verify existing evidence, but each address may only verify a given
 * piece of evidence once.
 *
 * @dev Evidence IDs are derived as `keccak256(abi.encodePacked(ipfsHash))`.
 *      This means a given IPFS hash can only be certified once.
 */
contract IerahkwaVeritas is AccessControl, Pausable, ReentrancyGuard {

    // =========================================================================
    // ROLES
    // =========================================================================

    /// @notice Role identifier for Guardians who can certify evidence.
    bytes32 public constant GUARDIAN_ROLE = keccak256("GUARDIAN_ROLE");

    /// @notice Role identifier for the AI Mediator agent.
    bytes32 public constant AI_MEDIATOR_ROLE = keccak256("AI_MEDIATOR_ROLE");

    /// @notice Role identifier for pause/unpause operations.
    bytes32 public constant PAUSER_ROLE = keccak256("PAUSER_ROLE");

    // =========================================================================
    // CONSTANTS
    // =========================================================================

    /// @notice Minimum number of community verifications required for an
    ///         evidence entry to be considered verified.
    uint256 public constant VERIFICATION_THRESHOLD = 5;

    // =========================================================================
    // STRUCTS
    // =========================================================================

    /// @notice Represents a single piece of certified evidence.
    /// @param ipfsHash       IPFS content-identifier (CID) of the evidence payload.
    /// @param guardian       Address of the Guardian who certified the evidence.
    /// @param timestamp      Block timestamp at the time of certification.
    /// @param aiVerified     Whether the evidence was pre-verified by an AI agent.
    /// @param verificationCount Number of community verifications received.
    /// @param contentType    MIME-like descriptor of the evidence (e.g. "text/article",
    ///                       "image/satellite", "video/testimony").
    struct Evidence {
        string ipfsHash;
        address guardian;
        uint256 timestamp;
        bool aiVerified;
        uint256 verificationCount;
        string contentType;
    }

    // =========================================================================
    // STATE
    // =========================================================================

    /// @notice Mapping from evidence ID to its Evidence record.
    mapping(bytes32 => Evidence) private _evidence;

    /// @notice Tracks whether an evidence ID has been registered.
    mapping(bytes32 => bool) public evidenceExists;

    /// @notice Tracks whether a given address has already verified a given evidence ID.
    /// @dev evidenceId => verifier address => bool
    mapping(bytes32 => mapping(address => bool)) public hasVerified;

    /// @notice Array of all evidence IDs, for enumeration.
    bytes32[] public evidenceIds;

    /// @notice Total number of evidence entries certified.
    uint256 public totalEvidenceCount;

    // =========================================================================
    // EVENTS
    // =========================================================================

    /// @notice Emitted when a new piece of evidence is certified on-chain.
    /// @param evidenceId   Unique identifier for the evidence (keccak256 of IPFS hash).
    /// @param ipfsHash     IPFS CID of the evidence.
    /// @param guardian     Address of the certifying Guardian.
    /// @param aiVerified   Whether AI pre-verification was performed.
    /// @param contentType  Content type descriptor.
    /// @param timestamp    Block timestamp of certification.
    event EvidenceCertified(
        bytes32 indexed evidenceId,
        string ipfsHash,
        address indexed guardian,
        bool aiVerified,
        string contentType,
        uint256 timestamp
    );

    /// @notice Emitted when a community member verifies an existing evidence entry.
    /// @param evidenceId         Unique identifier for the evidence.
    /// @param verifier           Address that performed the verification.
    /// @param newVerificationCount Updated total verification count.
    /// @param thresholdReached   True if this verification caused the threshold to be met.
    event EvidenceVerified(
        bytes32 indexed evidenceId,
        address indexed verifier,
        uint256 newVerificationCount,
        bool thresholdReached
    );

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    /**
     * @notice Deploys the Veritas Protocol.
     * @param _admin      Address that will receive DEFAULT_ADMIN_ROLE and GUARDIAN_ROLE.
     * @param _aiMediator Address of the AI Mediator agent (receives GUARDIAN and AI_MEDIATOR roles).
     */
    constructor(address _admin, address _aiMediator) {
        require(_admin != address(0), "Veritas: admin is zero address");
        require(_aiMediator != address(0), "Veritas: AI mediator is zero address");

        _grantRole(DEFAULT_ADMIN_ROLE, _admin);
        _grantRole(GUARDIAN_ROLE, _admin);
        _grantRole(PAUSER_ROLE, _admin);

        _grantRole(AI_MEDIATOR_ROLE, _aiMediator);
        _grantRole(GUARDIAN_ROLE, _aiMediator);
    }

    // =========================================================================
    // EVIDENCE CERTIFICATION
    // =========================================================================

    /**
     * @notice Certify a new piece of evidence on-chain.
     * @dev Only callable by addresses with GUARDIAN_ROLE. The evidence ID is
     *      derived deterministically from `_ipfsHash`, so the same hash cannot
     *      be certified twice.
     *
     * @param _ipfsHash    IPFS content-identifier of the evidence.
     * @param _aiVerified  True if the evidence was pre-verified by an AI agent.
     * @param _contentType MIME-like content type descriptor.
     * @return evidenceId  The computed unique identifier for the evidence.
     */
    function certifyEvidence(
        string calldata _ipfsHash,
        bool _aiVerified,
        string calldata _contentType
    )
        external
        onlyRole(GUARDIAN_ROLE)
        whenNotPaused
        nonReentrant
        returns (bytes32 evidenceId)
    {
        require(bytes(_ipfsHash).length > 0, "Veritas: empty IPFS hash");
        require(bytes(_contentType).length > 0, "Veritas: empty content type");

        evidenceId = keccak256(abi.encodePacked(_ipfsHash));
        require(!evidenceExists[evidenceId], "Veritas: evidence already certified");

        _evidence[evidenceId] = Evidence({
            ipfsHash: _ipfsHash,
            guardian: msg.sender,
            timestamp: block.timestamp,
            aiVerified: _aiVerified,
            verificationCount: 0,
            contentType: _contentType
        });

        evidenceExists[evidenceId] = true;
        evidenceIds.push(evidenceId);
        totalEvidenceCount++;

        emit EvidenceCertified(
            evidenceId,
            _ipfsHash,
            msg.sender,
            _aiVerified,
            _contentType,
            block.timestamp
        );
    }

    // =========================================================================
    // COMMUNITY VERIFICATION
    // =========================================================================

    /**
     * @notice Verify an existing piece of evidence.
     * @dev Any address may call this function, but each address may only
     *      verify a specific evidence entry once. The certifying Guardian
     *      cannot verify their own evidence.
     *
     * @param _evidenceId The unique identifier of the evidence to verify.
     */
    function verifyEvidence(bytes32 _evidenceId)
        external
        whenNotPaused
        nonReentrant
    {
        require(evidenceExists[_evidenceId], "Veritas: evidence does not exist");
        require(!hasVerified[_evidenceId][msg.sender], "Veritas: already verified by caller");
        require(
            _evidence[_evidenceId].guardian != msg.sender,
            "Veritas: guardian cannot verify own evidence"
        );

        hasVerified[_evidenceId][msg.sender] = true;
        _evidence[_evidenceId].verificationCount++;

        uint256 newCount = _evidence[_evidenceId].verificationCount;
        bool thresholdReached = newCount >= VERIFICATION_THRESHOLD;

        emit EvidenceVerified(
            _evidenceId,
            msg.sender,
            newCount,
            thresholdReached
        );
    }

    // =========================================================================
    // VIEW FUNCTIONS
    // =========================================================================

    /**
     * @notice Retrieve all details for a given evidence entry.
     * @param _evidenceId The unique identifier of the evidence.
     * @return The Evidence struct containing all stored fields.
     */
    function getEvidence(bytes32 _evidenceId)
        external
        view
        returns (Evidence memory)
    {
        require(evidenceExists[_evidenceId], "Veritas: evidence does not exist");
        return _evidence[_evidenceId];
    }

    /**
     * @notice Check whether an evidence entry has reached the community
     *         verification threshold.
     * @param _evidenceId The unique identifier of the evidence.
     * @return True if `verificationCount >= VERIFICATION_THRESHOLD`.
     */
    function isVerified(bytes32 _evidenceId) external view returns (bool) {
        require(evidenceExists[_evidenceId], "Veritas: evidence does not exist");
        return _evidence[_evidenceId].verificationCount >= VERIFICATION_THRESHOLD;
    }

    /**
     * @notice Return the total number of evidence entries registered.
     * @return The length of the evidenceIds array.
     */
    function getEvidenceCount() external view returns (uint256) {
        return evidenceIds.length;
    }

    /**
     * @notice Get an evidence ID by its index in the enumeration array.
     * @param _index The zero-based index.
     * @return The evidence ID at that index.
     */
    function getEvidenceIdByIndex(uint256 _index) external view returns (bytes32) {
        require(_index < evidenceIds.length, "Veritas: index out of bounds");
        return evidenceIds[_index];
    }

    /**
     * @notice Compute the evidence ID for a given IPFS hash without registering it.
     * @dev Useful for off-chain tooling to predict evidence IDs before certification.
     * @param _ipfsHash The IPFS content-identifier.
     * @return The keccak256 hash of the packed IPFS hash string.
     */
    function computeEvidenceId(string calldata _ipfsHash)
        external
        pure
        returns (bytes32)
    {
        return keccak256(abi.encodePacked(_ipfsHash));
    }

    // =========================================================================
    // GUARDIAN MANAGEMENT
    // =========================================================================

    /**
     * @notice Grant GUARDIAN_ROLE to a new address.
     * @param _guardian The address to promote.
     */
    function addGuardian(address _guardian) external onlyRole(DEFAULT_ADMIN_ROLE) {
        require(_guardian != address(0), "Veritas: guardian is zero address");
        require(!hasRole(GUARDIAN_ROLE, _guardian), "Veritas: already a guardian");
        _grantRole(GUARDIAN_ROLE, _guardian);
    }

    /**
     * @notice Revoke GUARDIAN_ROLE from an address.
     * @param _guardian The address to demote.
     */
    function removeGuardian(address _guardian) external onlyRole(DEFAULT_ADMIN_ROLE) {
        require(hasRole(GUARDIAN_ROLE, _guardian), "Veritas: not a guardian");
        _revokeRole(GUARDIAN_ROLE, _guardian);
    }

    // =========================================================================
    // PAUSE
    // =========================================================================

    /// @notice Pause all certification and verification operations.
    function pause() external onlyRole(PAUSER_ROLE) {
        _pause();
    }

    /// @notice Resume operations.
    function unpause() external onlyRole(PAUSER_ROLE) {
        _unpause();
    }
}
