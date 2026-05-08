using Microsoft.AspNetCore.Mvc;
using TransGuide.Services.Services;

[Route("api/[controller]")]
[ApiController]
public class SignResultController : ControllerBase
{
    private readonly SignSessionService _service;

    public SignResultController(SignSessionService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid session id"
            });
        }

        var session = await _service.GetAsync(id);

        
        if (session == null)
        {
            return Ok(new
            {
                success = true,
                status = "processing",
                word = "",
                ended = false,
                hasResult = false
            });
        }

       
        if (!session.IsEnded)
        {
            return Ok(new
            {
                success = true,
                status = "processing",
                word = session.Word ?? "",
                ended = false,
                hasResult = !string.IsNullOrEmpty(session.Word)
            });
        }

        
        return Ok(new
        {
            success = true,
            status = "done",
            word = session.Word ?? "",
            ended = true,
            hasResult = !string.IsNullOrEmpty(session.Word)
        });
    }
}