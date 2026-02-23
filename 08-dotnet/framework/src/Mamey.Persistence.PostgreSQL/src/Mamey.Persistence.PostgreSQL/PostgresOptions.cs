using Mamey.Persistence.SQL;

namespace Mamey.Postgres;

public class PostgresOptions : SQLOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to seed the database with initial data.
    /// </summary>
    public bool Seed { get; set; }
}
