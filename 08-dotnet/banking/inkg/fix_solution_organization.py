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
    print("Creating organized solution with proper project mapping...")
    
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
    
    # Generate the complete solution file
    solution_content = []
    solution_content.append("Microsoft Visual Studio Solution File, Format Version 12.00")
    solution_content.append("# Visual Studio Version 17")
    solution_content.append("VisualStudioVersion = 17.0.31903.59")
    solution_content.append("MinimumVisualStudioVersion = 10.0.40219.1")
    solution_content.append("")
    
    # Add solution folders
    solution_content.append("# Solution Folders")
    for name, guid in folder_guids.items():
        solution_content.append(f'Project("{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}") = "{name}", "{name}", "{{{guid}}}"')
        solution_content.append("EndProject")
    solution_content.append("")
    
    # Add projects organized by category
    all_project_guids = []
    for category, projs in categories.items():
        if projs:
            solution_content.append(f"# {category}")
            for project in projs:
                solution_content.append(project)
                solution_content.append("EndProject")
                # Extract GUID for later use
                guid = extract_project_guid(project)
                if guid:
                    all_project_guids.append((guid, category))
            solution_content.append("")
    
    # Add Global sections
    solution_content.append("Global")
    solution_content.append("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution")
    solution_content.append("\t\tDebug|Any CPU = Debug|Any CPU")
    solution_content.append("\t\tRelease|Any CPU = Release|Any CPU")
    solution_content.append("\tEndGlobalSection")
    solution_content.append("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution")
    
    # Add project configurations
    for guid, category in all_project_guids:
        solution_content.append(f"\t\t{{{guid}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU")
        solution_content.append(f"\t\t{{{guid}}}.Debug|Any CPU.Build.0 = Debug|Any CPU")
        solution_content.append(f"\t\t{{{guid}}}.Release|Any CPU.ActiveCfg = Release|Any CPU")
        solution_content.append(f"\t\t{{{guid}}}.Release|Any CPU.Build.0 = Release|Any CPU")
    
    solution_content.append("\tEndGlobalSection")
    solution_content.append("\tGlobalSection(SolutionProperties) = preSolution")
    solution_content.append("\t\tHideSolutionNode = FALSE")
    solution_content.append("\tEndGlobalSection")
    solution_content.append("\tGlobalSection(NestedProjects) = preSolution")
    
    # Add nested projects (organize projects into folders)
    for guid, category in all_project_guids:
        folder_guid = folder_guids[category]
        solution_content.append(f"\t\t{{{guid}}} = {{{folder_guid}}}")
    
    solution_content.append("\tEndGlobalSection")
    solution_content.append("\tGlobalSection(ExtensibilityGlobals) = postSolution")
    solution_content.append("\t\tSolutionGuid = {12345678-1234-1234-1234-123456789012}")
    solution_content.append("\tEndGlobalSection")
    solution_content.append("EndGlobal")
    
    # Write to file
    with open('Mamey.Government.Organized.sln', 'w') as f:
        f.write('\n'.join(solution_content))
    
    print("Complete organized solution file saved as: Mamey.Government.Organized.sln")
    print(f"Total lines: {len(solution_content)}")

if __name__ == "__main__":
    main()
