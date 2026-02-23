namespace Mamey.Microservice.Infrastructure.Mongo
{
    internal class MongoOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public bool DisableTransactions { get; set; }
        public bool Seed { get; set; }
    }
}