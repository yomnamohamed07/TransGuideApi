using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGuide.Data.Helper;
using TransGuide.Data.MappingProfiles.Inputs;
using TransGuide.Data.Services;

using TransGuideApi.Errors;

namespace TransGuideApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IRouteServices _routeServices;
        private readonly IStationService _stationService;

        public AdminDashboardController(
            IRouteServices routeServices,
            IStationService stationService)
        {
            _routeServices = routeServices;
            _stationService = stationService;
        }



        [HttpPost("CreateRoute")]
        public async Task<IActionResult> CreateRoute([FromBody] AddRouteDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiExceptionResponse(400, "Data is required"));

            var result = await _routeServices.CreateRouteAsync(dto);
            return Ok(result);
        }

        [HttpPut("UpdateRoute")]
        public async Task<IActionResult> UpdateRoute([FromBody] UpdateRouteDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiExceptionResponse(400, "Data is required"));

            var result = await _routeServices.UpdateRouteAsync(dto);
            return Ok(result);
        }

        [HttpDelete("DeleteRoute")]
        public async Task<IActionResult> SoftDeleteRoute(int id)
        {
            var result = await _routeServices.SoftDeleteRouteAsync(id);

            if (!result)
                return NotFound(new ApiExceptionResponse(404, "Route not found"));

            return Ok("Deleted successfully");
        }

        [HttpPatch("UpdateRouteStatus")]
        public async Task<IActionResult> UpdateRouteStatus(int id, RouteStatusEnum status)
        {
            var result = await _routeServices.UpdateRouteStatus(id, status);

            if (!result)
                return BadRequest(new ApiExceptionResponse(400, "Failed to update status"));

            return Ok("Status updated successfully");
        }



        [HttpPost("CreateStation")]
        public async Task<IActionResult> CreateStation([FromBody] AddStationDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiExceptionResponse(400, "Data is required"));

            var result = await _stationService.CreateStationAsync(dto);
            return Ok(result);
        }

        [HttpPut("UpdateStation")]
        public async Task<IActionResult> UpdateStation([FromBody] UpdateStationDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiExceptionResponse(400, "Data is required"));

            var result = await _stationService.UpdateStationAsync(dto);
            return Ok(result);
        }

        [HttpDelete("DeleteStation")]
        public async Task<IActionResult> SoftDeleteStation(int id)
        {
            var result = await _stationService.SoftDeleteStationAsync(id);

            if (!result)
                return NotFound(new ApiExceptionResponse(404, "Station not found"));

            return Ok("Deleted successfully");
        }

        [HttpGet("GetStation/{id}")]
        public async Task<IActionResult> GetStationById(int id)
        {
            var result = await _stationService.GetStationById(id);

            if (result == null)
                return NotFound(new ApiExceptionResponse(404, "Station not found"));

            return Ok(result);
        }

        [HttpGet("GetAllStations")]
        public async Task<IActionResult> GetAllStations(string? search, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _stationService.GetAllStations(search, pageIndex, pageSize);

            if (result == null || result.Data.Count == 0)
                return NotFound(new ApiExceptionResponse(404, "No stations found"));

            return Ok(result);

        }
    }
}