// SPDX-License-Identifier: Soberana-1.0
pragma solidity ^0.8.20;

/**
 * @title FiscalAllocation
 * @notice Automatically allocates platform revenue to public services
 * @dev NO TAXES - all funding comes from platform transaction fees
 * 
 * CONSTITUTIONAL GUARANTEE:
 * "No citizen shall be required to pay taxes. All public services
 *  shall be funded exclusively through sovereign platform revenues."
 */
contract FiscalAllocation {
    address public education;      // 25% - Schools, universities
    address public healthcare;     // 25% - Hospitals, medicines
    address public infrastructure; // 20% - Roads, water, electricity
    address public technology;     // 15% - MameyNode, AI, Atabey
    address public security;       // 10% - Emergency, defense
    address public reserve;        //  5% - Emergency fund

    address public consejo; // Consejo Digital Soberano (governance)

    event Allocated(uint256 total, uint256 edu, uint256 health, uint256 infra, uint256 tech, uint256 sec, uint256 res);

    constructor(
        address _education, address _healthcare, address _infrastructure,
        address _technology, address _security, address _reserve, address _consejo
    ) {
        education = _education;
        healthcare = _healthcare;
        infrastructure = _infrastructure;
        technology = _technology;
        security = _security;
        reserve = _reserve;
        consejo = _consejo;
    }

    /**
     * @notice Allocate incoming platform revenue to public services
     * @dev Called automatically by BDETPaymentEngine on every transaction
     */
    function allocate() external payable {
        require(msg.value > 0, "Nothing to allocate");
        
        uint256 total = msg.value;
        uint256 edu = (total * 25) / 100;
        uint256 health = (total * 25) / 100;
        uint256 infra = (total * 20) / 100;
        uint256 tech = (total * 15) / 100;
        uint256 sec = (total * 10) / 100;
        uint256 res = total - edu - health - infra - tech - sec; // remainder ~5%

        payable(education).transfer(edu);
        payable(healthcare).transfer(health);
        payable(infrastructure).transfer(infra);
        payable(technology).transfer(tech);
        payable(security).transfer(sec);
        payable(reserve).transfer(res);

        emit Allocated(total, edu, health, infra, tech, sec, res);
    }

    /**
     * @notice Update allocation addresses (requires Consejo 3/4 vote)
     */
    function updateAddresses(
        address _edu, address _health, address _infra,
        address _tech, address _sec, address _res
    ) external {
        require(msg.sender == consejo, "Only Consejo Digital Soberano");
        education = _edu;
        healthcare = _health;
        infrastructure = _infra;
        technology = _tech;
        security = _sec;
        reserve = _res;
    }

    /**
     * @notice Allocation percentages are FIXED in the constitution
     * @dev Cannot be changed - 25/25/20/15/10/5
     * No function exists to change percentages. By design.
     */
}
