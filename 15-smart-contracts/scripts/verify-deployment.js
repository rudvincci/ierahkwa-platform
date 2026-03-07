// ============================================================================
// VERIFY DEPLOYMENT — Ierahkwa Ne Kanienke
// Reads deployments/mameynode-777777.json and verifies each contract
// is deployed correctly by calling name(), symbol(), totalSupply(), etc.
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

function loadDeployment() {
  const filePath = path.join(__dirname, "..", "deployments", "mameynode-777777.json");
  if (!fs.existsSync(filePath)) {
    throw new Error(`Deployment file not found: ${filePath}`);
  }
  return JSON.parse(fs.readFileSync(filePath, "utf8"));
}

// ---------------------------------------------------------------------------
//  Verification functions
// ---------------------------------------------------------------------------

async function verifyERC20(name, address) {
  const results = { contract: name, address, checks: [] };

  try {
    // Check code exists at address
    const code = await hre.ethers.provider.getCode(address);
    if (code === "0x" || code === "0x0") {
      results.checks.push({ check: "bytecode", status: "FAIL", detail: "No contract code at address" });
      results.status = "FAIL";
      return results;
    }
    results.checks.push({ check: "bytecode", status: "PASS", detail: `${code.length / 2 - 1} bytes` });

    // Try ERC20 interface
    const token = await hre.ethers.getContractAt("IERC20Metadata", address);

    // name()
    try {
      const tokenName = await token.name();
      results.checks.push({ check: "name()", status: "PASS", detail: tokenName });
    } catch {
      results.checks.push({ check: "name()", status: "WARN", detail: "Not available" });
    }

    // symbol()
    try {
      const tokenSymbol = await token.symbol();
      results.checks.push({ check: "symbol()", status: "PASS", detail: tokenSymbol });
    } catch {
      results.checks.push({ check: "symbol()", status: "WARN", detail: "Not available" });
    }

    // decimals()
    try {
      const decimals = await token.decimals();
      results.checks.push({ check: "decimals()", status: "PASS", detail: decimals.toString() });
    } catch {
      results.checks.push({ check: "decimals()", status: "WARN", detail: "Not available" });
    }

    // totalSupply()
    try {
      const supply = await token.totalSupply();
      results.checks.push({
        check: "totalSupply()",
        status: "PASS",
        detail: `${hre.ethers.formatEther(supply)} tokens`,
      });
    } catch {
      results.checks.push({ check: "totalSupply()", status: "WARN", detail: "Not available" });
    }

    results.status = "PASS";
  } catch (err) {
    results.checks.push({ check: "general", status: "FAIL", detail: err.message });
    results.status = "FAIL";
  }

  return results;
}

async function verifyWampumToken(address) {
  const results = await verifyERC20("WampumToken", address);

  try {
    const wmp = await hre.ethers.getContractAt("WampumToken", address);

    // treasury()
    try {
      const treasury = await wmp.treasury();
      results.checks.push({ check: "treasury()", status: "PASS", detail: treasury });
    } catch {
      results.checks.push({ check: "treasury()", status: "WARN", detail: "Not available" });
    }

    // transferFee()
    try {
      const fee = await wmp.transferFee();
      results.checks.push({ check: "transferFee()", status: "PASS", detail: `${fee} bps` });
    } catch {
      results.checks.push({ check: "transferFee()", status: "WARN", detail: "Not available" });
    }

    // MAX_SUPPLY
    try {
      const maxSupply = await wmp.MAX_SUPPLY();
      results.checks.push({
        check: "MAX_SUPPLY",
        status: "PASS",
        detail: `${hre.ethers.formatEther(maxSupply)} WMP`,
      });
    } catch {
      results.checks.push({ check: "MAX_SUPPLY", status: "WARN", detail: "Not available" });
    }

    // circulatingSupply()
    try {
      const circulating = await wmp.circulatingSupply();
      results.checks.push({
        check: "circulatingSupply()",
        status: "PASS",
        detail: `${hre.ethers.formatEther(circulating)} WMP`,
      });
    } catch {
      results.checks.push({ check: "circulatingSupply()", status: "WARN", detail: "Not available" });
    }
  } catch (err) {
    results.checks.push({ check: "WampumToken-specific", status: "WARN", detail: err.message });
  }

  return results;
}

async function verifyFactory(name, address) {
  const results = { contract: name, address, checks: [] };

  try {
    const code = await hre.ethers.provider.getCode(address);
    if (code === "0x" || code === "0x0") {
      results.checks.push({ check: "bytecode", status: "FAIL", detail: "No contract code" });
      results.status = "FAIL";
      return results;
    }
    results.checks.push({ check: "bytecode", status: "PASS", detail: `${code.length / 2 - 1} bytes` });

    // Try getTokenCount()
    const factoryName = name === "IGTFactory" ? "IGTFactory" : "SNTFactory";
    try {
      const factory = await hre.ethers.getContractAt(factoryName, address);
      const count = await factory.getTokenCount();
      results.checks.push({
        check: "getTokenCount()",
        status: "PASS",
        detail: `${count} tokens deployed`,
      });
    } catch {
      results.checks.push({ check: "getTokenCount()", status: "WARN", detail: "Not available" });
    }

    results.status = "PASS";
  } catch (err) {
    results.checks.push({ check: "general", status: "FAIL", detail: err.message });
    results.status = "FAIL";
  }

  return results;
}

