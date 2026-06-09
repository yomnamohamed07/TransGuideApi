

namespace TransGuide.Data.MappingProfiles.Outputs
{
    public class RabbitMqSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public bool UseSsl { get; set; }
        public string QueueName { get; set; }
    }
}
