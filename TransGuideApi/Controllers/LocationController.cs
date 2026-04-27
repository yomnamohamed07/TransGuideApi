using Microsoft.AspNetCore.Mvc;
using TransGuide.Data.MaPppingProfiles;
using TransGuide.Data.Services;
using TransGuideApi.Errors;

namespace TransGuideApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationServices _locationService;
        private readonly IGeoLocationService geoLocationService;

        public LocationController(ILocationServices locationServices, IGeoLocationService geoLocationService)
        {
            _locationService = locationServices;
            this.geoLocationService = geoLocationService;
        }


        // [HttpGet("GetAllRoutes")]
        //    public async Task<IActionResult> GetRoutes(int pageIndex = 1, int pageSize = 10)
        //   {
        //   try
        //     {
        //    var routes = await _locationService.GetAllRoutesPaginatedAsync(pageIndex, pageSize);
        //    if (!routes.Data.Any())
        //    return NotFound(new ApiExceptionResponse(404, "No routes found"));

        //  return Ok(routes);
        // }//
        // catch (Exception ex)
        // {
        //   return StatusCode(500, new ApiExceptionResponse(500, "Something went wrong", ex.Message));
        //  }
        //   }
        [HttpGet("test-geo")]
        public async Task<IActionResult> TestGeo(double lat, double lng)
        {
            var result = await geoLocationService.GetNearestStationAsync((decimal)lat, (decimal)lng);
            if (result == null)
                return BadRequest("No Station Found");
            return Ok(result);
        }
        [HttpPost("SearchRoutes")]
        public async Task<IActionResult> SearchRoutes([FromBody] FilterDto filter, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                if (filter == null)
                    return BadRequest(new ApiExceptionResponse(400, "Filter object is required"));

                var routes = await _locationService.GetRoutesAsync(pageIndex, pageSize, filter);
                if (!routes.Data.Any())
                    return NotFound(new ApiExceptionResponse(404, "No routes match your search"));

                return Ok(routes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "Something went wrong", ex.Message));
            }
        }
    }
}