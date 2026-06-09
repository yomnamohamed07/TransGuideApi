using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/sign")]
public class SignController : ControllerBase
{
    private readonly SignSessionService _service;

    public SignController(SignSessionService service)
    {
        _service = service;
    }

    // create session
    [HttpPost("create")]
    public async Task<IActionResult> Create()
    {
        var id = await _service.CreateAsync();

        return Ok(new
        {
            success = true,
            sessionId = id
        });
    }

    // end session manually
    [HttpPost("end/{id}")]
    public async Task<IActionResult> End(Guid id)
    {
        var result = await _service.EndSessionAsync(id);

        if (!result)
        {
            return NotFound(new
            {
                success = false,
                message = "Session not found"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Session ended",
            sessionId = id
        });
    }
}