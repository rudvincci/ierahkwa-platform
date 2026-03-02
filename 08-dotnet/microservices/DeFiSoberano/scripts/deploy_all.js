// ============================================================================
// IERAHKWA SOVEREIGN DEFI -- COMPREHENSIVE DEPLOYMENT SCRIPT
// Deploys ALL Ierahkwa contracts in dependency order with verification support.
// Usage:
//   npx hardhat run scripts/deploy_all.js --network <network>
//   npx hardhat run scripts/deploy_all.js --network polygon --verify
// ============================================================================

const { ethers, network, run } = require("hardhat");
const fs = require("fs");
const path = require("path");

// Check if --verify flag was passed
const VERIFY = process.argv.includes("--verify");

async function verifyContract(address, constructorArguments) {
  if (!VERIFY) return;
  if (network.name === "hardhat" || network.name === "localhost" || network.name === "mameynode") {
    return;
  }

  console.log(`   Verifying on explorer...`);
  try {
    await run("verify:verify", {
      address,
      constructorArguments,
    });
    console.log(`   Verified.`);
  } catch (err) {
    if (err.message.includes("Already Verified")) {
      console.log(`   Already verified.`);
    } else {
      console.log(`   Verification failed: ${err.message}`);
    }
  }
}

async function main() {
  console.log("=========================================================================");
  console.log("   IERAHKWA SOVEREIGN DEFI -- FULL DEPLOYMENT");
  console.log(`   Network: ${network.name} (chainId: ${network.config.chainId})`);
  console.log(`   Verification: ${VERIFY ? "ENABLED" : "DISABLED"}`);
  console.log("=========================================================================\n");

  const [deployer] = await ethers.getSigners();
  console.log("Deployer:", deployer.address);
  console.log("Balance:", ethers.formatEther(await ethers.provider.getBalance(deployer.address)), "ETH\n");

  const addresses = {};

  // =========================================================================
  // 1. IerahkwaToken (WMP / IRHK)
  // =========================================================================
  console.log("1/13  Deploying IerahkwaToken (IRHK)...");
  const IerahkwaToken = await ethers.getContractFactory("IerahkwaToken");
  const token = await IerahkwaToken.deploy(deployer.address);
  await token.waitForDeployment();
  addresses.IerahkwaToken = await token.getAddress();
  console.log(`      Address: ${addresses.IerahkwaToken}`);
  await verifyContract(addresses.IerahkwaToken, [deployer.address]);

  // =========================================================================
  // 2. IerahkwaReputation ($MATTR)
  // =========================================================================
  console.log("\n2/13  Deploying IerahkwaReputation (MATTR)...");
  const IerahkwaReputation = await ethers.getContractFactory("IerahkwaReputation");
  const reputation = await IerahkwaReputation.deploy(deployer.address, deployer.address);
  await reputation.waitForDeployment();
  addresses.IerahkwaReputation = await reputation.getAddress();
  console.log(`      Address: ${addresses.IerahkwaReputation}`);
  await verifyContract(addresses.IerahkwaReputation, [deployer.address, deployer.address]);

  // =========================================================================
  // 3. IerahkwaManifesto (references Reputation indirectly)
  // =========================================================================
  console.log("\n3/13  Deploying IerahkwaManifesto (GARDN)...");
  const IerahkwaManifesto = await ethers.getContractFactory("IerahkwaManifesto");
  const manifesto = await IerahkwaManifesto.deploy();
  await manifesto.waitForDeployment();
  addresses.IerahkwaManifesto = await manifesto.getAddress();
  console.log(`      Address: ${addresses.IerahkwaManifesto}`);
  await verifyContract(addresses.IerahkwaManifesto, []);

  // =========================================================================
  // 4. IerahkwaGenesis (references Manifesto)
  // =========================================================================
  console.log("\n4/13  Deploying IerahkwaGenesis (IGENESIS)...");
  const IerahkwaGenesis = await ethers.getContractFactory("IerahkwaGenesis");
  const genesis = await IerahkwaGenesis.deploy(addresses.IerahkwaManifesto);
  await genesis.waitForDeployment();
  addresses.IerahkwaGenesis = await genesis.getAddress();
  console.log(`      Address: ${addresses.IerahkwaGenesis}`);
  await verifyContract(addresses.IerahkwaGenesis, [addresses.IerahkwaManifesto]);

  // =========================================================================
  // 5. IerahkwaTreasury (references Reputation)
  // =========================================================================
  console.log("\n5/13  Deploying IerahkwaTreasury...");
  const IerahkwaTreasury = await ethers.getContractFactory("IerahkwaTreasury");
  const treasury = await IerahkwaTreasury.deploy(deployer.address, addresses.IerahkwaReputation);
  await treasury.waitForDeployment();
  addresses.IerahkwaTreasury = await treasury.getAddress();
  console.log(`      Address: ${addresses.IerahkwaTreasury}`);
  await verifyContract(addresses.IerahkwaTreasury, [deployer.address, addresses.IerahkwaReputation]);

  // =========================================================================
  // 6. IerahkwaQuadraticVoting (references Reputation)
  // =========================================================================
  console.log("\n6/13  Deploying IerahkwaQuadraticVoting...");
  const IerahkwaQuadraticVoting = await ethers.getContractFactory("IerahkwaQuadraticVoting");
  const quadraticVoting = await IerahkwaQuadraticVoting.deploy(deployer.address, addresses.IerahkwaReputation);
  await quadraticVoting.waitForDeployment();
  addresses.IerahkwaQuadraticVoting = await quadraticVoting.getAddress();
  console.log(`      Address: ${addresses.IerahkwaQuadraticVoting}`);
  await verifyContract(addresses.IerahkwaQuadraticVoting, [deployer.address, addresses.IerahkwaReputation]);

  // =========================================================================
  // 7. IerahkwaVeritas
  // =========================================================================
  console.log("\n7/13  Deploying IerahkwaVeritas...");
  const IerahkwaVeritas = await ethers.getContractFactory("IerahkwaVeritas");
  const veritas = await IerahkwaVeritas.deploy(deployer.address, deployer.address);
  await veritas.waitForDeployment();
  addresses.IerahkwaVeritas = await veritas.getAddress();
  console.log(`      Address: ${addresses.IerahkwaVeritas}`);
  await verifyContract(addresses.IerahkwaVeritas, [deployer.address, deployer.address]);

  // =========================================================================
  // 8. IerahkwaOracle
  // =========================================================================
  console.log("\n8/13  Deploying IerahkwaOracle...");
  const IerahkwaOracle = await ethers.getContractFactory("IerahkwaOracle");
  const oracle = await IerahkwaOracle.deploy(deployer.address);
  await oracle.waitForDeployment();
  addresses.IerahkwaOracle = await oracle.getAddress();
  console.log(`      Address: ${addresses.IerahkwaOracle}`);
  await verifyContract(addresses.IerahkwaOracle, [deployer.address]);

  // =========================================================================
  // 9. IerahkwaPulse (references Token indirectly; needs 5 lead guardians)
  // =========================================================================
  console.log("\n9/13  Deploying IerahkwaPulse...");
  // For deployment, use deployer + 4 derived addresses as lead guardians.
  // In production these should be real guardian addresses.
  const leadGuardians = [
    deployer.address,
    ethers.Wallet.createRandom().address,
    ethers.Wallet.createRandom().address,
    ethers.Wallet.createRandom().address,
    ethers.Wallet.createRandom().address,
  ];
  const IerahkwaPulse = await ethers.getContractFactory("IerahkwaPulse");
  const pulse = await IerahkwaPulse.deploy(leadGuardians);
  await pulse.waitForDeployment();
  addresses.IerahkwaPulse = await pulse.getAddress();
  console.log(`      Address: ${addresses.IerahkwaPulse}`);
  console.log(`      Lead Guardians: ${leadGuardians[0]} (deployer) + 4 generated`);
  await verifyContract(addresses.IerahkwaPulse, [leadGuardians]);

  // =========================================================================
  // 10. IerahkwaDestruct (references Manifesto)
  // =========================================================================
  console.log("\n10/13 Deploying IerahkwaDestruct...");
  const linkedContracts = [
    addresses.IerahkwaTreasury,
    addresses.IerahkwaReputation,
    addresses.IerahkwaVeritas,
  ];
  const IerahkwaDestruct = await ethers.getContractFactory("IerahkwaDestruct");
  const destruct = await IerahkwaDestruct.deploy(addresses.IerahkwaManifesto, linkedContracts);
  await destruct.waitForDeployment();
  addresses.IerahkwaDestruct = await destruct.getAddress();
  console.log(`      Address: ${addresses.IerahkwaDestruct}`);
  await verifyContract(addresses.IerahkwaDestruct, [addresses.IerahkwaManifesto, linkedContracts]);

  // =========================================================================
  // 11. SovereignGovernance (requires IVotes token + TimelockController)
  // =========================================================================
  console.log("\n11/13 Deploying TimelockController + SovereignGovernance...");

  // Deploy TimelockController first
  const TimelockController = await ethers.getContractFactory("TimelockController");
  const minDelay = 2 * 24 * 60 * 60; // 2 days
  const timelock = await TimelockController.deploy(
    minDelay,
    [deployer.address],  // proposers
    [deployer.address],  // executors
    deployer.address     // admin
  );
  await timelock.waitForDeployment();
  addresses.TimelockController = await timelock.getAddress();
  console.log(`      TimelockController: ${addresses.TimelockController}`);

  const SovereignGovernance = await ethers.getContractFactory("SovereignGovernance");
  const governance = await SovereignGovernance.deploy(
    addresses.IerahkwaToken,
    addresses.TimelockController
  );
  await governance.waitForDeployment();
  addresses.SovereignGovernance = await governance.getAddress();
  console.log(`      SovereignGovernance: ${addresses.SovereignGovernance}`);
  await verifyContract(addresses.SovereignGovernance, [
    addresses.IerahkwaToken,
    addresses.TimelockController,
  ]);

  // =========================================================================
  // 12. SovereignStaking
  // =========================================================================
  console.log("\n12/13 Deploying SovereignStaking...");
  const SovereignStaking = await ethers.getContractFactory("SovereignStaking");
  const staking = await SovereignStaking.deploy(
    addresses.IerahkwaToken,
    addresses.IerahkwaToken
  );
  await staking.waitForDeployment();
  addresses.SovereignStaking = await staking.getAddress();
  console.log(`      Address: ${addresses.SovereignStaking}`);
  await verifyContract(addresses.SovereignStaking, [
    addresses.IerahkwaToken,
    addresses.IerahkwaToken,
  ]);

  // =========================================================================
  // 13. SovereignVault
  // =========================================================================
  console.log("\n13/13 Deploying SovereignVault...");
  const SovereignVault = await ethers.getContractFactory("SovereignVault");
  const vault = await SovereignVault.deploy(
    addresses.IerahkwaToken,
    "Ierahkwa Sovereign Vault",
    "iVault",
    deployer.address
  );
  await vault.waitForDeployment();
  addresses.SovereignVault = await vault.getAddress();
  console.log(`      Address: ${addresses.SovereignVault}`);
  await verifyContract(addresses.SovereignVault, [
    addresses.IerahkwaToken,
    "Ierahkwa Sovereign Vault",
    "iVault",
    deployer.address,
  ]);

  // =========================================================================
  // SUMMARY
  // =========================================================================
  console.log("\n=========================================================================");
  console.log("   DEPLOYMENT COMPLETE");
  console.log("=========================================================================\n");

  console.log("Contract Addresses:");
  console.log("-------------------------------------------------------------------------");
  const longestKey = Math.max(...Object.keys(addresses).map((k) => k.length));
  for (const [name, addr] of Object.entries(addresses)) {
    console.log(`  ${name.padEnd(longestKey + 2)} ${addr}`);
  }
  console.log("-------------------------------------------------------------------------");

  // =========================================================================
  // SAVE DEPLOYMENTS
  // =========================================================================
  const deploymentInfo = {
    network: network.name,
    chainId: network.config.chainId,
    deployer: deployer.address,
    timestamp: new Date().toISOString(),
    contracts: addresses,
  };

  const deploymentsDir = path.resolve(__dirname, "../deployments");
  if (!fs.existsSync(deploymentsDir)) {
    fs.mkdirSync(deploymentsDir, { recursive: true });
  }

  // Save network-specific file
  const networkFile = path.join(deploymentsDir, `${network.name}.json`);
  fs.writeFileSync(networkFile, JSON.stringify(deploymentInfo, null, 2));
  console.log(`\nSaved to ${networkFile}`);

  // Save/update consolidated deployments.json
  const consolidatedFile = path.join(deploymentsDir, "deployments.json");
  let consolidated = {};
  if (fs.existsSync(consolidatedFile)) {
    try {
      consolidated = JSON.parse(fs.readFileSync(consolidatedFile, "utf-8"));
    } catch {
      consolidated = {};
    }
  }
  consolidated[network.name] = deploymentInfo;
  fs.writeFileSync(consolidatedFile, JSON.stringify(consolidated, null, 2));
  console.log(`Updated ${consolidatedFile}`);

  console.log("\nDone.\n");
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error("\nDeployment failed:");
    console.error(error);
    process.exit(1);
  });
