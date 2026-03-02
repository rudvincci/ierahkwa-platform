// SPDX-License-Identifier: MIT
// ============================================================================
// IERAHKWA GENESIS BADGE -- Soulbound NFT for the First 100 Guardians
// Non-transferable badge minted exclusively to Manifesto signers.
// Tier assignment: GENESIS_ARCHITECT (1-5), GENESIS_GUARDIAN (6-25),
// GENESIS_SENTINEL (26-100). On-chain SVG metadata.
// Gobierno Soberano de Ierahkwa Ne Kanienke
// ============================================================================

pragma solidity ^0.8.26;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/Strings.sol";
import "@openzeppelin/contracts/utils/Base64.sol";

/// @notice Minimal interface for the IerahkwaManifesto contract.
interface IManifesto {
    function hasSigned(address signer) external view returns (bool);
}

/**
 * @title IerahkwaGenesis -- Genesis Badge SBT
 * @notice Soulbound ERC-721 badge for the first 100 Guardians of Ierahkwa.
 *         Requires the caller to have signed the IerahkwaManifesto before
 *         claiming. Tokens are permanently non-transferable.
 *
 * Guardian tiers:
 *   1  - 5    GENESIS_ARCHITECT  -- Founders who shaped the blueprint
 *   6  - 25   GENESIS_GUARDIAN   -- Early protectors of the vision
 *   26 - 100  GENESIS_SENTINEL   -- Watchers who secured the perimeter
 */
