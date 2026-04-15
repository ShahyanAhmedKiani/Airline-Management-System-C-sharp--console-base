// =============================================================================
// File: Models/Booking.cs
// Purpose: Represents a complete flight booking record.
//          Encapsulates all booking data in one cohesive object.
// =============================================================================

namespace AirlineReservationSystem.Models
{
    /// <summary>
    /// A complete booking record associating a Passenger with flight details.
    ///
    /// ✦ ENCAPSULATION: All fields private; exposed through read-only properties
    ///   or init-only setters to prevent accidental mutation after booking.
    /// </summary>
    public class Booking
    {
        // ── Properties ────────────────────────────────────────────────────────

        public string    BookingId       { get; init; } = Guid.NewGuid().ToString()[..8].ToUpper();
        public Passenger Passenger       { get; init; } = null!;
        public string    Airline         { get; init; } = string.Empty;
        public string    Source          { get; init; } = string.Empty;
        public string    Destination     { get; init; } = string.Empty;
        public string    DateOfJourney   { get; init; } = string.Empty;
        public string    DepartureTime   { get; init; } = string.Empty;
        public string    ArrivalTime     { get; init; } = string.Empty;
        public string    FlightType      { get; init; } = string.Empty;  // LOCAL / GLOBAL
        public decimal   TotalFare       { get; init; }
        public DateTime  BookingDateTime { get; init; } = DateTime.Now;

        // ── Computed ──────────────────────────────────────────────────────────

        public string FlightSummary
            => $"{Airline} | {Source} → {Destination} | {DateOfJourney} | Dep: {DepartureTime}";

        // ── Serialisation helpers (for flat-file storage) ─────────────────────

        /// <summary>Converts booking to a pipe-delimited line for text file storage.</summary>
        public string Serialize()
        {
            return string.Join("|",
                BookingId,
                Passenger.PassportNumber,
                Airline,
                Passenger.FirstName,
                Passenger.LastName,
                Passenger.Cnic,
                Passenger.Contact,
                Passenger.Email,
                Passenger.Age,
                Passenger.Gender,
                DateOfJourney,
                Source,
                Destination,
                Passenger.SeatCount,
                DepartureTime,
                ArrivalTime,
                FlightType,
                TotalFare,
                BookingDateTime.ToString("dd-MM-yyyy HH:mm")
            );
        }

        /// <summary>Re-creates a Booking from a stored serialised line.</summary>
        public static Booking? Deserialize(string line)
        {
            try
            {
                var p = line.Split('|');
                if (p.Length < 19) return null;

                var passenger = new Passenger
                {
                    PassportNumber = p[1],
                    FirstName      = p[3],
                    LastName       = p[4],
                    Cnic           = p[5],
                    Contact        = p[6],
                    Email          = p[7],
                    Age            = int.Parse(p[8]),
                    Gender         = p[9][0],
                    SeatCount      = int.Parse(p[13])
                };

                return new Booking
                {
                    BookingId     = p[0],
                    Passenger     = passenger,
                    Airline       = p[2],
                    DateOfJourney = p[10],
                    Source        = p[11],
                    Destination   = p[12],
                    DepartureTime = p[14],
                    ArrivalTime   = p[15],
                    FlightType    = p[16],
                    TotalFare     = decimal.Parse(p[17]),
                    BookingDateTime = DateTime.Parse(p[18])
                };
            }
            catch
            {
                return null;  // Corrupted record — skip gracefully
            }
        }

        // ── Display dictionary for ConsoleHelper.PrintTicketBox ───────────────

        public Dictionary<string, string> ToDisplayFields()
            => new()
            {
                ["Booking ID"]       = BookingId,
                ["Passport / PNR"]   = Passenger.PassportNumber,
                ["Airline"]          = Airline,
                ["Passenger Name"]   = Passenger.FullName,
                ["CNIC"]             = Passenger.Cnic,
                ["Contact"]          = Passenger.Contact,
                ["Email"]            = Passenger.Email,
                ["Date of Journey"]  = DateOfJourney,
                ["Source"]           = Source,
                ["Destination"]      = Destination,
                ["Seats"]            = Passenger.SeatCount.ToString(),
                ["Departure"]        = DepartureTime,
                ["Arrival"]          = ArrivalTime,
                ["Flight Type"]      = FlightType,
                ["Total Fare"]       = $"PKR {TotalFare:N0}",
                ["Booked On"]        = BookingDateTime.ToString("dd-MM-yyyy HH:mm"),
                ["Baggage Allowance"]= $"12 kg per person",
            };
    }
}
