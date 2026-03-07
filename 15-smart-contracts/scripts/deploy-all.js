// ============================================================================
// MASTER DEPLOYMENT SCRIPT — Ierahkwa Ne Kanienke
// Orchestrates all deployment phases in correct order and saves
// consolidated addresses to deployments/mameynode-777777.json
// MameyNode Sovereign Blockchain — Chain ID 777777
// ============================================================================

const hre = require("hardhat");
const fs = require("fs");
const path = require("path");

// Import individual deployment modules
const { main: deployCore } = require("./deploy-core");
const { main: deployGovernance } = require("./deploy-governance");
const { main: deployIgt } = require("./deploy-igt");
const { main: deploySnt } = require("./deploy-snt");
const { main: deployDefi } = require("./deploy-defi");
const { main: deployPayments } = require("./deploy-payments");
const { main: deployCultural } = require("./deploy-cultural");

// ---------------------------------------------------------------------------
//  Helpers
// ---------------------------------------------------------------------------

function log(msg) {
  const ts = new Date().toISOString();
  console.log(`[${ts}] ${msg}`);
}

function loadDeployment() {
  const filePath = path.join(__dirname, "..", "deployments", "mameynode-777777.json");
  if (fs.existsSync(filePath)) {
    return JSON.parse(fs.readFileSync(filePath, "utf8"));
  }
  return null;
}

function saveDeployment(deployment) {
  const dirPath = path.join(__dirname, "..", "deployments");
  if (!fs.existsSync(dirPath)) {
    fs.mkdirSync(dirPath, { recursive: true });
  }
  const filePath = path.join(dirPath, "mameynode-777777.json");
  fs.writeFileSync(filePath, JSON.stringify(deployment, null, 2));
  log(`Final deployment saved to ${filePath}`);
}

function formatDuration(ms) {
  const seconds = Math.floor(ms / 1000);
  const minutes = Math.floor(seconds / 60);
  const remainingSeconds = seconds % 60;
  if (minutes > 0) {
    return `${minutes}m ${remainingSeconds}s`;
  }
  return `${seconds}s`;
}

// ---------------------------------------------------------------------------
//  Deployment phases
// ---------------------------------------------------------------------------

const PHASES = [
  { name: "Core",       fn: deployCore,       label: "WampumToken, BDETToken, SovereignID" },
  { name: "Governance", fn: deployGovernance,  label: "DAOGovernor, Treasury, ConsejoMultisig" },
  { name: "IGT Tokens", fn: deployIgt,         label: "IGTFactory + 109 governance tokens" },
  { name: "SNT Tokens", fn: deploySnt,         label: "SNTFactory + 574 nation tokens" },
  { name: "DeFi",       fn: deployDefi,        label: "Staking, DEX, Lending" },
  { name: "Payments",   fn: deployPayments,    label: "BDETPaymentEngine, CreatorMonetization" },
  { name: "Cultural",   fn: deployCultural,    label: "AuthenticityNFT, LandRegistry, CulturalAssets" },
];

// ---------------------------------------------------------------------------
//  Main
// ---------------------------------------------------------------------------

