namespace Boondocks.Services.DataAccess
{
    using System.Data;
    using Interfaces;

    public static class IConnectionFactoryExtensions
    {
        /// <summary>
        ///     Creates and opens a connection.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IDbConnection CreateAndOpen(this IDbConnectionFactory factory)
        {
            //Create the connection
            var connection = factory.Create();

            //Open it!
            connection.Open();

            //Well, that was actually pretty easy.
            return connection;
        }
    }
}