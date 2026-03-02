
using TransGuide.Data.Respositories;
using TransGuide.Infrustructure.Respositories;
using Microsoft.AspNetCore.Mvc;
using TransGuideApi.Errors;
using TransGuide.Services;
using TransGuide.Data.Services;
using StackExchange.Redis;
using Microsoft.AspNetCore.Identity;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data;
using AutoMapper;
using TransGuide.Services.Mapper;
using TransGuide.Services.Services;
using TransGuide.Infrastructure.Repositories;
using TransGuide.Data.Repositories;
using TransiGuide.Services.Services;
using TransiGuide.Data.Repositories;
using TransiGuide.Infrastructure.Repositories;
using TransGuide.Services.Mappings;
namespace TransGuideApi.Extentions
{
	
		public static class AddApplicationServices
		{
			public static IServiceCollection AddApplicationService(this IServiceCollection Services, IConfiguration configuration)
			{
				
			Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

		
            Services.Configure<ApiBehaviorOptions>(
					Options => Options.InvalidModelStateResponseFactory = (actioncontext) => {

						var errors = actioncontext.ModelState.Where(e => e.Value.Errors.Count() > 0)
						.SelectMany(e => e.Value.Errors)
						.Select(e => e.ErrorMessage)
						.ToList();
						var invaliderrors = new InvalidBadRequestResponse()
						{
							Errors = errors
						};
						return new BadRequestObjectResult(invaliderrors);
					}

					);
            Services.AddAutoMapper(m => m.AddProfile(typeof(RouteProfile)));
            Services.AddAutoMapper(m => m.AddProfile(typeof(HistoryProfile)));

            Services.AddScoped<TransGuide.Data.Respositories.IHistoryRepository, TransGuide.Infrustructure.Respositories.HistoryRepository>();
			Services.AddScoped<IHistoryServices, HistoryServices>();
			Services.AddScoped<IRouteRepository, RouteRepository>();
			Services.AddScoped<ILocationServices, LocationServices>(); 
			Services.AddAutoMapper(typeof(UserProfileMapping));

         Services.AddScoped<IAuthService, AuthService>();

          Services.AddSignalR();

           Services.AddScoped<INotificationRepository, NotificationRepository>();
            Services.AddScoped<INotificationService, NotificationService>();


            Services.AddSingleton<IConnectionMultiplexer>((_) =>
            {
                return ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnectionString"));
            });
            Services.AddScoped<IServicesManager, ServicesManager>();
            //Services.AddIdentity<UserProfile, IdentityRole>();
            //Services.AddIdentiy<>().AddEntityFrameworkStores<TransGuideDbContext>() .AddDefaultTokenProviders();
            Services.AddIdentity<UserProfile, IdentityRole<int>>().AddEntityFrameworkStores<TransGuideDbContext>().AddDefaultTokenProviders();
            Services.AddHttpContextAccessor();
             // Services.AddScoped<ICurrentUserService, CurrentUserService>();


            return Services;
			}
		}
	}

