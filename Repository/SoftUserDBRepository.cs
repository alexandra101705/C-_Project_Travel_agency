using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Agentie_turism_transport_csharp.Domain;
using log4net;

namespace Agentie_turism_transport_csharp.repository
{
    public class SoftUserDBRepository : ISoftUserRepository
    {
        private static readonly ILog log = LogManager.GetLogger("SoftUserDbRepository");
        private readonly IDictionary<string, string> props;

        public SoftUserDBRepository(IDictionary<string, string> props)
        {
            log.Info("Initializing SoftUserDbRepository");
            this.props = props;
        }

        public SoftUser FindOne(long id)
        {
            log.InfoFormat("Finding user with ID {0}", id);
            IDbConnection con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "SELECT id, username, password FROM soft_users WHERE id=@id";
                comm.Parameters.Add(CreateParameter(comm, "@id", id));

                using (var dataR = comm.ExecuteReader())
                {
                    if (dataR.Read())
                    {
                        var user = new SoftUser(
                            dataR.GetString(1), // Username
                            dataR.GetString(2)  // Password
                        );
                        user.Id = dataR.GetInt64(0); // ID
                        return user;
                    }
                }
            }
            return null;
        }

        public IEnumerable<SoftUser> FindAll()
        {
            log.Info("Finding all users");
            IDbConnection con = DBUtils.getConnection(props);
            IList<SoftUser> users = new List<SoftUser>();

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "SELECT id, username, password FROM soft_users";

                using (var dataR = comm.ExecuteReader())
                {
                    while (dataR.Read())
                    {
                        var user = new SoftUser(
                            dataR.GetString(1),
                            dataR.GetString(2)
                        );
                        user.Id = dataR.GetInt64(0);
                        users.Add(user);
                    }
                }
            }
            return users;
        }

        public void Save(SoftUser user)
        {
            log.InfoFormat("Saving user {0}", user);
            IDbConnection con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "INSERT INTO soft_users (username, password) VALUES (@username, @password)";
                comm.Parameters.Add(CreateParameter(comm, "@username", user.username));
                comm.Parameters.Add(CreateParameter(comm, "@password", user.password));

                var result = comm.ExecuteNonQuery();
            }
        }

        public void Delete(long id)
        {
            log.InfoFormat("Deleting user with ID {0}", id);
            IDbConnection con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "DELETE FROM soft_users WHERE id=@id";
                comm.Parameters.Add(CreateParameter(comm, "@id", id));
                var result = comm.ExecuteNonQuery();
            }
        }

        public void Update(SoftUser user)
        {
            log.InfoFormat("Updating user {0}", user);
            IDbConnection con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "UPDATE soft_users SET username=@username, password=@password WHERE id=@id";
                comm.Parameters.Add(CreateParameter(comm, "@username", user.username));
                comm.Parameters.Add(CreateParameter(comm, "@password", user.password));
                comm.Parameters.Add(CreateParameter(comm, "@id", user.Id));

                var result = comm.ExecuteNonQuery();
            }
        }

        private IDbDataParameter CreateParameter(IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }
    }
}
