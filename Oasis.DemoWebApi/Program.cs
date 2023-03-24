using Oasis.MicroService;

var builder = WebApplication.CreateBuilder(args);

// Add microservices to the container.
var microServiceConfigurations = new List<MicroServiceConfiguration>();
builder.Configuration.GetSection("MicroServices").Bind(microServiceConfigurations);
builder.AddMicroServices(microServiceConfigurations);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
