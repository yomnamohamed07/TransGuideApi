using Microsoft.AspNetCore.Mvc;
using TransGuide.Data;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Services.DTOS;

namespace TransGuideApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserFeedbackController : ControllerBase
    {
        private readonly TransGuideDbContext _context;

        public UserFeedbackController(TransGuideDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] UserFeedback feedback)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم إرسال الملاحظات بنجاح" });
        }
    }
}