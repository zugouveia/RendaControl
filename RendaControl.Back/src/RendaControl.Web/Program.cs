using RendaControl.Application;
using RendaControl.Persistence;
using RendaControl.Persistence.Context;
using RendaControl.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
const string FrontendCorsPolicy = "FrontendCorsPolicy";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
    [
        "http://localhost:5500",
        "https://rendacontrol-front.vercel.app"
    ];
var resetDatabaseOnStartup = builder.Configuration.GetValue<bool>("Startup:ResetDatabaseOnStartup");
var seedMockDataOnStartup = builder.Configuration.GetValue<bool>("Startup:SeedMockDataOnStartup");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (resetDatabaseOnStartup)
    {
        dbContext.Database.EnsureDeleted();
    }

    dbContext.Database.EnsureCreated();
    if (seedMockDataOnStartup)
    {
        await MockDataSeeder.SemearTudoAsync(dbContext);
    }
}

app.Run();
