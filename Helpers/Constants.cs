// =============================================================================
// File: Helpers/Constants.cs
// Purpose: Application-wide constants — single source of truth
// =============================================================================

namespace AirlineReservationSystem.Helpers
{
    /// <summary>
    /// Centralised constants used across the application.
    /// Avoids magic strings and magic numbers scattered in code.
    /// </summary>
    public static class Constants
    {
        // ── Admin ────────────────────────────────────────────────────────────
        public const string AdminPassword      = "Admin@1234";
        public const string DataFilePath       = "airlinereservation.txt";

        // ── Fare ─────────────────────────────────────────────────────────────
        public const decimal LocalFarePerSeat  = 6000m;
        public const decimal GlobalFarePerSeat = 45000m;
        public const int     MaxBaggageKg      = 12;

        // ── Airlines ─────────────────────────────────────────────────────────
        public static readonly string[] Airlines = { "PIA", "AIRBLUE", "SERENE AIR" };

        // ── Local Cities ─────────────────────────────────────────────────────
        public static readonly string[] LocalCities =
        {
            "Islamabad", "Karachi", "Peshawar", "Quetta", "Skardu", "Multan"
        };

        // ── Global Countries ─────────────────────────────────────────────────
        public static readonly string[] GlobalCountries =
        {
            "Pakistan", "Italy", "Germany", "Dubai", "France", "Oman"
        };

        // ── Console Colours ──────────────────────────────────────────────────
        public const ConsoleColor PrimaryColor   = ConsoleColor.Cyan;
        public const ConsoleColor AccentColor    = ConsoleColor.Yellow;
        public const ConsoleColor ErrorColor     = ConsoleColor.Red;
        public const ConsoleColor SuccessColor   = ConsoleColor.Green;
        public const ConsoleColor NeutralColor   = ConsoleColor.White;
    }
}
