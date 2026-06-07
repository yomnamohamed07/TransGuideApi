

namespace TransGuide.Data.MappingProfiles.Inputs
{
    
    public class LandmarkRequest
    {
        public Guid session_id { get; set; }
        public float[] landmarks { get; set; } = [];
    }
}
