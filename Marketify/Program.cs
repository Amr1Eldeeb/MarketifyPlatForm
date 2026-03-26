using Marketify;
using Marketify.Date;
using Marketify.Entites;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDependences(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//builder.Services
//    .AddIdentityApiEndpoints<ApplicationUser>().AddEntityFrameworkStores<ApplicationDbContext>();
var app = builder.Build();
app.UseCors("AllowAll");
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

app.UseHttpsRedirection();
app.UseAuthorization();
//app.MapIdentityApi<ApplicationUser>();
app.MapControllers();

app.Run();