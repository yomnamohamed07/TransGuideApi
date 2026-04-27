



    using global::TransGuide.Data.MappingProfiles.Inputs;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace TransGuide.Data.Services
    {
        public interface IAuthorizationService
        {
           public Task<string> AddRoleAsync(AddRoleDto dto);
           public Task<string> EditRoleAsync(EditRoleDto dto);
            public Task DeleteRoleAsync(int id);
           public Task<IReadOnlyList<RoleDto>> GetRolesAsync();
           public Task<RoleDto> GetRoleByIdAsync(int id);
           public Task<string> UpdateUserRolesAsync(UpdateUserRolesDto dto);
           
        }
    }