async function main() {
  const totalStart = Date.now();

  log("╔═══════════════════════════════════════════════════════════════╗");
  log("║                                                               ║");
  log("║     IERAHKWA NE KANIENKE — FULL DEPLOYMENT                   ║");
  log("║     Sovereign Digital Nation — MameyNode Chain 777777         ║");
  log("║     685 tokens | 574 nations | 72M indigenous people         ║");
  log("║                                                               ║");
  log("╚═══════════════════════════════════════════════════════════════╝");
  log("");

  const [deployer] = await hre.ethers.getSigners();
  const network = await hre.ethers.provider.getNetwork();

  log(`Deployer:  ${deployer.address}`);
  log(`Network:   ${hre.network.name} (chainId: ${network.chainId})`);
  log(`Treasury:  ${process.env.TREASURY_ADDRESS || deployer.address}`);
  log(`Phases:    ${PHASES.length}`);
  log("");

  // Determine which phases to skip (via env var SKIP_PHASES="Core,IGT Tokens")
  const skipPhases = (process.env.SKIP_PHASES || "").split(",").map((s) => s.trim()).filter(Boolean);
  if (skipPhases.length > 0) {
    log(`Skipping phases: ${skipPhases.join(", ")}`);
    log("");
  }

  const results = [];
  let deployment = null;

  for (let i = 0; i < PHASES.length; i++) {
    const phase = PHASES[i];
    const phaseNum = i + 1;

    if (skipPhases.includes(phase.name)) {
      log(`━━━ Phase ${phaseNum}/${PHASES.length}: ${phase.name} — SKIPPED ━━━`);
      results.push({ name: phase.name, status: "skipped", duration: 0 });
      continue;
    }

    log(`━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);
    log(`Phase ${phaseNum}/${PHASES.length}: ${phase.name}`);
    log(`  ${phase.label}`);
    log(`━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);

    const phaseStart = Date.now();

    try {
      deployment = await phase.fn();
      const duration = Date.now() - phaseStart;
      log(`✅ Phase ${phaseNum} (${phase.name}) completed in ${formatDuration(duration)}`);
      results.push({ name: phase.name, status: "success", duration });
    } catch (err) {
      const duration = Date.now() - phaseStart;
      log(`❌ Phase ${phaseNum} (${phase.name}) FAILED after ${formatDuration(duration)}`);
      log(`   Error: ${err.message}`);
      results.push({ name: phase.name, status: "failed", duration, error: err.message });

      // Continue to next phase unless it's a critical failure
      if (phase.name === "Core") {
        log("   Core deployment failed — cannot continue.");
        break;
      }
    }

    log("");
  }

  // -----------------------------------------------------------------------
  //  Deploy SovereignBridge (bonus — not in a separate phase)
  // -----------------------------------------------------------------------
  if (!skipPhases.includes("Bridge")) {
    log("━━━ Bonus: Deploying SovereignBridge ━━━");
    try {
      deployment = deployment || loadDeployment();
      if (deployment) {
        const admin = process.env.ADMIN_ADDRESS || deployer.address;
        const treasury = process.env.TREASURY_ADDRESS || deployer.address;

        // Default relayers — in production use real addresses
        const relayers = [];
        for (let i = 1; i <= 5; i++) {
          relayers.push(process.env[`RELAYER_${i}`] || deployer.address);
        }

        const SovereignBridge = await hre.ethers.getContractFactory("SovereignBridge");
        const bridge = await SovereignBridge.deploy(admin, relayers, treasury);
        await bridge.waitForDeployment();
        const bridgeAddr = await bridge.getAddress();

        log(`  ✅ SovereignBridge: ${bridgeAddr}`);

        deployment.contracts.SovereignBridge = {
          address: bridgeAddr,
          deployedAt: new Date().toISOString(),
          constructorArgs: [admin, relayers, treasury],
          relayers,
        };

        saveDeployment(deployment);
      }
    } catch (err) {
      log(`  ❌ SovereignBridge: ${err.message}`);
    }
    log("");
  }

  // -----------------------------------------------------------------------
  //  Final metadata
  // -----------------------------------------------------------------------
  deployment = deployment || loadDeployment();
  if (deployment) {
    deployment.completedAt = new Date().toISOString();
    deployment.totalDuration = formatDuration(Date.now() - totalStart);
    deployment.phases = results.map((r) => ({
      name: r.name,
      status: r.status,
      duration: formatDuration(r.duration),
      ...(r.error ? { error: r.error } : {}),
    }));
    saveDeployment(deployment);
  }

  // -----------------------------------------------------------------------
  //  Final Summary
  // -----------------------------------------------------------------------
  const totalDuration = Date.now() - totalStart;

  log("╔═══════════════════════════════════════════════════════════════╗");
  log("║            FULL DEPLOYMENT SUMMARY                           ║");
  log("╠═══════════════════════════════════════════════════════════════╣");

  for (const result of results) {
    const icon = result.status === "success" ? "✅" : result.status === "skipped" ? "⏭️ " : "❌";
    const dur = result.duration > 0 ? ` (${formatDuration(result.duration)})` : "";
    log(`║  ${icon} ${result.name.padEnd(15)} ${result.status.toUpperCase()}${dur}`);
  }

  log("╠═══════════════════════════════════════════════════════════════╣");

  if (deployment) {
    const contractCount = Object.keys(deployment.contracts).length;
    const deployed = Object.values(deployment.contracts).filter((c) => c.address).length;
    const failed = contractCount - deployed;

    log(`║  Contracts: ${deployed} deployed, ${failed} pending`);
    log(`║  Duration:  ${formatDuration(totalDuration)}`);

    // Count tokens
    const igtCount = deployment.contracts.IGTFactory?.totalTokens || 0;
    const sntCount = deployment.contracts.SNTFactory?.totalTokens || 0;
    log(`║  IGT:       ${igtCount} tokens`);
    log(`║  SNT:       ${sntCount} tokens`);
    log(`║  Total:     ${igtCount + sntCount} tokens`);
  }

  log("╚═══════════════════════════════════════════════════════════════╝");

  // List all deployed addresses
  if (deployment) {
    log("");
    log("Deployed Contract Addresses:");
    log("─────────────────────────────────────────────────────────────────");
    for (const [name, info] of Object.entries(deployment.contracts)) {
      if (info.address) {
        log(`  ${name}: ${info.address}`);
      }
    }
    log("─────────────────────────────────────────────────────────────────");
  }
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error("FATAL:", error);
    process.exit(1);
  });
