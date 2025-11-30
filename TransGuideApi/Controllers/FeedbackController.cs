using Microsoft.AspNetCore.Mvc;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Repositories;
using TransGuideApi.DTOs;

namespace TransGuideApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackRepository _feedbackRepo;

    public FeedbackController(IFeedbackRepository feedbackRepo)
    {
        _feedbackRepo = feedbackRepo;
    }

    [HttpPost]
    public async Task<IActionResult> AddFeedback(FeedbackDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
            return BadRequest("الملاحظة فارغة. يرجى كتابة تعليقك.");

        var feedback = new Feedback
        {
            Title = dto.Title,
            Content = dto.Content,
            UserProfileId = dto.UserProfileId,
            RouteId = dto.RouteId,
            TripStatusId = dto.TripStatusId
        };

        await _feedbackRepo.AddAsync(feedback);

        return Ok(feedback);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFeedback()
    {
        var feedbacks = await _feedbackRepo.GetAllAsync();
        return Ok(feedbacks);
    }
}