// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Enumerable.sol";
import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/utils/Pausable.sol";

/**
 * @title AuthenticityNFT
 * @author Ierahkwa Ne Kanienke — Sovereign Digital Nation
 * @notice Certificados de autenticidad para artesanias indigenas soberanas.
 *         Cada NFT certifica origen, tecnica y materiales de productos artesanales.
 * @dev Soulbound — no transferible excepto por admin para recuperacion.
 */
contract AuthenticityNFT is ERC721, ERC721Enumerable, AccessControl, Pausable {

    bytes32 public constant ARTISAN_ROLE = keccak256("ARTISAN_ROLE");
    bytes32 public constant VERIFIER_ROLE = keccak256("VERIFIER_ROLE");

    struct Certificate {
        address artisan;
        string productName;
        string nation;
        string technique;
        string materials;
        uint256 createdAt;
        bytes32 locationHash;
        bool verified;
        uint256 verifiedAt;
        address verifiedBy;
    }

    mapping(uint256 => Certificate) public certificates;
    uint256 public totalCertificates;
    mapping(address => uint256[]) private _artisanCerts;

    event CertificateCreated(uint256 indexed tokenId, address indexed artisan, string productName, string nation);
    event CertificateVerified(uint256 indexed tokenId, address indexed verifier);
    event CertificateRevoked(uint256 indexed tokenId);

    error CertificateNotFound();
    error AlreadyVerified();
    error SoulboundTransferBlocked();

    constructor(address admin) ERC721("Sovereign Authenticity Certificate", "AUTH") {
        _grantRole(DEFAULT_ADMIN_ROLE, admin);
        _grantRole(ARTISAN_ROLE, admin);
        _grantRole(VERIFIER_ROLE, admin);
    }

    /// @notice Crear certificado de autenticidad
    function createCertificate(
        string calldata productName,
        string calldata nation,
        string calldata technique,
        string calldata materials,
        bytes32 locationHash
    ) external onlyRole(ARTISAN_ROLE) whenNotPaused returns (uint256) {
        totalCertificates++;
        uint256 tokenId = totalCertificates;

        certificates[tokenId] = Certificate({
            artisan: msg.sender,
            productName: productName,
            nation: nation,
            technique: technique,
            materials: materials,
            createdAt: block.timestamp,
            locationHash: locationHash,
            verified: false,
            verifiedAt: 0,
            verifiedBy: address(0)
        });

        _artisanCerts[msg.sender].push(tokenId);
        _safeMint(msg.sender, tokenId);

        emit CertificateCreated(tokenId, msg.sender, productName, nation);
        return tokenId;
    }

    /// @notice Verificar autenticidad
    function verifyCertificate(uint256 tokenId) external onlyRole(VERIFIER_ROLE) {
        Certificate storage cert = certificates[tokenId];
        if (cert.artisan == address(0)) revert CertificateNotFound();
        if (cert.verified) revert AlreadyVerified();
        cert.verified = true;
        cert.verifiedAt = block.timestamp;
        cert.verifiedBy = msg.sender;
        emit CertificateVerified(tokenId, msg.sender);
    }

    /// @notice Revocar certificado
    function revokeCertificate(uint256 tokenId) external onlyRole(DEFAULT_ADMIN_ROLE) {
        if (certificates[tokenId].artisan == address(0)) revert CertificateNotFound();
        _burn(tokenId);
        delete certificates[tokenId];
        emit CertificateRevoked(tokenId);
    }

    function getArtisanCertificates(address artisan) external view returns (uint256[] memory) {
        return _artisanCerts[artisan];
    }

    function isAuthentic(uint256 tokenId) external view returns (bool) {
        return certificates[tokenId].verified;
    }

    // --- Soulbound ---
    function _update(address to, uint256 tokenId, address auth)
        internal override(ERC721, ERC721Enumerable) returns (address)
    {
        address from = _ownerOf(tokenId);
        if (from != address(0) && to != address(0)) {
            if (!hasRole(DEFAULT_ADMIN_ROLE, auth)) revert SoulboundTransferBlocked();
        }
        return super._update(to, tokenId, auth);
    }

    function _increaseBalance(address account, uint128 value)
        internal override(ERC721, ERC721Enumerable)
    { super._increaseBalance(account, value); }

    function supportsInterface(bytes4 interfaceId)
        public view override(ERC721, ERC721Enumerable, AccessControl) returns (bool)
    { return super.supportsInterface(interfaceId); }

    function pause() external onlyRole(DEFAULT_ADMIN_ROLE) { _pause(); }
    function unpause() external onlyRole(DEFAULT_ADMIN_ROLE) { _unpause(); }
}
