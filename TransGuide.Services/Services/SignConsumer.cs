using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TransGuide.Data.MappingProfiles.Outputs;

namespace TransGuide.Services.Services
{
    public class SignConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RabbitMqSettings _settings;

        private IConnection _connection;
        private IChannel _channel;

        public SignConsumer(
            IServiceScopeFactory scopeFactory,
            IOptions<RabbitMqSettings> options)
        {
            _scopeFactory = scopeFactory;
            _settings = options.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting RabbitMQ Consumer...");

            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,

                // ✅ FIX: SSL كان ناقص
                Ssl = new SslOption
                {
                    Enabled = _settings.UseSsl,
                    ServerName = _settings.Host
                }
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            Console.WriteLine("RabbitMQ Connected + Queue Ready");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Listening for messages...");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                Console.WriteLine($"Received: {message}");

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var ai = scope.ServiceProvider.GetRequiredService<AiService>();

                    var result = await ai.Predict(message);

                    Console.WriteLine($"Result: {result}");

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");

                    await _channel.BasicNackAsync(
                        ea.DeliveryTag,
                        false,
                        true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _settings.QueueName,
                autoAck: false,
                consumer: consumer
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping consumer...");

            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}