// SPDX-License-Identifier: Sovereign-1.0
pragma solidity 0.8.24;

import {ERC721} from "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import {ERC721Enumerable} from "@openzeppelin/contracts/token/ERC721/extensions/ERC721Enumerable.sol";
import {AccessControl} from "@openzeppelin/contracts/access/AccessControl.sol";
import {Pausable} from "@openzeppelin/contracts/utils/Pausable.sol";

/**
 * @title LandRegistry
 * @author Ierahkwa Ne Kanienke — Sovereign Digital Nation
 * @notice Sovereign land registry implemented as soulbound ERC-721 tokens.
 *         Each token represents a registered territory belonging to an indigenous nation.
 *         Tokens are non-transferable (soulbound) except by DEFAULT_ADMIN_ROLE for
 *         recovery or reassignment to tribal councils.
 * @dev Territories are registered by REGISTRAR_ROLE holders and cannot be transferred
 *      by their owners. This ensures immutable association between a land parcel and
 *      its rightful sovereign nation/community.
 *
 *      Coordinates are stored as int256 with 6 decimal places of precision
 *      (e.g., 40123456 represents 40.123456 degrees).
 *
 *      Instead of an on-chain `getTerritoriesByNation()` mapping (which is gas-expensive
 *      for writes and reads), territories are indexed off-chain via the
 *      TerritoryRegistered event.
 */
