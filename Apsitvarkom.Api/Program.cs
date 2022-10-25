using Apsitvarkom.Api;
using Apsitvarkom.DataAccess;
using Apsitvarkom.Models.DTO;
using Apsitvarkom.Models.Mapping;
using AutoMapper;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsLocal())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddControllers().AddNewtonsoftJson();
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

var mapsApiKey = builder.Configuration.GetValue<string>("Maps:GoogleMapsApiKey");
builder.Services.AddSingleton<IGeocoder>(_ => new Geocoder(mapsApiKey));

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsLocal())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(FrontEndPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();