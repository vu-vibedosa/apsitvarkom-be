using Apsitvarkom.Configuration;
using Apsitvarkom.DataAccess;
using Apsitvarkom.Models.Mapping;
using Apsitvarkom.Models.Public;
using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
    );
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Apsitvarkom REST API",
            Version = "v1",
            Contact = new()
            {
                Name = "vu-vibedosa",
                Url = new("https://github.com/vu-vibedosa")
            }
        }
     );
});

builder.Services
    .AddAutoMapper(typeof(PollutedLocationProfile))
    .AddAutoMapper(typeof(TidyingEventProfile))
    .AddValidatorsFromAssemblyContaining<CoordinatesCreateRequestValidator>()
    .AddFluentValidationRulesToSwagger();

const string FrontEndPolicy = "FrontEndPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontEndPolicy, policy => policy.WithOrigins(builder.Configuration.GetValue<string>("FrontEndOrigin")));
});

builder.Services.AddScoped<IPollutedLocationContext>(provider => provider.GetRequiredService<PollutedLocationContext>());
builder.Services.AddDbContext<PollutedLocationContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ApsitvarkomDatabase")));

builder.Services.AddScoped<IPollutedLocationRepository, PollutedLocationDatabaseRepository>();

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
    var pollutedLocationContext = services.GetRequiredService<IPollutedLocationContext>();

    // TODO: switch to migrations
    pollutedLocationContext.Instance.Database.EnsureCreated();

    if (!pollutedLocationContext.PollutedLocations.Any())
        await DbInitializer.InitializePollutedLocations(pollutedLocationContext);
}

app.UseCors(FrontEndPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();
