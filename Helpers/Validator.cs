// =============================================================================
// File: Helpers/Validator.cs
// Purpose: Input validation helpers — dates, CNICs, passwords, etc.
// =============================================================================

namespace AirlineReservationSystem.Helpers
{
    /// <summary>
    /// Static class providing validation methods.
    /// Separating validation from models keeps each class focused on one job.
    /// </summary>
    public static class Validator
    {
        // ── Date ─────────────────────────────────────────────────────────────

        /// <summary>Validates DD-MM-YYYY format and logical date range.</summary>
        public static bool IsValidDate(string date, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(date) || date.Length < 10)
            {
                error = "Date must be in DD-MM-YYYY format.";
                return false;
            }

            if (date[2] != '-' || date[5] != '-')
            {
                error = "Date format must be DD-MM-YYYY (e.g. 25-06-2025).";
                return false;
            }

            if (!int.TryParse(date[..2],  out int day)   ||
                !int.TryParse(date[3..5], out int month) ||
                !int.TryParse(date[6..],  out int year))
            {
                error = "Date contains non-numeric characters.";
                return false;
            }

            if (month < 1 || month > 12) { error = "Month must be 01–12.";    return false; }
            if (day   < 1 || day   > 31) { error = "Day must be 01–31.";      return false; }
            if (year  < DateTime.Today.Year)
            {
                error = "Journey date cannot be in the past.";
                return false;
            }

            return true;
        }

        // ── CNIC ─────────────────────────────────────────────────────────────

        /// <summary>Validates Pakistani CNIC in format XXXXX-XXXXXXX-X.</summary>
        public static bool IsValidCnic(string cnic, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(cnic) || cnic.Length != 15)
            {
                error = "CNIC must be exactly 15 characters (XXXXX-XXXXXXX-X).";
                return false;
            }

            if (cnic[5] != '-' || cnic[13] != '-')
            {
                error = "CNIC format must be XXXXX-XXXXXXX-X.";
                return false;
            }

            // Check digit-only segments
            string part1 = cnic[0..5];
            string part2 = cnic[6..13];
            string part3 = cnic[14..15];

            if (!part1.All(char.IsDigit) || !part2.All(char.IsDigit) || !part3.All(char.IsDigit))
            {
                error = "CNIC must contain digits only (except dashes).";
                return false;
            }

            return true;
        }

        // ── Password ─────────────────────────────────────────────────────────

        public static bool IsValidAdminPassword(string password)
            => password == Constants.AdminPassword;

        // ── Card helpers ─────────────────────────────────────────────────────

        public static bool IsValidCardNumber(string card, out string error)
        {
            error = string.Empty;
            var digits = card.Replace(" ", "").Replace("-", "");
            if (digits.Length < 13 || digits.Length > 19 || !digits.All(char.IsDigit))
            {
                error = "Card number must be 13–19 digits.";
                return false;
            }
            return true;
        }

        public static bool IsValidCvv(string cvv, out string error)
        {
            error = string.Empty;
            if (cvv.Length < 3 || cvv.Length > 4 || !cvv.All(char.IsDigit))
            {
                error = "CVV must be 3 or 4 digits.";
                return false;
            }
            return true;
        }

        public static bool IsValidEmail(string email, out string error)
        {
            error = string.Empty;
            if (!email.Contains('@') || !email.Contains('.'))
            {
                error = "Email must contain '@' and '.'";
                return false;
            }
            return true;
        }

        public static bool IsValidContact(string contact, out string error)
        {
            error = string.Empty;
            var digits = contact.Replace("-", "").Replace("+", "").Replace(" ", "");
            if (digits.Length < 10 || !digits.All(char.IsDigit))
            {
                error = "Contact must be at least 10 digits.";
                return false;
            }
            return true;
        }
    }
}
