using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace ConnectionUtils
{
    public class SqliteConnectionFactory : ConnectionFactory
    {
        public override IDbConnection createConnection(IDictionary<string, string> props)
        {
            if (!props.ContainsKey("ConnectionString"))
                throw new ArgumentException("Missing connection string in properties.");

            string connectionString = props["ConnectionString"];
            Console.WriteLine("SQLite --- Se deschide o conexiune la  ... {0}", connectionString);

            return new SqliteConnection(connectionString);
        }
    }
}