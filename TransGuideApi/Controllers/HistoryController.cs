using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGuide.Data.MaPppingProfiles;
using TransGuide.Services;
using TransGuide.Services.DTOS;
using TransGuideApi.Errors;

namespace TransGuideApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IServicesManager _servicesManager;

        public HistoryController(IServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        [Authorize]
        [HttpGet("MyHistory")]
        public async Task<IActionResult> GetMyHistory()
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;

           if (string.IsNullOrEmpty(userId))
               return Unauthorized();

            var history = await _servicesManager.HistoryServices.GetHistoryAsync(userId);

            if (history == null || !history.Trips.Any())
                return NotFound(new ApiExceptionResponse(404, "No trips found for this user"));

            return Ok(history.Trips); 
        }

        [Authorize]
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteHistory(string key)
        {
            try
            {
                var result = await _servicesManager.HistoryServices.DeleteHistoryAsync(key);

                if (!result)
                    return NotFound(new ApiExceptionResponse(404, "History not found"));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "Something went wrong", ex.Message));
            }
        }
    }
}
