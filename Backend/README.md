# Flashcards Backend

This is the backend for the Flashcards project, built using **ASP.NET Core**. It provides RESTful APIs for managing flashcards and decks, following a **CQRS (Command Query Responsibility Segregation)** pattern with an event-driven architecture.

## Technologies Used

- **ASP.NET Core**: For building the web API.
- **CQRS Pattern**: Separates command and query responsibilities.
- **EventSourcing**: Captures all changes to application state as a sequence of events.
- **OpenAPI**: For API documentation and testing.
- **Dependency Injection**: For managing services and handlers.

## Prerequisites

## How to Build
```bash
dotnet restore
dotnet build
```

## How to Run
```bash
cd Api
dotnet run
```