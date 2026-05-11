# OrderService

`OrderService` is a .NET 8 backend service for order search and checkout/payment workflow, built with Clean Architecture + CQRS.

## Project Structure
- `OrderService.API`  
  API endpoints, authentication/authorization, middleware, hosted workers.
- `OrderService.Application`  
  Use-case logic (commands/queries), handlers, DTOs, mappings, service abstractions.
- `OrderService.Domain`  
  Core entities, enums, constants, domain contracts, domain exceptions, repository/UoW interfaces.
- `OrderService.Infrastructure`  
  EF Core persistence, repositories, unit of work implementation, payment processors, messaging adapters, migrations.

## Design Documents
- `docs/System-Design.md`  
  Architecture, component interactions, data model, API contracts, checkout flow, reliability/performance/security notes.
- `docs/Delivery-Plan.md`  
  Scope, delivery phases, and time-tracking summary.

## Notes
- Target framework: `.NET 8`
