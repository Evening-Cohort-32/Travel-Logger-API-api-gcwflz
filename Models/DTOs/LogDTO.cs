namespace TravelLogger.Models.DTOs;

public class LogDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CityId { get; set; }
    public string Comments { get; set; }
    public DateTime LoggedAt { get; set; }
    public User User {get; set;}
}