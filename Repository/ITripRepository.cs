using Agentie_turism_transport_csharp.Domain;
using Agentie_turism_transport_csharp.Repository;

namespace Agentie_turism_transport_csharp.repository
{
    public interface ITripRepository : IRepository<long, Trip>
    {
        IEnumerable<Trip> FindTripsByObjectiveDateAndTimeRange(string objective, DateTime departureDate, int startHour, int endHour);
    }
}