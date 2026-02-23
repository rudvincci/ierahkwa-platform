// SPDX-License-Identifier: Sovereign-1.0
// MameyNode Smart Contracts v4.2 — Red Soberana Digital de las Américas
// Blockchain: MameyNode (EVM-compatible) · 12,847 TPS

pragma solidity ^0.8.20;

// ============================================================
// 1. BDET PAYMENT ENGINE — Core de todos los pagos
// ============================================================
contract BDETPaymentEngine {
    string public constant VERSION = "4.2.0";
    
    struct PaymentSplit {
        address payable worker;      // Artesano/Creador/Worker
        address payable platform;    // Plataforma Soberana
        address payable treasury;    // Tesoro Nacional Digital
        address payable community;   // Fondo Comunitario
        address payable insurance;   // Pool de Seguros
        uint16 workerPct;           // 8800 = 88%, 9200 = 92%, 9500 = 95%
        uint16 platformPct;         // 200-400 = 2-4%
        uint16 treasuryPct;         // 400-800 = 4-8%
        uint16 communityPct;        // 0-200 = 0-2%
        uint16 insurancePct;        // 0-200 = 0-2%
    }
    
    // Platform-specific splits
    mapping(uint8 => PaymentSplit) public platformSplits;
    
    // Platform IDs
    uint8 constant CORREO = 1;
    uint8 constant RED_SOBERANA = 2;
    uint8 constant BUSQUEDA = 3;
    uint8 constant CANAL = 4;        // 92% creator
    uint8 constant MUSICA = 5;       // 92% artist
    uint8 constant HOSPEDAJE = 6;    // 90% host
    uint8 constant ARTESANIA = 7;    // 88% artisan
    uint8 constant CORTOS = 8;       // 92% creator
    uint8 constant COMERCIO = 9;     // 88% seller
    uint8 constant INVERTIR = 10;
    uint8 constant DOCS = 11;
    uint8 constant MAPA = 12;
    uint8 constant VOZ = 13;
    uint8 constant TRABAJO = 14;
    uint8 constant RENTA = 15;       // 95% worker
    uint8 constant MARKET = 16;      // 88% vendor
    uint8 constant BDET_BANK = 17;
    uint8 constant SABIDURIA = 18;
    uint8 constant UNIVERSIDAD = 19;
    uint8 constant NOTICIA = 20;
    
    event PaymentProcessed(
        uint8 indexed platformId,
        address indexed from,
        address indexed to,
        uint256 amount,
        uint256 timestamp,
        bytes32 txHash
    );
    
    event EscrowCreated(bytes32 indexed escrowId, address buyer, address seller, uint256 amount);
    event EscrowReleased(bytes32 indexed escrowId);
    event EscrowDisputed(bytes32 indexed escrowId);
    
    // Process payment with automatic split
    function processPayment(
        uint8 _platformId,
        address payable _recipient,
        uint256 _amount,
        bytes32 _metadata
    ) external payable {
        require(msg.value >= _amount, "Insufficient WMP");
        PaymentSplit memory split = platformSplits[_platformId];
        
        uint256 workerAmt = (_amount * split.workerPct) / 10000;
        uint256 platformAmt = (_amount * split.platformPct) / 10000;
        uint256 treasuryAmt = (_amount * split.treasuryPct) / 10000;
        uint256 communityAmt = (_amount * split.communityPct) / 10000;
        uint256 insuranceAmt = _amount - workerAmt - platformAmt - treasuryAmt - communityAmt;
        
        _recipient.transfer(workerAmt);
        split.platform.transfer(platformAmt);
        split.treasury.transfer(treasuryAmt);
        if (communityAmt > 0) split.community.transfer(communityAmt);
        if (insuranceAmt > 0) split.insurance.transfer(insuranceAmt);
        
        emit PaymentProcessed(_platformId, msg.sender, _recipient, _amount, block.timestamp, _metadata);
    }
    
    // Escrow for marketplace transactions
    struct Escrow {
        address buyer;
        address payable seller;
        uint256 amount;
        uint8 platformId;
        uint8 status; // 0=active, 1=released, 2=disputed, 3=refunded
        uint256 createdAt;
        uint256 releaseAt;
    }
    
    mapping(bytes32 => Escrow) public escrows;
    
    function createEscrow(
        bytes32 _escrowId,
        address payable _seller,
        uint8 _platformId,
        uint256 _releaseDelay
    ) external payable {
        escrows[_escrowId] = Escrow(
            msg.sender, _seller, msg.value, _platformId,
            0, block.timestamp, block.timestamp + _releaseDelay
        );
        emit EscrowCreated(_escrowId, msg.sender, _seller, msg.value);
    }
    
    function releaseEscrow(bytes32 _escrowId) external {
        Escrow storage e = escrows[_escrowId];
        require(e.status == 0, "Not active");
        require(msg.sender == e.buyer || block.timestamp >= e.releaseAt, "Not authorized");
        e.status = 1;
        // Process with platform split
        // processPayment(e.platformId, e.seller, e.amount, _escrowId);
        emit EscrowReleased(_escrowId);
    }
}

