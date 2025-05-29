using CommonLibrary.Config;
using CommonLibrary.Interface;
using CommonLibrary.Services;
using SimulatorAPI.Service;

var builder = WebApplication.CreateBuilder(args);

//var appSettings = builder.Configuration.Get<AppSettings>();
//builder.Services.AddSingleton(appSettings);


//The section names in appsettings.json must match the class/property names (AppSettings → Kafka → properties).
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddHostedService<SimulatorService>();

//ToDo: Add logging, health checks, and other middlewares as needed

var app = builder.Build();
app.MapGet("/", () => "SimulatorAPI running");
app.Run();