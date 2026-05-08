using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;
using TransGuide.Data.MappingProfiles.Inputs;
using TransGuide.Data.Services;
using TransGuide.Services.Services;
using TransGuideApi;

[Route("api/[controller]")]
[ApiController]

public class SignController : ControllerBase
{
    private readonly IFramePublisher _publisher;
    private readonly SignSessionService _service;

    public SignController(IFramePublisher publisher, SignSessionService service)
    {
        _publisher = publisher;
        _service = service;
    }

    // CREATE SESSION
    [HttpPost("create")]
    public async Task<IActionResult> Create()
    {
        var id = await _service.CreateAsync();

        return Ok(new { sessionId = id });
    }

    // SEND FRAME
    [HttpPost("frame")]
    public async Task<IActionResult> Frame([FromForm] FrameDto dto)
    {
        using var ms = new MemoryStream();
        await dto.File.CopyToAsync(ms);

        await _publisher.PublishAsync(
            ms.ToArray(),
            dto.SessionId.ToString(),
            dto.Type);

        return Ok(new
        {
            success = true,
            message = "Frame received",
            sessionId = dto.SessionId,
            type = dto.Type
        });
    }

    // MANUAL END
    [HttpPost("end/{id}")]
    public async Task<IActionResult> End(Guid id)
    {
        await _service.EndAsync(id);
        return Ok();
    }
}
