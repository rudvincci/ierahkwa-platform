#!/usr/bin/env python3
"""
Add gRPC service definitions to Pupitre microservices.
"""

import os

def create_proto_file(api_path, service_name, entity_name):
    """Create a basic .proto file for the service"""
    proto_content = f'''syntax = "proto3";

option csharp_namespace = "Pupitre.{service_name}.Api.Protos";

package Pupitre.{service_name};

// gRPC service for {entity_name} operations
service {entity_name}Service {{
    // Get a single {entity_name} by ID
    rpc Get{entity_name} (Get{entity_name}Request) returns ({entity_name}Response) {{}};
    
    // List all {entity_name}s with pagination
    rpc List{entity_name}s (List{entity_name}sRequest) returns (List{entity_name}sResponse) {{}};
    
    // Stream {entity_name} updates
    rpc Stream{entity_name}Updates (Stream{entity_name}UpdatesRequest) returns (stream {entity_name}Response) {{}};
}}

message Get{entity_name}Request {{
    string id = 1;
}}

message List{entity_name}sRequest {{
    int32 page = 1;
    int32 page_size = 2;
}}

message List{entity_name}sResponse {{
    repeated {entity_name}Response items = 1;
    int32 total_count = 2;
    int32 page = 3;
    int32 page_size = 4;
}}

message {entity_name}Response {{
    string id = 1;
    string name = 2;
    int64 created_at = 3;
    int64 updated_at = 4;
}}

message Stream{entity_name}UpdatesRequest {{
    string filter = 1;
}}
'''
    proto_dir = os.path.join(api_path, "Protos")
    os.makedirs(proto_dir, exist_ok=True)
    proto_path = os.path.join(proto_dir, f"{entity_name}.proto")
    with open(proto_path, 'w') as f:
        f.write(proto_content)

def create_grpc_service(api_path, service_name, entity_name):
    """Create a gRPC service implementation"""
    grpc_content = f'''using Grpc.Core;
using Microsoft.Extensions.Logging;
using Pupitre.{service_name}.Api.Protos;
using Pupitre.{service_name}.Domain.Repositories;

namespace Pupitre.{service_name}.Api.Infrastructure.Grpc;

public class {entity_name}GrpcService : {entity_name}Service.{entity_name}ServiceBase
{{
    private readonly I{entity_name}Repository _repository;
    private readonly ILogger<{entity_name}GrpcService> _logger;

    public {entity_name}GrpcService(
        I{entity_name}Repository repository,
        ILogger<{entity_name}GrpcService> logger)
    {{
        _repository = repository;
        _logger = logger;
    }}

    public override async Task<{entity_name}Response> Get{entity_name}(
        Get{entity_name}Request request,
        ServerCallContext context)
    {{
        _logger.LogInformation("gRPC Get{entity_name} called for ID: {{Id}}", request.Id);
        
        // TODO: Implement actual retrieval logic
        // var entity = await _repository.GetAsync(new {entity_name}Id(Guid.Parse(request.Id)), context.CancellationToken);
        
        return new {entity_name}Response
        {{
            Id = request.Id,
            Name = "Sample",
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        }};
    }}

    public override async Task<List{entity_name}sResponse> List{entity_name}s(
        List{entity_name}sRequest request,
        ServerCallContext context)
    {{
        _logger.LogInformation("gRPC List{entity_name}s called - Page: {{Page}}, PageSize: {{PageSize}}", 
            request.Page, request.PageSize);
        
        var items = await _repository.BrowseAsync(context.CancellationToken);
        
        var response = new List{entity_name}sResponse
        {{
            TotalCount = items.Count,
            Page = request.Page,
            PageSize = request.PageSize
        }};
        
        // TODO: Add actual items to response
        
        return response;
    }}

    public override async Task Stream{entity_name}Updates(
        Stream{entity_name}UpdatesRequest request,
        IServerStreamWriter<{entity_name}Response> responseStream,
        ServerCallContext context)
    {{
        _logger.LogInformation("gRPC Stream{entity_name}Updates started with filter: {{Filter}}", request.Filter);
        
        // TODO: Implement actual streaming logic
        // This is a placeholder that sends updates every second
        while (!context.CancellationToken.IsCancellationRequested)
        {{
            await responseStream.WriteAsync(new {entity_name}Response
            {{
                Id = Guid.NewGuid().ToString(),
                Name = "StreamUpdate",
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }});
            
            await Task.Delay(1000, context.CancellationToken);
        }}
    }}
}}
'''
    grpc_dir = os.path.join(api_path, "Infrastructure", "Grpc")
    os.makedirs(grpc_dir, exist_ok=True)
    grpc_path = os.path.join(grpc_dir, f"{entity_name}GrpcService.cs")
    with open(grpc_path, 'w') as f:
        f.write(grpc_content)

