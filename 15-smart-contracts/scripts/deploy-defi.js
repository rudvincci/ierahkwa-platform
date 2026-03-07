// ============================================================================
// DEPLOY DEFI CONTRACTS — Ierahkwa Ne Kanienke
// SovereignStaking + SovereignDEX + SovereignLending
// Creates initial WMP/BDET pair on DEX
// MameyNode Sovereign Blockchain — Chain ID 777777
// ============================================================================

const hre = require("hardhat");
const fs = require("fs");
const path = require("path");

// ---------------------------------------------------------------------------
//  Helpers
// ---------------------------------------------------------------------------

function log(msg) {
  const ts = new Date().toISOString();
  console.log(`[${ts}] ${msg}`);
}

function logContract(name, address) {
  log(`  ✅ ${name} deployed at: ${address}`);
}

function loadDeployment() {
  const filePath = path.join(__dirname, "..", "deployments", "mameynode-777777.json");
  if (fs.existsSync(filePath)) {
    return JSON.parse(fs.readFileSync(filePath, "utf8"));
  }
  throw new Error("No deployment file found. Run deploy-core.js first.");
}

function saveDeployment(deployment) {
  const dirPath = path.join(__dirname, "..", "deployments");
  if (!fs.existsSync(dirPath)) {
    fs.mkdirSync(dirPath, { recursive: true });
  }
  const filePath = path.join(dirPath, "mameynode-777777.json");
  fs.writeFileSync(filePath, JSON.stringify(deployment, null, 2));
  log(`Deployment saved to ${filePath}`);
}

// ---------------------------------------------------------------------------
//  Main
// ---------------------------------------------------------------------------

