using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data;
using TransGuide.Services.Services;
using TransGuide.Data.Services;
using TransGuide.Services.Mapper;

namespace TransGuideApi.Extentions
{
    public  static class AddIdentityServices
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection Services , IConfiguration configuration)
        {

            Services.AddApplicationService(configuration);
            // Identity configuration
            Services.AddAutoMapper(typeof(UserProfileMapping));
            Services.AddScoped<IAuthService, AuthService>();

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
                    ValidIssuer = "TransiGuide",
                    ValidAudience = "TransiGuideUsers",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsASecureKeyForTransiGuide!2025"))
                };
            });


           Services.AddSingleton<ResetCodeService>();

            return Services;
        }
        
    }
}
