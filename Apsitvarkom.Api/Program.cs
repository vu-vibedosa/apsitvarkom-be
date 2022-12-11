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
using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Extensions.DependencyInjection;
using Castle.DynamicProxy;

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

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(autoFacBuilder =>
    {
        autoFacBuilder.Register(_ => new ApiKeyProvider
        {
            Geocoding = builder.Configuration.GetRequiredValue<string>("Geocoding:ApiKey")
        }).As<IApiKeyProvider>();
        autoFacBuilder.Register(c => new HttpClient
            {
                BaseAddress = new Uri(builder.Configuration.GetRequiredValue<string>("Geocoding:Url"))
            }).As<HttpClient>();

        autoFacBuilder.Register(c => new GoogleGeocoder(c.Resolve<HttpClient>(), c.Resolve<IApiKeyProvider>()))
            .As<IGeocoder>()
            .EnableInterfaceInterceptors().InterceptedBy(typeof(GeocoderRequestInterceptor))
            .InstancePerDependency();

        autoFacBuilder.RegisterType<GeocoderRequestInterceptor>().SingleInstance();
        autoFacBuilder.RegisterType<GeocoderRequestInterceptorAsync>().As<IAsyncInterceptor>();
    });

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseLogRequestStatistics();
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
