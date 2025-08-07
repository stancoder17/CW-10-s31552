# Trip Management API – .NET Core + EF Core (Database First)

This project was created as part of my studies in computer science at Polish-Japanese Academy of Information Technology.  
The goal was to practice the database-first approach using Entity Framework Core library.

The application connects to an existing database and exposes a few endpoints that allow:
- listing trips with optional pagination
- deleting a client 
- assigning a new client to a trip, with some validation

The project focuses on backend logic only, there is no frontend or user interface.

## Endpoints

### Trips listing
Returns a list of trips, ordered by start date.  
Supports pagination via page and pageSize query parameters.
The response includes trip details, related countries, and clients registered for the trip.

### Deleting client
Deletes a client by ID, but only if the client is not assigned to any trip.
If the client is linked to at least one trip, the server returns an appropriate error message and status code.

### Assigning client to a trip
Request body must include client data. Validation includes:
checking if a client with the same PESEL (unique for all Poles :-]) already exists
checking if the client is already assigned to this trip
verifying that the trip exists and has not yet started (DateFrom must be in the future)
PaymentDate can be null. RegisteredAt is automatically set to the current server time.


The code is separated into different layer:
1. Database access and business logic layer (there is no separate layer for database access, because of the small size of the project)
2. Controller layer

# CW-10-s31552