contract IerahkwaGenesis is ERC721, Ownable {
    using Strings for uint256;

    // =========================================================================
    // CONSTANTS
    // =========================================================================

    /// @notice Maximum number of genesis badges that can ever be minted.
    uint256 public constant MAX_GUARDIANS = 100;

    // =========================================================================
    // STATE
    // =========================================================================

    /// @notice Reference to the deployed IerahkwaManifesto contract.
    IManifesto public manifesto;

    /// @notice Counter for the next token ID (starts at 1).
    uint256 private _nextTokenId;

    /// @notice Total genesis badges minted so far.
    uint256 public totalClaimed;

    /// @notice Tracks whether an address has already claimed a genesis badge.
    mapping(address => bool) public hasClaimed;

    /// @notice Maps token ID to the guardian number (1-100).
    mapping(uint256 => uint256) public guardianNumber;

    /// @notice Maps token ID to the tier string.
    mapping(uint256 => string) public guardianTier;

    /// @notice Maps token ID to the claim timestamp.
    mapping(uint256 => uint256) public claimTimestamp;

    // =========================================================================
    // EVENTS
    // =========================================================================

    /// @notice Emitted when a genesis badge is claimed.
    event GenesisClaimed(
        address indexed guardian,
        uint256 indexed tokenId,
        string tier
    );

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================

    /**
     * @notice Deploy the Genesis Badge contract.
     * @param _manifesto Address of the deployed IerahkwaManifesto contract.
     */
    constructor(address _manifesto)
        ERC721("Ierahkwa Genesis Badge", "IGENESIS")
        Ownable(msg.sender)
    {
        require(_manifesto != address(0), "Genesis: manifesto is zero address");
        manifesto = IManifesto(_manifesto);
        _nextTokenId = 1;
    }

    // =========================================================================
    // CORE: CLAIM GENESIS BADGE
    // =========================================================================

    /**
     * @notice Claim a Genesis Badge. Requirements:
     *         1. Caller must have signed the IerahkwaManifesto.
     *         2. Caller has not already claimed.
     *         3. Total claims have not reached MAX_GUARDIANS (100).
     *
     * The guardian number determines the tier:
     *   1-5:    GENESIS_ARCHITECT
     *   6-25:   GENESIS_GUARDIAN
     *   26-100: GENESIS_SENTINEL
     */
    function claimGenesisBadge() external {
        require(
            manifesto.hasSigned(msg.sender),
            "Genesis: must sign the Manifesto first"
        );
        require(!hasClaimed[msg.sender], "Genesis: already claimed");
        require(totalClaimed < MAX_GUARDIANS, "Genesis: all 100 badges claimed");

        totalClaimed++;
        uint256 number = totalClaimed;
        uint256 tokenId = _nextTokenId;
        _nextTokenId++;

        string memory tier = _getTier(number);

        hasClaimed[msg.sender] = true;
        guardianNumber[tokenId] = number;
        guardianTier[tokenId] = tier;
        claimTimestamp[tokenId] = block.timestamp;

        _safeMint(msg.sender, tokenId);

        emit GenesisClaimed(msg.sender, tokenId, tier);
    }

    // =========================================================================
    // SOULBOUND: BLOCK ALL TRANSFERS
    // =========================================================================

    /**
     * @dev Override _update to make tokens soulbound.
     *      Only minting (from == address(0)) is allowed.
     *      All other transfers are permanently blocked.
     */
    function _update(
        address to,
        uint256 tokenId,
        address auth
    ) internal override returns (address) {
        address from = _ownerOf(tokenId);

        // Allow minting, block everything else
        if (from != address(0)) {
            revert("Genesis: soulbound -- transfers are disabled");
        }

        return super._update(to, tokenId, auth);
    }

    // =========================================================================
    // TIER LOGIC
    // =========================================================================

    /**
     * @dev Determine the guardian tier based on claim order.
     * @param number The guardian number (1-100).
     * @return The tier name as a string.
     */
    function _getTier(uint256 number) internal pure returns (string memory) {
        if (number <= 5) {
            return "GENESIS_ARCHITECT";
        } else if (number <= 25) {
            return "GENESIS_GUARDIAN";
        } else {
            return "GENESIS_SENTINEL";
        }
    }

    /**
     * @notice Get the tier for a given guardian number.
     * @param number The guardian number (1-100).
     * @return The tier name.
     */
    function getTier(uint256 number) external pure returns (string memory) {
        require(number >= 1 && number <= 100, "Genesis: invalid guardian number");
        return _getTier(number);
    }

    // =========================================================================
    // ON-CHAIN METADATA (SVG)
    // =========================================================================

    /**
     * @notice Returns fully on-chain JSON metadata with an embedded SVG image.
     * @param tokenId The token to query.
     * @return Base64-encoded data URI containing JSON metadata.
     */
    function tokenURI(uint256 tokenId)
        public
        view
        override
        returns (string memory)
    {
        _requireOwned(tokenId);

        uint256 number = guardianNumber[tokenId];
        string memory tier = guardianTier[tokenId];

        string memory json = string(
            abi.encodePacked(
                '{"name":"Ierahkwa Genesis #',
                number.toString(),
                '","description":"Soulbound Genesis Badge for the first 100 Guardians of Ierahkwa Ne Kanienke. Tier: ',
                tier,
                '.","attributes":[{"trait_type":"Guardian Number","display_type":"number","value":',
                number.toString(),
                '},{"trait_type":"Tier","value":"',
                tier,
                '"},{"trait_type":"Claimed At","display_type":"date","value":',
                claimTimestamp[tokenId].toString(),
                '}],"image":"data:image/svg+xml;base64,',
                Base64.encode(bytes(_generateSVG(number, tier))),
                '"}'
            )
        );

        return string(
            abi.encodePacked(
                "data:application/json;base64,",
                Base64.encode(bytes(json))
            )
        );
    }

    /**
     * @dev Generate an on-chain SVG for the genesis badge.
     * @param number The guardian number.
     * @param tier   The guardian tier string.
     * @return SVG markup as a string.
     */
    function _generateSVG(
        uint256 number,
        string memory tier
    ) internal pure returns (string memory) {
        // Determine tier color
        string memory tierColor;
        if (number <= 5) {
            tierColor = "#FFD700"; // Gold for Architects
        } else if (number <= 25) {
            tierColor = "#7C4DFF"; // Purple for Guardians
        } else {
            tierColor = "#00E676"; // Green for Sentinels
        }

        return string(
            abi.encodePacked(
                '<svg xmlns="http://www.w3.org/2000/svg" width="400" height="400" viewBox="0 0 400 400">',
                '<defs><radialGradient id="bg" cx="50%" cy="40%" r="70%"><stop offset="0%" stop-color="#1a1a2e"/><stop offset="100%" stop-color="#0a0a0f"/></radialGradient></defs>',
                '<rect width="400" height="400" fill="url(#bg)"/>',
                '<circle cx="200" cy="140" r="70" fill="none" stroke="',
                tierColor,
                '" stroke-width="2.5" opacity="0.8"/>',
                '<text x="200" y="135" text-anchor="middle" fill="',
                tierColor,
                '" font-size="36" font-family="monospace">&#x2726;</text>',
                '<text x="200" y="175" text-anchor="middle" fill="#ffffff" font-size="14" font-family="monospace">GENESIS #',
                number.toString(),
                '</text>',
                '<text x="200" y="260" text-anchor="middle" fill="',
                tierColor,
                '" font-size="13" font-family="monospace">',
                tier,
                '</text>',
                '<line x1="100" y1="290" x2="300" y2="290" stroke="#333" stroke-width="1"/>',
                '<text x="200" y="320" text-anchor="middle" fill="#555" font-size="10" font-family="monospace">Ierahkwa Ne Kanienke</text>',
                '<text x="200" y="365" text-anchor="middle" fill="#333" font-size="8" font-family="monospace">Los primeros 100. Inmutable. Soberano.</text>',
                '</svg>'
            )
        );
    }

    // =========================================================================
    // VIEW HELPERS
    // =========================================================================

    /// @notice Returns the next token ID (total minted + 1).
    function nextTokenId() external view returns (uint256) {
        return _nextTokenId;
    }

    /// @notice Returns how many genesis badges remain available.
    function remainingBadges() external view returns (uint256) {
        return MAX_GUARDIANS - totalClaimed;
    }
}
