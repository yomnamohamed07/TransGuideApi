using Microsoft.AspNetCore.Mvc;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.Services;
using TransGuideApi.Errors;

namespace TransGuideApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserFeedbackController : ControllerBase
    {
        private readonly IFeedbackService feedbackService;

        public UserFeedbackController(IFeedbackService feedbackService)
        { 
            this.feedbackService = feedbackService;
        }



        [HttpPost]
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
    }
}