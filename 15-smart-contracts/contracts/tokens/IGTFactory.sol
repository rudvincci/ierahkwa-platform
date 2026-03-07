// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import "@openzeppelin/contracts/access/Ownable.sol";
import "./IGTToken.sol";

/**
 * @title IGTFactory
 * @author Ierahkwa Ne Kanienke — Sovereign Digital Nation
 * @notice Factory contract that deploys Indigenous Governance Tokens (IGT)
 *         in gas-safe batches.
 * @dev The Consejo Soberano (owner) calls `deployBatch` to instantiate up to
 *      25 IGTToken contracts per transaction.  Each deployed token is indexed
 *      by the keccak256 hash of its symbol for O(1) lookups, and also
 *      appended to the `deployedTokens` array for off-chain enumeration.
 *
 *      The 103 IGT governance tokens that power the ecosystem can be deployed
 *      in as few as 5 batches (103 / 25 = 4.12 => 5 transactions).
 */
contract IGTFactory is Ownable {
    // -----------------------------------------------------------------------
    //  State
    // -----------------------------------------------------------------------

    /// @notice Mapping from keccak256(symbol) to the deployed IGTToken address.
    mapping(bytes32 => address) public tokens;

    /// @notice Ordered list of every token address deployed through this factory.
    address[] public deployedTokens;

    // -----------------------------------------------------------------------
    //  Constants
    // -----------------------------------------------------------------------

    /// @notice Maximum number of tokens that can be deployed in a single batch.
    uint256 public constant MAX_BATCH_SIZE = 25;

    // -----------------------------------------------------------------------
    //  Events
    // -----------------------------------------------------------------------

    /**
     * @dev Emitted each time a new IGTToken is deployed.
     * @param symbol       Ticker symbol of the token.
     * @param name         Human-readable name.
     * @param tokenAddress Address of the newly deployed ERC-20 contract.
     * @param supply       Initial supply minted to the treasury.
     */
    event TokenDeployed(
        string indexed symbol,
        string name,
        address indexed tokenAddress,
        uint256 supply
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
     * @notice Deploy a batch of IGTToken contracts.
     * @param symbols  Array of ticker symbols    (e.g. ["IGT-GOV", "IGT-DEF"]).
     * @param names    Array of human-readable names.
     * @param supplies Array of initial supplies (18-decimal wei).
     * @param treasury Address that receives every initial supply.
     */
    function deployBatch(
        string[] calldata symbols,
        string[] calldata names,
        uint256[] calldata supplies,
        address treasury
    ) external onlyOwner {
        uint256 length = symbols.length;

        if (length == 0) revert ZeroLengthBatch();
        if (length > MAX_BATCH_SIZE) revert BatchTooLarge(length, MAX_BATCH_SIZE);
        if (length != names.length || length != supplies.length) {
            revert ArrayLengthMismatch();
        }

        for (uint256 i; i < length; ) {
            bytes32 key = keccak256(bytes(symbols[i]));
            if (tokens[key] != address(0)) {
                revert SymbolAlreadyDeployed(symbols[i]);
            }

            IGTToken token = new IGTToken(
                names[i],
                symbols[i],
                supplies[i],
                treasury,
                owner() // Consejo Soberano retains ownership of each token
            );

            address tokenAddr = address(token);
            tokens[key] = tokenAddr;
            deployedTokens.push(tokenAddr);

            emit TokenDeployed(symbols[i], names[i], tokenAddr, supplies[i]);

            unchecked { ++i; }
        }
    }

    // -----------------------------------------------------------------------
    //  View helpers
    // -----------------------------------------------------------------------

    /**
     * @notice Look up a deployed token by its symbol string.
     * @param symbol Ticker symbol (case-sensitive).
     * @return The address of the deployed IGTToken, or address(0) if not found.
     */
    function getToken(string calldata symbol) external view returns (address) {
        return tokens[keccak256(bytes(symbol))];
    }

    /**
     * @notice Total number of IGT tokens deployed through this factory.
     */
    function getTokenCount() external view returns (uint256) {
        return deployedTokens.length;
    }
}
