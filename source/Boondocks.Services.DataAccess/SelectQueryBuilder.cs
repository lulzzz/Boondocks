namespace Boondocks.Services.DataAccess
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Base;
    using Dapper;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    ///     Dynamically builds a sql query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectQueryBuilder<T>
    {
        private readonly DynamicParameters _parameters = new DynamicParameters();
        private readonly IQueryCollection _query;
        private readonly string _select;
        private readonly SortableColumn[] _sortableColumns;

        private readonly List<string> _whereElements = new List<string>();
        private int _parameterIndex;

        /// <summary>
        /// </summary>
        /// <param name="select">(e.g. "select * from Devices")</param>
        /// <param name="query"></param>
        /// <param name="sortableColumns"></param>
        public SelectQueryBuilder(string select, IQueryCollection query, SortableColumn[] sortableColumns = null)
        {
            _select = select;
            _query = query;
            _sortableColumns = sortableColumns ?? new SortableColumn[] { };
        }

        /// <summary>
        ///     Attempts to add a guid parameter
        /// </summary>
        /// <param name="queryStringName">the name of the parameter as it would appear in the query string.</param>
        /// <param name="columnName">The name of the column to be filtered.</param>
        /// <returns>Returns true if the condition was added, false otherwise.</returns>
        public bool TryAddGuidParameter(string queryStringName, string columnName)
        {
            var value = ((string) _query[queryStringName]).TryParseGuid();

            if (value == null)
                return false;

            AddCondition(columnName, value);

            return true;
        }

        /// <summary>
        ///     Attempts to add a boolean parameter
        /// </summary>
        /// <param name="queryStringName">the name of the parameter as it would appear in the query string.</param>
        /// <param name="columnName">The name of the column to be filtered.</param>
        /// <returns>Returns true if the condition was added, false otherwise.</returns>
        public bool TryAddBitParameter(string queryStringName, string columnName)
        {
            var value = ((string) _query[queryStringName]).TryParseBool();

            if (value == null)
                return false;

            AddCondition(columnName, value);

            return true;
        }

        /// <summary>
        ///     Attempts to add an integer parameter
        /// </summary>
        /// <param name="queryStringName">the name of the parameter as it would appear in the query string.</param>
        /// <param name="columnName">The name of the column to be filtered.</param>
        /// <returns>Returns true if the condition was added, false otherwise.</returns>
        public bool TryAddIntParameter(string queryStringName, string columnName)
        {
            var value = ((string) _query[queryStringName]).TryParseInt();

            if (value == null)
                return false;

            AddCondition(columnName, value);

            return true;
        }

        /// <summary>
        ///     Adds a condition to the query
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void AddCondition(string columnName, object value)
        {
            var parameterName = $"P{_parameterIndex}";

            _parameters.Add(parameterName, value);

            _whereElements.Add($"[{columnName}] = @{parameterName}");

            _parameterIndex++;
        }

        private SortedColumn TryGetSortedColumn(int index)
        {
            if (_sortableColumns == null)
                return null;

            var queryStringKey = $"orderby{index}";

            string queryStringValue = _query[queryStringKey];

            if (string.IsNullOrWhiteSpace(queryStringValue))
                return null;

            //See if this is actually a sortable column
            var sortableAscendingColumn = _sortableColumns
                .FirstOrDefault(c => c.QueryStringName == queryStringValue);

            if (sortableAscendingColumn != null)
                return new SortedColumn(sortableAscendingColumn.ColumnName, SortDirection.Ascending);

            //See if this is a descending sortable column
            var sortableDescendingColumn = _sortableColumns
                .FirstOrDefault(c => c.QueryStringName + "_desc" == queryStringValue);

            if (sortableDescendingColumn != null)
                return new SortedColumn(sortableDescendingColumn.ColumnName, SortDirection.Descending);

            return null;
        }

        private SortedColumn[] GetDefaultSortedColumns()
        {
            return _sortableColumns
                .Where(c => c.IsDefault)
                .Select(c => new SortedColumn(c.ColumnName, c.DefaultSortDirection))
                .ToArray();
        }

        /// <summary>
        ///     Gets the effective sorted columns for this query.
        /// </summary>
        /// <returns></returns>
        private SortedColumn[] GetSortedColumns()
        {
            var sortedColumns = new List<SortedColumn>(_sortableColumns.Length);

            //Check for sortable columns
            for (var index = 0; index < 50; index++)
            {
                //Attempt to get this column
                var sortedColumn = TryGetSortedColumn(index);

                if (sortedColumn == null)
                    break;

                //Check to see if we got it 
                sortedColumns.Add(sortedColumn);
            }

            //Check to see if we got a sort from the query string
            if (sortedColumns.Count > 0)
                return sortedColumns.ToArray();

            //Just return the default sorted columns in this case
            return GetDefaultSortedColumns();
        }

        /// <summary>
        ///     Gets the where clause.
        /// </summary>
        /// <returns></returns>
        private string GetWhereClause()
        {
            if (_parameterIndex == 0) return null;

            //Format the where bits
            var wheres = string.Join(" and ", _whereElements);

            //where 
            return $" where {wheres}";
        }

        /// <summary>
        ///     gets the "order by " clause.
        /// </summary>
        /// <returns></returns>
        private string GetOrderByClause()
        {
            //Get the sorted coumns
            var sortedColumns = GetSortedColumns();

            if (sortedColumns == null || sortedColumns.Length == 0)
                return null;

            //Check to see if we got any
            return $" order by {string.Join(", ", sortedColumns.Select(c => c.ToSql()))}";
        }

        /// <summary>
        ///     Executes the query.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction">Optional.</param>
        /// <returns></returns>
        public IEnumerable<T> Execute(IDbConnection connection, IDbTransaction transaction = null)
        {
            //Get the where clause
            var whereClause = GetWhereClause();

            //Get the order by clause
            var orderByClause = GetOrderByClause();

            //Put the whole sql statement together.
            var sql = $"{_select} {whereClause} {orderByClause}";

            //Do it!  Do it now!
            return connection
                .Query<T>(sql, _parameters)
                .ToArray();
        }

        /// <summary>
        ///     Represents a column that is to be sorted upon.
        /// </summary>
        private class SortedColumn
        {
            public SortedColumn(string columnName, SortDirection direction)
            {
                ColumnName = columnName;
                Direction = direction;
            }

            private string ColumnName { get; }

            private SortDirection Direction { get; }

            public string ToSql()
            {
                var sql = $" [{ColumnName}] ";

                if (Direction == SortDirection.Descending) sql += " desc ";

                return sql;
            }
        }
    }
}