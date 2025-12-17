using TransGuideApi.MiddleWare;
using Microsoft.EntityFrameworkCore;
using TransGuide.Data;
using TransGuide.Infrustructure.data;

namespace TransGuideApi
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			//builder.Services.AddDbContext<TransGuideDbContext>(options =>
	      //  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
			builder.Services.AddDbContext<TransGuideDbContext>(options =>
			{
				options.UseSqlServer(
					builder.Configuration.GetConnectionString("DefaultConnection"));

				options.EnableSensitiveDataLogging();
			});

			var app = builder.Build();
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var loggerFactory = services.GetRequiredService<ILoggerFactory>();

				try
				{
					var dbcontext = services.GetRequiredService<TransGuideDbContext>();
					await dbcontext.Database.MigrateAsync();
					await dataseeding.SeedAsync(dbcontext);
				}
				catch (Exception ex)
				{
					var logger = loggerFactory.CreateLogger<Program>();
					logger.LogError(ex, "Error occurred during applying migration");
				}
			}
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMiddleware<ExceptionMiddleWare>();
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
