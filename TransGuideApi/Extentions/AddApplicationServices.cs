
using TransGuide.Data.Repositories;
using TransGuide.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using TransGuideApi.Errors;
using TransGuide.Services;
using TransGuide.Data.Services;
using TransGuide.Services.Mapper;
using StackExchange.Redis;
using TransGuide.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using TransGuide.Data;
namespace TransGuideApi.Extentions
{
	
		public static class AddApplicationServices
		{
			public static IServiceCollection AddApplicationService(this IServiceCollection Services, IConfiguration configuration)
			{
				Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            Services.AddScoped<IRouteRepository, RouteRepository>();
            Services.AddScoped<ILocationServices, LocationServices>();
            Services.AddAutoMapper(typeof(RouteProfile));



            //builder.Services.AddAutoMapper(m=>m.AddProfile(new MappingProfiles()));
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
			Services.AddScoped<TransGuide.Data.Respositories.IHistoryRepository, TransGuide.Infrustructure.Respositories.HistoryRepository>();
			Services.AddScoped<IHistoryServices, HistoryServices>();

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

