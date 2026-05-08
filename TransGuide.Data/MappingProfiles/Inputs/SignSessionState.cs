

namespace TransGuide.Data.MappingProfiles.Inputs
{
    public class SignSessionState
    {
        public Guid SessionId { get; set; }

        public string? Word { get; set; }
        public DateTime LastFrameTime { get; set; } = DateTime.Now;
        public bool IsEnded { get; set; } = false;

      
    }
}
