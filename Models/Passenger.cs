// =============================================================================
// File: Models/Passenger.cs
// OOP: INHERITANCE   — extends Person with passenger-specific data
//      ENCAPSULATION — private CNIC with format-validated setter
//      POLYMORPHISM  — overrides GetSummary() from Person
// =============================================================================

namespace AirlineReservationSystem.Models
{
    /// <summary>
    /// Represents an airline passenger.
    ///
    /// ✦ INHERITANCE: Extends <see cref="Person"/> — reuses FirstName, LastName,
    ///   Age, Gender, Email, Contact without rewriting them.
    ///
    /// ✦ ENCAPSULATION: Adds a private _cnic field with a guarded setter.
    ///
    /// ✦ POLYMORPHISM: Overrides GetSummary() to include flight-specific info.
    /// </summary>
    public class Passenger : Person
    {
        // ── Private backing field (Encapsulation) ─────────────────────────────
        private string _cnic       = string.Empty;
        private int    _seatCount;

        // ── Properties ────────────────────────────────────────────────────────

        /// <summary>Passport or PNR reference number.</summary>
        public string PassportNumber { get; set; } = string.Empty;

        /// <summary>
        /// Pakistani CNIC (XXXXX-XXXXXXX-X).
        /// Setter validates format — internal data stays consistent (Encapsulation).
        /// </summary>
        public string Cnic
        {
            get => _cnic;
            set
            {
                if (value.Length != 15 || value[5] != '-' || value[13] != '-')
                    throw new ArgumentException("CNIC must be in format XXXXX-XXXXXXX-X.");
                _cnic = value.Trim();
            }
        }

        /// <summary>Number of seats requested (min 1, max 9).</summary>
        public int SeatCount
        {
            get => _seatCount;
            set => _seatCount = (value < 1 || value > 9)
                ? throw new ArgumentOutOfRangeException(nameof(SeatCount), "Seat count must be 1–9.")
                : value;
        }

        // ── Constructors ──────────────────────────────────────────────────────

        public Passenger() { }

        public Passenger(
            string passportNumber,
            string firstName,
            string lastName,
            int    age,
            char   gender,
            string cnic,
            string email,
            string contact,
            int    seatCount)
            : base(firstName, lastName, age, gender, email, contact)   // calls Person constructor
        {
            PassportNumber = passportNumber;
            Cnic           = cnic;
            SeatCount      = seatCount;
        }

        // ── Polymorphism: override base method ────────────────────────────────

        /// <summary>
        /// Overrides Person.GetSummary() to add passenger-specific details.
        /// This is POLYMORPHISM — the same method name, different behaviour.
        /// </summary>
        public override string GetSummary()
            => $"{base.GetSummary()} | CNIC: {Cnic} | Passport: {PassportNumber} | Seats: {SeatCount}";
    }
}
