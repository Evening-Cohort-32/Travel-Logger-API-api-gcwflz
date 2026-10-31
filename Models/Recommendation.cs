namespace TravelLogger.Models;

public class Recommendation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CityId { get; set; }
    public string PlaceName { get; set; }
    public string Description { get; set; }

    public User User { get; set; }
    public City City { get; set; }
}