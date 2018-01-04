using System;
using System.Data.SqlClient;
using Dapper;
using Xunit;

namespace Boondocks.Services.DataAccess.Tests
{
    public class DataEntryTests
    {
        private const string ConnectionString = @"Server=localhost\sqlexpress;Database=Boondocks;User Id=boondocks;Password=#Px@S:w_j+V97ngz;";

        //#Px@S:w_j+V97ngz


        [Fact]
        public void CreateDeviceType()
        {
            using (var connection = CreateOpenConnection())
            {
                connection.Execute(@"insert DeviceTypes(Id, Name, CreatedUtc) values(@id, @name, @createdUtc)",
                    new
                    {
                        id = Guid.NewGuid(),
                        name = "Raspberry Pi 3",
                        createdUtc = DateTime.UtcNow 
                    });
            }

            Assert.True(true);
        }

        private SqlConnection CreateOpenConnection()
        {
            var cs = ConnectionString;
            //if (mars)
            //{
            //    var scsb = new SqlConnectionStringBuilder(cs)
            //    {
            //        MultipleActiveResultSets = true
            //    };
            //    cs = scsb.ConnectionString;
            //}
            var connection = new SqlConnection(cs);
            connection.Open();
            return connection;
        }
    }
}
