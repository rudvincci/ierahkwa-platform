#!/usr/bin/env python3
"""
Generate all service implementations from Authentik OpenAPI schema.
This script creates the actual API method implementations for all 23 services.
"""

import json
import os
import re
import hashlib
from pathlib import Path
from typing import Dict, List, Tuple, Optional

# Service name mapping (OpenAPI path -> C# service name)
SERVICE_MAP = {
    'admin': 'Admin',
    'authenticators': 'Authenticators',
    'core': 'Core',
    'crypto': 'Crypto',
    'enterprise': 'Enterprise',
    'events': 'Events',
    'flows': 'Flows',
    'managed': 'Managed',
    'oauth2': 'OAuth2',
    'outposts': 'Outposts',
    'policies': 'Policies',
    'propertymappings': 'PropertyMappings',
    'providers': 'Providers',
    'rac': 'Rac',
    'rbac': 'Rbac',
    'reports': 'Reports',
    'root': 'Root',
    'schema': 'Schema',
    'sources': 'Sources',
    'ssf': 'Ssf',
    'stages': 'Stages',
    'tasks': 'Tasks',
    'tenants': 'Tenants',
}

def to_pascal_case(s: str) -> str:
    """Convert to PascalCase."""
    # Remove special chars and split
    s = re.sub(r'[^a-zA-Z0-9]', '_', s)
    parts = s.split('_')
    return ''.join(word.capitalize() for word in parts if word)

def to_camel_case(s: str) -> str:
    """Convert to camelCase."""
    pascal = to_pascal_case(s)
    return pascal[0].lower() + pascal[1:] if pascal else ''

# C# keywords that need to be escaped
CSHARP_KEYWORDS = {
    'abstract', 'as', 'base', 'bool', 'break', 'byte', 'case', 'catch', 'char',
    'checked', 'class', 'const', 'continue', 'decimal', 'default', 'delegate',
    'do', 'double', 'else', 'enum', 'event', 'explicit', 'extern', 'false',
    'finally', 'fixed', 'float', 'for', 'foreach', 'goto', 'if', 'implicit',
    'in', 'int', 'interface', 'internal', 'is', 'lock', 'long', 'namespace',
    'new', 'null', 'object', 'operator', 'out', 'override', 'params', 'private',
    'protected', 'public', 'readonly', 'ref', 'return', 'sbyte', 'sealed',
    'short', 'sizeof', 'stackalloc', 'static', 'string', 'struct', 'switch',
    'this', 'throw', 'true', 'try', 'typeof', 'uint', 'ulong', 'unchecked',
    'unsafe', 'ushort', 'using', 'virtual', 'void', 'volatile', 'while'
}

def escape_csharp_keyword(name: str) -> str:
    """Escape C# keywords by prefixing with @."""
    if name.lower() in CSHARP_KEYWORDS:
        return f'@{name}'
    return name

def get_method_name(operation_id: str, path: str, method: str) -> str:
    """Generate C# method name from operation ID or path."""
    if operation_id:
        # Remove service prefix (e.g., "core_users_list" -> "UsersList")
        parts = operation_id.split('_')
        if len(parts) > 1:
            # Skip first part (service name)
            method_parts = parts[1:]
            method_name = ''.join(to_pascal_case(p) for p in method_parts)
            return method_name
        return to_pascal_case(operation_id)
    
    # Fallback: generate from path
    path_parts = path.strip('/').split('/')
    resource = path_parts[-1].rstrip('/') if path_parts else ''
    return to_pascal_case(f"{method}_{resource}")

def get_return_type(operation: Dict, components: Dict) -> str:
    """Determine C# return type from operation."""
    responses = operation.get('responses', {})
    
    # Check for 200/201 responses
    for status in ['200', '201']:
        if status in responses:
            response = responses[status]
            content = response.get('content', {})
            if 'application/json' in content:
                schema = content['application/json'].get('schema', {})
                
                # Handle $ref
                if '$ref' in schema:
                    ref_path = schema['$ref'].split('/')[-1]
                    return ref_path
                
                # Handle array
                if schema.get('type') == 'array':
                    items = schema.get('items', {})
                    if '$ref' in items:
                        item_type = items['$ref'].split('/')[-1]
                        return f'List<{item_type}>'
                    return 'List<object>'
                
                # Handle object
                if schema.get('type') == 'object':
                    return 'object'
    
    return 'object'

