
using TransGuide.Data.Repositories;
using TransGuide.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using TransGuideApi.Errors;
namespace TransGuideApi.Extentions
{
	
		public static class AddApplicationServices
		{
			public static IServiceCollection AddApplicationService(this IServiceCollection Services, IConfiguration configuration)
			{
				Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			

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

