using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGuide.Data.MappingProfiles.Inputs;
using TransGuideApi.Errors;
using AuthService = TransGuide.Data.Services.IAuthorizationService;

namespace TransGuideApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AuthorizationController : ControllerBase
    {
        private readonly AuthService _authorizationService;

        public AuthorizationController(AuthService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiExceptionResponse(400, "Data is required"));
            try { var result = await _authorizationService.AddRoleAsync(dto); return Ok(result); }
           catch(Exception ex)
            {
                return
                    StatusCode(500, ex.ToString());

            }
          
        }

        [HttpPut("EditRole")]
        public async Task<IActionResult> EditRole([FromBody] EditRoleDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiExceptionResponse(400, "Data is required"));

            var result = await _authorizationService.EditRoleAsync(dto);
            return Ok(result);
        }

        [HttpDelete("DeleteRole")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            await _authorizationService.DeleteRoleAsync(id);
            return Ok("Deleted successfully");
        }

        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _authorizationService.GetRolesAsync();
            if (result == null || result.Count == 0)
                return NotFound(new ApiExceptionResponse(404, "No roles found"));

            return Ok(result);
        }

        [HttpGet("GetRole")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var result = await _authorizationService.GetRoleByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("UpdateUserRoles")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiExceptionResponse(400, "Data is required"));

            var result = await _authorizationService.UpdateUserRolesAsync(dto);
            return Ok(result);
        }
    }
}