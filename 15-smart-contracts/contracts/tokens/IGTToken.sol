// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Burnable.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

/**
 * @title IGTToken
 * @author Ierahkwa Ne Kanienke — Sovereign Digital Nation
 * @notice ERC-20 Indigenous Governance Token deployed by the IGTFactory.
 * @dev Each IGT represents one governance vertical inside the sovereign
 *      ecosystem.  The factory creates an instance per symbol and mints the
 *      initial supply to the designated treasury.  The owner (Consejo
 *      Soberano) may mint additional tokens up to `maxSupply` which equals
 *      twice the initial supply, providing controlled expansion capacity.
 *
 *      Design goals:
 *        - Gas-efficient: no extra storage beyond what OZ provides.
 *        - Burnable by any holder for their own balance.
 *        - Immutable max-supply cap enforced on-chain.
 */
contract IGTToken is ERC20, ERC20Burnable, Ownable {
    // -----------------------------------------------------------------------
    //  State
    // -----------------------------------------------------------------------

    /// @notice Hard cap — no more tokens can ever exist beyond this amount.
    uint256 public immutable maxSupply;

    // -----------------------------------------------------------------------
    //  Errors
    // -----------------------------------------------------------------------

    /// @dev Thrown when a mint would push totalSupply beyond maxSupply.
    error ExceedsMaxSupply(uint256 requested, uint256 available);

    /// @dev Thrown when the initial supply is zero.
    error ZeroInitialSupply();

    /// @dev Thrown when the treasury address is the zero address.
    error ZeroTreasuryAddress();

    // -----------------------------------------------------------------------
    //  Constructor
    // -----------------------------------------------------------------------

    /**
     * @param _name         Human-readable token name  (e.g. "IGT Governance").
     * @param _symbol       Ticker symbol               (e.g. "IGT-GOV").
     * @param _initialSupply Total tokens minted at genesis (18-decimal wei).
     * @param _treasury     Address that receives the initial supply.
     * @param _owner        Address granted the Ownable role (Consejo Soberano).
     */
    constructor(
        string memory _name,
        string memory _symbol,
        uint256 _initialSupply,
        address _treasury,
        address _owner
    ) ERC20(_name, _symbol) Ownable(_owner) {
        if (_initialSupply == 0) revert ZeroInitialSupply();
        if (_treasury == address(0)) revert ZeroTreasuryAddress();

        maxSupply = _initialSupply * 2;
        _mint(_treasury, _initialSupply);
    }

    // -----------------------------------------------------------------------
    //  Owner-only minting
    // -----------------------------------------------------------------------

    /**
     * @notice Mint additional tokens to `to`. Callable only by the owner.
     * @dev Reverts if the resulting totalSupply would exceed `maxSupply`.
     * @param to     Recipient of the newly minted tokens.
     * @param amount Number of tokens (18-decimal wei) to mint.
     */
    function mint(address to, uint256 amount) external onlyOwner {
        uint256 available = maxSupply - totalSupply();
        if (amount > available) {
            revert ExceedsMaxSupply(amount, available);
        }
        _mint(to, amount);
    }
}
