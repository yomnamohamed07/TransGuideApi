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

        private async Task ConnectAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ConnectAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                using var scope = _scopeFactory.CreateScope();

                try
                {
                    var headers = ea.BasicProperties?.Headers;

                    if (headers == null)
                    {
                        await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                        return;
                    }

                    var sessionBytes =
                        headers["sessionId"] as byte[]
                        ?? (headers["sessionId"] as ReadOnlyMemory<byte>?)?.ToArray();

                    var typeBytes =
                        headers["type"] as byte[]
                        ?? (headers["type"] as ReadOnlyMemory<byte>?)?.ToArray();

                    var sessionId = Encoding.UTF8.GetString(sessionBytes!);
                    var type = Encoding.UTF8.GetString(typeBytes!);

                    var id = Guid.Parse(sessionId);

                    var sessionService =
                        scope.ServiceProvider.GetRequiredService<SignSessionService>();

                    if (type == "end")
                    {
                        await sessionService.EndAsync(id);
                        await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                        return;
                    }

                    var ai =
                        scope.ServiceProvider.GetRequiredService<AiService>();

                    var result = await ai.Predict(ea.Body.ToArray());

                    await sessionService.UpdateWordAsync(id, result);

                    await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch
                {
                    await _channel!.BasicNackAsync(
                        ea.DeliveryTag,
                        false,
                        false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _settings.QueueName,
                autoAck: false,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}