using System.Collections.Generic;

namespace Mamey.MicroMonolith.Infrastructure.Modules;

internal class ModuleInfoProvider
{
    public List<ModuleInfo> Modules { get; } = new();
}