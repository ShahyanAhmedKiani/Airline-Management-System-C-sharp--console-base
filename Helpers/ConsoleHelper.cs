// =============================================================================
// File: Helpers/ConsoleHelper.cs
// Purpose: Reusable console UI utilities — colours, banners, tables, prompts
// =============================================================================

using AirlineReservationSystem.Helpers;

namespace AirlineReservationSystem.Helpers
{
    /// <summary>
    /// Static helper class that wraps Console calls with colour management
    /// and formatted output helpers (banners, dividers, tables, etc.).
    /// </summary>
    public static class ConsoleHelper
    {
        // ── Output helpers ────────────────────────────────────────────────────

        public static void WriteColored(string text, ConsoleColor color, bool newLine = true)
        {
            Console.ForegroundColor = color;
            if (newLine) Console.WriteLine(text);
            else         Console.Write(text);
            Console.ResetColor();
        }

        public static void WriteSuccess(string msg)  => WriteColored($"  ✓  {msg}", Constants.SuccessColor);
        public static void WriteError(string msg)    => WriteColored($"  ✗  {msg}", Constants.ErrorColor);
        public static void WriteWarning(string msg)  => WriteColored($"  !  {msg}", Constants.AccentColor);
        public static void WriteInfo(string msg)     => WriteColored($"     {msg}", Constants.NeutralColor);
        public static void WriteTitle(string msg)    => WriteColored(msg,           Constants.PrimaryColor);

        // ── Structural helpers ────────────────────────────────────────────────

        public static void PrintDivider(char ch = '─', int width = 60)
        {
            WriteColored(new string(ch, width), Constants.PrimaryColor);
        }

        public static void PrintHeader(string title)
        {
            Console.Clear();
            PrintDivider('═');
            WriteColored($"  ✈   {title.ToUpper()}", Constants.PrimaryColor);
            PrintDivider('═');
            Console.WriteLine();
        }

        public static void PrintSplashScreen()
        {
            Console.Clear();
            Console.ForegroundColor = Constants.PrimaryColor;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║                                                          ║");
            Console.WriteLine("  ║          ✈   AIRLINE RESERVATION SYSTEM   ✈             ║");
            Console.WriteLine("  ║                                                          ║");
            Console.WriteLine("  ║              Professional Edition — C#                  ║");
            Console.WriteLine("  ║                                                          ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        // ── Ticket box ────────────────────────────────────────────────────────

        public static void PrintTicketRow(string label, string value)
        {
            Console.ForegroundColor = Constants.AccentColor;
            Console.Write($"  {label,-26}: ");
            Console.ForegroundColor = Constants.NeutralColor;
            Console.WriteLine(value);
            Console.ResetColor();
        }

        public static void PrintTicketBox(Dictionary<string, string> fields, string boxTitle = "BOARDING TICKET")
        {
            Console.WriteLine();
            PrintDivider('─');
            WriteColored($"  {boxTitle}", Constants.PrimaryColor);
            PrintDivider('─');
            foreach (var kv in fields)
                PrintTicketRow(kv.Key, kv.Value);
            PrintDivider('─');
            Console.WriteLine();
        }

        // ── Input helpers ─────────────────────────────────────────────────────

        public static string ReadLine(string prompt)
        {
            WriteColored($"  {prompt}: ", Constants.AccentColor, newLine: false);
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        public static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                var raw = ReadLine(prompt);
                if (int.TryParse(raw, out int val) && val >= min && val <= max)
                    return val;
                WriteError($"Please enter a number between {min} and {max}.");
            }
        }

        public static void Pause(string msg = "Press any key to continue...")
        {
            Console.WriteLine();
            WriteColored($"  {msg}", ConsoleColor.DarkGray);
            Console.ReadKey(true);
        }

        public static bool AskYesNo(string question)
        {
            var ans = ReadLine($"{question} (Y/N)").ToUpper();
            return ans == "Y" || ans == "YES";
        }

        // ── Schedule table ────────────────────────────────────────────────────

        public static void PrintScheduleTable(string airlineName, string[] departures, string[] arrivals, decimal fare)
        {
            PrintDivider();
            WriteColored($"  ✈  {airlineName} — FLIGHT SCHEDULE", Constants.PrimaryColor);
            PrintDivider();
            Console.ForegroundColor = Constants.AccentColor;
            Console.WriteLine($"  {"No.",-6}{"Departure",-15}{"Arrival",-15}{"Fare (PKR)",-15}{"Refundable",-12}");
            PrintDivider('-');
            Console.ResetColor();
            for (int i = 0; i < departures.Length; i++)
            {
                Console.WriteLine($"  {(i + 1),-6}{departures[i],-15}{arrivals[i],-15}{fare,-15:N0}{"Yes",-12}");
            }
            PrintDivider();
        }
    }
}
