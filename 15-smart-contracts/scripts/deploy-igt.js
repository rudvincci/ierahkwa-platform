// ============================================================================
// DEPLOY IGT TOKENS — Ierahkwa Ne Kanienke
// IGTFactory + 109 Indigenous Governance Tokens in batches of 20
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

function loadTokenData() {
  const filePath = path.join(__dirname, "..", "data", "igt-tokens.json");
  if (!fs.existsSync(filePath)) {
    throw new Error(`Token data not found: ${filePath}`);
  }
  return JSON.parse(fs.readFileSync(filePath, "utf8"));
}

/**
 * Split an array into chunks of at most `size` elements.
 */
function chunk(arr, size) {
  const result = [];
  for (let i = 0; i < arr.length; i += size) {
    result.push(arr.slice(i, i + size));
  }
  return result;
}

// ---------------------------------------------------------------------------
//  Main
// ---------------------------------------------------------------------------

async function main() {
  log("═══════════════════════════════════════════════════════════════");
  log("  DEPLOYING IGT TOKENS — Ierahkwa Sovereign Nation");
  log("═══════════════════════════════════════════════════════════════");

  const [deployer] = await hre.ethers.getSigners();
  const treasury = process.env.TREASURY_ADDRESS || deployer.address;
  const admin = process.env.ADMIN_ADDRESS || deployer.address;

  log(`Deployer:  ${deployer.address}`);
  log(`Treasury:  ${treasury}`);
  log(`Network:   ${hre.network.name}`);
  log("");

  const deployment = loadDeployment();
  const tokens = loadTokenData();

  log(`Loaded ${tokens.length} IGT tokens from data/igt-tokens.json`);

  // -----------------------------------------------------------------------
  //  1. Deploy IGTFactory
  // -----------------------------------------------------------------------
  log("");
  log("Phase 1 — Deploying IGTFactory...");

  let factoryAddress = deployment.contracts.IGTFactory?.address;

  if (factoryAddress) {
    log(`  IGTFactory already deployed at: ${factoryAddress}`);
    log("  Reusing existing factory.");
  } else {
    try {
      const IGTFactory = await hre.ethers.getContractFactory("IGTFactory");
      const factory = await IGTFactory.deploy(admin);
      await factory.waitForDeployment();
      factoryAddress = await factory.getAddress();

      logContract("IGTFactory", factoryAddress);

      deployment.contracts.IGTFactory = {
        address: factoryAddress,
        deployedAt: new Date().toISOString(),
        constructorArgs: [admin],
      };
      saveDeployment(deployment);
    } catch (err) {
      log(`  ❌ IGTFactory deployment FAILED: ${err.message}`);
      throw err;
    }
  }

  // -----------------------------------------------------------------------
  //  2. Deploy tokens in batches of 20
  // -----------------------------------------------------------------------
  log("");
  log("Phase 2 — Deploying IGT tokens in batches...");

  const BATCH_SIZE = 20;
  const batches = chunk(tokens, BATCH_SIZE);
  const factory = await hre.ethers.getContractAt("IGTFactory", factoryAddress);

  let totalDeployed = 0;
  const deployedTokens = [];

  for (let batchIdx = 0; batchIdx < batches.length; batchIdx++) {
    const batch = batches[batchIdx];
    const batchNum = batchIdx + 1;
    log("");
    log(`  Batch ${batchNum}/${batches.length} — ${batch.length} tokens`);

    const symbols = [];
    const names = [];
    const supplies = [];

    for (const token of batch) {
      symbols.push(token.symbol);
      names.push(token.name);
      // Convert raw supply string to 18-decimal wei
      supplies.push(hre.ethers.parseUnits(token.supply, 18));
    }

    try {
      log(`    Submitting deployBatch tx...`);
      const tx = await factory.deployBatch(symbols, names, supplies, treasury);
      log(`    Tx hash: ${tx.hash}`);

      const receipt = await tx.wait();
      log(`    Confirmed in block ${receipt.blockNumber} | Gas used: ${receipt.gasUsed.toString()}`);

      // Parse TokenDeployed events
      for (const eventLog of receipt.logs) {
        try {
          const parsed = factory.interface.parseLog({
            topics: eventLog.topics,
            data: eventLog.data,
          });
          if (parsed && parsed.name === "TokenDeployed") {
            const tokenAddr = parsed.args.tokenAddress;
            const tokenSymbol = parsed.args.symbol || parsed.args[0];
            const tokenName = parsed.args.name || parsed.args[1];
            deployedTokens.push({
              symbol: tokenSymbol,
              name: tokenName,
              address: tokenAddr,
            });
            totalDeployed++;
          }
        } catch {
          // Not a TokenDeployed event — skip
        }
      }

      log(`    ✅ Batch ${batchNum} complete — ${totalDeployed} tokens deployed so far`);
    } catch (err) {
      log(`    ❌ Batch ${batchNum} FAILED: ${err.message}`);

      // Try deploying remaining tokens individually
      log(`    Attempting individual deployment for failed batch...`);
      for (let i = 0; i < batch.length; i++) {
        const token = batch[i];
        try {
          // Check if already deployed
          const existing = await factory.getToken(token.symbol);
          if (existing !== hre.ethers.ZeroAddress) {
            log(`      ${token.symbol}: already deployed at ${existing}`);
            deployedTokens.push({
              symbol: token.symbol,
              name: token.name,
              address: existing,
            });
            totalDeployed++;
            continue;
          }

          const singleTx = await factory.deployBatch(
            [token.symbol],
            [token.name],
            [hre.ethers.parseUnits(token.supply, 18)],
            treasury
          );
          await singleTx.wait();
          const addr = await factory.getToken(token.symbol);
          log(`      ✅ ${token.symbol}: ${addr}`);
          deployedTokens.push({
            symbol: token.symbol,
            name: token.name,
            address: addr,
          });
          totalDeployed++;
        } catch (singleErr) {
          log(`      ❌ ${token.symbol}: ${singleErr.message}`);
        }
      }
    }
  }

  // -----------------------------------------------------------------------
  //  Save results
  // -----------------------------------------------------------------------
  deployment.contracts.IGTFactory = {
    ...deployment.contracts.IGTFactory,
    totalTokens: totalDeployed,
    tokens: deployedTokens,
  };

  saveDeployment(deployment);

  // -----------------------------------------------------------------------
  //  Summary
  // -----------------------------------------------------------------------
  log("");
  log("═══════════════════════════════════════════════════════════════");
  log("  IGT DEPLOYMENT SUMMARY");
  log("═══════════════════════════════════════════════════════════════");
  log(`  Factory:        ${factoryAddress}`);
  log(`  Total deployed: ${totalDeployed} / ${tokens.length}`);
  log(`  Treasury:       ${treasury}`);
  if (totalDeployed < tokens.length) {
    log(`  ⚠️  ${tokens.length - totalDeployed} tokens failed to deploy`);
  }
  log("═══════════════════════════════════════════════════════════════");

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
