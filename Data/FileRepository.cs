// =============================================================================
// File: Data/FileRepository.cs
// Purpose: All file I/O in one place — read, write, delete, clear bookings
//          This is the DATA ACCESS LAYER; no business logic lives here.
// =============================================================================

using AirlineReservationSystem.Models;

namespace AirlineReservationSystem.Data
{
    /// <summary>
    /// Handles persistence of Booking records to a flat text file.
    /// Using a repository pattern keeps file I/O isolated from business logic.
    /// </summary>
    public class FileRepository
    {
        private readonly string _filePath;

        public FileRepository(string filePath)
        {
            _filePath = filePath;
        }

        // ── Read ─────────────────────────────────────────────────────────────

        /// <summary>Loads all bookings from the file. Returns empty list on missing file.</summary>
        public List<Booking> LoadAll()
        {
            var bookings = new List<Booking>();

            if (!File.Exists(_filePath))
                return bookings;

            foreach (var line in File.ReadAllLines(_filePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var booking = Booking.Deserialize(line);
                if (booking != null)
                    bookings.Add(booking);
            }

            return bookings;
        }

        /// <summary>Returns all bookings matching the given CNIC.</summary>
        public List<Booking> FindByCnic(string cnic)
            => LoadAll().Where(b => b.Passenger.Cnic.Equals(cnic, StringComparison.OrdinalIgnoreCase)).ToList();

        /// <summary>Returns a single booking by its ID.</summary>
        public Booking? FindById(string bookingId)
            => LoadAll().FirstOrDefault(b => b.BookingId.Equals(bookingId, StringComparison.OrdinalIgnoreCase));

        // ── Write ─────────────────────────────────────────────────────────────

        /// <summary>Appends one booking record to the file.</summary>
        public void Save(Booking booking)
        {
            File.AppendAllText(_filePath, booking.Serialize() + Environment.NewLine);
        }

        // ── Delete ────────────────────────────────────────────────────────────

        /// <summary>Removes all bookings with the matching CNIC.</summary>
        public bool DeleteByCnic(string cnic)
        {
            var all = LoadAll();
            var remaining = all.Where(b => !b.Passenger.Cnic.Equals(cnic, StringComparison.OrdinalIgnoreCase)).ToList();

            if (remaining.Count == all.Count)
                return false;  // Nothing deleted

            RewriteAll(remaining);
            return true;
        }

        /// <summary>Removes a booking by its booking ID.</summary>
        public bool DeleteById(string bookingId)
        {
            var all = LoadAll();
            var remaining = all.Where(b => !b.BookingId.Equals(bookingId, StringComparison.OrdinalIgnoreCase)).ToList();

            if (remaining.Count == all.Count)
                return false;

            RewriteAll(remaining);
            return true;
        }

        // ── Clear ─────────────────────────────────────────────────────────────

        /// <summary>Deletes all records permanently.</summary>
        public void ClearAll()
        {
            File.WriteAllText(_filePath, string.Empty);
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private void RewriteAll(List<Booking> bookings)
        {
            var lines = bookings.Select(b => b.Serialize());
            File.WriteAllLines(_filePath, lines);
        }

        public bool HasRecords() => LoadAll().Any();
    }
}
