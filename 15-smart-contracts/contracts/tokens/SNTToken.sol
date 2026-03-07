// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Burnable.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

/**
 * @title SNTToken
 * @author Ierahkwa Ne Kanienke — Sovereign Digital Nation
 * @notice ERC-20 Sovereign Nation Token — one instance per tribal nation.
 * @dev Extends IGTToken's pattern with on-chain cultural metadata: each SNT
 *      records the `region`, `language`, and `country` of the issuing nation.
 *      These immutable strings enable the SNTFactory to index tokens by region
 *      and provide downstream contracts with verifiable provenance.
 *
 *      574 tribal nations across 19 countries will each receive their own SNT,
 *      giving every community a distinct governance and economic identity on
 *      the MameyNode sovereign blockchain.
 *
 *      Mint/burn semantics mirror IGTToken:
 *        - initialSupply minted to treasury at deploy.
 *        - maxSupply = 2 * initialSupply.
 *        - Owner can mint up to maxSupply.
 *        - Any holder can burn their own tokens.
 */
contract SNTToken is ERC20, ERC20Burnable, Ownable {
    // -----------------------------------------------------------------------
    //  Cultural metadata (immutable after construction)
    // -----------------------------------------------------------------------

    /// @notice Geographic region of the tribal nation (e.g. "Great Plains").
    string public region;

    /// @notice Primary language of the tribal nation (e.g. "Lakota").
    string public language;

    /// @notice Country where the tribal nation is located (e.g. "US").
    string public country;

    // -----------------------------------------------------------------------
    //  State
    // -----------------------------------------------------------------------

    /// @notice Hard cap — totalSupply can never exceed this value.
    uint256 public immutable maxSupply;

    // -----------------------------------------------------------------------
    //  Errors
    // -----------------------------------------------------------------------

    error ExceedsMaxSupply(uint256 requested, uint256 available);
    error ZeroInitialSupply();
    error ZeroTreasuryAddress();

    // -----------------------------------------------------------------------
    //  Constructor
    // -----------------------------------------------------------------------

    /**
     * @param _name         Human-readable token name (e.g. "Navajo Nation Token").
     * @param _symbol       Ticker symbol             (e.g. "SNT-NAV").
     * @param _initialSupply Tokens minted to treasury at genesis.
     * @param _region       Geographic / cultural region.
     * @param _language     Primary language of the nation.
     * @param _country      ISO-3166 country code or full name.
     * @param _treasury     Recipient of the initial supply.
     * @param _owner        Ownable administrator (Consejo Soberano).
     */
    constructor(
        string memory _name,
        string memory _symbol,
        uint256 _initialSupply,
        string memory _region,
        string memory _language,
        string memory _country,
        address _treasury,
        address _owner
    ) ERC20(_name, _symbol) Ownable(_owner) {
        if (_initialSupply == 0) revert ZeroInitialSupply();
        if (_treasury == address(0)) revert ZeroTreasuryAddress();

        region   = _region;
        language = _language;
        country  = _country;

        maxSupply = _initialSupply * 2;
        _mint(_treasury, _initialSupply);
    }

    // -----------------------------------------------------------------------
    //  Owner-only minting
    // -----------------------------------------------------------------------

    /**
     * @notice Mint additional tokens. Reverts if maxSupply would be exceeded.
     * @param to     Recipient address.
     * @param amount Tokens to mint (18-decimal wei).
     */
    function mint(address to, uint256 amount) external onlyOwner {
        uint256 available = maxSupply - totalSupply();
        if (amount > available) {
            revert ExceedsMaxSupply(amount, available);
        }
        _mint(to, amount);
    }
}
