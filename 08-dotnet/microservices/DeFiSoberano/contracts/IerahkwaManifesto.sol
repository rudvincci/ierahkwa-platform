// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/Strings.sol";
import "@openzeppelin/contracts/utils/Base64.sol";

/**
 * @title IerahkwaManifesto — Guardian de Ierahkwa (GARDN)
 * @dev Soulbound NFT for Conscious Programmer Manifesto signers.
 *      Non-transferable. One per address. Guardian level based on sign order.
 *      Gobierno Soberano de Ierahkwa Ne Kanienke
 */
contract IerahkwaManifesto is ERC721, Ownable {
    using Strings for uint256;

    // ─── State ───────────────────────────────────────────────────────────

    uint256 private _nextTokenId;
    uint256 public totalSigners;

    mapping(address => bool) public hasSigned;
    mapping(uint256 => uint256) public signTimestamp;
    mapping(uint256 => address) public tokenSigner;

    // ─── Constants ───────────────────────────────────────────────────────

    uint256 public constant GENESIS_THRESHOLD = 100;
    uint256 public constant ELDER_THRESHOLD = 1000;

    // ─── Events ──────────────────────────────────────────────────────────

    event ManifestoSigned(
        address indexed signer,
        uint256 indexed tokenId,
        string level,
        uint256 timestamp
    );

    // ─── Constructor ─────────────────────────────────────────────────────

    constructor() ERC721("Guardian de Ierahkwa", "GARDN") Ownable(msg.sender) {
        _nextTokenId = 1;
    }

    // ─── Core: Sign the Manifesto ────────────────────────────────────────

    /**
     * @dev Sign the Conscious Programmer Manifesto.
     *      Mints a soulbound NFT. One per address. Cannot be transferred.
     */
    function signManifesto() external {
        require(!hasSigned[msg.sender], "Already signed the Manifesto");

        uint256 tokenId = _nextTokenId;
        _nextTokenId++;
        totalSigners++;

        hasSigned[msg.sender] = true;
        signTimestamp[tokenId] = block.timestamp;
        tokenSigner[tokenId] = msg.sender;

        _safeMint(msg.sender, tokenId);

        string memory level = _getLevel(totalSigners);

        emit ManifestoSigned(msg.sender, tokenId, level, block.timestamp);
    }

    // ─── Guardian Level ──────────────────────────────────────────────────

    /**
     * @dev Returns the guardian level for an address.
     *      Requires the address to have signed.
     */
    function getGuardianLevel(address signer) external view returns (string memory) {
        require(hasSigned[signer], "Address has not signed the Manifesto");
        uint256 tokenId = _findTokenBySigner(signer);
        return _getLevel(tokenId);
    }

    /**
     * @dev Internal level determination by sign order (tokenId).
     *      1-100:     Genesis Guardian
     *      101-1000:  Elder Guardian
     *      1001+:     Guardian
     */
    function _getLevel(uint256 signerOrder) internal pure returns (string memory) {
        if (signerOrder <= GENESIS_THRESHOLD) {
            return "Genesis Guardian";
        } else if (signerOrder <= ELDER_THRESHOLD) {
            return "Elder Guardian";
        } else {
            return "Guardian";
        }
    }

    // ─── Soulbound: Block All Transfers ──────────────────────────────────

    /**
     * @dev Override to make tokens soulbound.
     *      Only minting (from == address(0)) is allowed.
     */
    function _update(
        address to,
        uint256 tokenId,
        address auth
    ) internal override returns (address) {
        address from = _ownerOf(tokenId);

        // Allow minting (from == address(0)), block all other transfers
        if (from != address(0)) {
            revert("Soulbound: Guardian tokens are non-transferable");
        }

        return super._update(to, tokenId, auth);
    }

    // ─── On-Chain Metadata ───────────────────────────────────────────────

    /**
     * @dev Returns on-chain JSON metadata with guardian level and sign date.
     */
    function tokenURI(uint256 tokenId) public view override returns (string memory) {
        _requireOwned(tokenId);

        string memory level = _getLevel(tokenId);
        uint256 timestamp = signTimestamp[tokenId];
        address signer = tokenSigner[tokenId];

        string memory json = string(
            abi.encodePacked(
                '{"name":"Guardian de Ierahkwa #',
                tokenId.toString(),
                '","description":"Soulbound token del Manifiesto del Programador Consciente. Gobierno Soberano de Ierahkwa Ne Kanienke.","attributes":[{"trait_type":"Level","value":"',
                level,
                '"},{"trait_type":"Sign Order","display_type":"number","value":',
                tokenId.toString(),
                '},{"trait_type":"Signed At","display_type":"date","value":',
                timestamp.toString(),
                '},{"trait_type":"Signer","value":"',
                Strings.toHexString(uint160(signer), 20),
                '"}],"image":"data:image/svg+xml;base64,',
                Base64.encode(bytes(_generateSVG(tokenId, level))),
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
     * @dev Generates an on-chain SVG for the guardian token.
     */
    function _generateSVG(uint256 tokenId, string memory level) internal pure returns (string memory) {
        return string(
            abi.encodePacked(
                '<svg xmlns="http://www.w3.org/2000/svg" width="400" height="400" viewBox="0 0 400 400">',
                '<rect width="400" height="400" fill="#0a0a0f"/>',
                '<circle cx="200" cy="160" r="80" fill="none" stroke="#00e676" stroke-width="2" opacity="0.6"/>',
                '<text x="200" y="155" text-anchor="middle" fill="#00e676" font-size="40" font-family="monospace">&#x1F6E1;</text>',
                '<text x="200" y="195" text-anchor="middle" fill="#ffffff" font-size="14" font-family="monospace">GUARDIAN #',
                tokenId.toString(),
                '</text>',
                '<text x="200" y="280" text-anchor="middle" fill="#7c4dff" font-size="16" font-family="monospace">',
                level,
                '</text>',
                '<text x="200" y="320" text-anchor="middle" fill="#555" font-size="10" font-family="monospace">Ierahkwa Ne Kanienke</text>',
                '<text x="200" y="370" text-anchor="middle" fill="#333" font-size="8" font-family="monospace">El codigo es la intencion; la conciencia es el resultado.</text>',
                '</svg>'
            )
        );
    }

    // ─── Internal Helpers ────────────────────────────────────────────────

    /**
     * @dev Find token ID owned by a signer (linear scan, acceptable for read-only).
     */
    function _findTokenBySigner(address signer) internal view returns (uint256) {
        for (uint256 i = 1; i < _nextTokenId; i++) {
            if (tokenSigner[i] == signer) {
                return i;
            }
        }
        revert("Token not found for signer");
    }

    /**
     * @dev Returns the next token ID (total minted + 1).
     */
    function nextTokenId() external view returns (uint256) {
        return _nextTokenId;
    }
}
