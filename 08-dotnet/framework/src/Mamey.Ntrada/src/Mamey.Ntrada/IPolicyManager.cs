using System.Collections.Generic;

namespace Mamey.Ntrada
{
    internal interface IPolicyManager
    {
        IDictionary<string, string>  GetClaims(string policy);
    }
}