#!/usr/bin/env python3

import re
import uuid

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

def categorize_projects():
    """Define project categorization rules"""
    return {
        'Tests': ['Tests'],
        'Shared Libraries': ['Shared'],
        'Citizen Management': ['CitizenManagement'],
        'Citizenship Services': ['Citizenship'],
        'Passport Services': ['Passport'],
        'Diplomat Services': ['Diplomat'],
        'Portal & Gateway': ['Portal', 'Gateway'],
        'Support Services': ['Notification', 'Operation', 'Payment'],
        'Saga Services': ['Saga'],
        'Core Framework': []  # Default category
    }

def get_project_category(project_name, categorization):
    """Determine which category a project belongs to"""
    for category, keywords in categorization.items():
        if category == 'Core Framework':
            continue
        for keyword in keywords:
            if keyword in project_name:
                return category
    return 'Core Framework'

def main():
    print("Adding solution folders to existing Mamey.Government.sln...")
    
    # Read the current solution file
    with open('Mamey.Government.sln', 'r') as f:
        content = f.read()
    
    # Generate folder GUIDs
    folder_guids = create_solution_folders()
    
    # Define categorization rules
    categorization = categorize_projects()
    
    # Find all Mamey.Government projects
    project_pattern = r'Project\("\{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC\}"\) = "([^"]*Mamey\.Government[^"]*)", "([^"]*)", "\{([^}]+)\}"'
    projects = re.findall(project_pattern, content)
    
    print(f"Found {len(projects)} Mamey.Government projects")
    
    # Categorize projects
    categorized_projects = {}
    for project_name, project_path, project_guid in projects:
        category = get_project_category(project_name, categorization)
        if category not in categorized_projects:
            categorized_projects[category] = []
        categorized_projects[category].append((project_name, project_path, project_guid))
    
    # Print categorization summary
    print("\nProject categorization:")
    for category, projs in categorized_projects.items():
        print(f"  {category}: {len(projs)} projects")
    
    # Generate solution folder entries
    solution_folders = []
    for name, guid in folder_guids.items():
        solution_folders.append(f'Project("{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}") = "{name}", "{name}", "{{{guid}}}"')
        solution_folders.append("EndProject")
    
    # Generate nested projects mapping
    nested_projects = []
    for category, projs in categorized_projects.items():
        if projs and category in folder_guids:
            folder_guid = folder_guids[category]
            for project_name, project_path, project_guid in projs:
                nested_projects.append(f"\t\t{{{project_guid}}} = {{{folder_guid}}}")
    
    # Insert solution folders after the last project
    last_project_end = content.rfind('EndProject')
    if last_project_end != -1:
        insert_pos = content.find('\n', last_project_end) + 1
        solution_folders_text = '\n'.join(solution_folders) + '\n\n'
        content = content[:insert_pos] + solution_folders_text + content[insert_pos:]
    
    # Add NestedProjects section before EndGlobal
    nested_section = f"""
\tGlobalSection(NestedProjects) = preSolution
{chr(10).join(nested_projects)}
\tEndGlobalSection"""
    
    endglobal_pos = content.rfind('EndGlobal')
    if endglobal_pos != -1:
        content = content[:endglobal_pos] + nested_section + '\n' + content[endglobal_pos:]
    
    # Write the modified solution file
    with open('Mamey.Government.sln', 'w') as f:
        f.write(content)
    
    print("Solution folders and project organization added successfully!")
    print(f"Added {len(solution_folders)//2} solution folders")
    print(f"Organized {len(nested_projects)} projects into folders")

if __name__ == "__main__":
    main()
