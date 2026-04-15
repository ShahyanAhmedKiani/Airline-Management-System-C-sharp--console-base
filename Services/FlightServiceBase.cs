// =============================================================================
// File: Services/FlightServiceBase.cs  (+ LocalFlightService + GlobalFlightService)
// OOP: INHERITANCE   — Local and Global both extend FlightServiceBase
//      POLYMORPHISM  — both implement IFlightService; used interchangeably
//      ENCAPSULATION — times and routes stored in protected arrays
// =============================================================================

using AirlineReservationSystem.Helpers;

namespace AirlineReservationSystem.Services
{
    // =========================================================================
    // Abstract base — shared logic extracted here (DRY principle)
    // ✦ INHERITANCE: LocalFlightService and GlobalFlightService both inherit this
    // =========================================================================

    /// <summary>
    /// Abstract base class for flight services.
    /// Implements the shared DisplaySchedule() so subclasses don't repeat code.
    /// </summary>
    public abstract class FlightServiceBase : IFlightService
    {
        // ── IFlightService contract properties ────────────────────────────────
        public abstract string   FlightType      { get; }
        public abstract string[] Sources         { get; }
        public abstract string[] Destinations    { get; }
        public abstract string[] DepartureTimes  { get; }
        public abstract string[] ArrivalTimes    { get; }
        public abstract decimal  FarePerSeat     { get; }

        // ── Concrete index helpers (no duplication needed in subclasses) ──────

        public string GetSource(int index)
            => (index >= 1 && index <= Sources.Length) ? Sources[index - 1] : "Unknown";

        public string GetDestination(int index)
            => (index >= 1 && index <= Destinations.Length) ? Destinations[index - 1] : "Unknown";

        // ── Shared schedule display (Polymorphism — uses abstract properties) ─
        public void DisplaySchedule(string airlineName)
            => ConsoleHelper.PrintScheduleTable(airlineName, DepartureTimes, ArrivalTimes, FarePerSeat);
    }

    // =========================================================================
    // ✦ INHERITANCE + POLYMORPHISM: LocalFlightService extends FlightServiceBase
    // =========================================================================

    /// <summary>
    /// Domestic flight service covering Pakistani cities.
    /// Overrides abstract members with local-specific data.
    /// </summary>
    public sealed class LocalFlightService : FlightServiceBase
    {
        public override string FlightType => "LOCAL";

        public override string[] Sources => Constants.LocalCities;

        public override string[] Destinations => Constants.LocalCities;

        public override string[] DepartureTimes => new[] { "08:00", "14:00", "19:00" };

        public override string[] ArrivalTimes   => new[] { "11:05", "17:05", "22:05" };

        public override decimal FarePerSeat => Constants.LocalFarePerSeat;
    }

    // =========================================================================
    // ✦ INHERITANCE + POLYMORPHISM: GlobalFlightService extends FlightServiceBase
    // =========================================================================

    /// <summary>
    /// International flight service covering global destinations.
    /// Same interface as LocalFlightService but different data (Polymorphism).
    /// </summary>
    public sealed class GlobalFlightService : FlightServiceBase
    {
        public override string FlightType => "GLOBAL";

        public override string[] Sources => Constants.GlobalCountries;

        public override string[] Destinations => Constants.GlobalCountries;

        public override string[] DepartureTimes => new[] { "07:00", "18:00", "22:00" };

        public override string[] ArrivalTimes   => new[] { "11:05", "22:05", "02:05" };

        public override decimal FarePerSeat => Constants.GlobalFarePerSeat;
    }
}