async function main() {
  log("═══════════════════════════════════════════════════════════════");
  log("  DEPLOYING DEFI CONTRACTS — Ierahkwa Sovereign Nation");
  log("═══════════════════════════════════════════════════════════════");

  const [deployer] = await hre.ethers.getSigners();
  const admin = process.env.ADMIN_ADDRESS || deployer.address;
  const treasury = process.env.TREASURY_ADDRESS || deployer.address;

  log(`Deployer:  ${deployer.address}`);
  log(`Admin:     ${admin}`);
  log(`Network:   ${hre.network.name}`);
  log("");

  const deployment = loadDeployment();

  // Resolve core token addresses
  const wmpAddress = deployment.contracts.WampumToken?.address;
  const bdetAddress = deployment.contracts.BDETToken?.address;

  if (!wmpAddress) {
    log("⚠️  WampumToken address not found — DeFi contracts may need reconfiguration.");
  }

  // -----------------------------------------------------------------------
  //  1. Deploy SovereignStaking
  // -----------------------------------------------------------------------
  log("1/3 — Deploying SovereignStaking...");
  try {
    const stakingToken = wmpAddress || hre.ethers.ZeroAddress;
    const rewardToken = wmpAddress || hre.ethers.ZeroAddress;

    const SovereignStaking = await hre.ethers.getContractFactory("SovereignStaking");
    const staking = await SovereignStaking.deploy(stakingToken, rewardToken, admin);
    await staking.waitForDeployment();
    const stakingAddr = await staking.getAddress();

    logContract("SovereignStaking", stakingAddr);

    deployment.contracts.SovereignStaking = {
      address: stakingAddr,
      deployedAt: new Date().toISOString(),
      constructorArgs: [stakingToken, rewardToken, admin],
      stakingToken,
      rewardToken,
    };

    // Configure WampumToken staking rewards address if available
    if (wmpAddress) {
      try {
        const wmp = await hre.ethers.getContractAt("WampumToken", wmpAddress);
        const tx = await wmp.setStakingRewards(stakingAddr);
        await tx.wait();
        log("  Configured WampumToken staking rewards address");
      } catch (configErr) {
        log(`  ⚠️  Could not configure WampumToken: ${configErr.message}`);
      }
    }
  } catch (err) {
    log(`  ❌ SovereignStaking deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.SovereignStaking = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  2. Deploy SovereignDEX
  // -----------------------------------------------------------------------
  log("2/3 — Deploying SovereignDEX...");
  try {
    const SovereignDEX = await hre.ethers.getContractFactory("SovereignDEX");
    const dex = await SovereignDEX.deploy(admin, treasury);
    await dex.waitForDeployment();
    const dexAddr = await dex.getAddress();

    logContract("SovereignDEX", dexAddr);

    deployment.contracts.SovereignDEX = {
      address: dexAddr,
      deployedAt: new Date().toISOString(),
      constructorArgs: [admin, treasury],
    };

    // Create initial WMP/BDET pair if both tokens are available
    if (wmpAddress && bdetAddress) {
      log("  Creating WMP/BDET trading pair...");
      try {
        const dexContract = await hre.ethers.getContractAt("SovereignDEX", dexAddr);

        // Attempt to create the pair
        const createTx = await dexContract.createPair(wmpAddress, bdetAddress);
        await createTx.wait();
        log("  ✅ WMP/BDET pair created successfully");

        // Add initial liquidity if deployer has tokens
        const wmp = await hre.ethers.getContractAt("WampumToken", wmpAddress);
        const wmpBalance = await wmp.balanceOf(deployer.address);
        const initialLiquidity = hre.ethers.parseEther("1000000"); // 1M tokens

        if (wmpBalance >= initialLiquidity) {
          log("  Adding initial liquidity...");
          try {
            // Approve DEX to spend WMP
            const approveTx1 = await wmp.approve(dexAddr, initialLiquidity);
            await approveTx1.wait();

            // Approve DEX to spend BDET
            const bdet = await hre.ethers.getContractAt("IERC20", bdetAddress);
            const approveTx2 = await bdet.approve(dexAddr, initialLiquidity);
            await approveTx2.wait();

            const addLiqTx = await dexContract.addLiquidity(
              wmpAddress,
              bdetAddress,
              initialLiquidity,
              initialLiquidity,
              0, // min WMP
              0, // min BDET
              deployer.address,
              Math.floor(Date.now() / 1000) + 3600 // 1 hour deadline
            );
            await addLiqTx.wait();
            log("  ✅ Initial liquidity added: 1M WMP / 1M BDET");
          } catch (liqErr) {
            log(`  ⚠️  Could not add initial liquidity: ${liqErr.message}`);
          }
        } else {
          log("  ⚠️  Insufficient WMP balance for initial liquidity");
        }
      } catch (pairErr) {
        log(`  ⚠️  Could not create WMP/BDET pair: ${pairErr.message}`);
      }
    } else {
      log("  ⚠️  WMP and/or BDET not deployed — skipping pair creation");
    }
  } catch (err) {
    log(`  ❌ SovereignDEX deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.SovereignDEX = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  3. Deploy SovereignLending
  // -----------------------------------------------------------------------
  log("3/3 — Deploying SovereignLending...");
  try {
    const SovereignLending = await hre.ethers.getContractFactory("SovereignLending");
    const lending = await SovereignLending.deploy(admin, treasury);
    await lending.waitForDeployment();
    const lendingAddr = await lending.getAddress();

    logContract("SovereignLending", lendingAddr);

    deployment.contracts.SovereignLending = {
      address: lendingAddr,
      deployedAt: new Date().toISOString(),
      constructorArgs: [admin, treasury],
    };
  } catch (err) {
    log(`  ❌ SovereignLending deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.SovereignLending = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  Summary
  // -----------------------------------------------------------------------
  log("");
  log("═══════════════════════════════════════════════════════════════");
  log("  DEFI DEPLOYMENT SUMMARY");
  log("═══════════════════════════════════════════════════════════════");
  const defiContracts = ["SovereignStaking", "SovereignDEX", "SovereignLending"];
  for (const name of defiContracts) {
    const info = deployment.contracts[name];
    if (info?.address) {
      log(`  ${name}: ${info.address}`);
    } else {
      log(`  ${name}: SKIPPED (${info?.error || "not available"})`);
    }
  }
  log("═══════════════════════════════════════════════════════════════");

  saveDeployment(deployment);

  return deployment;
}

if (require.main === module) {
  main()
    .then(() => process.exit(0))
    .catch((error) => {
      console.error(error);
      process.exit(1);
    });
}

module.exports = { main };
