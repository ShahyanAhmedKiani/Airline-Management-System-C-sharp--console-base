# ✈️ Airline Reservation System — C# Professional Console App

## Project Structure
```
AirlineReservationSystem/
├── AirlineReservationSystem.csproj
├── Program.cs                    ← Entry point
├── Models/
│   ├── Person.cs                 ← Base class (Encapsulation + Inheritance root)
│   ├── Passenger.cs              ← Inherits Person (Inheritance)
│   ├── Booking.cs                ← Booking data model
│   └── Payment.cs                ← Payment model
├── Services/
│   ├── IFlightService.cs         ← Interface (Polymorphism)
│   ├── LocalFlightService.cs     ← Implements IFlightService
│   ├── GlobalFlightService.cs    ← Implements IFlightService
│   └── BookingService.cs         ← Business logic
├── Data/
│   └── FileRepository.cs         ← File I/O (Data persistence)
├── Admin/
│   └── AdminPanel.cs             ← Admin operations
├── Helpers/
│   ├── ConsoleHelper.cs          ← UI rendering utilities
│   ├── Validator.cs              ← Input validation
│   └── Constants.cs              ← App constants
└── UI/
    └── MenuHandler.cs            ← Navigation menus
```

## OOP Concepts Used
| Concept        | Where Used |
|----------------|-----------|
| Encapsulation  | `Person`, `Passenger`, `Booking`, `Payment` — private fields with public properties |
| Inheritance    | `Passenger` extends `Person`; `LocalFlightService` & `GlobalFlightService` extend `FlightServiceBase` |
| Polymorphism   | `IFlightService` interface — `GetSchedule()`, `GetSource()`, `GetDestination()` called uniformly |
| Abstraction    | `IFlightService`, abstract `FlightServiceBase` hide implementation details |

## How to Run in Visual Studio 2022
1. Open Visual Studio 2022
2. File → Open → Project/Solution → select `AirlineReservationSystem.csproj`
3. Press **F5** or **Ctrl+F5** to run
4. Admin password: `Admin@1234`