async function verifyGenericContract(name, address) {
  const results = { contract: name, address, checks: [] };

  try {
    const code = await hre.ethers.provider.getCode(address);
    if (code === "0x" || code === "0x0") {
      results.checks.push({ check: "bytecode", status: "FAIL", detail: "No contract code" });
      results.status = "FAIL";
      return results;
    }
    results.checks.push({ check: "bytecode", status: "PASS", detail: `${code.length / 2 - 1} bytes` });
    results.status = "PASS";
  } catch (err) {
    results.checks.push({ check: "general", status: "FAIL", detail: err.message });
    results.status = "FAIL";
  }

  return results;
}

async function verifySovereignBridge(address) {
  const results = { contract: "SovereignBridge", address, checks: [] };

  try {
    const code = await hre.ethers.provider.getCode(address);
    if (code === "0x" || code === "0x0") {
      results.checks.push({ check: "bytecode", status: "FAIL", detail: "No contract code" });
      results.status = "FAIL";
      return results;
    }
    results.checks.push({ check: "bytecode", status: "PASS", detail: `${code.length / 2 - 1} bytes` });

    const bridge = await hre.ethers.getContractAt("SovereignBridge", address);

    // VERSION
    try {
      const version = await bridge.VERSION();
      results.checks.push({ check: "VERSION", status: "PASS", detail: version });
    } catch {
      results.checks.push({ check: "VERSION", status: "WARN", detail: "Not available" });
    }

    // relayerThreshold
    try {
      const threshold = await bridge.relayerThreshold();
      results.checks.push({ check: "relayerThreshold()", status: "PASS", detail: `${threshold}` });
    } catch {
      results.checks.push({ check: "relayerThreshold()", status: "WARN", detail: "Not available" });
    }

    // Check supported chains
    try {
      const ethSupported = await bridge.supportedChains(1);
      const polySupported = await bridge.supportedChains(137);
      results.checks.push({
        check: "supportedChains",
        status: "PASS",
        detail: `ETH: ${ethSupported}, Polygon: ${polySupported}`,
      });
    } catch {
      results.checks.push({ check: "supportedChains", status: "WARN", detail: "Not available" });
    }

    // feeRecipient
    try {
      const feeRecipient = await bridge.feeRecipient();
      results.checks.push({ check: "feeRecipient()", status: "PASS", detail: feeRecipient });
    } catch {
      results.checks.push({ check: "feeRecipient()", status: "WARN", detail: "Not available" });
    }

    results.status = "PASS";
  } catch (err) {
    results.checks.push({ check: "general", status: "FAIL", detail: err.message });
    results.status = "FAIL";
  }

  return results;
}

// ---------------------------------------------------------------------------
//  Verify sample IGT/SNT tokens
// ---------------------------------------------------------------------------

async function verifyTokenSamples(factoryName, tokenList, maxSamples) {
  const sampleResults = [];

  if (!tokenList || tokenList.length === 0) {
    return sampleResults;
  }

  // Sample: first, last, and a few random ones
  const indices = [0, tokenList.length - 1];
  const step = Math.max(1, Math.floor(tokenList.length / maxSamples));
  for (let i = step; i < tokenList.length - 1; i += step) {
    if (indices.length < maxSamples) {
      indices.push(i);
    }
  }
  // Deduplicate
  const uniqueIndices = [...new Set(indices)].sort((a, b) => a - b);

  for (const idx of uniqueIndices) {
    const token = tokenList[idx];
    if (!token.address) continue;

    try {
      const result = await verifyERC20(`${factoryName}[${idx}] ${token.symbol}`, token.address);
      sampleResults.push(result);
    } catch (err) {
      sampleResults.push({
        contract: `${factoryName}[${idx}] ${token.symbol}`,
        address: token.address,
        status: "FAIL",
        checks: [{ check: "verify", status: "FAIL", detail: err.message }],
      });
    }
  }

  return sampleResults;
}

// ---------------------------------------------------------------------------
//  Main
// ---------------------------------------------------------------------------

