using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;

namespace ConnectionUtils
{
    public abstract class ConnectionFactory
    {
        protected ConnectionFactory() { }

        private static ConnectionFactory instance;

        public static ConnectionFactory getInstance()
        {
            if (instance == null)
            {
                instance = new SqliteConnectionFactory(); 
            }
            return instance;
        }

        public abstract IDbConnection createConnection(IDictionary<string, string> props);
    }
}