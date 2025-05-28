using CommonLibrary.Config;
using LocationSpeedService;

var builder = Host.CreateApplicationBuilder(args);

var settings = builder.Configuration.Get<AppSettings>();
builder.Services.AddSingleton(settings);
builder.Services.AddHostedService<KafkaConsumerService>();

var host = builder.Build();
host.Run();