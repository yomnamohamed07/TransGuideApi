using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransGuide.Data.MappingProfiles.Inputs;
using TransGuide.Data.Services;
using TransGuide.Services.Services;

namespace TransGuideApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignController : ControllerBase

{
    private readonly IFramePublisher _publisher;

        public SignController(IFramePublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpPost("frame")]
        public async Task<IActionResult> Send(FrameDto dto)
        {
            if (string.IsNullOrEmpty(dto.ImageBase64))
                return BadRequest("No frame");

           await _publisher.PublishAsync(dto.ImageBase64);

            return Ok("Queued");
        }
    }

}

