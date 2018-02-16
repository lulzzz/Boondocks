namespace Boondocks.Services.DataAccess
{
    public class SortableColumn
    {
        public SortableColumn(
            string queryStringName,
            string columnName,
            bool isDefault = false,
            SortDirection defaultSortDirection = SortDirection.Ascending)
        {
            QueryStringName = queryStringName;
            ColumnName = columnName;
            IsDefault = isDefault;
            DefaultSortDirection = defaultSortDirection;
        }

        public string QueryStringName { get; }

        public string ColumnName { get; }

        public bool IsDefault { get; }

        public SortDirection DefaultSortDirection { get; }
    }
}