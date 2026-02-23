namespace Mamey.Mifos.Entities
{
    public record DataTable(string ApplicationTableName, string RegisteredTableName,
        IEnumerable<ColumnHeader> ColumnHeaderData);
}

