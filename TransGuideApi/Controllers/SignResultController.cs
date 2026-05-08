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
        var session = await _service.GetAsync(id);

        // 🔥 أهم حل لمشكلة "Session Not Found"
        if (session == null)
        {
            return Ok(new
            {
                status = "processing",
                word = "",
                ended = false
            });
        }

        return Ok(new
        {
            word = session.Word ?? "",
            ended = session.IsEnded,
            status = session.IsEnded ? "done" : "processing"
        });
    }
}