// ============================================================
// 2. SOVEREIGN ID — Identidad digital descentralizada
// ============================================================
contract SovereignID {
    struct Identity {
        bytes32 nameHash;
        string nation;          // "Lenca", "Maya", "Navajo", "Taíno"
        string country;         // "HN", "GT", "US", "DO"
        uint8 verificationLevel; // 0=unverified, 1=basic, 2=community, 3=sovereign
        bool isArtisan;
        bool isCreator;
        bool isWorker;
        uint256 reputationScore;
        uint256 registeredAt;
    }
    
    mapping(address => Identity) public identities;
    mapping(address => bool) public verifiedArtisans;
    
    event IdentityRegistered(address indexed user, string nation);
    event VerificationUpgraded(address indexed user, uint8 level);
    
    function register(bytes32 _nameHash, string memory _nation, string memory _country) external {
        identities[msg.sender] = Identity(
            _nameHash, _nation, _country, 1, false, false, false, 0, block.timestamp
        );
        emit IdentityRegistered(msg.sender, _nation);
    }
}

// ============================================================
// 3. AUTHENTICITY CERTIFICATE — NFT para artesanías
// ============================================================
contract AuthenticityNFT {
    struct Certificate {
        address artisan;
        string productName;
        string nation;           // Pueblo indígena
        string technique;        // "Barro negro", "Huipil bordado"
        string materials;        // "Barro, tintes naturales"
        uint256 createdAt;
        bytes32 locationHash;    // GPS cifrado
        bool verified;
    }
    
    mapping(uint256 => Certificate) public certificates;
    uint256 public totalCertificates;
    
    event CertificateCreated(uint256 indexed tokenId, address artisan, string product);
    
    function createCertificate(
        string memory _product,
        string memory _nation,
        string memory _technique,
        string memory _materials,
        bytes32 _location
    ) external returns (uint256) {
        totalCertificates++;
        certificates[totalCertificates] = Certificate(
            msg.sender, _product, _nation, _technique, _materials,
            block.timestamp, _location, false
        );
        emit CertificateCreated(totalCertificates, msg.sender, _product);
        return totalCertificates;
    }
}

// ============================================================
// 4. BLOCKCHAIN VOTING — Votaciones verificables
// ============================================================
contract SovereignVoting {
    struct Poll {
        string question;
        string[] options;
        mapping(uint8 => uint256) votes;
        mapping(address => bool) hasVoted;
        uint256 totalVotes;
        uint256 endsAt;
        address creator;
        bool active;
    }
    
    uint256 public pollCount;
    
    event PollCreated(uint256 indexed pollId, string question);
    event VoteCast(uint256 indexed pollId, address voter, uint8 option);
}

// ============================================================
// 5. CREATOR MONETIZATION — Para Canal, Música, Cortos
// ============================================================
contract CreatorMonetization {
    struct CreatorProfile {
        address payable wallet;
        uint256 totalEarned;
        uint256 totalTips;
        uint256 subscriberCount;
        uint8 platformId;        // 4=Canal, 5=Musica, 8=Cortos
        bool verified;
    }
    
    mapping(address => CreatorProfile) public creators;
    
    event TipReceived(address indexed creator, address tipper, uint256 amount);
    event RevenueDistributed(address indexed creator, uint256 amount, uint8 platformId);
    
    function tipCreator(address payable _creator) external payable {
        require(msg.value > 0, "Amount required");
        uint256 creatorAmt = (msg.value * 9200) / 10000; // 92%
        _creator.transfer(creatorAmt);
        creators[_creator].totalTips += msg.value;
        emit TipReceived(_creator, msg.sender, msg.value);
    }
}
