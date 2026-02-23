using System.Collections.Generic;

namespace Mamey.MicroMonolith.Infrastructure.Modules;

internal record ModuleInfo(string Name, IEnumerable<string> Policies);