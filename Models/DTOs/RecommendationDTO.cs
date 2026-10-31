namespace TravelLogger.Models.DTOs;

public class RecommendationDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CityId { get; set; }
    public string PlaceName { get; set; }
    public string Description { get; set; }

}