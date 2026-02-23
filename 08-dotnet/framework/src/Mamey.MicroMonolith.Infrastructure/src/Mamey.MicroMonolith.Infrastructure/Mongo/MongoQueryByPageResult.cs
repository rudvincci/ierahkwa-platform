using System.Collections.Generic;

namespace Mamey.MicroMonolith.Infrastructure.Mongo
{
    public static partial class Extensions
    {
        public record MongoQueryByPageResult<TDocument>(IReadOnlyList<TDocument> Data, int TotalPages, long? TotalResults);
    }
}