async function main() {
  log("╔═══════════════════════════════════════════════════════════════╗");
  log("║         DEPLOYMENT VERIFICATION — Ierahkwa Ne Kanienke      ║");
  log("╚═══════════════════════════════════════════════════════════════╝");
  log("");

  const deployment = loadDeployment();
  const network = await hre.ethers.provider.getNetwork();

  log(`Network:    ${hre.network.name} (chainId: ${network.chainId})`);
  log(`Deployment: ${deployment.deployedAt || "unknown"}`);
  log(`Deployer:   ${deployment.deployer || "unknown"}`);
  log(`Contracts:  ${Object.keys(deployment.contracts).length}`);
  log("");

  const allResults = [];
  let passCount = 0;
  let failCount = 0;
  let warnCount = 0;

  // -----------------------------------------------------------------------
  //  Verify each contract
  // -----------------------------------------------------------------------

  for (const [name, info] of Object.entries(deployment.contracts)) {
    if (!info.address) {
      log(`⏭️  ${name}: SKIPPED (no address — was not deployed)`);
      continue;
    }

    log(`Verifying ${name} at ${info.address}...`);

    let result;

    // Choose appropriate verification method
    if (name === "WampumToken") {
      result = await verifyWampumToken(info.address);
    } else if (name === "BDETToken") {
      result = await verifyERC20(name, info.address);
    } else if (name === "IGTFactory" || name === "SNTFactory") {
      result = await verifyFactory(name, info.address);
    } else if (name === "SovereignBridge") {
      result = await verifySovereignBridge(info.address);
    } else {
      // Try ERC20 first, fall back to generic
      try {
        const token = await hre.ethers.getContractAt("IERC20Metadata", info.address);
        await token.name(); // Test if it's an ERC20
        result = await verifyERC20(name, info.address);
      } catch {
        result = await verifyGenericContract(name, info.address);
      }
    }

    allResults.push(result);

    // Print checks
    for (const check of result.checks) {
      const icon = check.status === "PASS" ? "  ✅" : check.status === "WARN" ? "  ⚠️ " : "  ❌";
      log(`${icon} ${check.check}: ${check.detail}`);
    }

    if (result.status === "PASS") {
      passCount++;
    } else {
      failCount++;
    }

    log("");
  }

  // -----------------------------------------------------------------------
  //  Verify sample IGT tokens
  // -----------------------------------------------------------------------
  const igtTokens = deployment.contracts.IGTFactory?.tokens;
  if (igtTokens && igtTokens.length > 0) {
    log("Verifying sample IGT tokens...");
    const igtSamples = await verifyTokenSamples("IGT", igtTokens, 5);
    for (const result of igtSamples) {
      allResults.push(result);
      const icon = result.status === "PASS" ? "✅" : "❌";
      log(`  ${icon} ${result.contract}: ${result.checks.map((c) => c.detail).join(" | ")}`);
      if (result.status === "PASS") passCount++;
      else failCount++;
    }
    log("");
  }

  // -----------------------------------------------------------------------
  //  Verify sample SNT tokens
  // -----------------------------------------------------------------------
  const sntTokens = deployment.contracts.SNTFactory?.tokens;
  if (sntTokens && sntTokens.length > 0) {
    log("Verifying sample SNT tokens...");
    const sntSamples = await verifyTokenSamples("SNT", sntTokens, 5);
    for (const result of sntSamples) {
      allResults.push(result);
      const icon = result.status === "PASS" ? "✅" : "❌";
      log(`  ${icon} ${result.contract}: ${result.checks.map((c) => c.detail).join(" | ")}`);
      if (result.status === "PASS") passCount++;
      else failCount++;
    }
    log("");
  }

  // -----------------------------------------------------------------------
  //  Final Summary
  // -----------------------------------------------------------------------
  log("╔═══════════════════════════════════════════════════════════════╗");
  log("║              VERIFICATION SUMMARY                            ║");
  log("╠═══════════════════════════════════════════════════════════════╣");
  log(`║  Total contracts verified: ${allResults.length}`);
  log(`║  Passed:  ${passCount}`);
  log(`║  Failed:  ${failCount}`);
  log("╠═══════════════════════════════════════════════════════════════╣");

  if (failCount === 0 && passCount > 0) {
    log("║  STATUS: ALL VERIFICATIONS PASSED                            ║");
  } else if (failCount > 0) {
    log("║  STATUS: SOME VERIFICATIONS FAILED                           ║");
  } else {
    log("║  STATUS: NO CONTRACTS TO VERIFY                              ║");
  }

  log("╚═══════════════════════════════════════════════════════════════╝");

  // Save verification report
  const reportPath = path.join(__dirname, "..", "deployments", "verification-report.json");
  const report = {
    verifiedAt: new Date().toISOString(),
    network: hre.network.name,
    chainId: Number(network.chainId),
    summary: { total: allResults.length, passed: passCount, failed: failCount },
    results: allResults,
  };
  fs.writeFileSync(reportPath, JSON.stringify(report, null, 2));
  log(`Verification report saved to ${reportPath}`);

  // Exit with error code if any verification failed
  if (failCount > 0) {
    process.exit(1);
  }
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error("Verification error:", error);
    process.exit(1);
  });
