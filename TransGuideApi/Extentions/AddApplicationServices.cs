using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TransGuide.Data.Repositories;
using TransGuide.Data.Services;
using TransGuide.Services;
using TransGuide.Services.Mapper;
using TransGuide.Services.Services;
using TransGuideApi.Errors;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Infrastructure.Repositories;
using TransGuide.Data.MappingProfiles.Outputs;
using TransGuide.Data.Respositories;
using TransGuide.Infrustructure.Respositories;
using TransiGuide.Data.Repositories;
using TransiGuide.Infrastructure.Repositories;
using TransiGuide.Services.Services;

namespace TransGuideApi.Extentions
{
    public static class AddApplicationServices
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            // ========================
            // Generic Repository
            // ========================
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // ========================
            // API Behavior (Validation Errors)
            // ========================
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(e => e.Value.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    var response = new InvalidBadRequestResponse
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            // ========================
            // AutoMapper Profiles
            // ========================
            services.AddAutoMapper(m => m.AddProfile(typeof(RouteProfile)));
            services.AddAutoMapper(m => m.AddProfile(typeof(HistoryProfile)));
            services.AddAutoMapper(m => m.AddProfile(typeof(FeedbackProfile)));
            services.AddAutoMapper(m => m.AddProfile(typeof(StationProfile)));

            // ========================
            // Repositories
            // ========================
            services.AddScoped<IHistoryRepository, HistoryRepository>();
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();

            // ========================
            // Services
            // ========================
            services.AddScoped<IHistoryServices, HistoryServices>();
            services.AddScoped<IServicesManager, ServicesManager>();
            services.AddScoped<ILocationServices, LocationServices>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IFeedbackService, FeedbackService>();

            services.AddScoped<IRouteServices, RouteServices>();
            services.AddScoped<IStationService, StationService>();
            services.AddScoped<IVoiceServices, VoiceServices>();

            // ⚠️ مهم: GeoLocation لازم Scoped مش Singleton
            services.AddScoped<IGeoLocationService, GeoLocationService>();

            // ========================
            // RabbitMQ / AI / Frame
            // ========================
            services.AddSingleton<IFramePublisher, FramePublisher>();

            services.AddHttpClient<AiService>();

            services.Configure<RabbitMqSettings>(
                configuration.GetSection("RabbitMQ"));

            services.AddHostedService<SignConsumer>();

            // ========================
            // SignalR
            // ========================
            services.AddSignalR();

            // ========================
            // Redis
            // ========================
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString("RedisConnectionString");

                var options = ConfigurationOptions.Parse(connectionString);

                options.AbortOnConnectFail = false;
                options.ConnectTimeout = 100000;

                return ConnectionMultiplexer.Connect(options);
            });

            // ========================
            // HttpContext
            // ========================
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
