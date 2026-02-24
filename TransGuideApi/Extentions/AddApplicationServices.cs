
using TransGuide.Data.Repositories;
using TransGuide.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using TransGuideApi.Errors;
using TransGuide.Services;
using TransGuide.Data.Services;
using TransGuide.Services.Mapper;
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
				
				return Services;
			}
		}
	}

