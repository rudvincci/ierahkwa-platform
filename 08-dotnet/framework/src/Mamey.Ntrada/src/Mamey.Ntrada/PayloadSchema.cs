using System.Dynamic;

namespace Mamey.Ntrada
{
    public class PayloadSchema
    {
        public ExpandoObject Payload { get; }
        public string Schema { get; }

        public PayloadSchema(ExpandoObject payload, string schema)
        {
            Payload = payload;
            Schema = schema;
        }
    }
}