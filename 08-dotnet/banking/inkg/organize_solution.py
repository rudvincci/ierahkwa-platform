#!/usr/bin/env python3

import subprocess
import re
import uuid

def get_project_guids():
    """Get all Mamey.Government project GUIDs from the solution file"""
    result = subprocess.run(['grep', '-A', '1', 'Mamey.Government', 'Mamey.Government.sln'], 
                          capture_output=True, text=True)
    
    projects = []
    lines = result.stdout.split('\n')
    for line in lines:
        if 'Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}")' in line and 'Mamey.Government' in line:
            projects.append(line.strip())
    
    return projects

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
        elif 'CitizenManagement' in project and 'Saga' not in project:
            categories['Citizen Management'].append(project)
        elif 'Citizenship' in project:
            categories['Citizenship Services'].append(project)
        elif 'Passport' in project and 'Saga' not in project:
            categories['Passport Services'].append(project)
        elif 'Diplomat' in project and 'Saga' not in project:
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

def extract_project_guid(project_line):
    """Extract GUID from project line"""
    match = re.search(r'\{([A-F0-9-]+)\}', project_line)
    return match.group(1) if match else None

def generate_guid():
    """Generate a new GUID"""
    return str(uuid.uuid4()).upper()

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
    print("Organizing Mamey.Government.sln with solution folders...")
    
    # Get all projects
    projects = get_project_guids()
    print(f"Found {len(projects)} Mamey.Government projects")
    
    # Categorize projects
    categories = categorize_projects(projects)
    
    # Generate folder GUIDs
    folder_guids = create_solution_folders()
    
    # Print categorization summary
    print("\nProject categorization:")
    for category, projs in categories.items():
        print(f"  {category}: {len(projs)} projects")
    
    print("\nSolution folder GUIDs:")
    for name, guid in folder_guids.items():
        print(f"  {name}: {guid}")
    
    # Generate solution folder entries
    print("\nSolution folder entries:")
    for name, guid in folder_guids.items():
        print(f'Project("{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}") = "{name}", "{name}", "{{{guid}}}"')
        print("EndProject")
    
    # Generate nested projects mapping
    print("\nNestedProjects entries:")
    for category, projs in categories.items():
        if projs:
            folder_guid = folder_guids[category]
            for project in projs:
                project_guid = extract_project_guid(project)
                if project_guid:
                    print(f"\t\t{{{project_guid}}} = {{{folder_guid}}}")

if __name__ == "__main__":
    main()