def generate_method(service_name: str, path: str, method: str, operation: Dict) -> Tuple[str, str]:
    """Generate C# method implementation."""
    operation_id = operation.get('operationId', '')
    summary = operation.get('summary', '')
    method_name = get_method_name(operation_id, path, service_name)
    
    # Get parameters
    parameters = operation.get('parameters', [])
    path_params = []
    query_params = []
    body_param = None
    
    for param in parameters:
        param_name = param.get('name', '')
        # Escape C# keywords
        param_name = escape_csharp_keyword(param_name)
        param_in = param.get('in', '')
        param_type = param.get('schema', {}).get('type', 'string')
        required = param.get('required', False)
        
        # Map OpenAPI types to C#
        type_map = {
            'string': 'string',
            'integer': 'int',
            'number': 'double',
            'boolean': 'bool'
        }
        csharp_type = type_map.get(param_type, 'string')
        
        if param_in == 'path':
            path_params.append((param_name, csharp_type, required))
        elif param_in == 'query':
            query_params.append((param_name, csharp_type, required))
    
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
    
    # Build path with parameters - replace {param} with {param} for string interpolation
    api_path = path
    for param_name, _, _ in path_params:
        # Keep the placeholder for string interpolation in C#
        pass  # Path will be interpolated in the implementation
    
    # Generate method signature
    # Order: required path params, required body param, optional query params, cancellation token
    param_list = []
    
    # Required path parameters
    for name, ptype, required in path_params:
        if required:
            param_list.append(f"{ptype} {name}")
    
    # Required body parameter
    if body_param:
        param_list.append("object request")
    
    # Optional query parameters
    for name, ptype, required in query_params:
        param_list.append(f"{ptype}? {name} = null")
    
    # Cancellation token (always optional)
    param_list.append("CancellationToken cancellationToken = default")
    
    # Always use object for return types (will be typed later)
    return_type = 'Task<object?>'
    # Check if it's a list endpoint (usually GET on collection)
    if method.upper() == 'GET' and not path_params:
        return_type = 'Task<PaginatedResult<object>>'
    
    # Generate implementation
    # Use unique variable names to avoid collisions - use hash of method name
    method_hash = hashlib.md5(method_name.encode()).hexdigest()[:8]
    url_var = f'url_{method_hash}'
    result_var = f'result_{method_hash}'
    
    # Replace path parameters in the URL template for string interpolation
    url_template = f"api/v3{api_path}"
    for param_name, _, _ in path_params:
        # Replace {param} with {param} for C# string interpolation
        url_template = url_template.replace(f'{{{param_name}}}', f'{{{param_name}}}')
    
    impl = f'''    /// <summary>
    /// {summary or f'{method.upper()} {path}'}
    /// </summary>
    public async {return_type} {method_name}Async({', '.join(param_list)})
    {{
        var client = GetHttpClient();
        var {url_var} = $\"{url_template}\";
'''
    
    # Add query string building
    if query_params:
        impl += '        var queryParams = new List<string>();\n'
        for param_name, param_type, _ in query_params:
            if param_type == 'bool':
                impl += f'        if ({param_name}.HasValue) queryParams.Add($"{param_name}={{{param_name}.Value.ToString().ToLower()}}");\n'
            elif param_type in ['int', 'double']:
                impl += f'        if ({param_name}.HasValue) queryParams.Add($"{param_name}={{{param_name}}}");\n'
            else:
                impl += f'        if (!string.IsNullOrEmpty({param_name})) queryParams.Add($"{param_name}={{{param_name}}}");\n'
        impl += f'        if (queryParams.Any()) {url_var} += "?" + string.Join("&", queryParams);\n'
    
    # Build HTTP request
    if method.upper() == 'GET':
        impl += f'        var response = await client.GetAsync({url_var}, cancellationToken);\n'
    elif method.upper() == 'POST':
        if body_param:
            impl += f'        var response = await client.PostAsJsonAsync({url_var}, request, cancellationToken);\n'
        else:
            impl += f'        var response = await client.PostAsync({url_var}, null, cancellationToken);\n'
    elif method.upper() == 'PUT':
        if body_param:
            impl += f'        var response = await client.PutAsJsonAsync({url_var}, request, cancellationToken);\n'
        else:
            impl += f'        var response = await client.PutAsync({url_var}, null, cancellationToken);\n'
    elif method.upper() == 'PATCH':
        if body_param:
            impl += f'        var response = await client.PatchAsJsonAsync({url_var}, request, cancellationToken);\n'
        else:
            # PATCH without body - use HttpMethod.Patch
            impl += f'        var request_msg = new HttpRequestMessage(HttpMethod.Patch, {url_var});\n'
            impl += f'        var response = await client.SendAsync(request_msg, cancellationToken);\n'
    elif method.upper() == 'DELETE':
        impl += f'        var response = await client.DeleteAsync({url_var}, cancellationToken);\n'
    
    # Handle return type
    if return_type == 'Task<PaginatedResult<object>>':
        impl += f'''        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var {result_var} = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions {{ PropertyNameCaseInsensitive = true }},
            cancellationToken);
        
        return {result_var} ?? new PaginatedResult<object> {{ Results = new List<object>() }};
    }}
'''
    else:
        impl += f'''        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var {result_var} = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions {{ PropertyNameCaseInsensitive = true }},
            cancellationToken);
        
        return {result_var};
    }}
'''
    
    # Generate interface signature
    interface_sig = f'''    /// <summary>
    /// {summary or f'{method.upper()} {path}'}
    /// </summary>
    {return_type} {method_name}Async({', '.join(param_list)});
'''
    
    return interface_sig, impl