contract LandRegistry is ERC721, ERC721Enumerable, AccessControl, Pausable {

    // ──────────────────────────────────────────────────────────────
    //  Constants & Roles
    // ──────────────────────────────────────────────────────────────

    /// @notice Role identifier for authorized territory registrars.
    bytes32 public constant REGISTRAR_ROLE = keccak256("REGISTRAR_ROLE");

    /// @notice Contract version.
    string public constant VERSION = "1.0.0";

    // ──────────────────────────────────────────────────────────────
    //  Structs
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Represents a sovereign territory registration.
     * @param name Human-readable name of the territory.
     * @param nation Name of the indigenous nation that owns this territory.
     * @param latitude Latitude in micro-degrees (6 decimal places, e.g., 40123456 = 40.123456).
     * @param longitude Longitude in micro-degrees (6 decimal places).
     * @param hectares Size of the territory in hectares.
     * @param registeredAt Timestamp when the territory was registered on-chain.
     * @param legalHash Keccak256 hash of the legal documentation supporting the claim.
     * @param isProtected Whether this territory has been designated as protected land.
     */
    struct Territory {
        string name;
        string nation;
        int256 latitude;
        int256 longitude;
        uint256 hectares;
        uint256 registeredAt;
        bytes32 legalHash;
        bool isProtected;
    }

    // ──────────────────────────────────────────────────────────────
    //  State Variables
    // ──────────────────────────────────────────────────────────────

    /// @notice Auto-incrementing token ID counter.
    uint256 private _nextTokenId;

    /// @notice Territory metadata keyed by token ID.
    mapping(uint256 => Territory) public territories;

    /// @notice Base URI for token metadata.
    string private _baseTokenURI;

    /// @notice Total area registered across all territories (in hectares).
    uint256 public totalHectaresRegistered;

    /// @notice Count of territories designated as protected.
    uint256 public protectedTerritoryCount;

    // ──────────────────────────────────────────────────────────────
    //  Events
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Emitted when a new territory is registered.
     * @param tokenId Token ID assigned to the territory.
     * @param owner Address that owns this territory token.
     * @param name Name of the territory.
     * @param nation Name of the indigenous nation.
     * @param latitude Latitude in micro-degrees.
     * @param longitude Longitude in micro-degrees.
     * @param hectares Size in hectares.
     * @param legalHash Hash of legal documentation.
     */
    event TerritoryRegistered(
        uint256 indexed tokenId,
        address indexed owner,
        string name,
        string indexed nation,
        int256 latitude,
        int256 longitude,
        uint256 hectares,
        bytes32 legalHash
    );

    /**
     * @notice Emitted when a territory's protection status changes.
     * @param tokenId Token ID of the territory.
     * @param isProtected New protection status.
     */
    event TerritoryProtected(
        uint256 indexed tokenId,
        bool isProtected
    );

    /**
     * @notice Emitted when a territory is reassigned by admin (recovery/tribal council transfer).
     * @param tokenId Token ID of the territory.
     * @param from Previous owner.
     * @param to New owner.
     * @param reason Reason for the administrative transfer.
     */
    event TerritoryReassigned(
        uint256 indexed tokenId,
        address indexed from,
        address indexed to,
        string reason
    );

    // ──────────────────────────────────────────────────────────────
    //  Errors
    // ──────────────────────────────────────────────────────────────

    error SoulboundTransferBlocked(uint256 tokenId);
    error TerritoryNotFound(uint256 tokenId);
    error InvalidHectares(uint256 hectares);
    error ZeroAddress();
    error EmptyName();
    error EmptyNation();

    // ──────────────────────────────────────────────────────────────
    //  Constructor
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Initializes the LandRegistry contract.
     * @param admin Address granted DEFAULT_ADMIN_ROLE.
     * @param baseURI Base URI for token metadata (e.g., "https://registry.soberano.bo/land/").
     */
    constructor(
        address admin,
        string memory baseURI
    ) ERC721("Sovereign Land Registry", "LAND") {
        if (admin == address(0)) revert ZeroAddress();

        _grantRole(DEFAULT_ADMIN_ROLE, admin);
        _grantRole(REGISTRAR_ROLE, admin);
        _baseTokenURI = baseURI;
    }

    // ──────────────────────────────────────────────────────────────
    //  Territory Registration
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Registers a new territory as a soulbound NFT.
     * @dev Only callable by REGISTRAR_ROLE. The token is minted to the specified address
     *      and cannot be transferred by the owner (soulbound).
     * @param to Address to mint the territory token to (typically a tribal council multisig).
     * @param name Human-readable name of the territory.
     * @param nation Name of the indigenous nation.
     * @param latitude Latitude in micro-degrees (6 decimal places).
     * @param longitude Longitude in micro-degrees (6 decimal places).
     * @param hectares Size of the territory in hectares (must be > 0).
     * @param legalHash Keccak256 hash of the legal documentation.
     * @return tokenId The ID of the newly minted territory token.
     */
    function registerTerritory(
        address to,
        string calldata name,
        string calldata nation,
        int256 latitude,
        int256 longitude,
        uint256 hectares,
        bytes32 legalHash
    ) external onlyRole(REGISTRAR_ROLE) whenNotPaused returns (uint256 tokenId) {
        if (to == address(0)) revert ZeroAddress();
        if (bytes(name).length == 0) revert EmptyName();
        if (bytes(nation).length == 0) revert EmptyNation();
        if (hectares == 0) revert InvalidHectares(hectares);

        tokenId = _nextTokenId;
        _nextTokenId++;

        _safeMint(to, tokenId);

        territories[tokenId] = Territory({
            name: name,
            nation: nation,
            latitude: latitude,
            longitude: longitude,
            hectares: hectares,
            registeredAt: block.timestamp,
            legalHash: legalHash,
            isProtected: false
        });

        totalHectaresRegistered += hectares;

        emit TerritoryRegistered(
            tokenId,
            to,
            name,
            nation,
            latitude,
            longitude,
            hectares,
            legalHash
        );
    }

    /**
     * @notice Marks a territory as protected or removes protection.
     * @dev Only callable by REGISTRAR_ROLE.
     * @param tokenId Token ID of the territory.
     * @param _isProtected Whether to mark as protected (true) or unprotected (false).
     */
    function setProtected(
        uint256 tokenId,
        bool _isProtected
    ) external onlyRole(REGISTRAR_ROLE) {
        if (territories[tokenId].registeredAt == 0) revert TerritoryNotFound(tokenId);

        bool currentlyProtected = territories[tokenId].isProtected;
        if (currentlyProtected != _isProtected) {
            territories[tokenId].isProtected = _isProtected;

            if (_isProtected) {
                protectedTerritoryCount++;
            } else {
                protectedTerritoryCount--;
            }

            emit TerritoryProtected(tokenId, _isProtected);
        }
    }

    /**
     * @notice Administrative transfer of a territory token for recovery or tribal council
     *         reassignment. Only callable by DEFAULT_ADMIN_ROLE.
     * @param tokenId Token ID of the territory to reassign.
     * @param to New owner address.
     * @param reason Documented reason for the administrative transfer.
     */
    function adminReassign(
        uint256 tokenId,
        address to,
        string calldata reason
    ) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (to == address(0)) revert ZeroAddress();
        if (territories[tokenId].registeredAt == 0) revert TerritoryNotFound(tokenId);

        address from = ownerOf(tokenId);
        // Use internal _update to bypass soulbound restriction
        _update(to, tokenId, address(0));

        emit TerritoryReassigned(tokenId, from, to, reason);
    }

    // ──────────────────────────────────────────────────────────────
    //  View Functions
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Returns the full territory metadata for a token.
     * @param tokenId Token ID to query.
     * @return territory The Territory struct for the given token.
     */
    function getTerritory(uint256 tokenId) external view returns (Territory memory territory) {
        if (territories[tokenId].registeredAt == 0) revert TerritoryNotFound(tokenId);
        return territories[tokenId];
    }

    /**
     * @notice Returns the total number of registered territories.
     * @return count The total supply of territory tokens.
     */
    function totalTerritories() external view returns (uint256 count) {
        return totalSupply();
    }

    // ──────────────────────────────────────────────────────────────
    //  Soulbound Override
    // ──────────────────────────────────────────────────────────────

    /**
     * @dev Override _update to enforce soulbound behavior.
     *      Tokens can only be:
     *      - Minted (from == address(0))
     *      - Transferred by adminReassign (auth == address(0), internal call)
     *      All other transfers are blocked.
     */
    function _update(
        address to,
        uint256 tokenId,
        address auth
    ) internal override(ERC721, ERC721Enumerable) returns (address) {
        address from = _ownerOf(tokenId);

        // Allow minting (from == address(0))
        if (from == address(0)) {
            return super._update(to, tokenId, auth);
        }

        // Allow admin reassignment (auth == address(0), called from adminReassign)
        if (auth == address(0)) {
            return super._update(to, tokenId, auth);
        }

        // Block all other transfers (soulbound)
        revert SoulboundTransferBlocked(tokenId);
    }

    // ──────────────────────────────────────────────────────────────
    //  Admin Functions
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Updates the base URI for token metadata.
     * @param baseURI New base URI string.
     */
    function setBaseURI(string calldata baseURI) external onlyRole(DEFAULT_ADMIN_ROLE) {
        _baseTokenURI = baseURI;
    }

    /**
     * @notice Pauses all territory registration and transfers.
     */
    function pause() external onlyRole(DEFAULT_ADMIN_ROLE) {
        _pause();
    }

    /**
     * @notice Unpauses all operations.
     */
    function unpause() external onlyRole(DEFAULT_ADMIN_ROLE) {
        _unpause();
    }

    // ──────────────────────────────────────────────────────────────
    //  Required Overrides
    // ──────────────────────────────────────────────────────────────

    /// @dev Returns the base URI for computing tokenURI.
    function _baseURI() internal view override returns (string memory) {
        return _baseTokenURI;
    }

    /// @inheritdoc ERC721Enumerable
    function _increaseBalance(
        address account,
        uint128 value
    ) internal override(ERC721, ERC721Enumerable) {
        super._increaseBalance(account, value);
    }

    /// @inheritdoc ERC721
    function supportsInterface(
        bytes4 interfaceId
    ) public view override(ERC721, ERC721Enumerable, AccessControl) returns (bool) {
        return super.supportsInterface(interfaceId);
    }
}
