#!/usr/bin/env python3
"""
Script to generate service implementations from Authentik OpenAPI schema.
This creates the actual API method implementations in the service classes.
"""

import json
import os
import re
from pathlib import Path
from typing import Dict, List, Tuple

def to_pascal_case(s: str) -> str:
    """Convert snake_case or kebab-case to PascalCase."""
    # Remove leading/trailing slashes and split
    parts = s.strip('/').split('/')
    # Convert each part
    result = []
    for part in parts:
        # Handle kebab-case and snake_case
        words = re.split(r'[-_]', part)
        result.append(''.join(word.capitalize() for word in words if word))
    return ''.join(result)

def to_camel_case(s: str) -> str:
    """Convert to camelCase."""
    pascal = to_pascal_case(s)
    return pascal[0].lower() + pascal[1:] if pascal else ''

def get_csharp_type(schema_ref: str, schema_def: Dict) -> str:
    """Convert OpenAPI schema to C# type."""
    if '$ref' in schema_ref:
        # Extract type name from reference
        ref_path = schema_ref.split('/')[-1]
        return ref_path
    elif 'type' in schema_def:
        type_map = {
            'string': 'string',
            'integer': 'int',
            'number': 'double',
            'boolean': 'bool',
            'array': 'List<object>',
            'object': 'object'
        }
        return type_map.get(schema_def['type'], 'object')
    return 'object'

def generate_method_signature(path: str, method: str, operation: Dict) -> Tuple[str, str]:
    """Generate C# method signature from OpenAPI operation."""
    operation_id = operation.get('operationId', '')
    summary = operation.get('summary', '')
    
    # Extract resource name from path
    path_parts = path.strip('/').split('/')
    resource = path_parts[-1].rstrip('/') if path_parts else ''
    
    # Generate method name
    if operation_id:
        # Use operationId if available
        method_name = to_pascal_case(operation_id)
    else:
        # Generate from path and method
        method_name = to_pascal_case(f"{method}_{resource}")
    
    # Get parameters
    parameters = operation.get('parameters', [])
    param_list = []
    path_params = []
    query_params = []
    body_param = None
    
    for param in parameters:
        param_name = param.get('name', '')
        param_in = param.get('in', '')
        param_type = 'string'  # Default
        required = param.get('required', False)
        
        if param_in == 'path':
            path_params.append((param_name, param_type, required))
        elif param_in == 'query':
            query_params.append((param_name, param_type, required))
    
    # Check for request body
    request_body = operation.get('requestBody', {})
    if request_body:
        content = request_body.get('content', {})
        if 'application/json' in content:
            schema = content['application/json'].get('schema', {})
            if '$ref' in schema:
                body_param = schema['$ref'].split('/')[-1]
            else:
                body_param = 'object'
    
    # Build parameter list
    for name, ptype, required in path_params:
        param_list.append(f"{ptype} {name}" + ("" if required else " = default"))
    
    for name, ptype, required in query_params:
        param_list.append(f"{ptype}? {name} = null")
    
    if body_param:
        param_list.append(f"{body_param} request")
    
    param_list.append("CancellationToken cancellationToken = default")
    
    # Get return type
    responses = operation.get('responses', {})
    return_type = 'Task<object>'
    if '200' in responses or '201' in responses:
        success_response = responses.get('200') or responses.get('201')
        if success_response:
            content = success_response.get('content', {})
            if 'application/json' in content:
                schema = content['application/json'].get('schema', {})
                if '$ref' in schema:
                    ref_type = schema['$ref'].split('/')[-1]
                    return_type = f'Task<{ref_type}>'
                elif schema.get('type') == 'array':
                    items = schema.get('items', {})
                    if '$ref' in items:
                        item_type = items['$ref'].split('/')[-1]
                        return_type = f'Task<List<{item_type}>>'
                    else:
                        return_type = 'Task<List<object>>'
    
    # Generate method signature
    method_sig = f"    /// <summary>\n"
    method_sig += f"    /// {summary or f'{method.upper()} {path}'}\n"
    method_sig += f"    /// </summary>\n"
    method_sig += f"    {return_type} {method_name}Async({', '.join(param_list)});"
    
    return method_name, method_sig

def main():
    schema_path = Path(__file__).parent.parent / 'schema.json'
    services_dir = Path(__file__).parent.parent / 'src' / 'Mamey.Authentik' / 'Services'
    
    if not schema_path.exists():
        print(f"Error: Schema file not found at {schema_path}")
        return
    
    with open(schema_path, 'r') as f:
        schema = json.load(f)
    
    paths = schema.get('paths', {})
    components = schema.get('components', {})
    
    # Group paths by service
    services = {}
    for path, methods in paths.items():
        parts = path.strip('/').split('/')
        if parts:
            service = parts[0]
            if service not in services:
                services[service] = []
            
            for method, operation in methods.items():
                if method in ['get', 'post', 'put', 'patch', 'delete']:
                    services[service].append((path, method.upper(), operation))
    
    print(f"Found {len(services)} service areas")
    print(f"Total endpoints: {sum(len(s) for s in services.values())}")
    
    # Generate method signatures for each service
    for service_name, endpoints in sorted(services.items()):
        print(f"\n{service_name}: {len(endpoints)} endpoints")
        
        # Generate interface methods
        interface_methods = []
        for path, method, operation in endpoints[:10]:  # Limit for now
            try:
                method_name, method_sig = generate_method_signature(path, method, operation)
                interface_methods.append(method_sig)
            except Exception as e:
                print(f"  Error generating method for {path} {method}: {e}")
        
        if interface_methods:
            print(f"  Generated {len(interface_methods)} method signatures")

if __name__ == '__main__':
    main()
