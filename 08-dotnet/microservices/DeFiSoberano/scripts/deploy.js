// ============================================================================
// IERAHKWA SOVEREIGN DEFI - DEPLOYMENT SCRIPT
// Deploys all smart contracts to the specified network
// ============================================================================

const { ethers, network } = require("hardhat");

async function main() {
  console.log("🪶 ═══════════════════════════════════════════════════════════");
  console.log("   IERAHKWA SOVEREIGN DEFI - DEPLOYMENT");
  console.log(`   Network: ${network.name}`);
  console.log("🪶 ═══════════════════════════════════════════════════════════\n");

  const [deployer] = await ethers.getSigners();
  console.log("Deploying contracts with account:", deployer.address);
  console.log("Account balance:", ethers.formatEther(await ethers.provider.getBalance(deployer.address)), "ETH\n");

  // Deploy Treasury (multisig or timelock in production)
  const treasury = deployer.address; // Use deployer as treasury for now

  // 1. Deploy IRHK Token
  console.log("1. Deploying IerahkwaToken (IRHK)...");
  const IerahkwaToken = await ethers.getContractFactory("IerahkwaToken");
  const token = await IerahkwaToken.deploy(treasury);
  await token.waitForDeployment();
  const tokenAddress = await token.getAddress();
  console.log("   ✓ IerahkwaToken deployed to:", tokenAddress);

  // 2. Deploy Staking Contract
  console.log("\n2. Deploying SovereignStaking...");
  const SovereignStaking = await ethers.getContractFactory("SovereignStaking");
  const staking = await SovereignStaking.deploy(tokenAddress, tokenAddress);
  await staking.waitForDeployment();
  const stakingAddress = await staking.getAddress();
  console.log("   ✓ SovereignStaking deployed to:", stakingAddress);

  // 3. Deploy TimelockController for Governance
  console.log("\n3. Deploying TimelockController...");
  const TimelockController = await ethers.getContractFactory("TimelockController");
  const minDelay = 2 * 24 * 60 * 60; // 2 days
  const proposers = [deployer.address];
  const executors = [deployer.address];
  const admin = deployer.address;
  const timelock = await TimelockController.deploy(minDelay, proposers, executors, admin);
  await timelock.waitForDeployment();
  const timelockAddress = await timelock.getAddress();
  console.log("   ✓ TimelockController deployed to:", timelockAddress);

  // 4. Deploy Governance
  console.log("\n4. Deploying SovereignGovernance...");
  const SovereignGovernance = await ethers.getContractFactory("SovereignGovernance");
  const governance = await SovereignGovernance.deploy(tokenAddress, timelockAddress);
  await governance.waitForDeployment();
  const governanceAddress = await governance.getAddress();
  console.log("   ✓ SovereignGovernance deployed to:", governanceAddress);

  // Setup: Transfer tokens to staking contract for rewards
  console.log("\n5. Setting up contracts...");
  const rewardAmount = ethers.parseEther("1000000"); // 1M tokens for rewards
  await token.transfer(stakingAddress, rewardAmount);
  console.log("   ✓ Transferred 1,000,000 IRHK to staking contract for rewards");

  // Grant MINTER_ROLE to staking contract (optional, for inflationary rewards)
  const MINTER_ROLE = await token.MINTER_ROLE();
  // await token.grantRole(MINTER_ROLE, stakingAddress);
  // console.log("   ✓ Granted MINTER_ROLE to staking contract");

  // Summary
  console.log("\n🪶 ═══════════════════════════════════════════════════════════");
  console.log("   DEPLOYMENT COMPLETE!");
  console.log("🪶 ═══════════════════════════════════════════════════════════\n");
  
  console.log("Contract Addresses:");
  console.log("─────────────────────────────────────────────────────────────");
  console.log(`IerahkwaToken (IRHK):    ${tokenAddress}`);
  console.log(`SovereignStaking:        ${stakingAddress}`);
  console.log(`TimelockController:      ${timelockAddress}`);
  console.log(`SovereignGovernance:     ${governanceAddress}`);
  console.log("─────────────────────────────────────────────────────────────\n");

  // Save deployment info
  const deploymentInfo = {
    network: network.name,
    chainId: network.config.chainId,
    deployer: deployer.address,
    timestamp: new Date().toISOString(),
    contracts: {
      IerahkwaToken: tokenAddress,
      SovereignStaking: stakingAddress,
      TimelockController: timelockAddress,
      SovereignGovernance: governanceAddress
    }
  };

  const fs = require("fs");
  const deploymentsDir = "./deployments";
  if (!fs.existsSync(deploymentsDir)) {
    fs.mkdirSync(deploymentsDir);
  }
  fs.writeFileSync(
    `${deploymentsDir}/${network.name}.json`,
    JSON.stringify(deploymentInfo, null, 2)
  );
  console.log(`Deployment info saved to ${deploymentsDir}/${network.name}.json`);

  // Verification commands
  if (network.name !== "hardhat" && network.name !== "localhost") {
    console.log("\nTo verify contracts on Etherscan:");
    console.log(`npx hardhat verify --network ${network.name} ${tokenAddress} "${treasury}"`);
    console.log(`npx hardhat verify --network ${network.name} ${stakingAddress} "${tokenAddress}" "${tokenAddress}"`);
  }
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
