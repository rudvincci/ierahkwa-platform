// SPDX-License-Identifier: MIT
// ============================================================================
// SOVEREIGN DIGITAL IDENTITY (SWID) — Identidad Digital Soberana
// Ierahkwa Ne Kanienke — Nacion Digital Soberana
// Blockchain: MameyNode (EVM-compatible)
// ============================================================================

pragma solidity 0.8.24;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Enumerable.sol";
import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/utils/Pausable.sol";

/**
 * @title SovereignID
 * @author Ierahkwa Sovereign Development Council
 * @notice Sovereign Digital Identity (SWID) is a soulbound ERC-721 NFT that
 *         represents a citizen's digital identity within the Ierahkwa sovereign
 *         nation. Each citizen receives exactly one non-transferable identity
 *         token that tracks verification level, nation, and reputation.
 * @dev Soulbound token: transfers are blocked except for admin-initiated
 *      recovery operations. One identity per address enforced.
 *
 * Key features:
 *   - Soulbound (non-transferable) identity tokens
 *   - Multi-level verification system (0-3)
 *   - Nation and country tracking for 574 tribal nations
 *   - Artisan and creator flags for economic roles
 *   - Reputation score system
 *   - Admin recovery for lost wallets
 *
 * Verification Levels:
 *   0 - Unverified: Registered but not verified
 *   1 - Basic: Email/phone verification
 *   2 - Community: Vouched by community members
 *   3 - Sovereign: Full biometric + community verification
 *
 * Roles:
 *   - DEFAULT_ADMIN_ROLE: Full administrative control, wallet recovery
 *   - REGISTRAR_ROLE: Can register new citizens
 *   - VERIFIER_ROLE: Can upgrade verification levels and manage roles
 */
