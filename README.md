# NegotiationApp
## Software Mind internship task
This is an ASP.NET (Web API) application that enables clients to negotiate product prices with employees, managing the process of offering, accepting, rejecting, and canceling negotiations.


## Technologies used

Following technologies were used:

-   **ASP.NET Core 9.0**: Framework for building web applications and APIs.
-   **C#**: Programming language.
-   **Swagger/OpenAPI**: For automatic API documentation and interactive testing.
-   **MediatR**: Implementation of the Mediator and CQRS (Command Query Responsibility Segregation) patterns for managing business logic.
-   **FluentValidation**: Library for input data validation.
-   **JWT (JSON Web Tokens)**: For user authorization and authentication.
-   **xUnit**: Testing framework for unit and integration tests.

## Project structure
```
NegotiationApp.sln 
├───NegotiationApp.Api            # Presentation layer (Web API)
├───NegotiationApp.Application    # Business logic, MediatR Handlers, DTOs, Validators, Service Interfaces
├───NegotiationApp.Domain         # Entities, Exceptions, Enums, Repository Interfaces
├───NegotiationApp.Infrastructure # Implementations of repositories (In-Memory), Services (SecurityService)
└───NegotiationApp.Tests          # Unit and Integration Tests
```


## Key features

-   **Product Management:**
    * Adding new products.
    * Retrieving product details by ID.
    * Retrieving a list of all products.
-   **Negotiation Management:**
    * Initiating a new negotiation for a product.
    * Client proposing a price (with attempt limits and response time).
    * Employee accepting an offer.
    * Employee rejecting an offer.
    * Canceling a negotiation (by system or manually).
    * Retrieving negotiation details by ID.
    * Retrieving a list of negotiations for a given product.
-   **Authentication and Authorization:**
    * JWT-based authentication.
    * Role-based authorization


## Test Users

For testing purposes, a simplified authentication is implemented in the application:

| Role     | Username   | Password   |
| :------- | :--------- | :--------- |
| Employee | `employee` | `password` |
