using Microsoft.AspNetCore.Mvc;
using TransGuide.Data.MappingProfiles.Inputs;
using TransGuide.Data.Services;
using TransGuide.Services.Services;

[Route("api/[controller]")]
[ApiController]
public class SignController : ControllerBase
{
    private readonly IFramePublisher _publisher;
    private readonly SignSessionService _service;

    public SignController(
        IFramePublisher publisher,
        SignSessionService service)
    {
        _publisher = publisher;
        _service = service;
    }

    // CREATE SESSION

    [HttpPost("create")]
    public async Task<IActionResult> Create()
    {
        var id = await _service.CreateAsync();

        return Ok(new
        {
            success = true,
            sessionId = id,
            message = "Session created successfully"
        });
    }


    // SEND FRAME

    [HttpPost("frame")]
    public async Task<IActionResult> Frame([FromForm] FrameDto dto)
    {
        if (dto.SessionId == Guid.Empty)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid session id"
            });
        }

        if (dto.File == null || dto.File.Length == 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Empty frame"
            });
        }

        using var ms = new MemoryStream();
        await dto.File.CopyToAsync(ms);

        var bytes = ms.ToArray();

        await _publisher.PublishAsync(
            bytes,
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


    // END SESSION

    [HttpPost("end/{id}")]
    public async Task<IActionResult> End(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid session id"
            });
        }

        await _service.EndAsync(id);

        return Ok(new
        {
            success = true,
            message = "Session ended"
        });
    }
}