using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TransGuide.Data;
using TransGuide.Data.Entities.Identity;
using TransGuide.Services.Services;
using TransGuide.Services.Mapper;
using TransGuide.Data.Services;
using System.Security.Claims;

namespace TransGuideApi.Extentions
{
    public static class AddIdentityServices
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddAutoMapper(typeof(UserProfileMapping));

            Services.AddScoped<IAuthService, AuthService>();
            Services.AddScoped<IAuthorizationService, AuthorizationService>();
           

           
            Services.AddIdentity<UserProfile, IdentityRole<int>>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<TransGuideDbContext>()
            .AddDefaultTokenProviders();

            
            Services.AddAuthentication(options =>
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

                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:Key"])),

                    RoleClaimType = ClaimTypes.Role
                };
            });

            return Services;
        }
    }
}