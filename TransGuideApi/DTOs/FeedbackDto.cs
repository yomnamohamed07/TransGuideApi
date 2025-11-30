namespace TransGuideApi.DTOs;

public class FeedbackDto
{
    public string? Title { get; set; }
    public string Content { get; set; } 
    public int? UserProfileId { get; set; }
    public int? TripStatusId { get; set; }
    public int? RouteId { get; set; }
}