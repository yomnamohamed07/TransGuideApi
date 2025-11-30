namespace TransGuideApi.DTOs;

public class RatingDto
{
    public int UserId { get; set; }
    public int TripId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
}