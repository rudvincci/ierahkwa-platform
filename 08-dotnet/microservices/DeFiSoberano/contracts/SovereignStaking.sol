// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "@openzeppelin/contracts/token/ERC20/IERC20.sol";
import "@openzeppelin/contracts/token/ERC20/utils/SafeERC20.sol";
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/security/Pausable.sol";

/**
 * @title SovereignStaking
 * @dev Staking contract for IRHK tokens with tiered rewards
 * Based on 7 Generations philosophy - longer stakes = better rewards
 */
contract SovereignStaking is ReentrancyGuard, Ownable, Pausable {
    using SafeERC20 for IERC20;

    IERC20 public immutable stakingToken;
    IERC20 public immutable rewardToken;

    // Staking tiers (in days)
    uint256 public constant TIER_1_DAYS = 30;   // 1 month - 5% APY
    uint256 public constant TIER_2_DAYS = 90;   // 3 months - 10% APY
    uint256 public constant TIER_3_DAYS = 180;  // 6 months - 15% APY
    uint256 public constant TIER_4_DAYS = 365;  // 1 year - 25% APY
    uint256 public constant TIER_5_DAYS = 730;  // 2 years - 40% APY
    uint256 public constant TIER_6_DAYS = 1825; // 5 years - 70% APY
    uint256 public constant TIER_7_DAYS = 2555; // 7 years - 100% APY (7 Generations)

    // APY rates (basis points, 10000 = 100%)
    mapping(uint256 => uint256) public tierAPY;

    struct Stake {
        uint256 amount;
        uint256 startTime;
        uint256 lockDays;
        uint256 tier;
        uint256 lastRewardTime;
        bool active;
    }

    // User stakes (user => stakeId => Stake)
    mapping(address => mapping(uint256 => Stake)) public stakes;
    mapping(address => uint256) public stakeCount;
    mapping(address => uint256) public totalStaked;

    // Global stats
    uint256 public totalStakedGlobal;
    uint256 public totalRewardsPaid;

    // Events
    event Staked(address indexed user, uint256 indexed stakeId, uint256 amount, uint256 tier, uint256 lockDays);
    event Unstaked(address indexed user, uint256 indexed stakeId, uint256 amount, uint256 reward);
    event RewardClaimed(address indexed user, uint256 indexed stakeId, uint256 reward);
    event EmergencyWithdraw(address indexed user, uint256 indexed stakeId, uint256 amount);

    constructor(address _stakingToken, address _rewardToken) Ownable(msg.sender) {
        stakingToken = IERC20(_stakingToken);
        rewardToken = IERC20(_rewardToken);

        // Initialize tier APYs
        tierAPY[1] = 500;   // 5%
        tierAPY[2] = 1000;  // 10%
        tierAPY[3] = 1500;  // 15%
        tierAPY[4] = 2500;  // 25%
        tierAPY[5] = 4000;  // 40%
        tierAPY[6] = 7000;  // 70%
        tierAPY[7] = 10000; // 100%
    }

    /**
     * @dev Stake tokens with a specific tier
     */
    function stake(uint256 amount, uint256 tier) external nonReentrant whenNotPaused {
        require(amount > 0, "Cannot stake 0");
        require(tier >= 1 && tier <= 7, "Invalid tier");

        uint256 lockDays = getTierLockDays(tier);
        uint256 stakeId = stakeCount[msg.sender]++;

        stakes[msg.sender][stakeId] = Stake({
            amount: amount,
            startTime: block.timestamp,
            lockDays: lockDays,
            tier: tier,
            lastRewardTime: block.timestamp,
            active: true
        });

        totalStaked[msg.sender] += amount;
        totalStakedGlobal += amount;

        stakingToken.safeTransferFrom(msg.sender, address(this), amount);

        emit Staked(msg.sender, stakeId, amount, tier, lockDays);
    }

    /**
     * @dev Unstake tokens after lock period
     */
    function unstake(uint256 stakeId) external nonReentrant {
        Stake storage userStake = stakes[msg.sender][stakeId];
        require(userStake.active, "Stake not active");
        
        uint256 unlockTime = userStake.startTime + (userStake.lockDays * 1 days);
        require(block.timestamp >= unlockTime, "Still locked");

        uint256 reward = calculateReward(msg.sender, stakeId);
        uint256 amount = userStake.amount;

        userStake.active = false;
        totalStaked[msg.sender] -= amount;
        totalStakedGlobal -= amount;
        totalRewardsPaid += reward;

        stakingToken.safeTransfer(msg.sender, amount);
        if (reward > 0) {
            rewardToken.safeTransfer(msg.sender, reward);
        }

        emit Unstaked(msg.sender, stakeId, amount, reward);
    }

    /**
     * @dev Claim rewards without unstaking
     */
    function claimReward(uint256 stakeId) external nonReentrant {
        Stake storage userStake = stakes[msg.sender][stakeId];
        require(userStake.active, "Stake not active");

        uint256 reward = calculateReward(msg.sender, stakeId);
        require(reward > 0, "No rewards to claim");

        userStake.lastRewardTime = block.timestamp;
        totalRewardsPaid += reward;

        rewardToken.safeTransfer(msg.sender, reward);

        emit RewardClaimed(msg.sender, stakeId, reward);
    }

    /**
     * @dev Calculate pending reward
     */
    function calculateReward(address user, uint256 stakeId) public view returns (uint256) {
        Stake memory userStake = stakes[user][stakeId];
        if (!userStake.active) return 0;

        uint256 timeElapsed = block.timestamp - userStake.lastRewardTime;
        uint256 apy = tierAPY[userStake.tier];
        
        // reward = (amount * apy * timeElapsed) / (365 days * 10000)
        return (userStake.amount * apy * timeElapsed) / (365 days * 10000);
    }

    /**
     * @dev Get lock days for a tier
     */
    function getTierLockDays(uint256 tier) public pure returns (uint256) {
        if (tier == 1) return TIER_1_DAYS;
        if (tier == 2) return TIER_2_DAYS;
        if (tier == 3) return TIER_3_DAYS;
        if (tier == 4) return TIER_4_DAYS;
        if (tier == 5) return TIER_5_DAYS;
        if (tier == 6) return TIER_6_DAYS;
        if (tier == 7) return TIER_7_DAYS;
        revert("Invalid tier");
    }

    /**
     * @dev Get user's active stakes
     */
    function getUserStakes(address user) external view returns (Stake[] memory) {
        uint256 count = stakeCount[user];
        Stake[] memory userStakes = new Stake[](count);
        
        for (uint256 i = 0; i < count; i++) {
            userStakes[i] = stakes[user][i];
        }
        
        return userStakes;
    }

    /**
     * @dev Emergency withdraw (forfeit rewards)
     */
    function emergencyWithdraw(uint256 stakeId) external nonReentrant {
        Stake storage userStake = stakes[msg.sender][stakeId];
        require(userStake.active, "Stake not active");

        uint256 amount = userStake.amount;
        
        // 10% penalty for early withdrawal
        uint256 penalty = amount / 10;
        uint256 returnAmount = amount - penalty;

        userStake.active = false;
        totalStaked[msg.sender] -= amount;
        totalStakedGlobal -= amount;

        stakingToken.safeTransfer(msg.sender, returnAmount);
        stakingToken.safeTransfer(owner(), penalty); // Penalty to treasury

        emit EmergencyWithdraw(msg.sender, stakeId, returnAmount);
    }

    /**
     * @dev Update tier APY (admin only)
     */
    function setTierAPY(uint256 tier, uint256 apy) external onlyOwner {
        require(tier >= 1 && tier <= 7, "Invalid tier");
        require(apy <= 20000, "APY too high"); // Max 200%
        tierAPY[tier] = apy;
    }

    /**
     * @dev Pause staking (emergency)
     */
    function pause() external onlyOwner {
        _pause();
    }

    /**
     * @dev Unpause staking
     */
    function unpause() external onlyOwner {
        _unpause();
    }

    /**
     * @dev Recover stuck tokens (not staking/reward tokens)
     */
    function recoverToken(address token, uint256 amount) external onlyOwner {
        require(token != address(stakingToken) && token != address(rewardToken), "Cannot recover staking tokens");
        IERC20(token).safeTransfer(owner(), amount);
    }
}
