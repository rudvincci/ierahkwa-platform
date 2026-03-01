#!/usr/bin/env python3
"""
Ierahkwa Mediator Test Suite
Tests the AI mediator's empathy analysis against polarized and positive
messages, measures response latency, verifies output quality, and generates
a test report.

Environment variables
---------------------
OLLAMA_URL      Ollama API endpoint   (default: http://localhost:11434)
OLLAMA_MODEL    Model to use          (default: mistral)
LOG_DIR         Logging directory     (default: logs/)
"""

import json
import logging
import os
import sys
import time
from datetime import datetime, timezone
from pathlib import Path

try:
    import requests
except ImportError:
    sys.exit("requests is required: pip install requests")

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

OLLAMA_URL = os.environ.get("OLLAMA_URL", "http://localhost:11434")
OLLAMA_MODEL = os.environ.get("OLLAMA_MODEL", "mistral")
LOG_DIR = Path(os.environ.get("LOG_DIR", "logs"))

LOG_DIR.mkdir(parents=True, exist_ok=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler(LOG_DIR / "test_mediator.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("ierahkwa.test_mediator")

# ---------------------------------------------------------------------------
# Analysis prompt (same as mediator.py)
# ---------------------------------------------------------------------------

ANALYSIS_PROMPT = (
    "You are an empathy and conflict-resolution analyst. "
    "Given the following message from a community discussion, respond ONLY with "
    "valid JSON containing exactly two keys:\n"
    '  "toxicity": a float between 0.0 (completely peaceful) and 1.0 (extremely toxic),\n'
    '  "reformulation": a non-violent, empathetic version of the message that preserves '
    "the speaker's intent while removing hostility.\n\n"
    "Message:\n{message}\n\n"
    "Respond ONLY with the JSON object, no markdown, no extra text."
)

# ---------------------------------------------------------------------------
# Test messages
# ---------------------------------------------------------------------------

POLARIZED_MESSAGES = [
    {
        "id": "POL-01",
        "text": "This proposal is idiotic and anyone who supports it is a fool. We should throw it in the trash.",
        "expected_min_toxicity": 0.5,
        "description": "Insults directed at people and their ideas",
    },
    {
        "id": "POL-02",
        "text": "Your community has never contributed anything worthwhile. You should be excluded from all decisions.",
        "expected_min_toxicity": 0.6,
        "description": "Exclusionary language targeting a community",
    },
    {
        "id": "POL-03",
        "text": "If you don't agree with us, you are a traitor to your people and deserve no voice here.",
        "expected_min_toxicity": 0.6,
        "description": "Threats and labeling dissenters as traitors",
    },
    {
        "id": "POL-04",
        "text": "Shut up and stop wasting everyone's time with your incompetent ramblings.",
        "expected_min_toxicity": 0.5,
        "description": "Silencing and personal attacks",
    },
    {
        "id": "POL-05",
        "text": "These people are destroying everything we built. They need to be removed by any means necessary.",
        "expected_min_toxicity": 0.7,
        "description": "Dehumanizing language with implicit violence",
    },
]

POSITIVE_MESSAGES = [
    {
        "id": "POS-01",
        "text": "I appreciate your perspective, even though I see it differently. Can we find common ground?",
        "expected_max_toxicity": 0.2,
        "description": "Respectful disagreement seeking compromise",
    },
    {
        "id": "POS-02",
        "text": "Thank you for your hard work on this proposal. I have some suggestions that might strengthen it.",
        "expected_max_toxicity": 0.15,
        "description": "Constructive feedback with appreciation",
    },
    {
        "id": "POS-03",
        "text": "I want to understand your concerns better. Could you explain what worries you about this plan?",
        "expected_max_toxicity": 0.1,
        "description": "Active listening and empathy",
    },
    {
        "id": "POS-04",
        "text": "Our community is stronger when we work together. Let us combine our different strengths.",
        "expected_max_toxicity": 0.1,
        "description": "Unity and collaboration message",
    },
    {
        "id": "POS-05",
        "text": "I respect the elders' decision, and I would like to offer additional information for their consideration.",
        "expected_max_toxicity": 0.1,
        "description": "Respectful deference with constructive input",
    },
]

# ---------------------------------------------------------------------------
# Ollama interaction
# ---------------------------------------------------------------------------


def analyse_message(message: str) -> dict:
    """Call Ollama to analyse a message. Returns parsed JSON or raises."""
    prompt = ANALYSIS_PROMPT.format(message=message)
    payload = {
        "model": OLLAMA_MODEL,
        "prompt": prompt,
        "stream": False,
        "options": {"temperature": 0.3},
    }
    url = f"{OLLAMA_URL}/api/generate"

    start = time.monotonic()
    resp = requests.post(url, json=payload, timeout=120)
    elapsed = time.monotonic() - start
    resp.raise_for_status()

    data = resp.json()
    raw = data.get("response", "").strip()

    # Strip markdown fences
    if raw.startswith("```"):
        raw = raw.split("\n", 1)[-1].rsplit("```", 1)[0].strip()

    result = json.loads(raw)
    result["_latency_seconds"] = elapsed
    return result

# ---------------------------------------------------------------------------
# Test runner
# ---------------------------------------------------------------------------


def run_test(test_case: dict, test_type: str) -> dict:
    """Run a single test case and return the result."""
    test_id = test_case["id"]
    text = test_case["text"]
    description = test_case["description"]

    result = {
        "id": test_id,
        "type": test_type,
        "description": description,
        "input": text,
        "passed": False,
        "errors": [],
        "toxicity": None,
        "reformulation": None,
        "latency_seconds": None,
    }

    try:
        analysis = analyse_message(text)
        toxicity = float(analysis.get("toxicity", -1))
        reformulation = str(analysis.get("reformulation", ""))
        latency = analysis.get("_latency_seconds", 0)

        result["toxicity"] = toxicity
        result["reformulation"] = reformulation
        result["latency_seconds"] = round(latency, 3)

        # Validate toxicity range
        if not (0.0 <= toxicity <= 1.0):
            result["errors"].append(f"Toxicity {toxicity} out of range [0, 1]")

        # Validate reformulation is non-empty
        if not reformulation.strip():
            result["errors"].append("Reformulation is empty")

        # Validate reformulation is different from original for toxic messages
        if test_type == "polarized" and reformulation.strip().lower() == text.strip().lower():
            result["errors"].append("Reformulation is identical to original")

        # Type-specific checks
        if test_type == "polarized":
            expected_min = test_case.get("expected_min_toxicity", 0.5)
            if toxicity < expected_min:
                result["errors"].append(
                    f"Toxicity {toxicity:.2f} below expected minimum {expected_min:.2f}"
                )
        elif test_type == "positive":
            expected_max = test_case.get("expected_max_toxicity", 0.2)
            if toxicity > expected_max:
                result["errors"].append(
                    f"Toxicity {toxicity:.2f} above expected maximum {expected_max:.2f}"
                )

        if not result["errors"]:
            result["passed"] = True

    except requests.RequestException as exc:
        result["errors"].append(f"HTTP error: {exc}")
    except json.JSONDecodeError as exc:
        result["errors"].append(f"JSON parse error: {exc}")
    except (KeyError, ValueError, TypeError) as exc:
        result["errors"].append(f"Validation error: {exc}")

    status = "PASS" if result["passed"] else "FAIL"
    logger.info(
        "[%s] %s -- toxicity=%.2f latency=%.3fs errors=%s",
        status,
        test_id,
        result["toxicity"] if result["toxicity"] is not None else -1,
        result["latency_seconds"] if result["latency_seconds"] is not None else -1,
        result["errors"] or "none",
    )
    return result

# ---------------------------------------------------------------------------
# Report generation
# ---------------------------------------------------------------------------


def generate_report(results: list[dict]) -> str:
    """Generate a formatted test report."""
    total = len(results)
    passed = sum(1 for r in results if r["passed"])
    failed = total - passed

    latencies = [r["latency_seconds"] for r in results if r["latency_seconds"] is not None]
    avg_latency = sum(latencies) / len(latencies) if latencies else 0
    max_latency = max(latencies) if latencies else 0
    min_latency = min(latencies) if latencies else 0

    polarized_results = [r for r in results if r["type"] == "polarized"]
    positive_results = [r for r in results if r["type"] == "positive"]

    lines = [
        "=" * 70,
        "  IERAHKWA MEDIATOR TEST REPORT",
        "=" * 70,
        "",
        f"Date: {datetime.now(timezone.utc).isoformat()}",
        f"Model: {OLLAMA_MODEL}",
        f"Ollama URL: {OLLAMA_URL}",
        "",
        "--- SUMMARY ---",
        f"Total tests:  {total}",
        f"Passed:       {passed}",
        f"Failed:       {failed}",
        f"Pass rate:    {(passed / total * 100) if total else 0:.1f}%",
        "",
        "--- LATENCY ---",
        f"Average: {avg_latency:.3f}s",
        f"Minimum: {min_latency:.3f}s",
        f"Maximum: {max_latency:.3f}s",
        "",
    ]

    # Polarized section
    lines.append("--- POLARIZED MESSAGES (expected high toxicity) ---")
    lines.append("")
    for r in polarized_results:
        status = "PASS" if r["passed"] else "FAIL"
        lines.append(f"  [{status}] {r['id']}: {r['description']}")
        lines.append(f"         Toxicity: {r['toxicity']}")
        lines.append(f"         Latency: {r['latency_seconds']}s")
        if r["reformulation"]:
            reformulation_preview = r["reformulation"][:120]
            lines.append(f"         Reformulation: {reformulation_preview}")
        if r["errors"]:
            for err in r["errors"]:
                lines.append(f"         ERROR: {err}")
        lines.append("")

    # Positive section
    lines.append("--- POSITIVE MESSAGES (expected low toxicity) ---")
    lines.append("")
    for r in positive_results:
        status = "PASS" if r["passed"] else "FAIL"
        lines.append(f"  [{status}] {r['id']}: {r['description']}")
        lines.append(f"         Toxicity: {r['toxicity']}")
        lines.append(f"         Latency: {r['latency_seconds']}s")
        if r["errors"]:
            for err in r["errors"]:
                lines.append(f"         ERROR: {err}")
        lines.append("")

    # Quality assessment
    lines.append("--- QUALITY ASSESSMENT ---")
    if polarized_results:
        pol_toxicities = [r["toxicity"] for r in polarized_results if r["toxicity"] is not None]
        if pol_toxicities:
            lines.append(f"  Polarized avg toxicity: {sum(pol_toxicities)/len(pol_toxicities):.2f}")
    if positive_results:
        pos_toxicities = [r["toxicity"] for r in positive_results if r["toxicity"] is not None]
        if pos_toxicities:
            lines.append(f"  Positive avg toxicity: {sum(pos_toxicities)/len(pos_toxicities):.2f}")

    # Discrimination check: polarized should be significantly higher than positive
    if pol_toxicities and pos_toxicities:
        pol_avg = sum(pol_toxicities) / len(pol_toxicities)
        pos_avg = sum(pos_toxicities) / len(pos_toxicities)
        gap = pol_avg - pos_avg
        lines.append(f"  Discrimination gap: {gap:.2f}")
        if gap > 0.3:
            lines.append("  Verdict: GOOD -- Model clearly distinguishes toxic from positive.")
        elif gap > 0.15:
            lines.append("  Verdict: ACCEPTABLE -- Model partially distinguishes. Tuning recommended.")
        else:
            lines.append("  Verdict: POOR -- Model fails to distinguish. Model change or fine-tuning required.")

    lines.append("")
    lines.append("=" * 70)

    return "\n".join(lines)

# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------


def main() -> None:
    logger.info("Starting Mediator test suite -- model=%s", OLLAMA_MODEL)

    # Verify Ollama is reachable
    try:
        resp = requests.get(f"{OLLAMA_URL}/api/tags", timeout=10)
        resp.raise_for_status()
        models = [m.get("name", "") for m in resp.json().get("models", [])]
        logger.info("Ollama is reachable. Available models: %s", models)
    except requests.RequestException as exc:
        logger.error("Cannot reach Ollama at %s: %s", OLLAMA_URL, exc)
        sys.exit(1)

    results = []

    # Run polarized tests
    logger.info("Running %d polarized message tests...", len(POLARIZED_MESSAGES))
    for tc in POLARIZED_MESSAGES:
        result = run_test(tc, "polarized")
        results.append(result)

    # Run positive tests
    logger.info("Running %d positive message tests...", len(POSITIVE_MESSAGES))
    for tc in POSITIVE_MESSAGES:
        result = run_test(tc, "positive")
        results.append(result)

    # Generate and save report
    report = generate_report(results)
    report_path = LOG_DIR / "test_mediator_report.txt"
    with open(report_path, "w", encoding="utf-8") as fh:
        fh.write(report)
    logger.info("Report saved to %s", report_path)

    # Save raw results as JSON
    results_path = LOG_DIR / "test_mediator_results.json"
    with open(results_path, "w", encoding="utf-8") as fh:
        json.dump(results, fh, indent=2, ensure_ascii=False)
    logger.info("Raw results saved to %s", results_path)

    # Print report
    print(report)

    # Exit with failure if any test failed
    failed = sum(1 for r in results if not r["passed"])
    if failed:
        logger.warning("%d tests failed.", failed)
        sys.exit(1)
    else:
        logger.info("All tests passed.")
        sys.exit(0)


if __name__ == "__main__":
    main()
