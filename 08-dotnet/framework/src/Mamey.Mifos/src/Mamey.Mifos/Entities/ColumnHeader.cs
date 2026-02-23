namespace Mamey.Mifos.Entities
{
    public record ColumnHeader(string ColumnName, string ColumnType, int ColumnLength,
        string ColumnDisplayType, bool IsColumnNullable, bool IsColumnPrimaryKey,
        IEnumerable<ColumnValue> ColumnValues, string ColumnCode);
}

