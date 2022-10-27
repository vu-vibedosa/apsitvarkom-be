using Apsitvarkom.Configuration;
using Apsitvarkom.DataAccess;
using Apsitvarkom.Models.DTO;
using Apsitvarkom.Models.Mapping;
using AutoMapper;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAutoMapper(typeof(PollutedLocationProfile))
    .AddValidatorsFromAssemblyContaining<PollutedLocationDTOValidator>();

const string FrontEndPolicy = "FrontEndPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontEndPolicy, policy => policy.WithOrigins(builder.Configuration.GetValue<string>("FrontEndOrigin")));
});

builder.Services.AddScoped<ILocationDTORepository<PollutedLocationDTO>>(serviceProvider =>
{
    var mapper = serviceProvider.GetRequiredService<IMapper>();
    return PollutedLocationDTOFileRepository.FromFile(sourcePath: "PollutedLocationsMock.json", mapper: mapper);
});

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
}

app.UseCors(FrontEndPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();