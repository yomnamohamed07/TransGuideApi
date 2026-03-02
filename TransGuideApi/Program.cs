using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TransGuide.Data;
using TransGuide.Data.Entities.Identity;
using TransGuide.Services.Services;
using TransGuideApi.MiddleWare;
using TransGuideApi.Extentions;
using TransGuide.Infrustructure.data;
using TransGuide.Data.Repositories;
using TransGuide.Infrastructure.Repositories;

namespace TransGuideApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

           
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TransiGuide API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter token as: Bearer <your-token>"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            // DbContext
            builder.Services.AddDbContext<TransGuideDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            });

            // Identity configuration
            builder.Services.AddIdentity<UserProfile, IdentityRole<int>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<TransGuideDbContext>()
            .AddDefaultTokenProviders();

            // JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "TransiGuide",
                    ValidAudience = "TransiGuideUsers",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsASecureKeyForTransiGuide!2025"))
                };
            });

            // Application Services
            builder.Services.AddSingleton<ResetCodeService>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddApplicationService(builder.Configuration);

      
            var app = builder.Build();

            
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var dbContext = services.GetRequiredService<TransGuideDbContext>();
                    await dbContext.Database.MigrateAsync();
                    await dataseeding.SeedAsync(dbContext);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "Error occurred during applying migration");
                }
            }

          
            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddleWare>();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}