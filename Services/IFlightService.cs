// =============================================================================
// File: Services/IFlightService.cs
// OOP: POLYMORPHISM — interface contract; all flight types honour same methods
//      ABSTRACTION  — callers depend on the interface, not concrete classes
// =============================================================================

namespace AirlineReservationSystem.Services
{
    /// <summary>
    /// Contract that ALL flight service implementations must satisfy.
    ///
    /// ✦ POLYMORPHISM: BookingService holds an IFlightService reference.
    ///   At runtime it can be a LocalFlightService or GlobalFlightService —
    ///   the calling code never changes (Open/Closed principle).
    ///
    /// ✦ ABSTRACTION: Hides whether flights are domestic or international.
    /// </summary>
    public interface IFlightService
    {
        /// <summary>Type label shown to the user.</summary>
        string FlightType { get; }

        /// <summary>All origin options.</summary>
        string[] Sources { get; }

        /// <summary>All destination options.</summary>
        string[] Destinations { get; }

        /// <summary>Departure times per slot index (0-based).</summary>
        string[] DepartureTimes { get; }

        /// <summary>Arrival times per slot index (0-based).</summary>
        string[] ArrivalTimes { get; }

        /// <summary>Price per seat.</summary>
        decimal FarePerSeat { get; }

        /// <summary>Returns the source name for the given 1-based index.</summary>
        string GetSource(int index);

        /// <summary>Returns the destination name for the given 1-based index.</summary>
        string GetDestination(int index);

        /// <summary>Prints the full schedule to the console.</summary>
        void DisplaySchedule(string airlineName);
    }
}
