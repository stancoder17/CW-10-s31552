using CW_10_s31552.Exceptions;
using CW_10_s31552.Models.DTOs;
using CW_10_s31552.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW_10_s31552.Controllers;

[ApiController]
[Route("api")]
public class TripAgencyController(IDbService service) : ControllerBase
{
    [HttpGet("trips")]
    public async Task<IActionResult> GetTrips(CancellationToken cancellationToken, int page = 1, int pageSize = 10)
    {
        try
        {
            return Ok(await service.GetTripsAsync(page, pageSize, cancellationToken));
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("clients/{idClient:int}")]
    public async Task<IActionResult> DeleteClient(int idClient, CancellationToken cancellationToken)
    {
        try
        {
            await service.DeleteClientAsync(idClient, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("trips/{idTrip:int}/clients")]
    public async Task<IActionResult> AddClientToTrip(int idTrip, [FromBody] AddClientToTripDto dto, CancellationToken cancellationToken)
    {
        try
        {
           return Ok(await service.AddClientToTripAsync(idTrip, dto, cancellationToken)); 
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}