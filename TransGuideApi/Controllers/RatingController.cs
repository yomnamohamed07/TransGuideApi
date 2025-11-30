using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Repositories;
using TransGuideApi.Hub_SignalR;
using TransGuideApi.DTOs;

namespace TransGuideApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingController : ControllerBase
{
    private readonly IRatingRepository _ratingRepo;
    private readonly IHubContext<RatingHub> _hubContext;

    public RatingController(IRatingRepository ratingRepo, IHubContext<RatingHub> hubContext)
    {
        _ratingRepo = ratingRepo;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> AddRating(RatingDto dto)
    {
        if (dto.Score < 1 || dto.Score > 5)
            return BadRequest("التقييم يجب أن يكون بين 1 و 5.");

        var rating = new Rating
        {
            UserId = dto.UserId,
            TripId = dto.TripId,
            Score = dto.Score,
            Comment = dto.Comment
        };

        var newRating = await _ratingRepo.AddAsync(rating);

        await _hubContext.Clients.All.SendAsync("ReceiveNewRating", newRating);

        return Ok(newRating);
    }
    

    [HttpGet("trip/{tripId}")]
    public async Task<IActionResult> GetRatingsByTrip(int tripId)
    {
        var ratings = await _ratingRepo.GetRatingsByTripIdAsync(tripId);
        return Ok(ratings);
    }

    [HttpGet("average/{tripId}")]
    public async Task<IActionResult> GetAverageRating(int tripId)
    {
        var average = await _ratingRepo.GetAverageRatingForTripAsync(tripId);
        return Ok(new { AverageRating = average });
    }

    [HttpGet("average/all")]
    public async Task<IActionResult> GetOverallAverage()
    {
        var avg = await _ratingRepo.GetOverallAverageAsync();
        return Ok(new { OverallAverage = avg });
    }
}