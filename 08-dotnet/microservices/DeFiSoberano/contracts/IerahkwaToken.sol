// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Burnable.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Permit.sol";
import "@openzeppelin/contracts/access/AccessControl.sol";
import "@openzeppelin/contracts/security/Pausable.sol";
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";

/**
 * @title IerahkwaToken (IRHK)
 * @dev Sovereign digital currency for the IERAHKWA Nation
 * Features: Mintable, Burnable, Pausable, Role-based access
 */
contract IerahkwaToken is ERC20, ERC20Burnable, ERC20Permit, AccessControl, Pausable, ReentrancyGuard {
    bytes32 public constant MINTER_ROLE = keccak256("MINTER_ROLE");
    bytes32 public constant PAUSER_ROLE = keccak256("PAUSER_ROLE");
    bytes32 public constant TREASURY_ROLE = keccak256("TREASURY_ROLE");

    // Sovereign treasury address
    address public treasury;
    
    // Transaction fee (basis points, 100 = 1%)
    uint256 public transferFee = 0; // No fee by default
    
    // Maximum supply (7 generations concept)
    uint256 public constant MAX_SUPPLY = 7_000_000_000 * 10**18; // 7 billion
    
    // Whitelist for fee exemption
    mapping(address => bool) public feeExempt;
    
    // Events
    event TreasuryUpdated(address indexed oldTreasury, address indexed newTreasury);
    event TransferFeeUpdated(uint256 oldFee, uint256 newFee);
    event FeeExemptionUpdated(address indexed account, bool exempt);

    constructor(address _treasury) 
        ERC20("Ierahkwa Sovereign Token", "IRHK") 
        ERC20Permit("Ierahkwa Sovereign Token") 
    {
        require(_treasury != address(0), "Treasury cannot be zero address");
        
        treasury = _treasury;
        
        _grantRole(DEFAULT_ADMIN_ROLE, msg.sender);
        _grantRole(MINTER_ROLE, msg.sender);
        _grantRole(PAUSER_ROLE, msg.sender);
        _grantRole(TREASURY_ROLE, _treasury);
        
        // Fee exempt treasury and deployer
        feeExempt[_treasury] = true;
        feeExempt[msg.sender] = true;
        
        // Initial mint to treasury (1% of max supply)
        _mint(_treasury, 70_000_000 * 10**18);
    }

    /**
     * @dev Mint new tokens (only MINTER_ROLE)
     */
    function mint(address to, uint256 amount) public onlyRole(MINTER_ROLE) {
        require(totalSupply() + amount <= MAX_SUPPLY, "Exceeds maximum supply");
        _mint(to, amount);
    }

    /**
     * @dev Pause all transfers (emergency)
     */
    function pause() public onlyRole(PAUSER_ROLE) {
        _pause();
    }

    /**
     * @dev Unpause transfers
     */
    function unpause() public onlyRole(PAUSER_ROLE) {
        _unpause();
    }

    /**
     * @dev Update treasury address
     */
    function setTreasury(address _treasury) public onlyRole(DEFAULT_ADMIN_ROLE) {
        require(_treasury != address(0), "Treasury cannot be zero address");
        address oldTreasury = treasury;
        treasury = _treasury;
        feeExempt[_treasury] = true;
        emit TreasuryUpdated(oldTreasury, _treasury);
    }

    /**
     * @dev Update transfer fee (max 5%)
     */
    function setTransferFee(uint256 _fee) public onlyRole(TREASURY_ROLE) {
        require(_fee <= 500, "Fee cannot exceed 5%");
        uint256 oldFee = transferFee;
        transferFee = _fee;
        emit TransferFeeUpdated(oldFee, _fee);
    }

    /**
     * @dev Set fee exemption for an address
     */
    function setFeeExempt(address account, bool exempt) public onlyRole(DEFAULT_ADMIN_ROLE) {
        feeExempt[account] = exempt;
        emit FeeExemptionUpdated(account, exempt);
    }

    /**
     * @dev Override transfer to include fee logic
     */
    function _transfer(address from, address to, uint256 amount) internal virtual override whenNotPaused {
        if (transferFee > 0 && !feeExempt[from] && !feeExempt[to]) {
            uint256 fee = (amount * transferFee) / 10000;
            uint256 netAmount = amount - fee;
            
            super._transfer(from, treasury, fee);
            super._transfer(from, to, netAmount);
        } else {
            super._transfer(from, to, amount);
        }
    }

    /**
     * @dev Sovereign airdrop to multiple addresses
     */
    function airdrop(address[] calldata recipients, uint256[] calldata amounts) 
        external 
        onlyRole(TREASURY_ROLE) 
        nonReentrant 
    {
        require(recipients.length == amounts.length, "Arrays length mismatch");
        require(recipients.length <= 500, "Too many recipients");
        
        for (uint256 i = 0; i < recipients.length; i++) {
            _transfer(msg.sender, recipients[i], amounts[i]);
        }
    }

    /**
     * @dev Burn tokens from treasury (deflationary mechanism)
     */
    function treasuryBurn(uint256 amount) external onlyRole(TREASURY_ROLE) {
        _burn(treasury, amount);
    }
}
