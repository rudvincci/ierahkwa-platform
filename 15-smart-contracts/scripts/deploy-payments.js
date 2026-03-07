// ============================================================================
// DEPLOY PAYMENT CONTRACTS — Ierahkwa Ne Kanienke
// BDETPaymentEngine + CreatorMonetization
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
  log("  DEPLOYING PAYMENT CONTRACTS — Ierahkwa Sovereign Nation");
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

  // -----------------------------------------------------------------------
  //  1. Deploy BDETPaymentEngine
  // -----------------------------------------------------------------------
  log("1/2 — Deploying BDETPaymentEngine...");
  try {
    const paymentToken = bdetAddress || wmpAddress || hre.ethers.ZeroAddress;

    const BDETPaymentEngine = await hre.ethers.getContractFactory("BDETPaymentEngine");
    const engine = await BDETPaymentEngine.deploy(paymentToken, treasury, admin);
    await engine.waitForDeployment();
    const engineAddr = await engine.getAddress();

    logContract("BDETPaymentEngine", engineAddr);

    deployment.contracts.BDETPaymentEngine = {
      address: engineAddr,
      deployedAt: new Date().toISOString(),
      constructorArgs: [paymentToken, treasury, admin],
      paymentToken,
    };
  } catch (err) {
    log(`  ❌ BDETPaymentEngine deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.BDETPaymentEngine = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  2. Deploy CreatorMonetization
  // -----------------------------------------------------------------------
  log("2/2 — Deploying CreatorMonetization...");
  try {
    const paymentToken = wmpAddress || hre.ethers.ZeroAddress;

    const CreatorMonetization = await hre.ethers.getContractFactory("CreatorMonetization");
    const creator = await CreatorMonetization.deploy(paymentToken, treasury, admin);
    await creator.waitForDeployment();
    const creatorAddr = await creator.getAddress();

    logContract("CreatorMonetization", creatorAddr);

    deployment.contracts.CreatorMonetization = {
      address: creatorAddr,
      deployedAt: new Date().toISOString(),
      constructorArgs: [paymentToken, treasury, admin],
      paymentToken,
    };
  } catch (err) {
    log(`  ❌ CreatorMonetization deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.CreatorMonetization = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  Summary
  // -----------------------------------------------------------------------
  log("");
  log("═══════════════════════════════════════════════════════════════");
  log("  PAYMENT CONTRACTS DEPLOYMENT SUMMARY");
  log("═══════════════════════════════════════════════════════════════");
  const payContracts = ["BDETPaymentEngine", "CreatorMonetization"];
  for (const name of payContracts) {
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
