using Microsoft.Extensions.Options;
using System.Text;
using RabbitMQ.Client;
using TransGuide.Data.MappingProfiles.Outputs;
using TransGuide.Data.Services;

namespace TransGuide.Services.Services
{
    public class FramePublisher : IFramePublisher
    {
        private readonly RabbitMqSettings _settings;

        public FramePublisher(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;
        }

        public async Task PublishAsync(string frame)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,

                // ✅ SSL مهم جدًا لـ CloudAMQP
                Ssl = new SslOption
                {
                    Enabled = _settings.UseSsl,
                    ServerName = _settings.Host
                }
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            var body = Encoding.UTF8.GetBytes(frame);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _settings.QueueName,
                body: body
            );
        }
    }
}