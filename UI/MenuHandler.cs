// =============================================================================
// File: UI/MenuHandler.cs
// Purpose: Drives the main menu and sub-menu navigation.
//          Wires together services — no business logic of its own.
// =============================================================================

using AirlineReservationSystem.Admin;
using AirlineReservationSystem.Data;
using AirlineReservationSystem.Helpers;
using AirlineReservationSystem.Services;

namespace AirlineReservationSystem.UI
{
    /// <summary>
    /// Top-level navigation controller.
    /// Instantiates services once and routes user choices to them.
    ///
    /// ✦ POLYMORPHISM: Passes either LocalFlightService or GlobalFlightService
    ///   to BookingService.RunBookingWizard() — the method handles both the same way.
    /// </summary>
    public class MenuHandler
    {
        // ── Dependencies wired in constructor ─────────────────────────────────
        private readonly FileRepository  _repo;
        private readonly BookingService  _bookingService;
        private readonly AdminPanel      _adminPanel;

        // ✦ POLYMORPHISM: both services satisfy the IFlightService contract
        private readonly IFlightService  _localService;
        private readonly IFlightService  _globalService;

        public MenuHandler()
        {
            _repo           = new FileRepository(Constants.DataFilePath);
            _bookingService = new BookingService(_repo);
            _adminPanel     = new AdminPanel(_repo);
            _localService   = new LocalFlightService();   // IFlightService reference
            _globalService  = new GlobalFlightService();  // IFlightService reference
        }

        // ── Main application loop ─────────────────────────────────────────────

        public void Run()
        {
            ConsoleHelper.PrintSplashScreen();
            ConsoleHelper.Pause("Press any key to enter the system...");

            bool running = true;
            while (running)
            {
                ShowMainMenu();
                int choice = ConsoleHelper.ReadInt("Your choice", 1, 5);

                switch (choice)
                {
                    case 1: HandleBookFlight();      break;
                    case 2: HandleSearchFlight();    break;
                    case 3: HandleFlightSchedule();  break;
                    case 4: _adminPanel.Run();       break;
                    case 5: running = false;         break;
                }
            }

            ConsoleHelper.WriteColored("\n  Thank you for using our Airline Reservation System. Safe travels! ✈", Constants.SuccessColor);
            Thread.Sleep(1500);
        }

        // ── Menu displays ─────────────────────────────────────────────────────

        private static void ShowMainMenu()
        {
            ConsoleHelper.PrintHeader("Main Menu");
            ConsoleHelper.WriteInfo("  1.  Book a Flight");
            ConsoleHelper.WriteInfo("  2.  Search My Booking");
            ConsoleHelper.WriteInfo("  3.  View Flight Schedule");
            ConsoleHelper.WriteInfo("  4.  Administrator Panel");
            ConsoleHelper.WriteInfo("  5.  Exit");
            Console.WriteLine();
        }

        // ── Handlers ─────────────────────────────────────────────────────────

        private void HandleBookFlight()
        {
            ConsoleHelper.PrintHeader("Book a Flight");
            ConsoleHelper.WriteInfo("  1. Local (Domestic)");
            ConsoleHelper.WriteInfo("  2. Global (International)");
            Console.WriteLine();
            int type = ConsoleHelper.ReadInt("Flight type", 1, 2);

            // ✦ POLYMORPHISM: same method; different service object
            IFlightService service = type == 1 ? _localService : _globalService;
            _bookingService.RunBookingWizard(service);
        }

        private void HandleSearchFlight()
        {
            ConsoleHelper.PrintHeader("Search Booking by CNIC");

            string cnic;
            while (true)
            {
                cnic = ConsoleHelper.ReadLine("Enter CNIC (XXXXX-XXXXXXX-X)");
                if (Validator.IsValidCnic(cnic, out string err)) break;
                ConsoleHelper.WriteError(err);
            }

            var results = _repo.FindByCnic(cnic);
            if (!results.Any())
            {
                ConsoleHelper.WriteWarning("No bookings found for this CNIC.");
            }
            else
            {
                ConsoleHelper.WriteSuccess($"Found {results.Count} booking(s):");
                foreach (var b in results)
                    ConsoleHelper.PrintTicketBox(b.ToDisplayFields());
            }

            ConsoleHelper.Pause();
        }

        private void HandleFlightSchedule()
        {
            ConsoleHelper.PrintHeader("Flight Schedule");
            ConsoleHelper.WriteInfo("  1. Local Schedule");
            ConsoleHelper.WriteInfo("  2. Global Schedule");
            Console.WriteLine();
            int type = ConsoleHelper.ReadInt("Select", 1, 2);

            // ✦ POLYMORPHISM: selecting airline then calling DisplaySchedule on either service
            ConsoleHelper.WriteTitle("\nSelect Airline:");
            for (int i = 0; i < Constants.Airlines.Length; i++)
                ConsoleHelper.WriteInfo($"  {i + 1}. {Constants.Airlines[i]}");

            int airlineChoice = ConsoleHelper.ReadInt("Choice", 1, Constants.Airlines.Length);
            string airline    = Constants.Airlines[airlineChoice - 1];

            IFlightService service = type == 1 ? _localService : _globalService;
            service.DisplaySchedule(airline);  // ← Polymorphism: same call, different data

            ConsoleHelper.Pause();
        }
    }
}