def update_api_csproj(api_path, service_name, entity_name):
    """Update the Api.csproj to include proto files"""
    csproj_path = os.path.join(api_path, f"Pupitre.{service_name}.Api.csproj")
    if not os.path.exists(csproj_path):
        return
    
    with open(csproj_path, 'r') as f:
        content = f.read()
    
    # Check if already has Protobuf
    if "Protobuf Include" in content:
        return
    
    # Add Protobuf reference before closing </Project>
    protobuf_section = f'''
  <ItemGroup>
    <Protobuf Include="Protos\\{entity_name}.proto" GrpcServices="Server" />
  </ItemGroup>
'''
    content = content.replace('</Project>', f'{protobuf_section}</Project>')
    
    with open(csproj_path, 'w') as f:
        f.write(content)

def add_grpc(service_type, service_name, entity_name):
    base_dir = "/Volumes/Barracuda/mamey-io/code-final/Pupitre"
    api_path = os.path.join(base_dir, "src", "Services", service_type, f"Pupitre.{service_name}", "src", f"Pupitre.{service_name}.Api")
    
    if not os.path.exists(api_path):
        print(f"  Skipping {service_name} - Api path not found")
        return
    
    create_proto_file(api_path, service_name, entity_name)
    create_grpc_service(api_path, service_name, entity_name)
    update_api_csproj(api_path, service_name, entity_name)
    
    print(f"Created gRPC service for Pupitre.{service_name}")

if __name__ == "__main__":
    # Foundation services
    foundation = [
        ("Users", "User"),
        ("GLEs", "GLE"),
        ("Curricula", "Curriculum"),
        ("Lessons", "Lesson"),
        ("Assessments", "Assessment"),
        ("Progress", "LearningProgress"),
        ("Rewards", "Reward"),
        ("Notifications", "Notification"),
        ("Parents", "Parent"),
        ("Analytics", "Analytic"),
    ]
    
    # AI services
    ai = [
        ("AITutors", "Tutor"),
        ("AIAssessments", "AIAssessment"),
        ("AIContent", "ContentGeneration"),
        ("AISpeech", "SpeechRequest"),
        ("AIAdaptive", "AdaptiveLearning"),
        ("AIBehavior", "Behavior"),
        ("AISafety", "SafetyCheck"),
        ("AIRecommendations", "AIRecommendation"),
        ("AITranslation", "TranslationRequest"),
        ("AIVision", "VisionAnalysis"),
    ]
    
    # Support services
    support = [
        ("Educators", "Educator"),
        ("Fundraising", "Campaign"),
        ("Bookstore", "Book"),
        ("Aftercare", "AftercarePlan"),
        ("Accessibility", "AccessProfile"),
        ("Compliance", "ComplianceRecord"),
        ("Ministries", "MinistryData"),
        ("Operations", "OperationMetric"),
    ]
    
    for svc, entity in foundation:
        add_grpc("Foundation", svc, entity)
    
    for svc, entity in ai:
        add_grpc("AI", svc, entity)
    
    for svc, entity in support:
        add_grpc("Support", svc, entity)
    
    print("\nDone! Created gRPC services for all 28 services.")
