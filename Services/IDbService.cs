using CW_10_s31552.Models.DTOs;

namespace CW_10_s31552.Services;

public interface IDbService
{
    public Task<GetTripWithPagesDto> GetTripsAsync(int page, int pageSize, CancellationToken cancellationToken);
    public Task DeleteClientAsync(int idClient, CancellationToken cancellationToken);
    public Task<GetClientTripDto> AddClientToTripAsync(int idTrip, AddClientToTripDto dto, CancellationToken cancellationToken);
}