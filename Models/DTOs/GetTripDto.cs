namespace CW_10_s31552.Models.DTOs;

public class GetTripDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime DateFrom { get; set; } 
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<GetCountryDto> Countries { get; set; } = [];
    public List<GetClientDto> Clients { get; set; } = [];
}