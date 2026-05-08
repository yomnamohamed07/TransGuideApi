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

        private IConnection? _connection;
        private IChannel? _channel;

        public SignConsumer(
            IServiceScopeFactory scopeFactory,
            IOptions<RabbitMqSettings> options)
        {
            _scopeFactory = scopeFactory;
            _settings = options.Value;
        }

        // =========================
        // CONNECT (SAFE + RETRY READY)
        // =========================
        private async Task ConnectAsync()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _settings.Host,
                    Port = _settings.Port,
                    UserName = _settings.Username,
                    Password = _settings.Password,
                    VirtualHost = _settings.VirtualHost,

                    RequestedConnectionTimeout = TimeSpan.FromSeconds(10),

                    Ssl = new SslOption
                    {
                        Enabled = _settings.UseSsl,
                        ServerName = _settings.Host
                    }
                };

                _connection = await factory.CreateConnectionAsync("SignConsumer");
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                    queue: _settings.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                Console.WriteLine("✅ RabbitMQ Connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ RabbitMQ failed: " + ex.Message);

                _connection = null;
                _channel = null;
            }
        }

      
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ConnectAsync();

            
            if (_channel == null)
            {
                Console.WriteLine("⚠️ RabbitMQ not available. Consumer paused.");

                await Task.Delay(Timeout.Infinite, stoppingToken);
                return;
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();

                    var sessionId = Encoding.UTF8.GetString(
                        (byte[])ea.BasicProperties.Headers!["sessionId"]);

                    var type = Encoding.UTF8.GetString(
                        (byte[])ea.BasicProperties.Headers!["type"]);

                    var id = Guid.Parse(sessionId);

                    using var scope = _scopeFactory.CreateScope();

                    var sessionService =
                        scope.ServiceProvider.GetRequiredService<SignSessionService>();

                    // END SESSION
                    if (type == "end")
                    {
                        await sessionService.EndAsync(id);
                        await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                        return;
                    }

                    // AI PROCESSING
                    var ai = scope.ServiceProvider.GetRequiredService<AiService>();

                    var base64 = Convert.ToBase64String(body);

                    var result = await ai.Predict(base64);

                    await sessionService.UpdateWordAsync(id, result);

                    await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Consumer Error: " + ex.Message);

                    await _channel!.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _settings.QueueName,
                autoAck: false,
                consumer: consumer);

            Console.WriteLine("🚀 Consumer Running");

            // Keep alive
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        // =========================
        // CLEANUP
        // =========================
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_channel != null)
                    await _channel.CloseAsync();

                if (_connection != null)
                    await _connection.CloseAsync();
            }
            catch { }

            await base.StopAsync(cancellationToken);
        }
    }
}