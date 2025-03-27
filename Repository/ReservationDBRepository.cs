using System;
using System.Collections.Generic;
using System.Data;
using Agentie_turism_transport_csharp.Domain;
using Agentie_turism_transport_csharp.Repository;
using log4net;

namespace Agentie_turism_transport_csharp.repository
{
    public class ReservationDBRepository : IReservationRepository
    {
        private static readonly ILog log = LogManager.GetLogger("ReservationDbRepository");
        private readonly IDictionary<string, string> props;
        private readonly TripDBRepository tripRepository;

        public ReservationDBRepository(IDictionary<string, string> props, TripDBRepository tripRepository)
        {
            log.Info("Initializing ReservationDbRepository");
            this.props = props;
            this.tripRepository = tripRepository;
        }

        public Reservation FindOne(long id)
        {
            log.InfoFormat("Finding reservation with ID {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "SELECT id, clientName, clientPhone, ticketCount, trip FROM reservations WHERE id=@id";
                comm.Parameters.Add(CreateParameter(comm, "@id", id));

                using (var dataR = comm.ExecuteReader())
                {
                    if (dataR.Read())
                    {
                        var trip = tripRepository.FindOne(dataR.GetInt64(4));
                        var reservation = new Reservation(
                            dataR.GetString(1),
                            dataR.GetString(2),
                            dataR.GetInt32(3),
                            trip
                        );
                        reservation.Id = dataR.GetInt64(0);
                        return reservation;
                    }
                }
            }
            return null;
        }

        public IEnumerable<Reservation> FindAll()
        {
            log.Info("Finding all reservations");
            IDbConnection con = DBUtils.getConnection(props);
            IList<Reservation> reservations = new List<Reservation>();

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "SELECT id, clientName, clientPhone, ticketCount, trip FROM reservations";

                using (var dataR = comm.ExecuteReader())
                {
                    while (dataR.Read())
                    {
                        var trip = tripRepository.FindOne(dataR.GetInt64(4));
                        var reservation = new Reservation(
                            dataR.GetString(1),
                            dataR.GetString(2),
                            dataR.GetInt32(3),
                            trip
                        );
                        reservation.Id = dataR.GetInt64(0);
                        reservations.Add(reservation);
                    }
                }
            }
            return reservations;
        }

        public void Save(Reservation reservation)
        {
            log.InfoFormat("Saving reservation {0}", reservation);
            IDbConnection con = DBUtils.getConnection(props);
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "INSERT INTO reservations (clientName, clientPhone, ticketCount, trip) VALUES (@name, @phone, @tickets, @trip)";
                comm.Parameters.Add(CreateParameter(comm, "@name", reservation.clientName));
                comm.Parameters.Add(CreateParameter(comm, "@phone", reservation.clientPhone));
                comm.Parameters.Add(CreateParameter(comm, "@tickets", reservation.ticketCount));
                comm.Parameters.Add(CreateParameter(comm, "@trip", reservation.trip.Id));

                comm.ExecuteNonQuery();
            }
        }

        public void Delete(long id)
        {
            log.InfoFormat("Deleting reservation with ID {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "DELETE FROM reservations WHERE id=@id";
                comm.Parameters.Add(CreateParameter(comm, "@id", id));
                comm.ExecuteNonQuery();
            }
        }

        public void Update(Reservation reservation)
        {
            log.InfoFormat("Updating reservation {0}", reservation);
            IDbConnection con = DBUtils.getConnection(props);
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "UPDATE reservations SET clientName=@name, clientPhone=@phone, ticketCount=@tickets, trip=@trip WHERE id=@id";
                comm.Parameters.Add(CreateParameter(comm, "@name", reservation.clientName));
                comm.Parameters.Add(CreateParameter(comm, "@phone", reservation.clientPhone));
                comm.Parameters.Add(CreateParameter(comm, "@tickets", reservation.ticketCount));
                comm.Parameters.Add(CreateParameter(comm, "@trip", reservation.trip.Id));
                comm.Parameters.Add(CreateParameter(comm, "@id", reservation.Id));

                comm.ExecuteNonQuery();
            }
        }

        private IDbDataParameter CreateParameter(IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }
    }
}