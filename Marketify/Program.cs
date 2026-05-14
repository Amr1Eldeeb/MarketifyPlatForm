using FluentValidation.AspNetCore;
using Hangfire;
using Marketify;
using Marketify.Date;
using Marketify.Entites;
using Marketify.PaymentServices;
using Marketify.Roles;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System.Threading.RateLimiting;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDependences(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(
              builder.Configuration.GetConnectionString("HangfireConnection")));

builder.Services.AddHangfireServer();



Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate:
        "{Timestamp:yyyy-MM-dd HH:mm:ss} " +
        "[{Level:u3}] " +
        "[{SourceContext}] " +
        "{Message:lj}" +
        "{NewLine}{Exception}")
    .CreateLogger();
builder.Services.AddRateLimiter(options =>
{
    options.AddConcurrencyLimiter("concurrency", opt =>
    {
        opt.PermitLimit = 2;

        opt.QueueLimit = 1;

        opt.QueueProcessingOrder =
            QueueProcessingOrder.OldestFirst;
    });
});
builder.Host.UseSerilog();
builder.Services.AddHangfireServer();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<PaymobService>();
var app = builder.Build();
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });

    app.MapGet("/", () => Results.Redirect("/swagger"));
}
using (var scope = app.Services.CreateScope())
{
    await DatabaseSeeder.SeedRolesAsync(scope.ServiceProvider);
}
app.UseRateLimiter();

app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();