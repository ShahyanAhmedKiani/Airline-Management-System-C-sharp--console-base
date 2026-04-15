// =============================================================================
// File: Program.cs
// Purpose: Application entry point — minimal; just launches MenuHandler.
//          All wiring and logic lives in dedicated classes.
// =============================================================================

using AirlineReservationSystem.UI;

// Set console properties for a clean experience
Console.Title       = "✈  Airline Reservation System";
Console.OutputEncoding = System.Text.Encoding.UTF8;

try
{
    var app = new MenuHandler();
    app.Run();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n  Unexpected error: {ex.Message}");
    Console.ResetColor();
    Console.ReadKey();
}