contract SovereignID is
    ERC721,
    ERC721Enumerable,
    AccessControl,
    Pausable
{
    // =========================================================================
    // CONSTANTS
    // =========================================================================

    /// @notice Role for registering new sovereign identities.
    bytes32 public constant REGISTRAR_ROLE = keccak256("REGISTRAR_ROLE");

    /// @notice Role for upgrading verification levels and managing citizen attributes.
    bytes32 public constant VERIFIER_ROLE = keccak256("VERIFIER_ROLE");

    /// @notice Maximum verification level.
    uint8 public constant MAX_VERIFICATION_LEVEL = 3;

    /// @notice Maximum reputation score.
    uint256 public constant MAX_REPUTATION = 10_000;

    // =========================================================================
    // DATA STRUCTURES
    // =========================================================================

    /**
     * @notice On-chain identity data for each sovereign citizen.
     * @param nameHash          keccak256 hash of the citizen's full name (privacy).
     * @param nation            Indigenous nation name (e.g., "Lenca", "Maya", "Navajo").
     * @param country           ISO 3166-1 alpha-2 country code (e.g., "HN", "US", "MX").
     * @param verificationLevel Current verification level (0-3).
     * @param registeredAt      Block timestamp when the identity was created.
     * @param isArtisan         True if the citizen is a registered artisan.
     * @param isCreator         True if the citizen is a registered content creator.
     * @param reputationScore   Reputation score (0-10000).
     */
    struct Identity {
        bytes32 nameHash;
        string nation;
        string country;
        uint8 verificationLevel;
        uint256 registeredAt;
        bool isArtisan;
        bool isCreator;
        uint256 reputationScore;
    }

    // =========================================================================
    // STATE VARIABLES
    // =========================================================================

    /// @notice Monotonically increasing token ID counter.
    uint256 private _nextTokenId;

    /// @notice Mapping from token ID to identity data.
    mapping(uint256 => Identity) private _identities;

    /// @notice Mapping from address to token ID (one identity per address).
    mapping(address => uint256) private _citizenToken;

    /// @notice Mapping to check if an address already has an identity.
    mapping(address => bool) public hasCitizenship;

    /// @notice Mapping from nameHash to boolean (prevent duplicate registrations).
    mapping(bytes32 => bool) public nameHashUsed;

    /// @notice Total registered citizens.
    uint256 public citizenCount;

    /// @notice Base URI for token metadata.
    string private _baseTokenURI;

    // =========================================================================
    // EVENTS
    // =========================================================================

    /// @notice Emitted when a new sovereign identity is registered.
    event CitizenRegistered(
        uint256 indexed tokenId,
        address indexed citizen,
        string nation,
        string country,
        uint256 timestamp
    );

    /// @notice Emitted when a citizen's verification level is upgraded.
    event VerificationUpgraded(
        uint256 indexed tokenId,
        address indexed citizen,
        uint8 oldLevel,
        uint8 newLevel,
        address indexed verifier
    );

    /// @notice Emitted when a citizen's artisan status changes.
    event ArtisanStatusUpdated(
        uint256 indexed tokenId,
        address indexed citizen,
        bool isArtisan
    );

    /// @notice Emitted when a citizen's creator status changes.
    event CreatorStatusUpdated(
        uint256 indexed tokenId,
        address indexed citizen,
        bool isCreator
    );

    /// @notice Emitted when a citizen's reputation score is updated.
    event ReputationUpdated(
        uint256 indexed tokenId,
        address indexed citizen,
        uint256 oldScore,
        uint256 newScore
    );

    /// @notice Emitted when an identity is recovered to a new wallet.
    event IdentityRecovered(
        uint256 indexed tokenId,
        address indexed oldAddress,
        address indexed newAddress
    );

    /// @notice Emitted when an identity is revoked.
    event IdentityRevoked(
        uint256 indexed tokenId,
        address indexed citizen,
        address indexed revokedBy
    );

    // =========================================================================
    // ERRORS
    // =========================================================================

    /// @notice Thrown when providing the zero address.
    error ZeroAddress();

    /// @notice Thrown when a citizen already has an identity.
    error AlreadyRegistered(address citizen);

    /// @notice Thrown when an address has no identity.
    error NotRegistered(address citizen);

    /// @notice Thrown when the nameHash is already used by another citizen.
    error NameHashAlreadyUsed(bytes32 nameHash);

    /// @notice Thrown when the verification level is invalid.
    error InvalidVerificationLevel(uint8 level);

    /// @notice Thrown when attempting to downgrade verification.
    error CannotDowngradeVerification(uint8 current, uint8 proposed);

    /// @notice Thrown when soulbound transfer is attempted.
    error SoulboundTokenNonTransferable();

    /// @notice Thrown when the reputation score exceeds maximum.
    error ReputationTooHigh(uint256 proposed, uint256 maximum);

    /// @notice Thrown when the nation string is empty.
    error EmptyNation();

    /// @notice Thrown when the country string is empty.
    error EmptyCountry();

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    /**
     * @notice Deploys the SovereignID contract.
     * @param _admin   Address that receives all initial roles.
     * @param baseURI  Base URI for token metadata.
     */
    constructor(
        address _admin,
        string memory baseURI
    ) ERC721("Sovereign Digital Identity", "SWID") {
        if (_admin == address(0)) revert ZeroAddress();

        _baseTokenURI = baseURI;

        _grantRole(DEFAULT_ADMIN_ROLE, _admin);
        _grantRole(REGISTRAR_ROLE, _admin);
        _grantRole(VERIFIER_ROLE, _admin);
    }

    // =========================================================================
    // REGISTRATION
    // =========================================================================

    /**
     * @notice Register a new sovereign digital identity for a citizen.
     * @dev One identity per address. nameHash must be unique.
     * @param to        Address of the new citizen.
     * @param nameHash  keccak256 hash of the citizen's legal name.
     * @param nation    Indigenous nation name.
     * @param country   ISO country code.
     * @return tokenId  The newly minted token ID.
     */
    function register(
        address to,
        bytes32 nameHash,
        string calldata nation,
        string calldata country
    ) external onlyRole(REGISTRAR_ROLE) whenNotPaused returns (uint256 tokenId) {
        if (to == address(0)) revert ZeroAddress();
        if (hasCitizenship[to]) revert AlreadyRegistered(to);
        if (nameHashUsed[nameHash]) revert NameHashAlreadyUsed(nameHash);
        if (bytes(nation).length == 0) revert EmptyNation();
        if (bytes(country).length == 0) revert EmptyCountry();

        _nextTokenId++;
        tokenId = _nextTokenId;

        _identities[tokenId] = Identity({
            nameHash: nameHash,
            nation: nation,
            country: country,
            verificationLevel: 0,
            registeredAt: block.timestamp,
            isArtisan: false,
            isCreator: false,
            reputationScore: 0
        });

        hasCitizenship[to] = true;
        _citizenToken[to] = tokenId;
        nameHashUsed[nameHash] = true;
        citizenCount++;

        _safeMint(to, tokenId);

        emit CitizenRegistered(tokenId, to, nation, country, block.timestamp);
    }

    // =========================================================================
    // VERIFICATION
    // =========================================================================

    /**
     * @notice Upgrade a citizen's verification level.
     * @dev Can only upgrade, never downgrade. Level must be 0-3.
     * @param tokenId Token ID of the identity to upgrade.
     * @param level   New verification level.
     */
    function upgradeVerification(
        uint256 tokenId,
        uint8 level
    ) external onlyRole(VERIFIER_ROLE) {
        _requireOwned(tokenId);
        if (level > MAX_VERIFICATION_LEVEL) {
            revert InvalidVerificationLevel(level);
        }

        Identity storage identity = _identities[tokenId];
        if (level <= identity.verificationLevel) {
            revert CannotDowngradeVerification(identity.verificationLevel, level);
        }

        uint8 oldLevel = identity.verificationLevel;
        identity.verificationLevel = level;

        address citizen = ownerOf(tokenId);
        emit VerificationUpgraded(tokenId, citizen, oldLevel, level, msg.sender);
    }

    // =========================================================================
    // CITIZEN ATTRIBUTES
    // =========================================================================

    /**
     * @notice Set or remove artisan status for a citizen.
     * @param tokenId   Token ID of the identity.
     * @param isArtisan New artisan status.
     */
    function setArtisanStatus(
        uint256 tokenId,
        bool isArtisan
    ) external onlyRole(VERIFIER_ROLE) {
        _requireOwned(tokenId);
        Identity storage identity = _identities[tokenId];
        identity.isArtisan = isArtisan;

        emit ArtisanStatusUpdated(tokenId, ownerOf(tokenId), isArtisan);
    }

    /**
     * @notice Set or remove creator status for a citizen.
     * @param tokenId   Token ID of the identity.
     * @param isCreator New creator status.
     */
    function setCreatorStatus(
        uint256 tokenId,
        bool isCreator
    ) external onlyRole(VERIFIER_ROLE) {
        _requireOwned(tokenId);
        Identity storage identity = _identities[tokenId];
        identity.isCreator = isCreator;

        emit CreatorStatusUpdated(tokenId, ownerOf(tokenId), isCreator);
    }

    /**
     * @notice Update a citizen's reputation score.
     * @dev Score is clamped to MAX_REPUTATION (10000).
     * @param tokenId Token ID of the identity.
     * @param score   New reputation score.
     */
    function updateReputation(
        uint256 tokenId,
        uint256 score
    ) external onlyRole(VERIFIER_ROLE) {
        _requireOwned(tokenId);
        if (score > MAX_REPUTATION) revert ReputationTooHigh(score, MAX_REPUTATION);

        Identity storage identity = _identities[tokenId];
        uint256 oldScore = identity.reputationScore;
        identity.reputationScore = score;

        emit ReputationUpdated(tokenId, ownerOf(tokenId), oldScore, score);
    }

    // =========================================================================
    // ADMIN RECOVERY & REVOCATION
    // =========================================================================

    /**
     * @notice Recover an identity to a new wallet address.
     * @dev Only admin can perform this (for lost wallet recovery). This is
     *      the only way to transfer a soulbound token.
     * @param tokenId    Token ID of the identity to recover.
     * @param newAddress New wallet address for the citizen.
     */
    function recoverIdentity(
        uint256 tokenId,
        address newAddress
    ) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (newAddress == address(0)) revert ZeroAddress();
        if (hasCitizenship[newAddress]) revert AlreadyRegistered(newAddress);

        address oldAddress = ownerOf(tokenId);

        // Clear old mappings
        hasCitizenship[oldAddress] = false;
        _citizenToken[oldAddress] = 0;

        // Set new mappings
        hasCitizenship[newAddress] = true;
        _citizenToken[newAddress] = tokenId;

        // Perform the admin-only transfer (bypasses soulbound check via _isAdminTransfer flag)
        _adminTransferInProgress = true;
        _transfer(oldAddress, newAddress, tokenId);
        _adminTransferInProgress = false;

        emit IdentityRecovered(tokenId, oldAddress, newAddress);
    }

    /**
     * @notice Revoke a citizen's identity (burn the token).
     * @dev Only admin can revoke identities. Clears all associated data.
     * @param tokenId Token ID to revoke.
     */
    function revokeIdentity(uint256 tokenId) external onlyRole(DEFAULT_ADMIN_ROLE) {
        address citizen = ownerOf(tokenId);
        Identity storage identity = _identities[tokenId];

        // Clear mappings
        hasCitizenship[citizen] = false;
        _citizenToken[citizen] = 0;
        nameHashUsed[identity.nameHash] = false;
        citizenCount--;

        // Burn the token
        _adminTransferInProgress = true;
        _burn(tokenId);
        _adminTransferInProgress = false;

        // Clear identity data
        delete _identities[tokenId];

        emit IdentityRevoked(tokenId, citizen, msg.sender);
    }

    /// @dev Flag to allow admin transfers/burns to bypass soulbound check.
    bool private _adminTransferInProgress;

    // =========================================================================
    // PAUSE
    // =========================================================================

    /**
     * @notice Pause all registrations.
     */
    function pause() external onlyRole(DEFAULT_ADMIN_ROLE) {
        _pause();
    }

    /**
     * @notice Unpause registrations.
     */
    function unpause() external onlyRole(DEFAULT_ADMIN_ROLE) {
        _unpause();
    }

    // =========================================================================
    // METADATA
    // =========================================================================

    /**
     * @notice Set the base URI for token metadata.
     * @param baseURI New base URI string.
     */
    function setBaseURI(string calldata baseURI) external onlyRole(DEFAULT_ADMIN_ROLE) {
        _baseTokenURI = baseURI;
    }

    /**
     * @dev Returns the base URI for computing tokenURI.
     */
    function _baseURI() internal view override returns (string memory) {
        return _baseTokenURI;
    }

    // =========================================================================
    // VIEW FUNCTIONS
    // =========================================================================

    /**
     * @notice Get the full identity data for a token.
     * @param tokenId Token ID to query.
     * @return The Identity struct for the token.
     */
    function getIdentity(uint256 tokenId) external view returns (Identity memory) {
        _requireOwned(tokenId);
        return _identities[tokenId];
    }

    /**
     * @notice Get the token ID for a citizen's address.
     * @param citizen Address to look up.
     * @return The token ID, or 0 if not registered.
     */
    function tokenOfCitizen(address citizen) external view returns (uint256) {
        return _citizenToken[citizen];
    }

    /**
     * @notice Get the verification level for a citizen.
     * @param citizen Address to check.
     * @return The verification level (0-3).
     */
    function verificationLevelOf(address citizen) external view returns (uint8) {
        uint256 tokenId = _citizenToken[citizen];
        if (tokenId == 0) return 0;
        return _identities[tokenId].verificationLevel;
    }

    /**
     * @notice Check if a citizen is a verified artisan.
     * @param citizen Address to check.
     * @return True if the citizen is a registered artisan.
     */
    function isArtisan(address citizen) external view returns (bool) {
        uint256 tokenId = _citizenToken[citizen];
        if (tokenId == 0) return false;
        return _identities[tokenId].isArtisan;
    }

    /**
     * @notice Check if a citizen is a verified creator.
     * @param citizen Address to check.
     * @return True if the citizen is a registered creator.
     */
    function isCreator(address citizen) external view returns (bool) {
        uint256 tokenId = _citizenToken[citizen];
        if (tokenId == 0) return false;
        return _identities[tokenId].isCreator;
    }

    /**
     * @notice Get the reputation score for a citizen.
     * @param citizen Address to check.
     * @return The reputation score (0-10000).
     */
    function reputationOf(address citizen) external view returns (uint256) {
        uint256 tokenId = _citizenToken[citizen];
        if (tokenId == 0) return 0;
        return _identities[tokenId].reputationScore;
    }

    // =========================================================================
    // SOULBOUND ENFORCEMENT (REQUIRED OVERRIDES)
    // =========================================================================

    /**
     * @dev Override _update to enforce soulbound (non-transferable) behavior.
     *      Allows minting (from == 0), admin recovery, and admin revocation.
     *      Blocks all other transfers.
     */
    function _update(
        address to,
        uint256 tokenId,
        address auth
    ) internal override(ERC721, ERC721Enumerable) returns (address) {
        address from = _ownerOf(tokenId);

        // Allow minting (from == address(0))
        // Allow admin-initiated transfers/burns
        // Block all other transfers (soulbound)
        if (from != address(0) && to != address(0) && !_adminTransferInProgress) {
            revert SoulboundTokenNonTransferable();
        }
        if (from != address(0) && to == address(0) && !_adminTransferInProgress) {
            revert SoulboundTokenNonTransferable();
        }

        return super._update(to, tokenId, auth);
    }

    /**
     * @dev Override _increaseBalance for ERC721Enumerable compatibility.
     */
    function _increaseBalance(
        address account,
        uint128 amount
    ) internal override(ERC721, ERC721Enumerable) {
        super._increaseBalance(account, amount);
    }

    /**
     * @dev Override supportsInterface for AccessControl + ERC721Enumerable.
     */
    function supportsInterface(
        bytes4 interfaceId
    ) public view override(ERC721, ERC721Enumerable, AccessControl) returns (bool) {
        return super.supportsInterface(interfaceId);
    }
}
