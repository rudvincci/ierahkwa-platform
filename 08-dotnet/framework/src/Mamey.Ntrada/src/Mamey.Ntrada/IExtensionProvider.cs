using System.Collections.Generic;

namespace Mamey.Ntrada
{
    internal interface IExtensionProvider
    {
        IEnumerable<IEnabledExtension> GetAll();
    }
}