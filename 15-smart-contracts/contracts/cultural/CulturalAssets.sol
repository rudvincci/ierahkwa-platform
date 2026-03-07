// SPDX-License-Identifier: Sovereign-1.0
pragma solidity 0.8.24;

import {ERC1155} from "@openzeppelin/contracts/token/ERC1155/ERC1155.sol";
import {ERC1155Supply} from "@openzeppelin/contracts/token/ERC1155/extensions/ERC1155Supply.sol";
import {AccessControl} from "@openzeppelin/contracts/access/AccessControl.sol";
import {Pausable} from "@openzeppelin/contracts/utils/Pausable.sol";
import {IERC2981} from "@openzeppelin/contracts/interfaces/IERC2981.sol";
import {IERC165} from "@openzeppelin/contracts/utils/introspection/IERC165.sol";

/**
 * @title CulturalAssets
 * @author Ierahkwa Ne Kanienke — Sovereign Digital Nation
 * @notice ERC-1155 multi-token contract for cultural assets with EIP-2981 royalty support.
 *         Represents indigenous cultural items such as artifacts, music, art, documents,
 *         ceremonial objects, textiles, pottery, and jewelry.
 * @dev Each token ID maps to an AssetMetadata struct containing the original creator,
 *      asset category, and per-token royalty configuration. The contract implements
 *      IERC2981 so that marketplaces can query and honor royalty payments.
 *
 *      Default royalty: 10% (1,000 basis points) to the original creator.
 *      Royalty can be customized per token at mint time.
 */
