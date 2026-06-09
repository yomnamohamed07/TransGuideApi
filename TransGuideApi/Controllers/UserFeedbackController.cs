using Microsoft.AspNetCore.Mvc;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.Services;
using TransGuideApi.Errors;

namespace TransGuideApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserFeedbacksController : ControllerBase
    {
        private readonly IFeedbackService feedbackService;

        public UserFeedbacksController(IFeedbackService feedbackService)
        { 
            this.feedbackService = feedbackService;
        }



        [HttpPost("SubmitFeedack")]
        public async Task<IActionResult> SubmitFeedback(FeedbackDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiExceptionResponse(400, "Invalid data"));

                var isSubmitted = await feedbackService.SubmitFeedbackAsync(dto);

                if (!isSubmitted)
                    return BadRequest(new ApiExceptionResponse(400, "Feedback could not be submitted"));

                return Ok(new { Message = "Feedback submitted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new ApiExceptionResponse(500, "Something went wrong", ex.Message));
            }
        }


        [HttpGet("GetFeedbacksCount")]
        public async Task<IActionResult> CountFeedbacks()
        {
            int count = await feedbackService.CountFeedbacks();
            if (count == 0)
                return NotFound(new ApiExceptionResponse(404, "Not Feedacks Found"));
            else
                return Ok(new { Count = count });
        }

        [HttpGet("GetAllFeedbacks")]
        public async Task<IActionResult> GetAllFeedacks()
        {
            var feedbacks = await feedbackService.GetAllFeedBacks();
            if (feedbacks == null || !feedbacks.Any())
                return NotFound(new ApiExceptionResponse(404, "Not Feedbacks Found"));
            else
                return Ok(feedbacks);
        }
    }
}