namespace TravelLogger.Models.DTOs;

public class CityDTO
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<LogDTO> Logs { get; set; } = new();
    public List<RecommendationDTO> Recommendations { get; set; } = new();
    public List<UserDTO> Users { get; set; } = new();
}