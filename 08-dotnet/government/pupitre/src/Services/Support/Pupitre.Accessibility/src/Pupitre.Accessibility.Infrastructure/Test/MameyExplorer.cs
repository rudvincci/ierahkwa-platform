using System;
using System.Reflection;

namespace Pupitre.Accessibility.Infrastructure.Test;

public class MameyExplorer
{
    public static void ExploreAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        foreach (var assembly in assemblies)
        {
            if (assembly.FullName?.Contains("Mamey") == true)
            {
                Console.WriteLine($"Assembly: {assembly.FullName}");
                
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.Namespace?.Contains("Minio") == true || 
                            type.Namespace?.Contains("Redis") == true ||
                            type.Namespace?.Contains("PostgreSQL") == true)
                        {
                            Console.WriteLine($"  Type: {type.Namespace}.{type.Name}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Error exploring types: {ex.Message}");
                }
            }
        }
    }
}






















