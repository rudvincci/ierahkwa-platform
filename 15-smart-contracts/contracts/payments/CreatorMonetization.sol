// SPDX-License-Identifier: Sovereign-1.0
pragma solidity 0.8.24;

import {Ownable} from "@openzeppelin/contracts/access/Ownable.sol";
import {ReentrancyGuard} from "@openzeppelin/contracts/utils/ReentrancyGuard.sol";
import {Pausable} from "@openzeppelin/contracts/utils/Pausable.sol";
import {Address} from "@openzeppelin/contracts/utils/Address.sol";

/**
 * @title CreatorMonetization
 * @author Ierahkwa Ne Kanienke — Sovereign Digital Nation
 * @notice Monetization engine for content creators on Canal, Musica, and Cortos platforms.
 *         Supports tipping with messages, monthly subscription models, revenue sharing
 *         between collaborators, and creator withdrawals.
 * @dev Expanded from the original CreatorMonetization in BDETContracts.sol v4.2.
 *      Platform fee: 8% (800 basis points) — 92% goes directly to the creator.
 *      Subscription payments are held in the contract and claimable by creators.
 *      Collaborator revenue sharing is configured per-creator with immutable splits
 *      that are set once per collaborator configuration.
 */
contract CreatorMonetization is Ownable, ReentrancyGuard, Pausable {
    using Address for address payable;

    // ──────────────────────────────────────────────────────────────
    //  Constants
    // ──────────────────────────────────────────────────────────────

    /// @notice Contract version.
    string public constant VERSION = "5.0.0";

    /// @notice Platform fee in basis points (8% = 800 bps). 92% goes to creator.
    uint256 public constant PLATFORM_FEE_BPS = 800;

    /// @notice Basis points denominator (100% = 10,000).
    uint256 public constant BPS_DENOMINATOR = 10_000;

    /// @notice Duration of a subscription period (30 days).
    uint256 public constant SUBSCRIPTION_PERIOD = 30 days;

    /// @notice Maximum number of collaborators per revenue share configuration.
    uint256 public constant MAX_COLLABORATORS = 10;

    /// @notice Maximum tip message length in bytes.
    uint256 public constant MAX_TIP_MESSAGE_LENGTH = 280;

    // ──────────────────────────────────────────────────────────────
    //  Structs
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Profile for a registered creator.
     * @param wallet Creator's payment wallet address.
     * @param totalEarned Cumulative earnings (tips + subscriptions + revenue).
     * @param totalTips Cumulative tip amount received.
     * @param subscriberCount Current number of active subscribers.
     * @param platformId Platform ID (4=Canal, 5=Musica, 8=Cortos).
     * @param verified Whether the creator has been verified by the platform.
     * @param subscriptionPrice Monthly subscription price (0 = free/no subscriptions).
     * @param withdrawableBalance Balance available for withdrawal.
     */
    struct CreatorProfile {
        address payable wallet;
        uint256 totalEarned;
        uint256 totalTips;
        uint256 subscriberCount;
        uint8 platformId;
        bool verified;
        uint256 subscriptionPrice;
        uint256 withdrawableBalance;
    }

    /**
     * @notice Subscription record for a subscriber to a creator.
     * @param subscribedAt Timestamp when the subscription started or was last renewed.
     * @param expiresAt Timestamp when the current subscription period expires.
     * @param active Whether the subscription is currently active.
     */
    struct Subscription {
        uint256 subscribedAt;
        uint256 expiresAt;
        bool active;
    }

    /**
     * @notice Revenue sharing configuration between a creator and their collaborators.
     * @param collaborators Array of collaborator addresses.
     * @param shares Array of share percentages in basis points (must sum to 10,000).
     * @param active Whether this revenue share configuration is active.
     */
    struct RevenueShare {
        address payable[] collaborators;
        uint16[] shares;
        bool active;
    }

    // ──────────────────────────────────────────────────────────────
    //  State Variables
    // ──────────────────────────────────────────────────────────────

    /// @notice Creator profiles, keyed by creator address.
    mapping(address => CreatorProfile) public creators;

    /// @notice Subscription records: subscriber => creator => Subscription.
    mapping(address => mapping(address => Subscription)) public subscriptions;

    /// @notice Revenue share configurations, keyed by creator address.
    mapping(address => RevenueShare) private _revenueShares;

    /// @notice Address that receives the platform fee.
    address payable public platformFeeRecipient;

    /// @notice Total platform fees collected (lifetime).
    uint256 public totalPlatformFees;

    /// @notice Total number of registered creators.
    uint256 public totalCreators;

    // ──────────────────────────────────────────────────────────────
    //  Events
    // ──────────────────────────────────────────────────────────────

    /// @notice Emitted when a creator registers their profile.
    event CreatorRegistered(
        address indexed creator,
        uint8 platformId,
        uint256 subscriptionPrice
    );

    /// @notice Emitted when a creator receives a tip.
    event TipReceived(
        address indexed creator,
        address indexed tipper,
        uint256 amount,
        string message
    );

    /// @notice Emitted when revenue is distributed to a creator.
    event RevenueDistributed(
        address indexed creator,
        uint256 amount,
        uint8 platformId
    );

    /// @notice Emitted when a user subscribes to a creator.
    event SubscriptionCreated(
        address indexed subscriber,
        address indexed creator,
        uint256 amount,
        uint256 expiresAt
    );

    /// @notice Emitted when a subscription is renewed.
    event SubscriptionRenewed(
        address indexed subscriber,
        address indexed creator,
        uint256 expiresAt
    );

    /// @notice Emitted when a creator withdraws their balance.
    event CreatorWithdrawal(
        address indexed creator,
        uint256 amount
    );

    /// @notice Emitted when a revenue share configuration is set.
    event RevenueShareConfigured(
        address indexed creator,
        uint256 collaboratorCount
    );

    /// @notice Emitted when revenue is distributed among collaborators.
    event RevenueShared(
        address indexed creator,
        address indexed collaborator,
        uint256 amount
    );

    /// @notice Emitted when a creator profile is verified.
    event CreatorVerified(address indexed creator);

    // ──────────────────────────────────────────────────────────────
    //  Errors
    // ──────────────────────────────────────────────────────────────

    error CreatorAlreadyRegistered(address creator);
    error CreatorNotRegistered(address creator);
    error InvalidPlatformId(uint8 platformId);
    error SubscriptionNotAvailable(address creator);
    error InsufficientPayment(uint256 sent, uint256 required);
    error SubscriptionStillActive(address subscriber, address creator);
    error NoBalanceToWithdraw(address creator);
    error TooManyCollaborators(uint256 count);
    error InvalidShareTotal(uint256 total);
    error RevenueShareAlreadyActive(address creator);
    error ZeroAddress();
    error ZeroAmount();
    error TipMessageTooLong(uint256 length);

    // ──────────────────────────────────────────────────────────────
    //  Constructor
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Initializes the CreatorMonetization contract.
     * @param admin Address granted ownership (admin functions).
     * @param _platformFeeRecipient Address that receives the 8% platform fee.
     */
    constructor(
        address admin,
        address payable _platformFeeRecipient
    ) Ownable(admin) {
        if (_platformFeeRecipient == address(0)) revert ZeroAddress();
        platformFeeRecipient = _platformFeeRecipient;
    }

    // ──────────────────────────────────────────────────────────────
    //  Creator Registration
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Registers a new creator profile.
     * @dev Only valid platform IDs are: 4 (Canal), 5 (Musica), 8 (Cortos).
     * @param _platformId Platform ID the creator belongs to.
     * @param _subscriptionPrice Monthly subscription price in wei (0 for free/no subs).
     */
    function registerCreator(
        uint8 _platformId,
        uint256 _subscriptionPrice
    ) external {
        if (_platformId != 4 && _platformId != 5 && _platformId != 8) {
            revert InvalidPlatformId(_platformId);
        }
        if (creators[msg.sender].wallet != address(0)) {
            revert CreatorAlreadyRegistered(msg.sender);
        }

        creators[msg.sender] = CreatorProfile({
            wallet: payable(msg.sender),
            totalEarned: 0,
            totalTips: 0,
            subscriberCount: 0,
            platformId: _platformId,
            verified: false,
            subscriptionPrice: _subscriptionPrice,
            withdrawableBalance: 0
        });

        totalCreators++;

        emit CreatorRegistered(msg.sender, _platformId, _subscriptionPrice);
    }

    /**
     * @notice Updates the subscription price for a creator.
     * @dev Only the creator themselves can update their price.
     * @param _newPrice New monthly subscription price in wei.
     */
    function setSubscriptionPrice(uint256 _newPrice) external {
        if (creators[msg.sender].wallet == address(0)) {
            revert CreatorNotRegistered(msg.sender);
        }
        creators[msg.sender].subscriptionPrice = _newPrice;
    }

    // ──────────────────────────────────────────────────────────────
    //  Tipping
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Sends a tip to a creator with an optional message.
     * @dev 92% goes to the creator (or is split among collaborators), 8% platform fee.
     * @param _creator Address of the creator to tip.
     * @param _message Optional message from the tipper (max 280 bytes).
     */
    function tipCreator(
        address _creator,
        string calldata _message
    ) external payable nonReentrant whenNotPaused {
        if (msg.value == 0) revert ZeroAmount();
        if (creators[_creator].wallet == address(0)) {
            revert CreatorNotRegistered(_creator);
        }
        if (bytes(_message).length > MAX_TIP_MESSAGE_LENGTH) {
            revert TipMessageTooLong(bytes(_message).length);
        }

        uint256 fee = (msg.value * PLATFORM_FEE_BPS) / BPS_DENOMINATOR;
        uint256 creatorAmount = msg.value - fee;

        // Send platform fee
        platformFeeRecipient.sendValue(fee);
        totalPlatformFees += fee;

        // Distribute to creator (or collaborators if revenue share is active)
        _distributeToCreator(_creator, creatorAmount);

        creators[_creator].totalTips += msg.value;
        creators[_creator].totalEarned += creatorAmount;

        emit TipReceived(_creator, msg.sender, msg.value, _message);
    }

    // ──────────────────────────────────────────────────────────────
    //  Subscriptions
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Subscribes to a creator for one monthly period.
     * @dev Requires exact or excess payment matching the creator's subscription price.
     *      If subscription is still active, it will be extended from the current expiry.
     * @param _creator Address of the creator to subscribe to.
     */
    function subscribe(address _creator) external payable nonReentrant whenNotPaused {
        CreatorProfile storage profile = creators[_creator];
        if (profile.wallet == address(0)) revert CreatorNotRegistered(_creator);
        if (profile.subscriptionPrice == 0) revert SubscriptionNotAvailable(_creator);
        if (msg.value < profile.subscriptionPrice) {
            revert InsufficientPayment(msg.value, profile.subscriptionPrice);
        }

        Subscription storage sub = subscriptions[msg.sender][_creator];

        uint256 fee = (profile.subscriptionPrice * PLATFORM_FEE_BPS) / BPS_DENOMINATOR;
        uint256 creatorAmount = profile.subscriptionPrice - fee;

        // Send platform fee
        platformFeeRecipient.sendValue(fee);
        totalPlatformFees += fee;

        // Credit to creator's withdrawable balance
        _distributeToCreator(_creator, creatorAmount);

        if (sub.active && sub.expiresAt > block.timestamp) {
            // Extend existing subscription
            sub.expiresAt += SUBSCRIPTION_PERIOD;
            emit SubscriptionRenewed(msg.sender, _creator, sub.expiresAt);
        } else {
            // New subscription or expired renewal
            if (!sub.active || sub.expiresAt <= block.timestamp) {
                if (!sub.active) {
                    profile.subscriberCount++;
                }
                sub.active = true;
            }
            sub.subscribedAt = block.timestamp;
            sub.expiresAt = block.timestamp + SUBSCRIPTION_PERIOD;
            emit SubscriptionCreated(
                msg.sender,
                _creator,
                profile.subscriptionPrice,
                sub.expiresAt
            );
        }

        profile.totalEarned += creatorAmount;

        // Refund excess
        uint256 excess = msg.value - profile.subscriptionPrice;
        if (excess > 0) {
            payable(msg.sender).sendValue(excess);
        }
    }

    /**
     * @notice Checks whether a subscriber's subscription to a creator is currently active.
     * @param _subscriber Address of the subscriber.
     * @param _creator Address of the creator.
     * @return isActive True if the subscription exists and has not expired.
     */
    function isSubscriptionActive(
        address _subscriber,
        address _creator
    ) external view returns (bool isActive) {
        Subscription storage sub = subscriptions[_subscriber][_creator];
        return sub.active && sub.expiresAt > block.timestamp;
    }

    // ──────────────────────────────────────────────────────────────
    //  Revenue Sharing
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Configures revenue sharing between a creator and their collaborators.
     * @dev The creator must include themselves in the collaborators array with their share.
     *      All shares must sum to exactly 10,000 (100%).
     *      Maximum 10 collaborators per configuration.
     * @param _collaborators Array of collaborator addresses (including the creator).
     * @param _shares Array of share percentages in basis points.
     */
    function setRevenueShare(
        address payable[] calldata _collaborators,
        uint16[] calldata _shares
    ) external {
        if (creators[msg.sender].wallet == address(0)) {
            revert CreatorNotRegistered(msg.sender);
        }
        if (_collaborators.length != _shares.length) {
            revert InvalidShareTotal(0);
        }
        if (_collaborators.length > MAX_COLLABORATORS) {
            revert TooManyCollaborators(_collaborators.length);
        }

        uint256 total = 0;
        for (uint256 i = 0; i < _shares.length; i++) {
            if (_collaborators[i] == address(0)) revert ZeroAddress();
            total += _shares[i];
        }
        if (total != BPS_DENOMINATOR) revert InvalidShareTotal(total);

        RevenueShare storage rs = _revenueShares[msg.sender];
        // Clear existing collaborators
        delete _revenueShares[msg.sender];

        rs.active = true;
        for (uint256 i = 0; i < _collaborators.length; i++) {
            rs.collaborators.push(_collaborators[i]);
            rs.shares.push(_shares[i]);
        }

        emit RevenueShareConfigured(msg.sender, _collaborators.length);
    }

    /**
     * @notice Deactivates the revenue share configuration for the calling creator.
     * @dev After deactivation, all revenue goes directly to the creator's balance.
     */
    function removeRevenueShare() external {
        if (creators[msg.sender].wallet == address(0)) {
            revert CreatorNotRegistered(msg.sender);
        }
        delete _revenueShares[msg.sender];
    }

    /**
     * @notice Returns the revenue share configuration for a creator.
     * @param _creator Address of the creator.
     * @return collaborators Array of collaborator addresses.
     * @return shares Array of share percentages in basis points.
     * @return active Whether revenue sharing is currently active.
     */
    function getRevenueShare(
        address _creator
    )
        external
        view
        returns (
            address payable[] memory collaborators,
            uint16[] memory shares,
            bool active
        )
    {
        RevenueShare storage rs = _revenueShares[_creator];
        return (rs.collaborators, rs.shares, rs.active);
    }

    // ──────────────────────────────────────────────────────────────
    //  Withdrawals
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Withdraws the creator's accumulated balance.
     * @dev Only the creator themselves can withdraw. Sends all withdrawable balance.
     */
    function withdraw() external nonReentrant whenNotPaused {
        CreatorProfile storage profile = creators[msg.sender];
        if (profile.wallet == address(0)) revert CreatorNotRegistered(msg.sender);

        uint256 balance = profile.withdrawableBalance;
        if (balance == 0) revert NoBalanceToWithdraw(msg.sender);

        profile.withdrawableBalance = 0;
        profile.wallet.sendValue(balance);

        emit CreatorWithdrawal(msg.sender, balance);
    }

    // ──────────────────────────────────────────────────────────────
    //  Internal Functions
    // ──────────────────────────────────────────────────────────────

    /**
     * @dev Distributes an amount to a creator, either directly to their withdrawable
     *      balance or split among collaborators if revenue sharing is active.
     * @param _creator Address of the creator.
     * @param _amount Amount to distribute (after platform fee).
     */
    function _distributeToCreator(address _creator, uint256 _amount) internal {
        RevenueShare storage rs = _revenueShares[_creator];

        if (rs.active && rs.collaborators.length > 0) {
            // Distribute among collaborators
            uint256 distributed = 0;
            for (uint256 i = 0; i < rs.collaborators.length; i++) {
                uint256 share;
                if (i == rs.collaborators.length - 1) {
                    // Last collaborator gets remainder to avoid rounding loss
                    share = _amount - distributed;
                } else {
                    share = (_amount * rs.shares[i]) / BPS_DENOMINATOR;
                }

                // If collaborator is a registered creator, add to their balance
                if (creators[rs.collaborators[i]].wallet != address(0)) {
                    creators[rs.collaborators[i]].withdrawableBalance += share;
                } else {
                    // Direct transfer for non-creator collaborators
                    rs.collaborators[i].sendValue(share);
                }

                distributed += share;

                emit RevenueShared(_creator, rs.collaborators[i], share);
            }
        } else {
            // All to creator's withdrawable balance
            creators[_creator].withdrawableBalance += _amount;
        }
    }

    // ──────────────────────────────────────────────────────────────
    //  Admin Functions
    // ──────────────────────────────────────────────────────────────

    /**
     * @notice Verifies a creator profile.
     * @dev Only callable by the contract owner.
     * @param _creator Address of the creator to verify.
     */
    function verifyCreator(address _creator) external onlyOwner {
        if (creators[_creator].wallet == address(0)) {
            revert CreatorNotRegistered(_creator);
        }
        creators[_creator].verified = true;
        emit CreatorVerified(_creator);
    }

    /**
     * @notice Updates the platform fee recipient address.
     * @param _newRecipient New fee recipient address.
     */
    function setPlatformFeeRecipient(address payable _newRecipient) external onlyOwner {
        if (_newRecipient == address(0)) revert ZeroAddress();
        platformFeeRecipient = _newRecipient;
    }

    /**
     * @notice Pauses all monetization operations.
     */
    function pause() external onlyOwner {
        _pause();
    }

    /**
     * @notice Unpauses all monetization operations.
     */
    function unpause() external onlyOwner {
        _unpause();
    }
}
