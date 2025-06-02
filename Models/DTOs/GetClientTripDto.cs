namespace CW_10_s31552.Models.DTOs;

public class GetClientTripDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Pesel { get; set; } = null!;
    public string TripName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? PaymentDate { get; set; }
}