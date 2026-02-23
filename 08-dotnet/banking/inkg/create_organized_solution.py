#!/usr/bin/env python3

import subprocess
import re
import uuid

def get_project_guids():
    """Extract project GUIDs from the solution file"""
    result = subprocess.run(['dotnet', 'sln', 'Mamey.Government.sln', 'list'], 
                          capture_output=True, text=True)
    projects = []
    for line in result.stdout.split('\n'):
        if 'Mamey.Government' in line:
            projects.append(line.strip())
    return projects

def generate_guid():
    """Generate a new GUID"""
    return str(uuid.uuid4()).upper()

def categorize_projects(projects):
    """Categorize projects into logical groups"""
    categories = {
        'Core Framework': [],
        'Shared Libraries': [],
        'Citizen Management': [],
        'Citizenship Services': [],
        'Passport Services': [],
        'Diplomat Services': [],
        'Portal & Gateway': [],
        'Support Services': [],
        'Saga Services': [],
        'Tests': []
    }
    
    for project in projects:
        if 'Tests' in project:
            categories['Tests'].append(project)
        elif 'Shared' in project:
            categories['Shared Libraries'].append(project)
        elif 'CitizenManagement' in project:
            categories['Citizen Management'].append(project)
        elif 'Citizenship' in project:
            categories['Citizenship Services'].append(project)
        elif 'Passport' in project:
            categories['Passport Services'].append(project)
        elif 'Diplomat' in project:
            categories['Diplomat Services'].append(project)
        elif 'Portal' in project or 'Gateway' in project:
            categories['Portal & Gateway'].append(project)
        elif 'Saga' in project:
            categories['Saga Services'].append(project)
        elif 'Notification' in project or 'Operation' in project or 'Payment' in project:
            categories['Support Services'].append(project)
        else:
            categories['Core Framework'].append(project)
    
    return categories

def create_solution_folders():
    """Create solution folder GUIDs"""
    return {
        'Core Framework': generate_guid(),
        'Shared Libraries': generate_guid(),
        'Citizen Management': generate_guid(),
        'Citizenship Services': generate_guid(),
        'Passport Services': generate_guid(),
        'Diplomat Services': generate_guid(),
        'Portal & Gateway': generate_guid(),
        'Support Services': generate_guid(),
        'Saga Services': generate_guid(),
        'Tests': generate_guid()
    }

def main():
    print("Creating organized Mamey.Government.sln with solution folders...")
    
    # Get all projects
    projects = get_project_guids()
    print(f"Found {len(projects)} Mamey.Government projects")
    
    # Categorize projects
    categories = categorize_projects(projects)
    
    # Generate folder GUIDs
    folder_guids = create_solution_folders()
    
    # Print categorization summary
    for category, projs in categories.items():
        print(f"{category}: {len(projs)} projects")
    
    print("\nSolution folders created with GUIDs:")
    for name, guid in folder_guids.items():
        print(f"  {name}: {guid}")

if __name__ == "__main__":
    main()
