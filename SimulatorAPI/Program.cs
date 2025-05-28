using CommonLibrary.Config;
using CommonLibrary.Interface;
using CommonLibrary.Services;
using SimulatorAPI.Service;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration.Get<AppSettings>();
builder.Services.AddSingleton(appSettings);

builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddHostedService<SimulatorService>();

var app = builder.Build();
app.MapGet("/", () => "SimulatorAPI running");
app.Run();