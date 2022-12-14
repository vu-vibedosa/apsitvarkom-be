using Apsitvarkom.Configuration;
using Apsitvarkom.DataAccess;
using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Apsitvarkom.ModelActions.Mapping;
using Apsitvarkom.ModelActions.Validation;
using Apsitvarkom.Models;
using Apsitvarkom.Api.Middleware;
using System.Reflection;

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
            Title = "Apsitvarkom RESTful API",
            Version = "v1",
            Contact = new()
            {
                Name = "vu-vibedosa",
                Url = new("https://github.com/vu-vibedosa")
            }
        }
     );
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services
    .AddAutoMapper(typeof(PollutedLocationProfile))
    .AddValidatorsFromAssemblyContaining<CoordinatesCreateRequestValidator>()
    .AddFluentValidationRulesToSwagger();

const string FrontEndPolicy = "FrontEndPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontEndPolicy, policy => policy.WithOrigins(builder.Configuration.GetRequiredValue<string>("FrontEndOrigin")).AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddScoped<IPollutedLocationContext>(provider => provider.GetRequiredService<PollutedLocationContext>());
builder.Services.AddDbContext<PollutedLocationContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ApsitvarkomDatabase")));

builder.Services.AddScoped<IPollutedLocationRepository, PollutedLocationDatabaseRepository>();
builder.Services.AddScoped<IRepository<CleaningEvent>, CleaningEventDatabaseRepository>();

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

    pollutedLocationContext.Instance.Database.Migrate();

    if (!pollutedLocationContext.PollutedLocations.Any())
        await DbInitializer.InitializePollutedLocations(pollutedLocationContext);
}

app.UseExceptionHandling();
app.UseCors(FrontEndPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();
