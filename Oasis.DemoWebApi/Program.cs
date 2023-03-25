using Oasis.MicroService;

var builder = WebApplication.CreateBuilder(args);

// Add microservices to the container.
var microServiceConfiguration = new MicroServiceConfiguration();
builder.Configuration.GetSection("MicroServices").Bind(microServiceConfiguration);
builder.AddMicroServices(microServiceConfiguration);

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
