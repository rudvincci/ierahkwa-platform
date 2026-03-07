// ============================================================================
// DEPLOY CORE CONTRACTS — Ierahkwa Ne Kanienke
// WampumToken (WMP) + BDETToken (BDET) + SovereignID
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
  return {
    network: "mameynode",
    chainId: 777777,
    deployedAt: new Date().toISOString(),
    deployer: "",
    contracts: {},
  };
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
  log("  DEPLOYING CORE CONTRACTS — Ierahkwa Sovereign Nation");
  log("═══════════════════════════════════════════════════════════════");

  const [deployer] = await hre.ethers.getSigners();
  const treasury = process.env.TREASURY_ADDRESS || deployer.address;
  const admin = process.env.ADMIN_ADDRESS || deployer.address;

  log(`Deployer:  ${deployer.address}`);
  log(`Treasury:  ${treasury}`);
  log(`Admin:     ${admin}`);
  log(`Network:   ${hre.network.name} (chainId: ${(await hre.ethers.provider.getNetwork()).chainId})`);
  log("");

  const deployment = loadDeployment();
  deployment.deployer = deployer.address;

  // -----------------------------------------------------------------------
  //  1. Deploy WampumToken (WMP)
  // -----------------------------------------------------------------------
  log("1/3 — Deploying WampumToken (WMP)...");
  try {
    const WampumToken = await hre.ethers.getContractFactory("WampumToken");
    const wmp = await WampumToken.deploy(treasury, admin);
    await wmp.waitForDeployment();
    const wmpAddress = await wmp.getAddress();

    logContract("WampumToken", wmpAddress);

    // Verify basic state
    const wmpName = await wmp.name();
    const wmpSymbol = await wmp.symbol();
    const wmpSupply = await wmp.totalSupply();
    log(`  Name: ${wmpName} | Symbol: ${wmpSymbol} | Supply: ${hre.ethers.formatEther(wmpSupply)} WMP`);

    deployment.contracts.WampumToken = {
      address: wmpAddress,
      name: wmpName,
      symbol: wmpSymbol,
      deployedAt: new Date().toISOString(),
      constructorArgs: [treasury, admin],
    };
  } catch (err) {
    log(`  ❌ WampumToken deployment FAILED: ${err.message}`);
    throw err;
  }

  // -----------------------------------------------------------------------
  //  2. Deploy BDETToken (Central Bank Digital Currency)
  // -----------------------------------------------------------------------
  log("2/3 — Deploying BDETToken (BDET)...");
  try {
    const BDETToken = await hre.ethers.getContractFactory("BDETToken");
    const bdet = await BDETToken.deploy(treasury, admin);
    await bdet.waitForDeployment();
    const bdetAddress = await bdet.getAddress();

    logContract("BDETToken", bdetAddress);

    const bdetName = await bdet.name();
    const bdetSymbol = await bdet.symbol();
    const bdetSupply = await bdet.totalSupply();
    log(`  Name: ${bdetName} | Symbol: ${bdetSymbol} | Supply: ${hre.ethers.formatEther(bdetSupply)} BDET`);

    deployment.contracts.BDETToken = {
      address: bdetAddress,
      name: bdetName,
      symbol: bdetSymbol,
      deployedAt: new Date().toISOString(),
      constructorArgs: [treasury, admin],
    };
  } catch (err) {
    log(`  ❌ BDETToken deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.BDETToken = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  3. Deploy SovereignID (Decentralized Identity)
  // -----------------------------------------------------------------------
  log("3/3 — Deploying SovereignID...");
  try {
    const SovereignID = await hre.ethers.getContractFactory("SovereignID");
    const sid = await SovereignID.deploy(admin);
    await sid.waitForDeployment();
    const sidAddress = await sid.getAddress();

    logContract("SovereignID", sidAddress);

    deployment.contracts.SovereignID = {
      address: sidAddress,
      deployedAt: new Date().toISOString(),
      constructorArgs: [admin],
    };
  } catch (err) {
    log(`  ❌ SovereignID deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.SovereignID = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  Summary
  // -----------------------------------------------------------------------
  log("");
  log("═══════════════════════════════════════════════════════════════");
  log("  CORE DEPLOYMENT SUMMARY");
  log("═══════════════════════════════════════════════════════════════");
  for (const [name, info] of Object.entries(deployment.contracts)) {
    if (info.address) {
      log(`  ${name}: ${info.address}`);
    } else {
      log(`  ${name}: SKIPPED (${info.error || "not available"})`);
    }
  }
  log("═══════════════════════════════════════════════════════════════");

  saveDeployment(deployment);

  return deployment;
}

// Allow both direct execution and import
if (require.main === module) {
  main()
    .then(() => process.exit(0))
    .catch((error) => {
      console.error(error);
      process.exit(1);
    });
}

module.exports = { main };
