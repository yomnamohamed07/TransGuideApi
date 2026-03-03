
using Microsoft.EntityFrameworkCore;
using TransGuide.Data;
using TransGuide.Infrustructure.data;
using TransGuideApi.MiddleWare;
using TransGuideApi.Extentions;
using System;

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
            builder.Services.AddApplicationService(builder.Configuration);


            //builder.Services.AddDbContext<TransGuideDbContext>(options =>
            // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDbContext<TransGuideDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));

                options.EnableSensitiveDataLogging();
            });
            builder.Services.AddDbContext<TransGuideDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDefaultConnection"));
            }
            );


            builder.Services.AddApplicationService(builder.Configuration);
            builder.Services.AddIdentityService(builder.Configuration);
       

            var app = builder.Build();


            #region update database
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
                #endregion

              
                if (app.Environment.IsDevelopment())
                {
                    app.UseMiddleware<ExceptionMiddleWare>();
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }


                app.UseHttpsRedirection();
                app.UseStaticFiles();
             
                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

                app.Run();


           
            }
        }
    }
}