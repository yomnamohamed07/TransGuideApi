
using Microsoft.EntityFrameworkCore;
using TransGuide.Data;
using TransGuide.Data.Repositories;
using TransGuide.Infrastructure.Repositories;
using TransGuideApi.Hub_SignalR;

namespace TransGuideApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
            builder.Services.AddSignalR();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddDbContext<TransGuideDbContext>(options =>
	        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //Repository
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IRatingRepository, RatingRepository>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseAuthorization();
			app.MapControllers();

            app.MapHub<RatingHub>("/ratingHub");

            app.Run();
		}
	}
}
