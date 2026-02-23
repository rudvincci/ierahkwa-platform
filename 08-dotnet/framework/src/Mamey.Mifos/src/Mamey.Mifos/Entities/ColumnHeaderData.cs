namespace Mamey.Mifos.Entities
{
    public class ColumnHeaderData
    {
        public string ApplicationTableName { get; set; }
        public string RegisteredTableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public int ColumnLength { get; set; }
        public string ColumnDisplayType { get; set; }
        public bool IsColumnNullable { get; set; }
        public bool IsColumnPrimaryKey { get; set; }
        public IEnumerable<object> ColumnValues { get; set; }
    }
}

