using System.Collections.Generic;

namespace Mamey.Ntrada.Extensions.Cors
{
    public class CorsOptions : IOptions
    {
        public bool AllowCredentials { get; set; }
        public IEnumerable<string> AllowedOrigins { get; set; }
        public IEnumerable<string> AllowedMethods { get; set; }
        public IEnumerable<string> AllowedHeaders { get; set; }
        public IEnumerable<string> ExposedHeaders { get; set; }
    }
}