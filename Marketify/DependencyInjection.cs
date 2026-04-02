using FluentValidation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Marketify.Authentication;
using Marketify.Contracts.Authenthication;
using Marketify.Date;
using Marketify.Entites;
using Marketify.Services;
using Marketify.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
namespace Marketify
{
    public  static class DependencyInjection
    {
        public static IServiceCollection AddDependences(this IServiceCollection services,
            IConfiguration configuration)
        {
            //services.AddCors(Options => Options.
            //AddDefaultPolicy(builder => builder.
            //AllowAnyOrigin().
            //AllowAnyHeader()
            //.AllowAnyMethod()

            //));
            services.AddScoped<IEmailSender, EmailServecies>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddAuthConfig( configuration);
            services.AddControllers();
            services.AddAuthorazationsConfig(configuration);
            var connencationString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection String is Not Found");
            services.AddDbContext
                <ApplicationDbContext>
                (options => options.UseSqlServer(connencationString));
           services.AddControllers(); 
            services.AddOpenApi();
            //services.AddFluentValidation();
            return services;
        }
        public static IServiceCollection AddAuthorazationsConfig(this IServiceCollection services,
            IConfiguration confg)
        {
        

            // to make code number not string
            services.ConfigureApplicationCookie(options => {
                options.Events.OnRedirectToLogin = context => {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            });
            return services;
        }
        public static IServiceCollection AddFluentValidation(this IServiceCollection services,
           IConfiguration confg)
        {


            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            _ = services.AddFluentValidationAutoValidation();


            return services;
        }
        public static IServiceCollection AddAuthConfig(this IServiceCollection services , IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.Configure<JwtOptions>(configuration.GetSection("Jwt")); 
            var Jwtsettings = configuration.GetSection("Jwt").Get<JwtOptions>();

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = Jwtsettings!.Issuer,
                    ValidAudience = Jwtsettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(Jwtsettings.key))
                
                };
            });
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider
                ; options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
