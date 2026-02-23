#!/usr/bin/env python3
"""
Comprehensive enhancement of FEAT and TASK JSON files.
Adds detailed descriptions, comprehensive requirements, and detailed acceptance criteria.
"""

import json
from pathlib import Path
from typing import Dict, Any

def enhance_description(desc: str, task_id: str, title: str) -> str:
    """Enhance description with more detail if needed."""
    if len(desc) > 200:
        return desc  # Already comprehensive
    
    # Add context about what this feature/task accomplishes
    enhanced = desc
    if "Implement" in desc or "Create" in desc or "Add" in desc:
        # Description is action-oriented, ensure it's complete
        if len(desc) < 150:
            enhanced = f"{desc} This includes comprehensive implementation, testing, documentation, and integration with the MameyFutureAI ecosystem."
    
    return enhanced

def enhance_requirements(reqs: list, task_id: str) -> list:
    """Enhance requirements list with more comprehensive items."""
    if len(reqs) >= 10:
        return reqs  # Already comprehensive
    
    enhanced = reqs.copy()
    
    # Add standard requirements if missing
    standard_additions = [
        "Add comprehensive error handling",
        "Implement input validation",
        "Add logging and monitoring",
        "Write unit tests",
        "Write integration tests",
        "Add documentation",
        "Ensure code follows best practices"
    ]
    
    for std_req in standard_additions:
        if std_req not in enhanced:
            # Only add if it makes sense for this feature/task
            if any(keyword in std_req.lower() for keyword in ["error", "validation", "test", "doc", "practice"]):
                if not any(existing.lower().startswith(std_req.lower().split()[0]) for existing in enhanced):
                    enhanced.append(std_req)
    
    return enhanced

def enhance_acceptance_criteria(criteria: list, task_id: str) -> list:
    """Enhance acceptance criteria with more detailed items."""
    if len(criteria) >= 8:
        return criteria  # Already comprehensive
    
    enhanced = criteria.copy()
    
    # Add standard acceptance criteria if missing
    standard_additions = [
        "All tests pass with >90% coverage",
        "Code follows project coding standards",
        "Documentation is complete and accurate",
        "Integration with dependent services works correctly",
        "Error handling covers all edge cases",
        "Performance meets specified requirements"
    ]
    
    for std_crit in standard_additions:
        if std_crit not in enhanced:
            if not any(existing.lower().startswith(std_crit.lower().split()[0]) for existing in enhanced):
                enhanced.append(std_crit)
    
    return enhanced

def enhance_feature(data: Dict[str, Any]) -> Dict[str, Any]:
    """Comprehensively enhance a feature file."""
    task_id = data.get("taskId", "")
    title = data.get("title", "")
    
    # Enhance description
    if "description" in data:
        data["description"] = enhance_description(data["description"], task_id, title)
    
    # Enhance requirements
    if "planningSources" in data and "requirements" in data["planningSources"]:
        data["planningSources"]["requirements"] = enhance_requirements(
            data["planningSources"]["requirements"], task_id
        )
    
    # Enhance acceptance criteria
    if "acceptanceCriteria" in data:
        data["acceptanceCriteria"] = enhance_acceptance_criteria(
            data["acceptanceCriteria"], task_id
        )
    
    # Ensure definitionOfDone is comprehensive
    dod = data.get("definitionOfDone", [])
    if len(dod) < 10:
        standard_dod = [
            "Implementation complete and tested",
            ">90% test coverage achieved",
            "All unit tests passing",
            "All integration tests passing",
            "Code review completed",
            "Module documentation updated",
            "Design documentation reviewed",
            "Performance requirements met",
            "Security requirements met",
            "Integration with ecosystem verified"
        ]
        for item in standard_dod:
            if item not in dod:
                dod.append(item)
        data["definitionOfDone"] = dod
    
    return data

def enhance_task(data: Dict[str, Any]) -> Dict[str, Any]:
    """Comprehensively enhance a task file."""
    task_id = data.get("taskId", "")
    title = data.get("title", "")
    
    # Enhance description
    if "description" in data:
        data["description"] = enhance_description(data["description"], task_id, title)
    
    # Enhance requirements
    if "planningSources" in data and "requirements" in data["planningSources"]:
        data["planningSources"]["requirements"] = enhance_requirements(
            data["planningSources"]["requirements"], task_id
        )
    
    # Enhance acceptance criteria
    if "acceptanceCriteria" in data:
        data["acceptanceCriteria"] = enhance_acceptance_criteria(
            data["acceptanceCriteria"], task_id
        )
    
    # Ensure definitionOfDone is comprehensive
    dod = data.get("definitionOfDone", [])
    if len(dod) < 10:
        standard_dod = [
            "Implementation complete and tested",
            ">90% test coverage achieved",
            "All unit tests passing",
            "All integration tests passing",
            "Code review completed",
            "Module documentation updated",
            "Design documentation reviewed",
            "Performance requirements met",
            "Security requirements met",
            "Integration verified"
        ]
        for item in standard_dod:
            if item not in dod:
                dod.append(item)
        data["definitionOfDone"] = dod
    
    return data

def process_file(file_path: Path) -> bool:
    """Process and enhance a single JSON file."""
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            data = json.load(f)
        
        # Enhance based on type
        if file_path.name.startswith("FEAT-"):
            data = enhance_feature(data)
        elif file_path.name.startswith("TASK-"):
            data = enhance_task(data)
        
        # Write back with proper formatting
        with open(file_path, 'w', encoding='utf-8') as f:
            json.dump(data, f, indent=2, ensure_ascii=False)
        
        return True
    except Exception as e:
        print(f"Error processing {file_path}: {e}")
        return False

def main():
    """Main function."""
    plans_dir = Path("scrum-master-plans/MameyFutureAI")
    
    if not plans_dir.exists():
        print(f"Directory not found: {plans_dir}")
        return
    
    processed = 0
    errors = 0
    
    for json_file in sorted(plans_dir.glob("FEAT-*.json")):
        if process_file(json_file):
            processed += 1
        else:
            errors += 1
    
    for json_file in sorted(plans_dir.glob("TASK-*.json")):
        if process_file(json_file):
            processed += 1
        else:
            errors += 1
    
    print(f"Enhanced: {processed} files")
    print(f"Errors: {errors} files")

if __name__ == "__main__":
    main()
