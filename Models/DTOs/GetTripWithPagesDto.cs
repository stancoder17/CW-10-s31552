namespace CW_10_s31552.Models.DTOs;

public class GetTripWithPagesDto
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public List<GetTripDto> Trips { get; set; } = [];
}