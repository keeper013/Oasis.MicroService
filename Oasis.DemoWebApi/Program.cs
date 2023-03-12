using System.Reflection;
using Oasis.MicroService;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Add microservices to the container.
IList<string> microServiceAssemblies = new List<string>();
config.GetSection("MicroServices").Bind(microServiceAssemblies);
builder.Services.AddMicroServices(microServiceAssemblies);

// cors
// var corsConfigConfig = new CorsConfiguration.CorsConfig();
// config.GetSection(CorsConfiguration.CorsConfig.SectionName).Bind(corsConfigConfig);
// var corsConfig = new CorsConfiguration(corsConfigConfig);
// corsConfig.ConfigureServices(builder.Services);

// byte[] formatter for protobuf budy
// builder.Services.AddControllers(options => options.InputFormatters.Add(new ByteArrayInputFormatter()));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var swaggerConfig = new SwaggerConfiguration();
swaggerConfig.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
swaggerConfig.Configure(app, app.Environment);

// corsConfig.Configure(app, app.Environment);

app.MapControllers();

app.Run();