def generate_service_files(schema_path: Path, output_dir: Path):
    """Generate all service implementation files."""
    with open(schema_path, 'r') as f:
        schema = json.load(f)
    
    paths = schema.get('paths', {})
    components = schema.get('components', {})
    
    # Group by service
    services = {}
    for path, methods in paths.items():
        parts = path.strip('/').split('/')
        if parts and parts[0] in SERVICE_MAP:
            service = parts[0]
            if service not in services:
                services[service] = []
            
            for method, operation in methods.items():
                if method in ['get', 'post', 'put', 'patch', 'delete']:
                    services[service].append((path, method.upper(), operation))
    
    # Generate files for each service
    for service_name, endpoints in sorted(services.items()):
        csharp_service_name = SERVICE_MAP[service_name]
        print(f"Generating {csharp_service_name} service: {len(endpoints)} endpoints")
        
        # Generate interface
        interface_methods = []
        implementation_methods = []
        
        for path, method, operation in endpoints:
            try:
                interface_sig, impl = generate_method(service_name, path, method, operation)
                interface_methods.append(interface_sig)
                implementation_methods.append(impl)
            except Exception as e:
                print(f"  Error generating method for {path} {method}: {e}")
                continue
        
        # Write interface file
        interface_file = output_dir / f'IAuthentik{csharp_service_name}Service.cs'
        interface_content = f'''using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik {csharp_service_name} API operations.
/// </summary>
public interface IAuthentik{csharp_service_name}Service
{{
{chr(10).join(interface_methods)}
}}
'''
        interface_file.write_text(interface_content)
        print(f"  Wrote {interface_file.name}")
        
        # Write implementation file
        impl_file = output_dir / f'Authentik{csharp_service_name}Service.cs'
        impl_content = f'''using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Authentik.Caching;
using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service implementation for Authentik {csharp_service_name} API operations.
/// </summary>
public class Authentik{csharp_service_name}Service : IAuthentik{csharp_service_name}Service
{{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<Authentik{csharp_service_name}Service> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="Authentik{csharp_service_name}Service"/> class.
    /// </summary>
    public Authentik{csharp_service_name}Service(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<Authentik{csharp_service_name}Service> logger,
        IAuthentikCache? cache = null)
    {{
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
        _cache = cache;
    }}

    /// <summary>
    /// Gets the HTTP client for making requests.
    /// </summary>
    protected HttpClient GetHttpClient() => _httpClientFactory.CreateClient("Authentik");

{chr(10).join(implementation_methods)}
}}
'''
        impl_file.write_text(impl_content)
        print(f"  Wrote {impl_file.name}")

if __name__ == '__main__':
    script_dir = Path(__file__).parent
    project_root = script_dir.parent
    schema_path = project_root / 'schema.json'
    services_dir = project_root / 'src' / 'Mamey.Authentik' / 'Services'
    
    if not schema_path.exists():
        print(f"Error: Schema file not found at {schema_path}")
        exit(1)
    
    generate_service_files(schema_path, services_dir)
    print(f"\n✅ Generated all service files in {services_dir}")
