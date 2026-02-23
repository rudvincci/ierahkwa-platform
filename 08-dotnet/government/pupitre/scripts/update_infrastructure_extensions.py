#!/usr/bin/env python3
"""
Update Infrastructure Extensions.cs files to wire up Redis and Composite repositories.
"""

import os
import re

def update_extensions_file(service_type, service_name, entity_name):
    base_dir = "/Volumes/Barracuda/mamey-io/code-final/Pupitre"
    file_path = os.path.join(
        base_dir, "src", "Services", service_type, 
        f"Pupitre.{service_name}", "src", 
        f"Pupitre.{service_name}.Infrastructure", "Extensions.cs"
    )
    
    if not os.path.exists(file_path):
        print(f"  File not found: {file_path}")
        return
    
    with open(file_path, 'r') as f:
        content = f.read()
    
    # Check if already updated
    if "AddRedisRepositories" in content:
        print(f"  Already updated: {service_name}")
        return
    
    # Add using statements for Redis and Composite
    redis_using = f"using Pupitre.{service_name}.Infrastructure.Redis;"
    composite_using = f"using Pupitre.{service_name}.Infrastructure.Composite;"
    
    # Find the last using statement
    lines = content.split('\n')
    last_using_idx = 0
    for i, line in enumerate(lines):
        if line.strip().startswith('using '):
            last_using_idx = i
    
    # Add new usings after the last one
    if redis_using not in content:
        lines.insert(last_using_idx + 1, redis_using)
        last_using_idx += 1
    if composite_using not in content:
        lines.insert(last_using_idx + 1, composite_using)
    
    content = '\n'.join(lines)
    
    # Find AddPostgresDb() and add Redis and Composite after it
    # Pattern: .AddPostgresDb()
    if ".AddPostgresDb()" in content and ".AddRedisRepositories()" not in content:
        content = content.replace(
            ".AddPostgresDb()",
            ".AddPostgresDb()\n                .AddRedisRepositories()\n                .AddCompositeRepositories()"
        )
    
    with open(file_path, 'w') as f:
        f.write(content)
    
    print(f"  Updated: {service_name}")

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
    
    print("Updating Foundation services...")
    for svc, entity in foundation:
        update_extensions_file("Foundation", svc, entity)
    
    print("\nUpdating AI services...")
    for svc, entity in ai:
        update_extensions_file("AI", svc, entity)
    
    print("\nUpdating Support services...")
    for svc, entity in support:
        update_extensions_file("Support", svc, entity)
    
    print("\nDone!")
