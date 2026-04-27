using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.MappingProfiles.Inputs;
using TransGuide.Data.MappingProfiles.Outputs;
using TransGuide.Data.Services;


namespace TransGuide.Services.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<UserProfile> _userManager;

        public AuthorizationService(RoleManager<IdentityRole<int>> roleManager,
                                    UserManager<UserProfile> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<string> AddRoleAsync(AddRoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RoleName))
                throw new Exception("Role name is required");

            var exists = await _roleManager.RoleExistsAsync(dto.RoleName);
            if (exists)
                throw new Exception("Role already exists");

            var role = new IdentityRole<int>(dto.RoleName);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                throw new Exception("Failed to create role");

            return role.Name;
        }

        public async Task<string> EditRoleAsync(EditRoleDto dto)
        {
            var role = await _roleManager.FindByIdAsync(dto.Id.ToString());
            if (role == null)
                throw new Exception("Role not found");

            role.Name = dto.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                throw new Exception("Update failed");

            return role.Name;
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
                throw new Exception("Role not found");

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                throw new Exception("Delete failed");
        }

        public async Task<IReadOnlyList<RoleDto>> GetRolesAsync()
        {
            var roles = _roleManager.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToList();

            return roles;
        }

        public async Task<RoleDto> GetRoleByIdAsync(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
                throw new Exception("Role not found");

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public async Task<string> UpdateUserRolesAsync(UpdateUserRolesDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null)
                throw new Exception("User not found");

            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);

            var result = await _userManager.AddToRolesAsync(user, dto.Roles);
            if (!result.Succeeded)
                throw new Exception("Failed to update roles");

            return "Roles updated successfully";
        }

    
    }
}