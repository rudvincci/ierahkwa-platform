// ============================================================================
// DEPLOY CULTURAL CONTRACTS — Ierahkwa Ne Kanienke
// AuthenticityNFT + LandRegistry + CulturalAssets
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
  log("  DEPLOYING CULTURAL CONTRACTS — Ierahkwa Sovereign Nation");
  log("═══════════════════════════════════════════════════════════════");

  const [deployer] = await hre.ethers.getSigners();
  const admin = process.env.ADMIN_ADDRESS || deployer.address;
  const treasury = process.env.TREASURY_ADDRESS || deployer.address;

  log(`Deployer:  ${deployer.address}`);
  log(`Admin:     ${admin}`);
  log(`Network:   ${hre.network.name}`);
  log("");

  const deployment = loadDeployment();

  // -----------------------------------------------------------------------
  //  1. Deploy AuthenticityNFT
  // -----------------------------------------------------------------------
  log("1/3 — Deploying AuthenticityNFT...");
  try {
    const AuthenticityNFT = await hre.ethers.getContractFactory("AuthenticityNFT");
    const nft = await AuthenticityNFT.deploy(
      "Ierahkwa Authenticity Certificate",
      "IAC",
      admin,
      treasury
    );
    await nft.waitForDeployment();
    const nftAddr = await nft.getAddress();

    logContract("AuthenticityNFT", nftAddr);

    deployment.contracts.AuthenticityNFT = {
      address: nftAddr,
      deployedAt: new Date().toISOString(),
      constructorArgs: [
        "Ierahkwa Authenticity Certificate",
        "IAC",
        admin,
        treasury,
      ],
    };
  } catch (err) {
    log(`  ❌ AuthenticityNFT deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.AuthenticityNFT = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  2. Deploy LandRegistry
  // -----------------------------------------------------------------------
  log("2/3 — Deploying LandRegistry...");
  try {
    const LandRegistry = await hre.ethers.getContractFactory("LandRegistry");
    const land = await LandRegistry.deploy(
      "Ierahkwa Sovereign Land Registry",
      "ISLR",
      admin
    );
    await land.waitForDeployment();
    const landAddr = await land.getAddress();

    logContract("LandRegistry", landAddr);

    deployment.contracts.LandRegistry = {
      address: landAddr,
      deployedAt: new Date().toISOString(),
      constructorArgs: [
        "Ierahkwa Sovereign Land Registry",
        "ISLR",
        admin,
      ],
    };
  } catch (err) {
    log(`  ❌ LandRegistry deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.LandRegistry = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  3. Deploy CulturalAssets
  // -----------------------------------------------------------------------
  log("3/3 — Deploying CulturalAssets...");
  try {
    const CulturalAssets = await hre.ethers.getContractFactory("CulturalAssets");
    const cultural = await CulturalAssets.deploy(
      "Ierahkwa Cultural Heritage",
      "ICH",
      admin,
      treasury
    );
    await cultural.waitForDeployment();
    const culturalAddr = await cultural.getAddress();

    logContract("CulturalAssets", culturalAddr);

    deployment.contracts.CulturalAssets = {
      address: culturalAddr,
      deployedAt: new Date().toISOString(),
      constructorArgs: [
        "Ierahkwa Cultural Heritage",
        "ICH",
        admin,
        treasury,
      ],
    };
  } catch (err) {
    log(`  ❌ CulturalAssets deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.CulturalAssets = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  Summary
  // -----------------------------------------------------------------------
  log("");
  log("═══════════════════════════════════════════════════════════════");
  log("  CULTURAL CONTRACTS DEPLOYMENT SUMMARY");
  log("═══════════════════════════════════════════════════════════════");
  const culturalContracts = ["AuthenticityNFT", "LandRegistry", "CulturalAssets"];
  for (const name of culturalContracts) {
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
