using Apsitvarkom.Configuration;
using Apsitvarkom.DataAccess;
using Apsitvarkom.Models.DTO;
using Apsitvarkom.Models.Mapping;
using Apsitvarkom.Utilities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAutoMapper(typeof(PollutedLocationProfile))
    .AddValidatorsFromAssemblyContaining<PollutedLocationDTOValidator>();

builder.Services.AddLogging();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFile("ApsitvarkomApi.log");

const string FrontEndPolicy = "FrontEndPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontEndPolicy, policy => policy.WithOrigins(builder.Configuration.GetValue<string>("FrontEndOrigin")));
});

builder.Services.AddDbContext<PollutedLocationContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ApsitvarkomDatabase")));

builder.Services.AddScoped<ILocationDTORepository<PollutedLocationDTO>, PollutedLocationDTODatabaseRepository>();

builder.Services.AddSingleton<IApiKeyProvider, ApiKeyProvider>(_ => new()
{
    Geocoding = builder.Configuration.GetRequiredValue<string>("Geocoding:ApiKey")
});

builder.Services.AddHttpClient<IGeocoder, GoogleGeocoder>(client =>
{
    var geocodingApiUrl = builder.Configuration.GetRequiredValue<string>("Geocoding:Url");
    client.BaseAddress = new Uri(geocodingApiUrl);
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var pollutedLocationContext = services.GetRequiredService<PollutedLocationContext>();

    // TODO: switch to migrations
    pollutedLocationContext.Database.EnsureCreated();

    if (!pollutedLocationContext.PollutedLocations.Any())
        DbInitializer.InitializePollutedLocations(pollutedLocationContext);
}

app.UseCors(FrontEndPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();
