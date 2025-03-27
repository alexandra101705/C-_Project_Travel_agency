using System;
using System.Data;
using System.IO;
using System.Reflection;
using Agentie_turism_transport_csharp.Domain;
using Agentie_turism_transport_csharp.repository;
using Agentie_turism_transport_csharp.Repository;
using ConnectionUtils;
using Microsoft.Extensions.Configuration;
using log4net;
using log4net.Config;

class Program
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
    static void Main()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        
        Logger.Info("Aplicația a pornit.");
        
        IDictionary<string, string> props = new Dictionary<string, string>
        {
            { "ConnectionString", "Data Source=turism.db;" }  
        };

        ConnectionFactory factory = ConnectionFactory.getInstance();
        using (IDbConnection connection = factory.createConnection(props))
        {
            connection.Open();
            Console.WriteLine("Conexiunea la SQLite este deschisă!");
        }
        
        TripDBRepository tripRepo = new TripDBRepository(props);
        ReservationDBRepository reservationRepo = new ReservationDBRepository(props, tripRepo);
        SoftUserDBRepository userRepo = new SoftUserDBRepository(props);

        
        Console.WriteLine("=== TRIPS ===");
        foreach (var trip in tripRepo.FindAll())
        {
            Console.WriteLine(trip);
        }

        Console.WriteLine("\n=== RESERVATIONS ===");
        foreach (var reservation in reservationRepo.FindAll())
        {
            Console.WriteLine(reservation);
        }

        Console.WriteLine("\n=== USERS ===");
        foreach (var user in userRepo.FindAll())
        {
            Console.WriteLine(user);
        }
        
        Console.WriteLine("=== TRIPS FILTER ===");
        DateTime searchDate = new DateTime(2025, 3, 18);
        IEnumerable<Trip> trips = tripRepo.FindTripsByObjectiveDateAndTimeRange("Castelul Banffy",searchDate, 10, 12);

        foreach (Trip trip in trips)
        {
            Console.WriteLine(trip);
        }

        Logger.Info("Application finished execution.");
    }
}