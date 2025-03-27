using System;
using System.Collections.Generic;
using System.Data;
using Agentie_turism_transport_csharp.Domain;
using log4net;


namespace Agentie_turism_transport_csharp.repository
{
    public class TripDBRepository : ITripRepository
    {
        private static readonly ILog log = LogManager.GetLogger("TripDbRepository");
        private readonly IDictionary<string, string> props;

        public TripDBRepository(IDictionary<string, string> props)
        {
            log.Info("Creating TripDbRepository");
            this.props = props;
        }

        public Trip FindOne(long id)
        {
            log.InfoFormat("Finding trip with ID {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "SELECT id, touristAttraction, transportCompany, departureTime, price, availableSeats FROM trips WHERE id=@id";
                var paramId = comm.CreateParameter();
                paramId.ParameterName = "@id";
                paramId.Value = id;
                comm.Parameters.Add(paramId);

                using (var dataR = comm.ExecuteReader())
                {
                    if (dataR.Read())
                    {
                        var trip = new Trip(
                            dataR.GetString(1),
                            dataR.GetString(2),
                            dataR.GetDateTime(3),
                            dataR.GetDouble(4),
                            dataR.GetInt32(5)
                        );
                        trip.Id = dataR.GetInt64(0);
                        return trip;
                    }
                }
            }
            return null;
        }

        public IEnumerable<Trip> FindAll()
        {
            IDbConnection con = DBUtils.getConnection(props);
            IList<Trip> trips = new List<Trip>();
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "SELECT id, touristAttraction, transportCompany, departureTime, price, availableSeats FROM trips";
                using (var dataR = comm.ExecuteReader())
                {
                    while (dataR.Read())
                    {
                        var trip = new Trip(
                            dataR.GetString(1),
                            dataR.GetString(2),
                            dataR.GetDateTime(3),
                            dataR.GetDouble(4),
                            dataR.GetInt32(5)
                        );
                        trip.Id = dataR.GetInt64(0);
                        trips.Add(trip);
                    }
                }
            }
            return trips;
        }

        public void Save(Trip trip)
        {
            log.InfoFormat("Saving trip {0}", trip);
            IDbConnection con = DBUtils.getConnection(props);
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "INSERT INTO trips (touristAttraction, transportCompany, departureTime, price, availableSeats) VALUES (@attraction, @company, @departure, @price, @seats)";
                comm.Parameters.Add(CreateParameter(comm, "@attraction", trip.touristAttraction));
                comm.Parameters.Add(CreateParameter(comm, "@company", trip.transportCompany));
                comm.Parameters.Add(CreateParameter(comm, "@departure", trip.departureTime));
                comm.Parameters.Add(CreateParameter(comm, "@price", trip.price));
                comm.Parameters.Add(CreateParameter(comm, "@seats", trip.availableSeats));

                var result = comm.ExecuteNonQuery();
            }
        }

        public void Delete(long id)
        {
            log.InfoFormat("Deleting trip with ID {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "DELETE FROM trips WHERE id=@id";
                comm.Parameters.Add(CreateParameter(comm, "@id", id));
                var result = comm.ExecuteNonQuery();
            }
        }

        public void Update(Trip trip)
        {
            log.InfoFormat("Updating trip {0}", trip);
            IDbConnection con = DBUtils.getConnection(props);
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "UPDATE trips SET touristAttraction=@attraction, transportCompany=@company, departureTime=@departure, price=@price, availableSeats=@seats WHERE id=@id";
                comm.Parameters.Add(CreateParameter(comm, "@attraction", trip.touristAttraction));
                comm.Parameters.Add(CreateParameter(comm, "@company", trip.transportCompany));
                comm.Parameters.Add(CreateParameter(comm, "@departure", trip.departureTime));
                comm.Parameters.Add(CreateParameter(comm, "@price", trip.price));
                comm.Parameters.Add(CreateParameter(comm, "@seats", trip.availableSeats));
                comm.Parameters.Add(CreateParameter(comm, "@id", trip.Id));

                var result = comm.ExecuteNonQuery();
            }
        }
        
        public IEnumerable<Trip> FindTripsByObjectiveDateAndTimeRange(string objective, DateTime departureDate, int startHour, int endHour)
        {
            log.InfoFormat("Finding trips for objective {0} on {1:yyyy-MM-dd} between {2} and {3}", objective, departureDate, startHour, endHour);
            IDbConnection con = DBUtils.getConnection(props);
            IList<Trip> trips = new List<Trip>();

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = @"
            SELECT id, touristAttraction, transportCompany, departureTime, price, availableSeats 
            FROM trips 
            WHERE touristAttraction = @objective 
            AND DATE(departureTime) = DATE(@departureDate)
            AND CAST(strftime('%H', departureTime) AS INTEGER) BETWEEN @startHour AND @endHour";

                comm.Parameters.Add(CreateParameter(comm, "@objective", objective));
                comm.Parameters.Add(CreateParameter(comm, "@departureDate", departureDate.ToString("yyyy-MM-dd")));
                comm.Parameters.Add(CreateParameter(comm, "@startHour", startHour));
                comm.Parameters.Add(CreateParameter(comm, "@endHour", endHour));

                using (var dataR = comm.ExecuteReader())
                {
                    while (dataR.Read())
                    {
                        var trip = new Trip(
                            dataR.GetString(1),
                            dataR.GetString(2),
                            dataR.GetDateTime(3),
                            dataR.GetDouble(4),
                            dataR.GetInt32(5)
                        );
                        trip.Id = dataR.GetInt64(0);
                        trips.Add(trip);
                    }
                }
            }
            return trips;
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