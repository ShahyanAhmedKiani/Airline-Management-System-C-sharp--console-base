// =============================================================================
// File: Services/BookingService.cs
// Purpose: Orchestrates the full booking wizard — collects input, validates,
//          builds models, persists to file.
//          Uses IFlightService via POLYMORPHISM — works for both LOCAL & GLOBAL.
// =============================================================================

using AirlineReservationSystem.Data;
using AirlineReservationSystem.Helpers;
using AirlineReservationSystem.Models;

namespace AirlineReservationSystem.Services
{
    /// <summary>
    /// Core booking business logic.
    ///
    /// ✦ POLYMORPHISM: Accepts an IFlightService — the same RunBookingWizard()
    ///   method works for both local and international flights without any if/else
    ///   branching on flight type. This is runtime polymorphism.
    /// </summary>
    public class BookingService
    {
        private readonly FileRepository _repo;

        public BookingService(FileRepository repo)
        {
            _repo = repo;
        }

        // ── Main wizard ───────────────────────────────────────────────────────

        /// <summary>
        /// Full booking wizard.
        /// Takes any IFlightService — LOCAL or GLOBAL — identically.
        /// This is POLYMORPHISM in action: the method doesn't care which type
        /// of flight service it receives; it calls the same interface methods.
        /// </summary>
        public void RunBookingWizard(IFlightService flightService)
        {
            ConsoleHelper.PrintHeader($"Book Flight — {flightService.FlightType}");

            // ── Step 1: Airline ───────────────────────────────────────────────
            string airline = SelectAirline();

            // ── Step 2: Date of Journey ───────────────────────────────────────
            string dateOfJourney = CollectDate("Enter Date of Journey (DD-MM-YYYY)");

            // ── Step 3: Source & Destination (Polymorphism: uses interface) ───
            (int srcIdx, int desIdx) = SelectRoutes(flightService);
            string source      = flightService.GetSource(srcIdx);       // ← Polymorphism
            string destination = flightService.GetDestination(desIdx);  // ← Polymorphism

            // ── Step 4: Flight slot ───────────────────────────────────────────
            flightService.DisplaySchedule(airline);                      // ← Polymorphism
            int slotIdx = ConsoleHelper.ReadInt("Select flight slot (1–3)", 1, 3) - 1;
            string departure = flightService.DepartureTimes[slotIdx];   // ← Polymorphism
            string arrival   = flightService.ArrivalTimes[slotIdx];     // ← Polymorphism

            // ── Step 5: Passenger details ─────────────────────────────────────
            ConsoleHelper.PrintHeader("Passenger Details");
            Passenger passenger = CollectPassengerDetails();

            // ── Step 6: Payment ───────────────────────────────────────────────
            ConsoleHelper.PrintHeader("Payment");
            Payment payment = CollectPayment(passenger.SeatCount * flightService.FarePerSeat);

            // ── Step 7: Build & save booking ──────────────────────────────────
            var booking = new Booking
            {
                Passenger     = passenger,
                Airline       = airline,
                Source        = source,
                Destination   = destination,
                DateOfJourney = dateOfJourney,
                DepartureTime = departure,
                ArrivalTime   = arrival,
                FlightType    = flightService.FlightType,
                TotalFare     = passenger.SeatCount * flightService.FarePerSeat
            };

            _repo.Save(booking);

            // ── Step 8: Print ticket ──────────────────────────────────────────
            ConsoleHelper.PrintTicketBox(booking.ToDisplayFields(), "✈  YOUR TICKET");
            ConsoleHelper.WriteSuccess($"Payment: {payment.GetPaymentSummary()}");
            ConsoleHelper.WriteWarning("Boarding pass will be issued after check-in.");
            ConsoleHelper.WriteWarning("Please keep your Booking ID and Passport for reference.");
            ConsoleHelper.Pause();
        }

        // ── Private helper methods ────────────────────────────────────────────

        private static string SelectAirline()
        {
            ConsoleHelper.WriteTitle("Select Airline:");
            for (int i = 0; i < Constants.Airlines.Length; i++)
                ConsoleHelper.WriteInfo($"  {i + 1}. {Constants.Airlines[i]}");

            int choice = ConsoleHelper.ReadInt("Choice", 1, Constants.Airlines.Length);
            return Constants.Airlines[choice - 1];
        }

