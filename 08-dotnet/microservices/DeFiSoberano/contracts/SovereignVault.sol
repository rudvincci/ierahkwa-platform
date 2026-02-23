// SPDX-License-Identifier: MIT
// ============================================================================
// IERAHKWA SOVEREIGN VAULT - Multi-asset yield vault
// Accepts deposits and generates yield through strategies
// ============================================================================

pragma solidity ^0.8.20;

import "@openzeppelin/contracts/token/ERC20/IERC20.sol";
import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/token/ERC20/utils/SafeERC20.sol";
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";
import "@openzeppelin/contracts/security/Pausable.sol";
import "@openzeppelin/contracts/access/AccessControl.sol";

contract SovereignVault is ERC20, ReentrancyGuard, Pausable, AccessControl {
    using SafeERC20 for IERC20;

    bytes32 public constant STRATEGIST_ROLE = keccak256("STRATEGIST_ROLE");
    bytes32 public constant GUARDIAN_ROLE = keccak256("GUARDIAN_ROLE");

    IERC20 public immutable asset;
    
    // Vault parameters
    uint256 public depositFee = 0;      // Basis points
    uint256 public withdrawFee = 50;    // 0.5% withdrawal fee
    uint256 public performanceFee = 1000; // 10% performance fee
    uint256 public constant MAX_FEE = 2000; // Max 20%
    
    address public feeRecipient;
    address public strategy;
    
    uint256 public totalDeposited;
    uint256 public lastHarvestTime;
    uint256 public totalHarvested;

    // Deposit limits
    uint256 public maxDeposit = type(uint256).max;
    uint256 public minDeposit = 0;
    mapping(address => uint256) public userDeposits;

    // Events
    event Deposited(address indexed user, uint256 assets, uint256 shares);
    event Withdrawn(address indexed user, uint256 assets, uint256 shares);
    event Harvested(uint256 profit, uint256 performanceFee);
    event StrategyUpdated(address indexed oldStrategy, address indexed newStrategy);
    event FeesUpdated(uint256 depositFee, uint256 withdrawFee, uint256 performanceFee);

    constructor(
        address _asset,
        string memory _name,
        string memory _symbol,
        address _feeRecipient
    ) ERC20(_name, _symbol) {
        require(_asset != address(0), "Invalid asset");
        require(_feeRecipient != address(0), "Invalid fee recipient");

        asset = IERC20(_asset);
        feeRecipient = _feeRecipient;

        _grantRole(DEFAULT_ADMIN_ROLE, msg.sender);
        _grantRole(STRATEGIST_ROLE, msg.sender);
        _grantRole(GUARDIAN_ROLE, msg.sender);
    }

    // =========================================================================
    // DEPOSIT & WITHDRAW
    // =========================================================================

    function deposit(uint256 assets) external nonReentrant whenNotPaused returns (uint256 shares) {
        require(assets > 0, "Cannot deposit 0");
        require(assets >= minDeposit, "Below minimum");
        require(assets <= maxDeposit, "Exceeds maximum");

        // Calculate shares
        shares = _convertToShares(assets);
        require(shares > 0, "Zero shares");

        // Transfer assets
        asset.safeTransferFrom(msg.sender, address(this), assets);

        // Apply deposit fee
        if (depositFee > 0) {
            uint256 fee = (assets * depositFee) / 10000;
            asset.safeTransfer(feeRecipient, fee);
            assets -= fee;
        }

        // Mint shares
        _mint(msg.sender, shares);
        
        userDeposits[msg.sender] += assets;
        totalDeposited += assets;

        // Deploy to strategy if set
        if (strategy != address(0)) {
            _deployToStrategy(assets);
        }

        emit Deposited(msg.sender, assets, shares);
    }

    function withdraw(uint256 shares) external nonReentrant returns (uint256 assets) {
        require(shares > 0, "Cannot withdraw 0");
        require(balanceOf(msg.sender) >= shares, "Insufficient shares");

        // Calculate assets
        assets = _convertToAssets(shares);
        require(assets > 0, "Zero assets");

        // Withdraw from strategy if needed
        uint256 available = asset.balanceOf(address(this));
        if (assets > available && strategy != address(0)) {
            _withdrawFromStrategy(assets - available);
        }

        // Burn shares
        _burn(msg.sender, shares);

        // Apply withdrawal fee
        if (withdrawFee > 0) {
            uint256 fee = (assets * withdrawFee) / 10000;
            asset.safeTransfer(feeRecipient, fee);
            assets -= fee;
        }

        // Update accounting
        if (userDeposits[msg.sender] >= assets) {
            userDeposits[msg.sender] -= assets;
        } else {
            userDeposits[msg.sender] = 0;
        }
        totalDeposited = totalDeposited > assets ? totalDeposited - assets : 0;

        // Transfer to user
        asset.safeTransfer(msg.sender, assets);

        emit Withdrawn(msg.sender, assets, shares);
    }

    // =========================================================================
    // HARVEST
    // =========================================================================

    function harvest() external onlyRole(STRATEGIST_ROLE) {
        require(strategy != address(0), "No strategy");

        uint256 balanceBefore = asset.balanceOf(address(this));
        
        // Call strategy harvest (interface would be defined separately)
        // IStrategy(strategy).harvest();
        
        uint256 balanceAfter = asset.balanceOf(address(this));
        uint256 profit = balanceAfter > balanceBefore ? balanceAfter - balanceBefore : 0;

        if (profit > 0) {
            // Take performance fee
            uint256 fee = (profit * performanceFee) / 10000;
            if (fee > 0) {
                asset.safeTransfer(feeRecipient, fee);
            }
            
            totalHarvested += profit - fee;
            emit Harvested(profit, fee);
        }

        lastHarvestTime = block.timestamp;
    }

    // =========================================================================
    // VIEW FUNCTIONS
    // =========================================================================

    function totalAssets() public view returns (uint256) {
        uint256 vaultBalance = asset.balanceOf(address(this));
        // In production, add strategy balance
        // if (strategy != address(0)) {
        //     vaultBalance += IStrategy(strategy).balanceOf();
        // }
        return vaultBalance;
    }

    function pricePerShare() public view returns (uint256) {
        uint256 supply = totalSupply();
        if (supply == 0) return 1e18;
        return (totalAssets() * 1e18) / supply;
    }

    function convertToShares(uint256 assets) external view returns (uint256) {
        return _convertToShares(assets);
    }

    function convertToAssets(uint256 shares) external view returns (uint256) {
        return _convertToAssets(shares);
    }

    function maxWithdraw(address owner) external view returns (uint256) {
        return _convertToAssets(balanceOf(owner));
    }

    function getUserInfo(address user) external view returns (
        uint256 shares,
        uint256 assets,
        uint256 deposited
    ) {
        shares = balanceOf(user);
        assets = _convertToAssets(shares);
        deposited = userDeposits[user];
    }

    // =========================================================================
    // INTERNAL
    // =========================================================================

    function _convertToShares(uint256 assets) internal view returns (uint256) {
        uint256 supply = totalSupply();
        if (supply == 0) return assets;
        return (assets * supply) / totalAssets();
    }

    function _convertToAssets(uint256 shares) internal view returns (uint256) {
        uint256 supply = totalSupply();
        if (supply == 0) return shares;
        return (shares * totalAssets()) / supply;
    }

    function _deployToStrategy(uint256 amount) internal {
        asset.safeTransfer(strategy, amount);
        // IStrategy(strategy).deposit(amount);
    }

    function _withdrawFromStrategy(uint256 amount) internal {
        // IStrategy(strategy).withdraw(amount);
    }

    // =========================================================================
    // ADMIN
    // =========================================================================

    function setStrategy(address _strategy) external onlyRole(DEFAULT_ADMIN_ROLE) {
        address oldStrategy = strategy;
        strategy = _strategy;
        emit StrategyUpdated(oldStrategy, _strategy);
    }

    function setFees(
        uint256 _depositFee,
        uint256 _withdrawFee,
        uint256 _performanceFee
    ) external onlyRole(DEFAULT_ADMIN_ROLE) {
        require(_depositFee <= MAX_FEE, "Deposit fee too high");
        require(_withdrawFee <= MAX_FEE, "Withdraw fee too high");
        require(_performanceFee <= MAX_FEE, "Performance fee too high");

        depositFee = _depositFee;
        withdrawFee = _withdrawFee;
        performanceFee = _performanceFee;

        emit FeesUpdated(_depositFee, _withdrawFee, _performanceFee);
    }

    function setFeeRecipient(address _feeRecipient) external onlyRole(DEFAULT_ADMIN_ROLE) {
        require(_feeRecipient != address(0), "Invalid recipient");
        feeRecipient = _feeRecipient;
    }

    function setDepositLimits(uint256 _min, uint256 _max) external onlyRole(DEFAULT_ADMIN_ROLE) {
        minDeposit = _min;
        maxDeposit = _max;
    }

    function pause() external onlyRole(GUARDIAN_ROLE) {
        _pause();
    }

    function unpause() external onlyRole(DEFAULT_ADMIN_ROLE) {
        _unpause();
    }

    function emergencyWithdraw() external onlyRole(DEFAULT_ADMIN_ROLE) {
        // Withdraw all from strategy
        if (strategy != address(0)) {
            // IStrategy(strategy).withdrawAll();
        }
        _pause();
    }

    function recoverTokens(address token, uint256 amount) external onlyRole(DEFAULT_ADMIN_ROLE) {
        require(token != address(asset), "Cannot recover vault asset");
        IERC20(token).safeTransfer(msg.sender, amount);
    }
}
