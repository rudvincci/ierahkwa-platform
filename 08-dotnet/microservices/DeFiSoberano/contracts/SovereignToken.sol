// SPDX-License-Identifier: MIT
// ============================================================================
// IERAHKWA SOVEREIGN TOKEN - ERC20 with Governance
// Native token for the IERAHKWA DeFi ecosystem
// ============================================================================

pragma solidity ^0.8.20;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Burnable.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Permit.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Votes.sol";
import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/security/Pausable.sol";
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";

contract SovereignToken is 
    ERC20, 
    ERC20Burnable, 
    ERC20Permit, 
    ERC20Votes, 
    AccessControl, 
    Pausable,
    ReentrancyGuard 
{
    bytes32 public constant MINTER_ROLE = keccak256("MINTER_ROLE");
    bytes32 public constant PAUSER_ROLE = keccak256("PAUSER_ROLE");
    bytes32 public constant GOVERNANCE_ROLE = keccak256("GOVERNANCE_ROLE");

    uint256 public constant MAX_SUPPLY = 1_000_000_000 * 10**18; // 1 billion tokens
    uint256 public constant INITIAL_SUPPLY = 100_000_000 * 10**18; // 100 million initial

    // Treasury and ecosystem addresses
    address public treasury;
    address public stakingRewards;
    address public liquidityPool;

    // Fee configuration
    uint256 public transferFee = 0; // Basis points (0 = no fee)
    uint256 public constant MAX_FEE = 500; // Max 5%
    mapping(address => bool) public feeExempt;

    // Events
    event TreasuryUpdated(address indexed oldTreasury, address indexed newTreasury);
    event TransferFeeUpdated(uint256 oldFee, uint256 newFee);
    event FeeExemptionUpdated(address indexed account, bool exempt);
    event TokensRecovered(address indexed token, uint256 amount);

    constructor(
        address _treasury,
        address _admin
    ) ERC20("IERAHKWA Sovereign Token", "IERA") ERC20Permit("IERAHKWA Sovereign Token") {
        require(_treasury != address(0), "Invalid treasury");
        require(_admin != address(0), "Invalid admin");

        treasury = _treasury;

        _grantRole(DEFAULT_ADMIN_ROLE, _admin);
        _grantRole(MINTER_ROLE, _admin);
        _grantRole(PAUSER_ROLE, _admin);
        _grantRole(GOVERNANCE_ROLE, _admin);

        // Fee exemptions
        feeExempt[_treasury] = true;
        feeExempt[_admin] = true;
        feeExempt[address(this)] = true;

        // Mint initial supply to treasury
        _mint(_treasury, INITIAL_SUPPLY);
    }

    // =========================================================================
    // MINTING
    // =========================================================================

    function mint(address to, uint256 amount) external onlyRole(MINTER_ROLE) {
        require(totalSupply() + amount <= MAX_SUPPLY, "Exceeds max supply");
        _mint(to, amount);
    }

    function mintToTreasury(uint256 amount) external onlyRole(MINTER_ROLE) {
        require(totalSupply() + amount <= MAX_SUPPLY, "Exceeds max supply");
        _mint(treasury, amount);
    }

    // =========================================================================
    // TRANSFERS WITH OPTIONAL FEE
    // =========================================================================

    function _transfer(
        address from,
        address to,
        uint256 amount
    ) internal virtual override whenNotPaused {
        uint256 feeAmount = 0;
        
        if (transferFee > 0 && !feeExempt[from] && !feeExempt[to]) {
            feeAmount = (amount * transferFee) / 10000;
            if (feeAmount > 0) {
                super._transfer(from, treasury, feeAmount);
            }
        }

        super._transfer(from, to, amount - feeAmount);
    }

    // =========================================================================
    // GOVERNANCE
    // =========================================================================

    function setTreasury(address _treasury) external onlyRole(GOVERNANCE_ROLE) {
        require(_treasury != address(0), "Invalid treasury");
        address oldTreasury = treasury;
        treasury = _treasury;
        feeExempt[_treasury] = true;
        emit TreasuryUpdated(oldTreasury, _treasury);
    }

    function setTransferFee(uint256 _fee) external onlyRole(GOVERNANCE_ROLE) {
        require(_fee <= MAX_FEE, "Fee too high");
        uint256 oldFee = transferFee;
        transferFee = _fee;
        emit TransferFeeUpdated(oldFee, _fee);
    }

    function setFeeExemption(address account, bool exempt) external onlyRole(GOVERNANCE_ROLE) {
        feeExempt[account] = exempt;
        emit FeeExemptionUpdated(account, exempt);
    }

    function setStakingRewards(address _stakingRewards) external onlyRole(GOVERNANCE_ROLE) {
        stakingRewards = _stakingRewards;
        feeExempt[_stakingRewards] = true;
    }

    function setLiquidityPool(address _liquidityPool) external onlyRole(GOVERNANCE_ROLE) {
        liquidityPool = _liquidityPool;
        feeExempt[_liquidityPool] = true;
    }

    // =========================================================================
    // PAUSE
    // =========================================================================

    function pause() external onlyRole(PAUSER_ROLE) {
        _pause();
    }

    function unpause() external onlyRole(PAUSER_ROLE) {
        _unpause();
    }

    // =========================================================================
    // RECOVERY
    // =========================================================================

    function recoverTokens(
        address token,
        uint256 amount
    ) external onlyRole(DEFAULT_ADMIN_ROLE) nonReentrant {
        require(token != address(this), "Cannot recover native token");
        IERC20(token).transfer(treasury, amount);
        emit TokensRecovered(token, amount);
    }

    // =========================================================================
    // OVERRIDES
    // =========================================================================

    function _afterTokenTransfer(
        address from,
        address to,
        uint256 amount
    ) internal override(ERC20, ERC20Votes) {
        super._afterTokenTransfer(from, to, amount);
    }

    function _mint(address to, uint256 amount) internal override(ERC20, ERC20Votes) {
        super._mint(to, amount);
    }

    function _burn(address account, uint256 amount) internal override(ERC20, ERC20Votes) {
        super._burn(account, amount);
    }
}