        private static string CollectDate(string prompt)
        {
            while (true)
            {
                string date = ConsoleHelper.ReadLine(prompt);
                if (Validator.IsValidDate(date, out string error))
                    return date;
                ConsoleHelper.WriteError(error);
            }
        }

        private static (int src, int des) SelectRoutes(IFlightService service)
        {
            Console.WriteLine();
            ConsoleHelper.WriteTitle("Available Locations:");
            for (int i = 0; i < service.Sources.Length; i++)
                ConsoleHelper.WriteInfo($"  {i + 1}. {service.Sources[i]}");

            while (true)
            {
                int src = ConsoleHelper.ReadInt("Select Source", 1, service.Sources.Length);
                int des = ConsoleHelper.ReadInt("Select Destination", 1, service.Destinations.Length);

                if (src == des)
                {
                    ConsoleHelper.WriteError("Source and destination cannot be the same.");
                    continue;
                }
                return (src, des);
            }
        }

        private static Passenger CollectPassengerDetails()
        {
            string passport = ConsoleHelper.ReadLine("Passport / PNR Number");
            string firstName = ConsoleHelper.ReadLine("First Name");
            string lastName  = ConsoleHelper.ReadLine("Last Name");

            int age = ConsoleHelper.ReadInt("Age", 1, 120);

            char gender;
            while (true)
            {
                string g = ConsoleHelper.ReadLine("Gender (M/F)").ToUpper();
                if (g == "M" || g == "F") { gender = g[0]; break; }
                ConsoleHelper.WriteError("Please enter M or F.");
            }

            string cnic;
            while (true)
            {
                cnic = ConsoleHelper.ReadLine("CNIC (XXXXX-XXXXXXX-X)");
                if (Validator.IsValidCnic(cnic, out string err)) break;
                ConsoleHelper.WriteError(err);
            }

            string email;
            while (true)
            {
                email = ConsoleHelper.ReadLine("Email Address");
                if (Validator.IsValidEmail(email, out string err)) break;
                ConsoleHelper.WriteError(err);
            }

            string contact;
            while (true)
            {
                contact = ConsoleHelper.ReadLine("Contact Number");
                if (Validator.IsValidContact(contact, out string err)) break;
                ConsoleHelper.WriteError(err);
            }

            int seats = ConsoleHelper.ReadInt("Number of Seats (1–9)", 1, 9);

            return new Passenger(passport, firstName, lastName, age, gender, cnic, email, contact, seats);
        }

        private static Payment CollectPayment(decimal amount)
        {
            ConsoleHelper.WriteInfo($"  Total Fare : PKR {amount:N0}");
            ConsoleHelper.WriteTitle("\nPayment Method:");
            ConsoleHelper.WriteInfo("  1. Credit Card");
            ConsoleHelper.WriteInfo("  2. Debit Card");
            int choice = ConsoleHelper.ReadInt("Choice", 1, 2);

            // ✦ POLYMORPHISM: CreditPayment and DebitPayment are both Payment objects.
            //   CollectCardDetails accepts the abstract Payment type.
            Payment payment = choice == 1 ? new CreditPayment() : new DebitPayment();
            payment.Amount  = amount;

            CollectCardDetails(payment);
            ConsoleHelper.WriteSuccess("Transaction Successful!");
            return payment;
        }

        private static void CollectCardDetails(Payment pay)
        {
            pay.BankName             = ConsoleHelper.ReadLine("Bank Name");
            pay.CardHolderFirstName  = ConsoleHelper.ReadLine("Card Holder First Name");
            pay.CardHolderLastName   = ConsoleHelper.ReadLine("Card Holder Last Name");

            string card;
            while (true)
            {
                card = ConsoleHelper.ReadLine("Card Number");
                if (Validator.IsValidCardNumber(card, out string err)) break;
                ConsoleHelper.WriteError(err);
            }
            pay.CardNumber = card;

            pay.ExpiryDate = CollectDate("Expiry Date (MM-YYYY → enter as 01-MM-YYYY)");

            string pin = ConsoleHelper.ReadLine("4-digit PIN");
            pay.SetPin(pin);

            while (true)
            {
                string cvvStr = ConsoleHelper.ReadLine("CVV (3–4 digits)");
                if (Validator.IsValidCvv(cvvStr, out string err) && int.TryParse(cvvStr, out int cvv))
                { pay.Cvv = cvv; break; }
                ConsoleHelper.WriteError(err);
            }
        }
    }
}
