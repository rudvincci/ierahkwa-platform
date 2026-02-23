using System.Collections.Generic;

namespace Mamey.Ntrada.Configuration
{
    public class Auth
    {
        public bool Enabled { get; set; }
        public bool Global { get; set; }
        public IDictionary<string, Policy> Policies { get; set; }
    }
}