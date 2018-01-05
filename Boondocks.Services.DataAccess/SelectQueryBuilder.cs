using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Boondocks.Services.Base;
using Boondocks.Services.Contracts;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace Boondocks.Services.DataAccess
{
    /// <summary>
    /// Dynamically builds a sql query. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectQueryBuilder<T>
    {
        private readonly string _select;
        private readonly IQueryCollection _query;
        private int _parameterIndex = 0;

        private readonly DynamicParameters _parameters = new DynamicParameters();

        private readonly List<string> _whereElements = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="select">(e.g. "select * from Devices")</param>
        /// <param name="query"></param>
        public SelectQueryBuilder(string select, IQueryCollection query)
        {
            _select = select;
            _query = query;
        }

        /// <summary>
        /// Attempts to add a guid parameter
        /// </summary>
        /// <param name="queryStringName">the name of the parameter as it would appear in the query string.</param>
        /// <param name="columnName">The name of the column to be filtered.</param>
        public void AddGuidParameter(string queryStringName, string columnName)
        {
            Guid? value = ((string) _query[queryStringName]).ParseGuid();

            if (value != null)
            {
                Add(columnName, value);
            }
        }

        private void Add(string columnName, object value)
        {
            string parameterName = $"P{_parameterIndex}";

            _parameters.Add(parameterName, value);

            _whereElements.Add($"[{columnName}] = @{parameterName}");

            _parameterIndex++;
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction">Optional.</param>
        /// <returns></returns>
        public IEnumerable<T> Execute(IDbConnection connection, IDbTransaction transaction = null)
        {
            if (_parameterIndex == 0)
            {
                //This is the simple case. Just perform a simple query.
                return connection
                    .Query<T>(_select)
                    .ToArray();
            }

            //Format the where bits
            string wheres = string.Join(" and ", _whereElements);

            //Put the whole sql statement together.
            string sql = $"{_select} where {wheres}";

            //Do it!  Do it now!
            return connection
                .Query<T>(sql, _parameters)
                .ToArray();
        }
    }
}