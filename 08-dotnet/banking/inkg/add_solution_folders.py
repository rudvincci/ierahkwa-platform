#!/usr/bin/env python3

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

def main():
    print("Creating solution folder GUIDs...")
    
    folder_guids = create_solution_folders()
    
    print("Solution folder GUIDs:")
    for name, guid in folder_guids.items():
        print(f"  {name}: {guid}")
    
    # Generate solution folder entries
    print("\nSolution folder entries to add:")
    for name, guid in folder_guids.items():
        print(f'Project("{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}") = "{name}", "{name}", "{{{guid}}}"')
        print("EndProject")

if __name__ == "__main__":
    main()
