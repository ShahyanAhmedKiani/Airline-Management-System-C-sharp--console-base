// =============================================================================
// File: Models/Person.cs
// OOP: ENCAPSULATION — private backing fields exposed through properties
//      INHERITANCE   — acts as the base class for Passenger
// =============================================================================

namespace AirlineReservationSystem.Models
{
    /// <summary>
    /// Base class representing a generic person.
    /// 
    /// ✦ ENCAPSULATION: All fields are private; access is controlled through
    ///   public properties with validation in setters.
    ///
    /// ✦ INHERITANCE: Passenger inherits from this class and extends it with
    ///   flight-specific information.
    /// </summary>
    public abstract class Person
    {
        // ── Private backing fields (Encapsulation) ────────────────────────────
        private string _firstName  = string.Empty;
        private string _lastName   = string.Empty;
        private int    _age;
        private char   _gender;
        private string _email      = string.Empty;
        private string _contact    = string.Empty;

        // ── Public properties with guarded setters ────────────────────────────

        public string FirstName
        {
            get => _firstName;
            set => _firstName = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("First name cannot be empty.")
                : value.Trim();
        }

        public string LastName
        {
            get => _lastName;
            set => _lastName = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Last name cannot be empty.")
                : value.Trim();
        }

        public int Age
        {
            get => _age;
            set => _age = (value < 1 || value > 120)
                ? throw new ArgumentOutOfRangeException(nameof(Age), "Age must be between 1 and 120.")
                : value;
        }

        public char Gender
        {
            get => _gender;
            set
            {
                char upper = char.ToUpper(value);
                _gender = (upper != 'M' && upper != 'F')
                    ? throw new ArgumentException("Gender must be 'M' or 'F'.")
                    : upper;
            }
        }

        public string Email
        {
            get => _email;
            set => _email = value.Trim();
        }

        public string Contact
        {
            get => _contact;
            set => _contact = value.Trim();
        }

        // ── Computed property ─────────────────────────────────────────────────
        public string FullName => $"{FirstName} {LastName}";

        // ── Constructor ───────────────────────────────────────────────────────
        protected Person() { }

        protected Person(string firstName, string lastName, int age, char gender, string email, string contact)
        {
            FirstName = firstName;
            LastName  = lastName;
            Age       = age;
            Gender    = gender;
            Email     = email;
            Contact   = contact;
        }

        // ── Virtual method (Polymorphism hook) ────────────────────────────────

        /// <summary>
        /// Returns a formatted summary of this person.
        /// Derived classes can override this (Polymorphism).
        /// </summary>
        public virtual string GetSummary()
            => $"Name: {FullName} | Age: {Age} | Gender: {Gender}";

        public override string ToString() => FullName;
    }
}
