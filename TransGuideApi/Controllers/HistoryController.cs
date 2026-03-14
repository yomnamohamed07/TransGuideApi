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

            return Ok(history); 
        }

        [Authorize]
        [HttpDelete("DeleteHisrory{key}")]
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
        [Authorize]
        [HttpDelete("DeleteTrip")]
        public async Task<IActionResult> DeleteTrip(string userId, string tripId)
        {
            try
            {
                var result = await _servicesManager.HistoryServices
                    .DeleteTripFromHistoryAsync(userId, tripId);

                if (!result)
                    return NotFound(new ApiExceptionResponse(404, "Trip not found"));

                return Ok("Trip deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new ApiExceptionResponse(500, "Something went wrong", ex.Message));
            }
        }
    }
}
