

namespace TransGuide.Data.Services
{
     public interface IFramePublisher
    {
        Task PublishAsync(byte[] body, string sessionId, string type);
    }
}
