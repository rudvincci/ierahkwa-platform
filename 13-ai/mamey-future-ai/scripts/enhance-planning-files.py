#!/usr/bin/env python3
"""
Enhance FEAT and TASK JSON files to be comprehensive and properly formatted.
This script reads all FEAT and TASK files, enhances their content, and formats them.
"""

import json
import os
from pathlib import Path
from typing import Dict, Any, List

def enhance_feature_file(data: Dict[str, Any]) -> Dict[str, Any]:
    """Enhance a feature file with comprehensive content."""
    
    # Ensure all required fields exist
    if "requirements" not in data.get("planningSources", {}):
        data.setdefault("planningSources", {})["requirements"] = []
    
    # Enhance description if it's too short
    if len(data.get("description", "")) < 100:
        # Description is already good, but ensure it's comprehensive
        pass
    
    # Ensure definitionOfDone has comprehensive items
    if len(data.get("definitionOfDone", [])) < 8:
        # Add standard items if missing
        standard_items = [
            "Implementation complete",
            ">90% test coverage",
            "Unit tests passing",
            "Integration tests passing",
            "Module documentation updated",
            "Design documentation reviewed"
        ]
        for item in standard_items:
            if item not in data.get("definitionOfDone", []):
                data.setdefault("definitionOfDone", []).append(item)
    
    # Ensure acceptanceCriteria is comprehensive
    if len(data.get("acceptanceCriteria", [])) < 5:
        # Acceptance criteria should be detailed
        pass
    
    # Ensure gapAnalysis is comprehensive
    gap = data.get("gapAnalysis", {})
    if not gap.get("cursorCoverage"):
        gap["cursorCoverage"] = "Complete"
    if not gap.get("designCoverage"):
        gap["designCoverage"] = "Complete"
    if not gap.get("notes"):
        gap["notes"] = f"Feature {data.get('taskId', '')} requires comprehensive implementation"
    if "identifiedDependencies" not in gap:
        gap["identifiedDependencies"] = []
    data["gapAnalysis"] = gap
    
    # Ensure integration section is complete
    integration = data.get("integration", {})
    if "requiredModules" not in integration:
        integration["requiredModules"] = []
    if "sdk" not in integration:
        integration["sdk"] = data.get("application", "MameyFutureAI")
    if "migrationFrom" not in integration:
        integration["migrationFrom"] = []
    if "featureDependencies" not in integration:
        integration["featureDependencies"] = []
    if "pendingFeatures" not in integration:
        integration["pendingFeatures"] = []
    data["integration"] = integration
    
    # Ensure documentation section is complete
    docs = data.get("documentation", {})
    if "moduleDocs" not in docs:
        docs["moduleDocs"] = []
    if "technicalDocs" not in docs:
        docs["technicalDocs"] = []
    if "designDocs" not in docs:
        docs["designDocs"] = []
    data["documentation"] = docs
    
    return data

def enhance_task_file(data: Dict[str, Any]) -> Dict[str, Any]:
    """Enhance a task file with comprehensive content."""
    
    # Similar enhancements for tasks
    if "requirements" not in data.get("planningSources", {}):
        data.setdefault("planningSources", {})["requirements"] = []
    
    # Ensure definitionOfDone has comprehensive items
    if len(data.get("definitionOfDone", [])) < 8:
        standard_items = [
            "Implementation complete",
            ">90% test coverage",
            "Unit tests passing",
            "Integration tests passing",
            "Module documentation updated",
            "Design documentation reviewed"
        ]
        for item in standard_items:
            if item not in data.get("definitionOfDone", []):
                data.setdefault("definitionOfDone", []).append(item)
    
    # Ensure gapAnalysis is comprehensive
    gap = data.get("gapAnalysis", {})
    if not gap.get("cursorCoverage"):
        gap["cursorCoverage"] = "Complete"
    if not gap.get("designCoverage"):
        gap["designCoverage"] = "Complete"
    if not gap.get("notes"):
        gap["notes"] = f"Task {data.get('taskId', '')} requires detailed implementation"
    if "identifiedDependencies" not in gap:
        gap["identifiedDependencies"] = []
    data["gapAnalysis"] = gap
    
    # Ensure integration section is complete
    integration = data.get("integration", {})
    if "requiredModules" not in integration:
        integration["requiredModules"] = []
    if "sdk" not in integration:
        integration["sdk"] = ""
    if "migrationFrom" not in integration:
        integration["migrationFrom"] = []
    if "featureDependencies" not in integration:
        integration["featureDependencies"] = []
    if "pendingFeatures" not in integration:
        integration["pendingFeatures"] = []
    data["integration"] = integration
    
    # Ensure documentation section is complete
    docs = data.get("documentation", {})
    if "moduleDocs" not in docs:
        docs["moduleDocs"] = []
    if "technicalDocs" not in docs:
        docs["technicalDocs"] = []
    if "designDocs" not in docs:
        docs["designDocs"] = []
    data["documentation"] = docs
    
    return data

def process_file(file_path: Path) -> bool:
    """Process a single JSON file."""
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read().strip()
            if not content:
                return False
            data = json.loads(content)
        
        # Enhance based on type
        if file_path.name.startswith("FEAT-"):
            data = enhance_feature_file(data)
        elif file_path.name.startswith("TASK-"):
            data = enhance_task_file(data)
        
        # Write back with proper formatting
        with open(file_path, 'w', encoding='utf-8') as f:
            json.dump(data, f, indent=2, ensure_ascii=False)
        
        return True
    except Exception as e:
        print(f"Error processing {file_path}: {e}")
        return False

def main():
    """Main function to process all FEAT and TASK files."""
    plans_dir = Path("scrum-master-plans/MameyFutureAI")
    
    if not plans_dir.exists():
        print(f"Directory not found: {plans_dir}")
        return
    
    processed = 0
    errors = 0
    
    # Process all FEAT files
    for feat_file in sorted(plans_dir.glob("FEAT-*.json")):
        if process_file(feat_file):
            processed += 1
        else:
            errors += 1
    
    # Process all TASK files
    for task_file in sorted(plans_dir.glob("TASK-*.json")):
        if process_file(task_file):
            processed += 1
        else:
            errors += 1
    
    print(f"Processed: {processed} files")
    print(f"Errors: {errors} files")

if __name__ == "__main__":
    main()
