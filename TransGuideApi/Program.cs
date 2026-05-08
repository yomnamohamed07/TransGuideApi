using Microsoft.EntityFrameworkCore;
using TransGuide.Data;
using TransGuideApi.MiddleWare;
using TransGuideApi.Extentions;
using AspNetCoreRateLimit;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.Repositories;
using TransGuide.Data.Services;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Infrustructure.data;

namespace TransGuideApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

        
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            
            builder.Services.AddApplicationService(builder.Configuration);
            builder.Services.AddIdentityService(builder.Configuration);

            
            builder.Services.AddDbContext<TransGuideDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));

                options.EnableSensitiveDataLogging();
            });

            builder.Services.AddDbContext<TransGuideDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("IdentityDefaultConnection"));
            });

           
            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(
                builder.Configuration.GetSection("IpRateLimiting"));

            builder.Services.Configure<IpRateLimitPolicies>(
                builder.Configuration.GetSection("IpRateLimitPolicies"));

            builder.Services.AddInMemoryRateLimiting();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin();
                });
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

                  
                    var userManager = services.GetRequiredService<UserManager<UserProfile>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

                    await RoleSeeding.SeedAsync(roleManager);
                    await UserSeeding.SeedAsync(userManager);

                    
                    var geo = services.GetRequiredService<IGeoLocationService>();
                    var stationRepo = services.GetRequiredService<IGenericRepository<Station>>();

                    var stations = await stationRepo.GetAllAsync();

                    if (stations != null && stations.Any())
                    {
                        await geo.AddStationsAsync(stations);
                    }
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "Error occurred during startup seeding");
                }
            }

        
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowFrontend");

            app.UseMiddleware<ExceptionMiddleWare>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}