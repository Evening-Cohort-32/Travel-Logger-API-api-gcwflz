namespace TravelLogger.Models;

public class Upvote
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RecommendationId { get; set; }


    public User User { get; set; }
    public Recommendation Recommendation { get; set; }
}