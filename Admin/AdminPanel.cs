// =============================================================================
// File: Admin/AdminPanel.cs
// Purpose: Password-protected admin operations — view, search, delete, clear
// =============================================================================

using AirlineReservationSystem.Data;
using AirlineReservationSystem.Helpers;
using AirlineReservationSystem.Models;

namespace AirlineReservationSystem.Admin
{
    /// <summary>
    /// Encapsulates all admin functionality behind a login prompt.
    /// The password check is the single entry point — internal methods
    /// are private so callers cannot bypass authentication.
    ///
    /// ✦ ENCAPSULATION: Private methods hidden from external code.
    /// </summary>
    public class AdminPanel
    {
        private readonly FileRepository _repo;

        public AdminPanel(FileRepository repo)
        {
            _repo = repo;
        }

        // ── Public entry point ────────────────────────────────────────────────

        public void Run()
        {
            ConsoleHelper.PrintHeader("Administrator Login");

            if (!Authenticate())
            {
                ConsoleHelper.WriteError("Authentication failed. Returning to main menu.");
                ConsoleHelper.Pause();
                return;
            }

            ConsoleHelper.WriteSuccess("Login successful.");
            RunAdminLoop();
        }

        // ── Private: Authentication ───────────────────────────────────────────

        private static bool Authenticate()
        {
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                // Mask password input
                ConsoleHelper.WriteColored($"  Password (attempt {attempt}/3): ", Constants.AccentColor, newLine: false);
                string pass = ReadMaskedInput();

                if (Validator.IsValidAdminPassword(pass))
                    return true;

                ConsoleHelper.WriteError("Incorrect password.");
            }
            return false;
        }

        private static string ReadMaskedInput()
        {
            var sb = new System.Text.StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter) { Console.WriteLine(); break; }
                if (key.Key == ConsoleKey.Backspace && sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    sb.Append(key.KeyChar);
                    Console.Write('*');
                }
            }
            return sb.ToString();
        }

        // ── Private: Admin loop ───────────────────────────────────────────────

        private void RunAdminLoop()
        {
            bool running = true;
            while (running)
            {
                ConsoleHelper.PrintHeader("Admin Panel");
                ConsoleHelper.WriteInfo("  1. View All Records");
                ConsoleHelper.WriteInfo("  2. Search by CNIC");
                ConsoleHelper.WriteInfo("  3. Delete Record by CNIC");
                ConsoleHelper.WriteInfo("  4. Delete Record by Booking ID");
                ConsoleHelper.WriteInfo("  5. Clear Entire Database");
                ConsoleHelper.WriteInfo("  6. Log Out");
                Console.WriteLine();

                int choice = ConsoleHelper.ReadInt("Select option", 1, 6);

                switch (choice)
                {
                    case 1: ViewAll();           break;
                    case 2: SearchByCnic();      break;
                    case 3: DeleteByCnic();      break;
                    case 4: DeleteById();        break;
                    case 5: ClearDatabase();     break;
                    case 6: running = false;     break;
                }
            }
        }

        // ── Private: Operations ───────────────────────────────────────────────

        private void ViewAll()
        {
            ConsoleHelper.PrintHeader("All Booking Records");
            var bookings = _repo.LoadAll();

            if (!bookings.Any())
            {
                ConsoleHelper.WriteWarning("No records found.");
                ConsoleHelper.Pause();
                return;
            }

            foreach (var b in bookings)
                PrintBookingSummary(b);

            ConsoleHelper.WriteSuccess($"Total records: {bookings.Count}");
            ConsoleHelper.Pause();
        }

        private void SearchByCnic()
        {
            ConsoleHelper.PrintHeader("Search by CNIC");
            string cnic = ReadValidCnic();
            var results = _repo.FindByCnic(cnic);

            if (!results.Any())
            {
                ConsoleHelper.WriteWarning($"No records found for CNIC: {cnic}");
            }
            else
            {
                ConsoleHelper.WriteSuccess($"{results.Count} record(s) found:");
                foreach (var b in results)
                    ConsoleHelper.PrintTicketBox(b.ToDisplayFields());
            }

            ConsoleHelper.Pause();
        }

        private void DeleteByCnic()
        {
            ConsoleHelper.PrintHeader("Delete Record by CNIC");
            string cnic = ReadValidCnic();

            var found = _repo.FindByCnic(cnic);
            if (!found.Any())
            {
                ConsoleHelper.WriteWarning("No record found.");
                ConsoleHelper.Pause();
                return;
            }

            ConsoleHelper.WriteWarning($"This will delete {found.Count} booking(s) for CNIC {cnic}.");
            if (!ConsoleHelper.AskYesNo("Are you sure?"))
            {
                ConsoleHelper.WriteInfo("Operation cancelled.");
                ConsoleHelper.Pause();
                return;
            }

            bool deleted = _repo.DeleteByCnic(cnic);
            if (deleted)
                ConsoleHelper.WriteSuccess("Record(s) deleted successfully.");
            else
                ConsoleHelper.WriteError("Deletion failed.");

            ConsoleHelper.Pause();
        }

        private void DeleteById()
        {
            ConsoleHelper.PrintHeader("Delete Record by Booking ID");
            string id = ConsoleHelper.ReadLine("Enter Booking ID").ToUpper();

            var record = _repo.FindById(id);
            if (record == null)
            {
                ConsoleHelper.WriteWarning("No booking found with that ID.");
                ConsoleHelper.Pause();
                return;
            }

            PrintBookingSummary(record);
            if (!ConsoleHelper.AskYesNo("Delete this booking?"))
            {
                ConsoleHelper.WriteInfo("Cancelled.");
                ConsoleHelper.Pause();
                return;
            }

            _repo.DeleteById(id);
            ConsoleHelper.WriteSuccess("Booking deleted.");
            ConsoleHelper.Pause();
        }

        private void ClearDatabase()
        {
            ConsoleHelper.PrintHeader("Clear Entire Database");
            ConsoleHelper.WriteWarning("WARNING: This will permanently delete ALL booking records!");

            if (!ConsoleHelper.AskYesNo("Type Y to confirm"))
            {
                ConsoleHelper.WriteInfo("Cancelled.");
                ConsoleHelper.Pause();
                return;
            }

            _repo.ClearAll();
            ConsoleHelper.WriteSuccess("Database cleared.");
            ConsoleHelper.Pause();
        }

        // ── Private: display helper ───────────────────────────────────────────

        private static void PrintBookingSummary(Booking b)
        {
            ConsoleHelper.PrintDivider('-');
            ConsoleHelper.PrintTicketRow("Booking ID",   b.BookingId);
            ConsoleHelper.PrintTicketRow("Passenger",    b.Passenger.FullName);
            ConsoleHelper.PrintTicketRow("CNIC",         b.Passenger.Cnic);
            ConsoleHelper.PrintTicketRow("Flight",       b.FlightSummary);
            ConsoleHelper.PrintTicketRow("Seats",        b.Passenger.SeatCount.ToString());
            ConsoleHelper.PrintTicketRow("Total Fare",   $"PKR {b.TotalFare:N0}");
            ConsoleHelper.PrintTicketRow("Booked On",    b.BookingDateTime.ToString("dd-MM-yyyy HH:mm"));
        }

        private static string ReadValidCnic()
        {
            while (true)
            {
                string cnic = ConsoleHelper.ReadLine("CNIC (XXXXX-XXXXXXX-X)");
                if (Validator.IsValidCnic(cnic, out string err)) return cnic;
                ConsoleHelper.WriteError(err);
            }
        }
    }
}
