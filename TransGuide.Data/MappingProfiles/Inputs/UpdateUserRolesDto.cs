

namespace TransGuide.Data.MappingProfiles.Inputs
{
    public class UpdateUserRolesDto
    {
        public int UserId { get; set; }
        public List<string> Roles
        {
            get; set;
        }
    }
}
