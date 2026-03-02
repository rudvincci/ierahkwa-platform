// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

/**
 * @title IerahkwaInheritance — Bóveda de Herencia Criptográfica (Dead Man's Switch)
 * @dev v11.0.0-PHANTOM: Si el Heartbeat del Guardián falla por > 180 días,
 *      el heredero designado puede reclamar la reputación ($MATTR) del Guardián.
 *      EIP-5192 Soulbound Legacy Transfer.
 *      Gobierno Soberano de Ierahkwa Ne Kanienke
 */

interface IIerahkwaReputation {
    function balanceOf(address account) external view returns (uint256);
    function transferSovereignty(address from, address to) external;
}

interface IIerahkwaPulse {
    function lastPulseTimestamp(address guardian) external view returns (uint256);
    function isLeadGuardian(address guardian) external view returns (bool);
}

contract IerahkwaInheritance {

    IIerahkwaReputation public reputation;
    IIerahkwaPulse public pulse;

    mapping(address => address) public designatedHeir;
    uint256 public constant INACTIVITY_LIMIT = 180 days;

    event HeirDesignated(address indexed guardian, address indexed heir);
    event LegacyTransferred(address indexed from, address indexed to);

    constructor(address _reputation, address _pulse) {
        require(_reputation != address(0) && _pulse != address(0), "Invalid addresses");
        reputation = IIerahkwaReputation(_reputation);
        pulse = IIerahkwaPulse(_pulse);
    }

    /// @notice Designar heredero para reclamar legado si el Guardián deja de pulsar.
    function designateHeir(address _heir) external {
        require(_heir != address(0), "Heir cannot be zero");
        designatedHeir[msg.sender] = _heir;
        emit HeirDesignated(msg.sender, _heir);
    }

    /// @notice Reclamar legado cuando el Guardián no ha pulsado en 180 días.
    function claimLegacy(address _guardian) external {
        require(designatedHeir[_guardian] == msg.sender, "No eres el heredero designado");
        require(
            pulse.lastPulseTimestamp(_guardian) > 0,
            "Guardian nunca pulso"
        );
        require(
            block.timestamp > pulse.lastPulseTimestamp(_guardian) + INACTIVITY_LIMIT,
            "El Guardian aun esta activo"
        );
        require(
            reputation.balanceOf(_guardian) > 0,
            "Guardian sin MATTR"
        );

        reputation.transferSovereignty(_guardian, msg.sender);
        emit LegacyTransferred(_guardian, msg.sender);
    }
}
