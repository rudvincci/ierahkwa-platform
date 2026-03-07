// ============================================================================
// DEPLOY GOVERNANCE CONTRACTS — Ierahkwa Ne Kanienke
// DAOGovernor + SovereignTreasury + ConsejoMultisig
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
  log("  DEPLOYING GOVERNANCE CONTRACTS — Ierahkwa Sovereign Nation");
  log("═══════════════════════════════════════════════════════════════");

  const [deployer] = await hre.ethers.getSigners();
  const admin = process.env.ADMIN_ADDRESS || deployer.address;
  const treasury = process.env.TREASURY_ADDRESS || deployer.address;

  log(`Deployer: ${deployer.address}`);
  log(`Admin:    ${admin}`);
  log(`Network:  ${hre.network.name}`);
  log("");

  const deployment = loadDeployment();

  // Resolve WampumToken address for governance voting
  const wmpAddress = deployment.contracts.WampumToken?.address;
  if (!wmpAddress) {
    log("⚠️  WampumToken not found in deployment. Governance voting token will be set later.");
  }

  // -----------------------------------------------------------------------
  //  Consejo Multisig signers — default to deployer if no env vars
  // -----------------------------------------------------------------------
  const signers = [];
  for (let i = 1; i <= 7; i++) {
    const envKey = `CONSEJO_SIGNER_${i}`;
    signers.push(process.env[envKey] || deployer.address);
  }
  const multisigThreshold = parseInt(process.env.CONSEJO_THRESHOLD || "4", 10);

  // -----------------------------------------------------------------------
  //  1. Deploy SovereignTreasury
  // -----------------------------------------------------------------------
  log("1/3 — Deploying SovereignTreasury...");
  try {
    const SovereignTreasury = await hre.ethers.getContractFactory("SovereignTreasury");
    const treasuryContract = await SovereignTreasury.deploy(admin);
    await treasuryContract.waitForDeployment();
    const treasuryAddr = await treasuryContract.getAddress();

    logContract("SovereignTreasury", treasuryAddr);

    deployment.contracts.SovereignTreasury = {
      address: treasuryAddr,
      deployedAt: new Date().toISOString(),
      constructorArgs: [admin],
    };
  } catch (err) {
    log(`  ❌ SovereignTreasury deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.SovereignTreasury = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  2. Deploy ConsejoMultisig
  // -----------------------------------------------------------------------
  log("2/3 — Deploying ConsejoMultisig...");
  log(`  Signers: ${signers.length} | Threshold: ${multisigThreshold}`);
  try {
    const ConsejoMultisig = await hre.ethers.getContractFactory("ConsejoMultisig");
    const consejo = await ConsejoMultisig.deploy(signers, multisigThreshold);
    await consejo.waitForDeployment();
    const consejoAddr = await consejo.getAddress();

    logContract("ConsejoMultisig", consejoAddr);

    deployment.contracts.ConsejoMultisig = {
      address: consejoAddr,
      deployedAt: new Date().toISOString(),
      constructorArgs: [signers, multisigThreshold],
      signers,
      threshold: multisigThreshold,
    };
  } catch (err) {
    log(`  ❌ ConsejoMultisig deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.ConsejoMultisig = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  3. Deploy DAOGovernor
  // -----------------------------------------------------------------------
  log("3/3 — Deploying DAOGovernor...");
  try {
    const votingToken = wmpAddress || hre.ethers.ZeroAddress;
    const treasuryAddr = deployment.contracts.SovereignTreasury?.address || hre.ethers.ZeroAddress;

    const DAOGovernor = await hre.ethers.getContractFactory("DAOGovernor");
    const governor = await DAOGovernor.deploy(votingToken, treasuryAddr, admin);
    await governor.waitForDeployment();
    const governorAddr = await governor.getAddress();

    logContract("DAOGovernor", governorAddr);

    deployment.contracts.DAOGovernor = {
      address: governorAddr,
      deployedAt: new Date().toISOString(),
      constructorArgs: [votingToken, treasuryAddr, admin],
      votingToken,
    };
  } catch (err) {
    log(`  ❌ DAOGovernor deployment FAILED: ${err.message}`);
    log("  ⚠️  Skipping — contract may not be compiled yet.");
    deployment.contracts.DAOGovernor = { address: null, error: err.message };
  }

  // -----------------------------------------------------------------------
  //  Summary
  // -----------------------------------------------------------------------
  log("");
  log("═══════════════════════════════════════════════════════════════");
  log("  GOVERNANCE DEPLOYMENT SUMMARY");
  log("═══════════════════════════════════════════════════════════════");
  const govContracts = ["SovereignTreasury", "ConsejoMultisig", "DAOGovernor"];
  for (const name of govContracts) {
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
