# Delivery Plan

## 1) Scopes
- JWT + scope-based authorization.
- Search orders by name.
- Checkout command flow with:
  - Idempotency handling,
  - Transactional state changes,
  - Payment success/failure/unknown handling,
  - Compensation persistence,
  - Outbox event creation for production/invoice/email.
- Outbox worker publishing integration events.

## 2) Delivery Phases
1. **Architecture & Setup**
   - Clean Architecture layers, project structure, dependencies
2. **Core Data Model**
   - Order/Payment/Outbox schema and EF mappings
3. **Security setup**
   - Auth policies, exception middleware
4. **Implement search order endpoint**
   - Search orders query + repository
5. **Implement Order checkout feature**
   - Payment flow, status transitions, outbox creation, idempotency, concurrency, compensation,...
6. **Implement OutboxWorker**
   - Outbox polling + publish + processed marking
7. **Documentation Packaging**
   - System design (including assumptions/validation), delivery plan

# Time Tracking
- Dive deep and understand the core requirements and constraints: 2h
- Decide high-level system architecture: 1h
- Design data model and API contracts: 2h
- Implementation with Clean Architecture + CQRS
	- Setup projects, layers, dependencies: 1h
	- Implement search orders: 2h
	- Implement checkout flow: 5h
	- Implement outbox worker: 2h
- Documentation (system design, assumptions/validation, delivery plan): 2h
