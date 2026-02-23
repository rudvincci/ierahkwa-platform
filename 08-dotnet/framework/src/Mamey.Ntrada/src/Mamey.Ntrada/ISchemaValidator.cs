using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mamey.Ntrada
{
    internal interface ISchemaValidator
    {
        Task<IEnumerable<Error>> ValidateAsync(string payload, string schema);
    }
}