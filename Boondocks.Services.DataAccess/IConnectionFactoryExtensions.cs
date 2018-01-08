﻿using System;
using System.Data;
using Boondocks.Services.DataAccess.Interfaces;
using Dapper;

namespace Boondocks.Services.DataAccess
{
    public static class IConnectionFactoryExtensions
    {
        public static IDbConnection CreateAndOpen(this IDbConnectionFactory factory)
        {
            var connection = factory.Create();

            connection.Open();

            return connection;
        }
    }

   
}