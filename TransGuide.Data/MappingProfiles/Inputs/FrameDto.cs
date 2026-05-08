

using Microsoft.AspNetCore.Http;

namespace TransGuide.Data.MappingProfiles.Inputs
{
    public class FrameDto
    {
        public Guid SessionId { get; set; }

        public IFormFile File { get; set; }
        public string Type { get; set; } 
    }
}
