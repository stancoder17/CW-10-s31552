using CW_10_s31552.Data;
using CW_10_s31552.Exceptions;
using CW_10_s31552.Models;
using CW_10_s31552.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CW_10_s31552.Services;

public class DbService(TripAgencyDbContext context) : IDbService
{
    public async Task<GetTripWithPagesDto> GetTripsAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        ValidatePageNumber(page);
        ValidatePageSize(pageSize);
        
        var trips = await context.Trips.Select(t => new GetTripDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(c => new GetCountryDto
                    {
                        Name = c.Name
                    }
                ).ToList(),
                Clients = t.ClientTrips.Select(c => new GetClientDto
                {
                    FirstName = c.IdClientNavigation.FirstName,
                    LastName = c.IdClientNavigation.LastName
                }).ToList()
            }).OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        var allTripsCount = await context.Trips.CountAsync(cancellationToken); // We need a count of ALL trips, trips.Count would return the count of the trips on the current page only.
        
        return new GetTripWithPagesDto
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = CalculatePagesCount(allTripsCount, pageSize),
            Trips = trips
        };
    }

    public async Task DeleteClientAsync(int idClient, CancellationToken cancellationToken)
    {
        // Check if the client has trips.
        var hasTrips = await context.ClientTrips.AnyAsync(c => c.IdClient == idClient, cancellationToken);
        if (hasTrips)
            throw new BadRequestException("Cannot delete client with trips.");

        var rowsAffected = await context.Clients.Where(c => c.IdClient == idClient).ExecuteDeleteAsync(cancellationToken); 
        
        // Check if the client exists
        if (rowsAffected == 0)
            throw new NotFoundException($"Client with id {idClient} not found.");
    }

    public async Task<GetClientTripDto> AddClientToTripAsync(int idTrip, AddClientToTripDto dto, CancellationToken cancellationToken)
    {
        // Check if the client exists.
        if (await context.Clients.AnyAsync(c => c.Pesel == dto.Pesel, cancellationToken))
            throw new BadRequestException($"Client with PESEL {dto.Pesel} already exists.");
        
        /*
         * I'm not checking if the client is already on this trip, because that would mean that he already exists, which we check above.
         */

        // Check if the trip exists.
        var trip = await context.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip, cancellationToken);
        if (trip == null)
            throw new NotFoundException($"Trip with id {idTrip} not found.");
        
        // Check if the trip has already started/taken place.
        if (trip.DateFrom < DateTime.Now)
            throw new BadRequestException("The trip has already started/taken place.");

        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var client = new Client
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Telephone = dto.Telephone,
                Pesel = dto.Pesel,
            };
            await context.Clients.AddAsync(client, cancellationToken);
            await context.SaveChangesAsync(cancellationToken); // IdClient that we'll need is auto-generated to the changes have to be saved here.

            var registeredAt = DateTime.Now;
            await context.ClientTrips.AddAsync(new ClientTrip
            {
                IdClient = client.IdClient,
                IdTrip = trip.IdTrip,
                RegisteredAt = registeredAt,
                PaymentDate = dto.PaymentDate
            }, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new GetClientTripDto
            {
                FirstName = client.FirstName,
                LastName = client.LastName,
                Pesel = client.Pesel,
                TripName = trip.Name,
                Description = trip.Description,
                DateFrom = trip.DateFrom,
                DateTo = trip.DateTo,
                RegisteredAt = registeredAt,
                PaymentDate = dto.PaymentDate
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static void ValidatePageNumber(int page)
    {
        if (page < 1)
            throw new BadRequestException("Page number must be greater than 0.");
    }

    private static void ValidatePageSize(int pageSize)
    {
        if (pageSize < 1)
            throw new BadRequestException("Page size must be greater than 0.");
    }
    
    private static int CalculatePagesCount(int totalItems, int pageSize)
    {
        return (int)Math.Ceiling((decimal)totalItems / pageSize); // 3 = 3; 3.33.. = 4
    }

}