// SPDX-License-Identifier: Sovereign-1.0
pragma solidity 0.8.24;

import {Ownable} from "@openzeppelin/contracts/access/Ownable.sol";
import {ReentrancyGuard} from "@openzeppelin/contracts/utils/ReentrancyGuard.sol";
import {Pausable} from "@openzeppelin/contracts/utils/Pausable.sol";
import {Address} from "@openzeppelin/contracts/utils/Address.sol";

/**
 * @title BDETPaymentEngine
 * @author Ierahkwa Ne Kanienke — Sovereign Digital Nation
 * @notice Core payment engine for the sovereign digital ecosystem. Handles automated
 *         5-way payment splits across 20 platform categories, plus an escrow system
 *         for marketplace transactions.
 * @dev Adapted from BDETContracts.sol v4.2.0 for OpenZeppelin v5.x.
 *      Each platform has a configurable PaymentSplit that divides incoming payments
 *      among: worker (artisan/creator), platform operator, national treasury,
 *      community fund, and insurance pool.
 *
 *      Split percentages are expressed in basis points (10000 = 100%).
 *      The insurance allocation absorbs rounding remainder for precision.
 */
contract BDETPaymentEngine is Ownable, ReentrancyGuard, Pausable {
    using Address for address payable;

    // ──────────────────────────────────────────────────────────────
    //  Constants
    // ──────────────────────────────────────────────────────────────

    /// @notice Contract version.
    string public constant VERSION = "5.0.0";

    /// @notice Basis points denominator (100% = 10,000).
    uint256 public constant BPS_DENOMINATOR = 10_000;

    /// @notice Maximum escrow release delay (90 days).
    uint256 public constant MAX_ESCROW_DELAY = 90 days;

    // ──────────────────────────────────────────────────────────────
    //  Platform IDs
    // ──────────────────────────────────────────────────────────────

    uint8 public constant CORREO = 1;
    uint8 public constant RED_SOBERANA = 2;
    uint8 public constant BUSQUEDA = 3;
    uint8 public constant CANAL = 4;          // 92% creator
    uint8 public constant MUSICA = 5;         // 92% artist
    uint8 public constant HOSPEDAJE = 6;      // 90% host
    uint8 public constant ARTESANIA = 7;      // 88% artisan
    uint8 public constant CORTOS = 8;         // 92% creator
    uint8 public constant COMERCIO = 9;       // 88% seller
    uint8 public constant INVERTIR = 10;
    uint8 public constant DOCS = 11;
    uint8 public constant MAPA = 12;
    uint8 public constant VOZ = 13;
    uint8 public constant TRABAJO = 14;
    uint8 public constant RENTA = 15;         // 95% worker
    uint8 public constant MARKET = 16;        // 88% vendor
    uint8 public constant BDET_BANK = 17;
    uint8 public constant SABIDURIA = 18;
    uint8 public constant UNIVERSIDAD = 19;
    uint8 public constant NOTICIA = 20;

    // ──────────────────────────────────────────────────────────────
    //  Structs
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Defines how a payment is split among 5 recipients for a given platform.
     * @param worker Artisan, creator, or worker receiving the majority share.
     * @param platform Sovereign platform operator address.
     * @param treasury National digital treasury address.
     * @param community Community fund address.
     * @param insurance Insurance pool address.
     * @param workerPct Worker share in basis points (e.g., 9200 = 92%).
     * @param platformPct Platform share in basis points.
     * @param treasuryPct Treasury share in basis points.
     * @param communityPct Community fund share in basis points.
     * @param insurancePct Insurance pool share in basis points.
     */
    struct PaymentSplit {
        address payable worker;
        address payable platform;
        address payable treasury;
        address payable community;
        address payable insurance;
        uint16 workerPct;
        uint16 platformPct;
        uint16 treasuryPct;
        uint16 communityPct;
        uint16 insurancePct;
    }

    /**
     * @notice Represents an escrowed transaction between a buyer and seller.
     * @param buyer Address that funded the escrow.
     * @param seller Address that will receive funds upon release.
     * @param amount Amount held in escrow (in wei).
     * @param platformId Platform where the transaction originated.
     * @param status 0=active, 1=released, 2=disputed, 3=refunded.
     * @param createdAt Timestamp when the escrow was created.
     * @param releaseAt Earliest timestamp when the escrow can be auto-released.
     */
    struct Escrow {
        address buyer;
        address payable seller;
        uint256 amount;
        uint8 platformId;
        uint8 status;
        uint256 createdAt;
        uint256 releaseAt;
    }

    // ──────────────────────────────────────────────────────────────
    //  State Variables
    // ──────────────────────────────────────────────────────────────

    /// @notice Platform-specific payment splits, keyed by platform ID.
    mapping(uint8 => PaymentSplit) public platformSplits;

    /// @notice Escrow records, keyed by unique escrow ID.
    mapping(bytes32 => Escrow) public escrows;

    /// @notice Total volume processed across all platforms (in wei).
    uint256 public totalVolumeProcessed;

    /// @notice Total number of payments processed.
    uint256 public totalPaymentsProcessed;

    /// @notice Address authorized to mediate escrow disputes.
    address public disputeMediator;

    // ──────────────────────────────────────────────────────────────
    //  Events
    // ──────────────────────────────────────────────────────────────

    /// @notice Emitted when a payment is processed through a platform split.
    event PaymentProcessed(
        uint8 indexed platformId,
        address indexed from,
        address indexed to,
        uint256 amount,
        uint256 timestamp,
        bytes32 txHash
    );

    /// @notice Emitted when a new escrow is created.
    event EscrowCreated(
        bytes32 indexed escrowId,
        address buyer,
        address seller,
        uint256 amount
    );

    /// @notice Emitted when an escrow is released to the seller.
    event EscrowReleased(bytes32 indexed escrowId);

    /// @notice Emitted when an escrow is disputed.
    event EscrowDisputed(bytes32 indexed escrowId);

    /// @notice Emitted when an escrow is refunded to the buyer.
    event EscrowRefunded(bytes32 indexed escrowId);

    /// @notice Emitted when a platform split configuration is updated.
    event PlatformSplitUpdated(uint8 indexed platformId);

    /// @notice Emitted when the dispute mediator is updated.
    event DisputeMediatorUpdated(address indexed newMediator);

    // ──────────────────────────────────────────────────────────────
    //  Errors
    // ──────────────────────────────────────────────────────────────

    error InsufficientPayment(uint256 sent, uint256 required);
    error InvalidPlatformId(uint8 platformId);
    error InvalidSplitPercentages(uint256 total);
    error EscrowNotActive(bytes32 escrowId);
    error EscrowNotReleasable(bytes32 escrowId);
    error UnauthorizedEscrowAction(bytes32 escrowId, address caller);
    error EscrowAlreadyExists(bytes32 escrowId);
    error InvalidEscrowDelay(uint256 delay);
    error ZeroAddress();
    error ZeroAmount();

    // ──────────────────────────────────────────────────────────────
    //  Constructor
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Initializes the BDETPaymentEngine with the contract admin.
     * @param admin Address granted ownership (can update splits and manage disputes).
     * @param _disputeMediator Address authorized to mediate escrow disputes.
     */
    constructor(
        address admin,
        address _disputeMediator
    ) Ownable(admin) {
        if (_disputeMediator == address(0)) revert ZeroAddress();
        disputeMediator = _disputeMediator;
    }

    // ──────────────────────────────────────────────────────────────
    //  Payment Processing
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Processes a payment, automatically splitting it according to the
     *         platform's configured PaymentSplit.
     * @dev The insurance amount absorbs any rounding remainder for precision.
     *      Caller must send at least `_amount` in msg.value.
     * @param _platformId Platform ID (1-20) determining the split configuration.
     * @param _recipient Worker/creator/artisan receiving the worker share.
     * @param _amount Total payment amount to split.
     * @param _metadata Arbitrary 32-byte reference hash (e.g., order ID, invoice hash).
     */
    function processPayment(
        uint8 _platformId,
        address payable _recipient,
        uint256 _amount,
        bytes32 _metadata
    ) external payable nonReentrant whenNotPaused {
        if (msg.value < _amount) revert InsufficientPayment(msg.value, _amount);
        if (_amount == 0) revert ZeroAmount();
        if (_platformId == 0 || _platformId > 20) revert InvalidPlatformId(_platformId);
        if (_recipient == address(0)) revert ZeroAddress();

        PaymentSplit memory split = platformSplits[_platformId];

        uint256 workerAmt = (_amount * split.workerPct) / BPS_DENOMINATOR;
        uint256 platformAmt = (_amount * split.platformPct) / BPS_DENOMINATOR;
        uint256 treasuryAmt = (_amount * split.treasuryPct) / BPS_DENOMINATOR;
        uint256 communityAmt = (_amount * split.communityPct) / BPS_DENOMINATOR;
        // Insurance absorbs rounding remainder
        uint256 insuranceAmt = _amount - workerAmt - platformAmt - treasuryAmt - communityAmt;

        // Distribute to worker (the actual recipient for this transaction)
        _recipient.sendValue(workerAmt);

        // Distribute to platform infrastructure
        if (platformAmt > 0 && split.platform != address(0)) {
            split.platform.sendValue(platformAmt);
        }

        // Distribute to national treasury
        if (treasuryAmt > 0 && split.treasury != address(0)) {
            split.treasury.sendValue(treasuryAmt);
        }

        // Distribute to community fund
        if (communityAmt > 0 && split.community != address(0)) {
            split.community.sendValue(communityAmt);
        }

        // Distribute to insurance pool
        if (insuranceAmt > 0 && split.insurance != address(0)) {
            split.insurance.sendValue(insuranceAmt);
        }

        totalVolumeProcessed += _amount;
        totalPaymentsProcessed++;

        // Refund excess
        uint256 excess = msg.value - _amount;
        if (excess > 0) {
            payable(msg.sender).sendValue(excess);
        }

        emit PaymentProcessed(
            _platformId,
            msg.sender,
            _recipient,
            _amount,
            block.timestamp,
            _metadata
        );
    }

    // ──────────────────────────────────────────────────────────────
    //  Escrow System
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Creates a new escrow for a marketplace transaction.
     * @dev The msg.value is locked in the contract until released, disputed, or refunded.
     * @param _escrowId Unique identifier for this escrow (typically a hash of order details).
     * @param _seller Address that will receive payment upon release.
     * @param _platformId Platform where the transaction originated (determines split on release).
     * @param _releaseDelay Minimum time (in seconds) before auto-release is allowed. Max 90 days.
     */
    function createEscrow(
        bytes32 _escrowId,
        address payable _seller,
        uint8 _platformId,
        uint256 _releaseDelay
    ) external payable nonReentrant whenNotPaused {
        if (msg.value == 0) revert ZeroAmount();
        if (_seller == address(0)) revert ZeroAddress();
        if (_platformId == 0 || _platformId > 20) revert InvalidPlatformId(_platformId);
        if (_releaseDelay > MAX_ESCROW_DELAY) revert InvalidEscrowDelay(_releaseDelay);
        if (escrows[_escrowId].createdAt != 0) revert EscrowAlreadyExists(_escrowId);

        escrows[_escrowId] = Escrow({
            buyer: msg.sender,
            seller: _seller,
            amount: msg.value,
            platformId: _platformId,
            status: 0, // active
            createdAt: block.timestamp,
            releaseAt: block.timestamp + _releaseDelay
        });

        emit EscrowCreated(_escrowId, msg.sender, _seller, msg.value);
    }

    /**
     * @notice Releases an active escrow, sending funds to the seller.
     * @dev Can be called by the buyer at any time, or by anyone after the release delay expires.
     *      Funds are sent directly to the seller (no split applied at release for simplicity;
     *      the split was accounted for at the platform level).
     * @param _escrowId Unique identifier of the escrow to release.
     */
    function releaseEscrow(bytes32 _escrowId) external nonReentrant whenNotPaused {
        Escrow storage e = escrows[_escrowId];
        if (e.status != 0) revert EscrowNotActive(_escrowId);

        bool isBuyer = msg.sender == e.buyer;
        bool isExpired = block.timestamp >= e.releaseAt;
        bool isMediator = msg.sender == disputeMediator;

        if (!isBuyer && !isExpired && !isMediator) {
            revert UnauthorizedEscrowAction(_escrowId, msg.sender);
        }

        e.status = 1; // released
        e.seller.sendValue(e.amount);

        emit EscrowReleased(_escrowId);
    }

    /**
     * @notice Marks an active escrow as disputed.
     * @dev Can only be called by the buyer or the dispute mediator.
     *      Once disputed, only the mediator can resolve it.
     * @param _escrowId Unique identifier of the escrow to dispute.
     */
    function disputeEscrow(bytes32 _escrowId) external {
        Escrow storage e = escrows[_escrowId];
        if (e.status != 0) revert EscrowNotActive(_escrowId);
        if (msg.sender != e.buyer && msg.sender != disputeMediator) {
            revert UnauthorizedEscrowAction(_escrowId, msg.sender);
        }

        e.status = 2; // disputed

        emit EscrowDisputed(_escrowId);
    }

    /**
     * @notice Refunds a disputed escrow to the buyer.
     * @dev Can only be called by the dispute mediator.
     * @param _escrowId Unique identifier of the escrow to refund.
     */
    function refundEscrow(bytes32 _escrowId) external nonReentrant {
        Escrow storage e = escrows[_escrowId];
        if (e.status != 2) revert EscrowNotActive(_escrowId); // must be disputed
        if (msg.sender != disputeMediator) {
            revert UnauthorizedEscrowAction(_escrowId, msg.sender);
        }

        e.status = 3; // refunded
        payable(e.buyer).sendValue(e.amount);

        emit EscrowRefunded(_escrowId);
    }

    /**
     * @notice Resolves a disputed escrow by releasing funds to the seller.
     * @dev Can only be called by the dispute mediator.
     * @param _escrowId Unique identifier of the escrow to resolve in seller's favor.
     */
    function resolveDisputeForSeller(bytes32 _escrowId) external nonReentrant {
        Escrow storage e = escrows[_escrowId];
        if (e.status != 2) revert EscrowNotActive(_escrowId); // must be disputed
        if (msg.sender != disputeMediator) {
            revert UnauthorizedEscrowAction(_escrowId, msg.sender);
        }

        e.status = 1; // released
        e.seller.sendValue(e.amount);

        emit EscrowReleased(_escrowId);
    }

    // ──────────────────────────────────────────────────────────────
    //  Admin Functions
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Sets or updates the payment split configuration for a platform.
     * @dev All five percentages must sum to exactly 10,000 (100%).
     * @param _platformId Platform ID (1-20).
     * @param _split The PaymentSplit struct with addresses and percentages.
     */
    function setPlatformSplit(
        uint8 _platformId,
        PaymentSplit calldata _split
    ) external onlyOwner {
        if (_platformId == 0 || _platformId > 20) revert InvalidPlatformId(_platformId);

        uint256 total = uint256(_split.workerPct) +
            uint256(_split.platformPct) +
            uint256(_split.treasuryPct) +
            uint256(_split.communityPct) +
            uint256(_split.insurancePct);

        if (total != BPS_DENOMINATOR) revert InvalidSplitPercentages(total);

        platformSplits[_platformId] = _split;

        emit PlatformSplitUpdated(_platformId);
    }

    /**
     * @notice Updates the dispute mediator address.
     * @param _mediator New mediator address.
     */
    function setDisputeMediator(address _mediator) external onlyOwner {
        if (_mediator == address(0)) revert ZeroAddress();
        disputeMediator = _mediator;
        emit DisputeMediatorUpdated(_mediator);
    }

    /**
     * @notice Pauses all payment processing and escrow operations.
     */
    function pause() external onlyOwner {
        _pause();
    }

    /**
     * @notice Unpauses all payment processing and escrow operations.
     */
    function unpause() external onlyOwner {
        _unpause();
    }
}