contract CulturalAssets is ERC1155, ERC1155Supply, AccessControl, Pausable, IERC2981 {

    // ──────────────────────────────────────────────────────────────
    //  Constants & Roles
    // ──────────────────────────────────────────────────────────────

    /// @notice Role identifier for authorized cultural asset creators/minters.
    bytes32 public constant CREATOR_ROLE = keccak256("CREATOR_ROLE");

    /// @notice Contract version.
    string public constant VERSION = "1.0.0";

    /// @notice Default royalty in basis points (10% = 1,000 bps).
    uint96 public constant DEFAULT_ROYALTY_BPS = 1_000;

    /// @notice Maximum royalty in basis points (50% = 5,000 bps).
    uint96 public constant MAX_ROYALTY_BPS = 5_000;

    /// @notice Basis points denominator (100% = 10,000).
    uint96 public constant BPS_DENOMINATOR = 10_000;

    // ──────────────────────────────────────────────────────────────
    //  Structs
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Metadata for a cultural asset token.
     * @param creator Address of the original creator (royalty recipient).
     * @param category Category of the cultural asset (e.g., "artifact", "music").
     * @param royaltyBps Royalty percentage in basis points for secondary sales.
     * @param createdAt Timestamp when the asset was first minted.
     */
    struct AssetMetadata {
        address creator;
        string category;
        uint96 royaltyBps;
        uint256 createdAt;
    }

    /**
     * @notice Default royalty configuration used when no per-token royalty is set.
     * @param receiver Address that receives the default royalty.
     * @param feeNumerator Royalty percentage in basis points.
     */
    struct RoyaltyInfo {
        address receiver;
        uint96 feeNumerator;
    }

    // ──────────────────────────────────────────────────────────────
    //  State Variables
    // ──────────────────────────────────────────────────────────────

    /// @notice Metadata for each token ID.
    mapping(uint256 => AssetMetadata) public assetMetadata;

    /// @notice Set of valid cultural asset categories.
    mapping(string => bool) public validCategories;

    /// @notice Array of all valid category names (for enumeration).
    string[] private _categoryList;

    /// @notice Default royalty configuration.
    RoyaltyInfo private _defaultRoyalty;

    /// @notice Contract-level metadata URI (for OpenSea-style contract info).
    string public name;

    /// @notice Contract symbol.
    string public symbol;

    /// @notice Total number of unique asset types created.
    uint256 public totalAssetTypes;

    // ──────────────────────────────────────────────────────────────
    //  Events
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Emitted when a new cultural asset is minted.
     * @param tokenId Token ID of the minted asset.
     * @param creator Address of the original creator.
     * @param category Category of the asset.
     * @param amount Number of tokens minted.
     * @param royaltyBps Royalty basis points for this token.
     */
    event CulturalAssetMinted(
        uint256 indexed tokenId,
        address indexed creator,
        string category,
        uint256 amount,
        uint96 royaltyBps
    );

    /**
     * @notice Emitted when a new category is added.
     * @param category Name of the added category.
     */
    event CategoryAdded(string category);

    /**
     * @notice Emitted when the default royalty configuration is updated.
     * @param receiver New default royalty receiver.
     * @param feeNumerator New default royalty in basis points.
     */
    event DefaultRoyaltyUpdated(address indexed receiver, uint96 feeNumerator);

    // ──────────────────────────────────────────────────────────────
    //  Errors
    // ──────────────────────────────────────────────────────────────

    error InvalidCategory(string category);
    error CategoryAlreadyExists(string category);
    error RoyaltyTooHigh(uint96 royaltyBps);
    error AssetAlreadyExists(uint256 tokenId);
    error ArrayLengthMismatch();
    error ZeroAddress();
    error ZeroAmount();
    error EmptyCategory();

    // ──────────────────────────────────────────────────────────────
    //  Constructor
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Initializes the CulturalAssets contract with default categories and royalty.
     * @param admin Address granted DEFAULT_ADMIN_ROLE and initial CREATOR_ROLE.
     * @param uri Base URI for token metadata (e.g., "https://assets.soberano.bo/cultural/{id}.json").
     * @param defaultRoyaltyReceiver Address that receives royalties when no per-token config exists.
     */
    constructor(
        address admin,
        string memory uri,
        address defaultRoyaltyReceiver
    ) ERC1155(uri) {
        if (admin == address(0)) revert ZeroAddress();
        if (defaultRoyaltyReceiver == address(0)) revert ZeroAddress();

        name = "Sovereign Cultural Assets";
        symbol = "CULTURE";

        _grantRole(DEFAULT_ADMIN_ROLE, admin);
        _grantRole(CREATOR_ROLE, admin);

        // Set default royalty
        _defaultRoyalty = RoyaltyInfo({
            receiver: defaultRoyaltyReceiver,
            feeNumerator: DEFAULT_ROYALTY_BPS
        });

        // Register default categories
        string[8] memory defaultCategories = [
            "artifact",
            "music",
            "art",
            "document",
            "ceremony",
            "textile",
            "pottery",
            "jewelry"
        ];

        for (uint256 i = 0; i < defaultCategories.length; i++) {
            validCategories[defaultCategories[i]] = true;
            _categoryList.push(defaultCategories[i]);
            emit CategoryAdded(defaultCategories[i]);
        }
    }

    // ──────────────────────────────────────────────────────────────
    //  Minting
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Mints a new cultural asset token.
     * @dev Only callable by CREATOR_ROLE. The token ID must not already exist.
     *      The caller becomes the asset's creator and royalty recipient.
     * @param to Address to mint tokens to.
     * @param tokenId Token ID for the new asset.
     * @param amount Number of tokens to mint.
     * @param category Cultural category (must be a valid registered category).
     * @param royaltyBps Royalty in basis points for secondary sales (max 50%).
     * @param data Additional data passed to the ERC-1155 receiver hook.
     */
    function mint(
        address to,
        uint256 tokenId,
        uint256 amount,
        string calldata category,
        uint96 royaltyBps,
        bytes calldata data
    ) external onlyRole(CREATOR_ROLE) whenNotPaused {
        if (to == address(0)) revert ZeroAddress();
        if (amount == 0) revert ZeroAmount();
        if (!validCategories[category]) revert InvalidCategory(category);
        if (royaltyBps > MAX_ROYALTY_BPS) revert RoyaltyTooHigh(royaltyBps);
        if (assetMetadata[tokenId].createdAt != 0) revert AssetAlreadyExists(tokenId);

        assetMetadata[tokenId] = AssetMetadata({
            creator: msg.sender,
            category: category,
            royaltyBps: royaltyBps == 0 ? DEFAULT_ROYALTY_BPS : royaltyBps,
            createdAt: block.timestamp
        });

        totalAssetTypes++;

        _mint(to, tokenId, amount, data);

        emit CulturalAssetMinted(
            tokenId,
            msg.sender,
            category,
            amount,
            royaltyBps == 0 ? DEFAULT_ROYALTY_BPS : royaltyBps
        );
    }

    /**
     * @notice Batch-mints multiple cultural asset tokens.
     * @dev Only callable by CREATOR_ROLE. All arrays must have the same length.
     *      Each token ID must not already exist.
     * @param to Address to mint tokens to.
     * @param tokenIds Array of token IDs.
     * @param amounts Array of amounts per token ID.
     * @param categories Array of category strings per token ID.
     * @param royaltyBpsArray Array of royalty basis points per token ID.
     * @param data Additional data passed to the ERC-1155 receiver hook.
     */
    function mintBatch(
        address to,
        uint256[] calldata tokenIds,
        uint256[] calldata amounts,
        string[] calldata categories,
        uint96[] calldata royaltyBpsArray,
        bytes calldata data
    ) external onlyRole(CREATOR_ROLE) whenNotPaused {
        if (to == address(0)) revert ZeroAddress();
        if (
            tokenIds.length != amounts.length ||
            tokenIds.length != categories.length ||
            tokenIds.length != royaltyBpsArray.length
        ) {
            revert ArrayLengthMismatch();
        }

        for (uint256 i = 0; i < tokenIds.length; i++) {
            if (amounts[i] == 0) revert ZeroAmount();
            if (!validCategories[categories[i]]) revert InvalidCategory(categories[i]);
            if (royaltyBpsArray[i] > MAX_ROYALTY_BPS) revert RoyaltyTooHigh(royaltyBpsArray[i]);
            if (assetMetadata[tokenIds[i]].createdAt != 0) revert AssetAlreadyExists(tokenIds[i]);

            uint96 effectiveRoyalty = royaltyBpsArray[i] == 0
                ? DEFAULT_ROYALTY_BPS
                : royaltyBpsArray[i];

            assetMetadata[tokenIds[i]] = AssetMetadata({
                creator: msg.sender,
                category: categories[i],
                royaltyBps: effectiveRoyalty,
                createdAt: block.timestamp
            });

            totalAssetTypes++;

            emit CulturalAssetMinted(
                tokenIds[i],
                msg.sender,
                categories[i],
                amounts[i],
                effectiveRoyalty
            );
        }

        _mintBatch(to, tokenIds, amounts, data);
    }

    // ──────────────────────────────────────────────────────────────
    //  EIP-2981 Royalty Info
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Returns royalty information for a token sale as per EIP-2981.
     * @dev If the token has custom metadata, returns the original creator as receiver
     *      with the token-specific royalty. Otherwise, returns the default royalty config.
     * @param tokenId Token ID being sold.
     * @param salePrice Sale price of the token (in the payment token's smallest unit).
     * @return receiver Address that should receive the royalty payment.
     * @return royaltyAmount Amount of royalty to pay.
     */
    function royaltyInfo(
        uint256 tokenId,
        uint256 salePrice
    ) external view override returns (address receiver, uint256 royaltyAmount) {
        AssetMetadata storage meta = assetMetadata[tokenId];

        if (meta.createdAt != 0) {
            // Per-token royalty
            receiver = meta.creator;
            royaltyAmount = (salePrice * meta.royaltyBps) / BPS_DENOMINATOR;
        } else {
            // Default royalty
            receiver = _defaultRoyalty.receiver;
            royaltyAmount = (salePrice * _defaultRoyalty.feeNumerator) / BPS_DENOMINATOR;
        }
    }

    // ──────────────────────────────────────────────────────────────
    //  Category Management
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Adds a new valid cultural asset category.
     * @dev Only callable by DEFAULT_ADMIN_ROLE.
     * @param category Name of the category to add.
     */
    function addCategory(string calldata category) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (bytes(category).length == 0) revert EmptyCategory();
        if (validCategories[category]) revert CategoryAlreadyExists(category);

        validCategories[category] = true;
        _categoryList.push(category);

        emit CategoryAdded(category);
    }

    /**
     * @notice Returns all registered categories.
     * @return categories Array of category name strings.
     */
    function getCategories() external view returns (string[] memory categories) {
        return _categoryList;
    }

    // ──────────────────────────────────────────────────────────────
    //  Admin Functions
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Updates the default royalty configuration.
     * @dev Only callable by DEFAULT_ADMIN_ROLE. Does not affect existing per-token royalties.
     * @param receiver Address that receives the default royalty.
     * @param feeNumerator Royalty percentage in basis points (max 50%).
     */
    function setDefaultRoyalty(
        address receiver,
        uint96 feeNumerator
    ) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (receiver == address(0)) revert ZeroAddress();
        if (feeNumerator > MAX_ROYALTY_BPS) revert RoyaltyTooHigh(feeNumerator);

        _defaultRoyalty = RoyaltyInfo({
            receiver: receiver,
            feeNumerator: feeNumerator
        });

        emit DefaultRoyaltyUpdated(receiver, feeNumerator);
    }

    /**
     * @notice Updates the base URI for all token metadata.
     * @param newUri New base URI string.
     */
    function setURI(string calldata newUri) external onlyRole(DEFAULT_ADMIN_ROLE) {
        _setURI(newUri);
    }

    /**
     * @notice Pauses all minting and transfer operations.
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

    /// @inheritdoc ERC1155Supply
    function _update(
        address from,
        address to,
        uint256[] memory ids,
        uint256[] memory values
    ) internal override(ERC1155, ERC1155Supply) {
        super._update(from, to, ids, values);
    }

    /**
     * @dev Supports ERC-165 interface detection for ERC-1155, ERC-2981, and AccessControl.
     */
    function supportsInterface(
        bytes4 interfaceId
    ) public view override(ERC1155, AccessControl, IERC165) returns (bool) {
        return
            interfaceId == type(IERC2981).interfaceId ||
            super.supportsInterface(interfaceId);
    }
}
