using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransGuide.Data.Services;
using TransGuide.Services.Services;
using TransGuideApi.Errors;

namespace TransGuideApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoiceController : ControllerBase
    {
        private readonly IVoiceServices _voiceService;

        public VoiceController(IVoiceServices voiceService)
        {
            _voiceService = voiceService;
        }

        [HttpPost("SendVoice")]
        public async Task<IActionResult> SendVoice([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new ApiExceptionResponse(400, "Audio file is required"));

                var result = await _voiceService.SendVoiceAsync(file);

                return Ok(new
                {
                    success = true,
                    message = "Voice processed successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new ApiExceptionResponse(500, "Something went wrong", ex.Message));
            }
        }
       
    }
}
