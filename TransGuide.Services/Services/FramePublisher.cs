using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using TransGuide.Data.MappingProfiles.Outputs;
using TransGuide.Data.Services;

namespace TransGuide.Services.Services
{
    public class FramePublisher :
        IFramePublisher,
        IAsyncDisposable
    {
        private IChannel? _channel;
        private IConnection? _connection;

        private readonly RabbitMqSettings _settings;

        private bool _initialized = false;

        public FramePublisher(
            IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;
        }

        public async Task InitializeAsync()
        {
            if (_initialized &&
                _connection != null &&
                _connection.IsOpen)
            {
                return;
            }

            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,

                Ssl = new SslOption
                {
                    Enabled = _settings.UseSsl,
                    ServerName = _settings.Host
                }
            };

            _connection =
                await factory.CreateConnectionAsync();

            _channel =
                await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            _initialized = true;

            Console.WriteLine(
                "✅ RabbitMQ Publisher Initialized");
        }

        public async Task PublishAsync(
            byte[] body,
            string sessionId,
            string type)
        {
            await InitializeAsync();

            var props = new BasicProperties
            {
                Headers = new Dictionary<string, object?>
                {
                    {
                        "sessionId",
                        Encoding.UTF8.GetBytes(sessionId)
                    },
                    {
                        "type",
                        Encoding.UTF8.GetBytes(type)
                    }
                }
            };

            await _channel!.BasicPublishAsync(
                exchange: "",
                routingKey: _settings.QueueName,
                mandatory: false,
                basicProperties: props,
                body: body);

            Console.WriteLine("📤 Frame Published");
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();
        }
    }
}