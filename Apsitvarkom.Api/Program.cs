using Apsitvarkom.Api;
using Apsitvarkom.DataAccess;
using Apsitvarkom.Models.DTO;
using Apsitvarkom.Models.Mapping;
using Apsitvarkom.Utilities;
using AutoMapper;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAutoMapper(typeof(PollutedLocationProfile))
    .AddValidatorsFromAssemblyContaining<PollutedLocationDTOValidator>();

//builder.Services.AddLogging();
//builder.Logging.ClearProviders();
//builder.Logging.AddFile("C:\\antriMetai\\PSI\\Apsitvarkom\\apsitvarkom-be\\Apsitvarkom.Utilities\\");


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