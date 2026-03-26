using FluentValidation;
using Marketify.Date;
using Marketify.Entites;
using Marketify.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Marketify.Authentication;
namespace Marketify
{
    public  static class DependencyInjection
    {
        public static IServiceCollection AddDependences(this IServiceCollection services,
            IConfiguration configuration)
        {   
            services.AddCors(Options => Options.
            AddDefaultPolicy(builder => builder.
            AllowAnyOrigin().
            AllowAnyHeader()
            .AllowAnyMethod()
            
            ));
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtProvider , JwtProvider>();
            services.AddControllers();
            services.AddAuthorazationsConfig(configuration);
            var connencationString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection String is Not Found");

            services.AddDbContext
                <ApplicationDbContext>
                (options => options.UseSqlServer(connencationString));
           services.AddControllers(); 
            services.AddOpenApi();
            return services;
        }
        public static IServiceCollection AddAuthorazationsConfig(this IServiceCollection services,
            IConfiguration confg)
        {


            services.AddIdentity<ApplicationUser, IdentityRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>();
            return services;
        }
        public static IServiceCollection AddFluentValidation(this IServiceCollection services,
           IConfiguration confg)
        {


            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            _ = services.AddFluentValidationAutoValidation();


            return services;
        }

    }
}
