// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import "@openzeppelin/contracts/access/Ownable.sol";
import "./SNTToken.sol";

/**
 * @title SNTFactory
 * @author Ierahkwa Ne Kanienke — Sovereign Digital Nation
 * @notice Factory contract that deploys Sovereign Nation Tokens (SNT) in
 *         gas-safe batches of up to 25.
 * @dev Mirrors IGTFactory's architecture with additional cultural metadata
 *      per token: region, language, and country.  Provides a secondary index
 *      (`getTokensByRegion`) so that downstream protocols can query all
 *      nations within a geographic area.
 *
 *      574 tribal nations can be deployed in ceil(574/25) = 23 batch
 *      transactions — well within MameyNode's block gas limits.
 */
contract SNTFactory is Ownable {
    // -----------------------------------------------------------------------
    //  State
    // -----------------------------------------------------------------------

    /// @notice keccak256(symbol) => deployed SNTToken address.
    mapping(bytes32 => address) public tokens;

    /// @notice Ordered list of every SNT address deployed by this factory.
    address[] public deployedTokens;

    /// @notice region string => array of token addresses in that region.
    mapping(bytes32 => address[]) internal _regionTokens;

    // -----------------------------------------------------------------------
    //  Structs
    // -----------------------------------------------------------------------

    /// @dev Metadata stored per token for off-chain convenience.
    struct TokenMeta {
        string symbol;
        string region;
        string language;
        string country;
    }

    /// @notice token address => metadata snapshot taken at deploy time.
    mapping(address => TokenMeta) public tokenMeta;

    // -----------------------------------------------------------------------
    //  Constants
    // -----------------------------------------------------------------------

    /// @notice Maximum tokens deployable in a single batch call.
    uint256 public constant MAX_BATCH_SIZE = 25;

    // -----------------------------------------------------------------------
    //  Events
    // -----------------------------------------------------------------------

    /**
     * @dev Emitted for each SNTToken deployed.
     * @param symbol       Ticker symbol.
     * @param name         Human-readable name.
     * @param tokenAddress Deployed contract address.
     * @param supply       Initial supply minted to treasury.
     * @param region       Geographic region of the nation.
     * @param language     Primary language.
     * @param country      Country code / name.
     */
    event TokenDeployed(
        string indexed symbol,
        string name,
        address indexed tokenAddress,
        uint256 supply,
        string region,
        string language,
        string country
    );

    // -----------------------------------------------------------------------
    //  Errors
    // -----------------------------------------------------------------------

    error BatchTooLarge(uint256 size, uint256 max);
    error ArrayLengthMismatch();
    error SymbolAlreadyDeployed(string symbol);
    error ZeroLengthBatch();

    // -----------------------------------------------------------------------
    //  Constructor
    // -----------------------------------------------------------------------

    /**
     * @param _owner Address of the Consejo Soberano multisig.
     */
    constructor(address _owner) Ownable(_owner) {}

    // -----------------------------------------------------------------------
    //  Batch deployment
    // -----------------------------------------------------------------------

    /**
     * @notice Deploy a batch of SNTToken contracts.
     * @param symbols   Ticker symbols      (e.g. ["SNT-NAV", "SNT-CHE"]).
     * @param names     Human-readable names (e.g. ["Navajo Nation Token", ...]).
     * @param supplies  Initial supplies (18-decimal wei).
     * @param regions   Geographic regions   (e.g. ["Southwest", "Southeast"]).
     * @param languages Primary languages    (e.g. ["Dine", "Tsalagi"]).
     * @param countries Country identifiers  (e.g. ["US", "US"]).
     * @param treasury  Recipient of initial supply for every token.
     */
    function deployBatch(
        string[] calldata symbols,
        string[] calldata names,
        uint256[] calldata supplies,
        string[] calldata regions,
        string[] calldata languages,
        string[] calldata countries,
        address treasury
    ) external onlyOwner {
        uint256 length = symbols.length;

        if (length == 0) revert ZeroLengthBatch();
        if (length > MAX_BATCH_SIZE) revert BatchTooLarge(length, MAX_BATCH_SIZE);
        if (
            length != names.length ||
            length != supplies.length ||
            length != regions.length ||
            length != languages.length ||
            length != countries.length
        ) {
            revert ArrayLengthMismatch();
        }

        for (uint256 i; i < length; ) {
            bytes32 key = keccak256(bytes(symbols[i]));
            if (tokens[key] != address(0)) {
                revert SymbolAlreadyDeployed(symbols[i]);
            }

            SNTToken token = new SNTToken(
                names[i],
                symbols[i],
                supplies[i],
                regions[i],
                languages[i],
                countries[i],
                treasury,
                owner()
            );

            address tokenAddr = address(token);
            tokens[key] = tokenAddr;
            deployedTokens.push(tokenAddr);

            // Index by region
            bytes32 regionKey = keccak256(bytes(regions[i]));
            _regionTokens[regionKey].push(tokenAddr);

            // Store metadata snapshot
            tokenMeta[tokenAddr] = TokenMeta({
                symbol:   symbols[i],
                region:   regions[i],
                language: languages[i],
                country:  countries[i]
            });

            emit TokenDeployed(
                symbols[i],
                names[i],
                tokenAddr,
                supplies[i],
                regions[i],
                languages[i],
                countries[i]
            );

            unchecked { ++i; }
        }
    }

    // -----------------------------------------------------------------------
    //  View helpers
    // -----------------------------------------------------------------------

    /**
     * @notice Look up a deployed SNT by its symbol.
     * @param symbol Ticker symbol (case-sensitive).
     * @return Deployed contract address, or address(0) if not found.
     */
    function getToken(string calldata symbol) external view returns (address) {
        return tokens[keccak256(bytes(symbol))];
    }

    /**
     * @notice Total number of SNT tokens deployed via this factory.
     */
    function getTokenCount() external view returns (uint256) {
        return deployedTokens.length;
    }

    /**
     * @notice Retrieve all SNT addresses belonging to a given region.
     * @param _region Region string (case-sensitive, must match deploy value).
     * @return Array of SNTToken addresses within the specified region.
     */
    function getTokensByRegion(string calldata _region)
        external
        view
        returns (address[] memory)
    {
        return _regionTokens[keccak256(bytes(_region))];
    }
}
