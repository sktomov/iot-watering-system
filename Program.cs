using WateringSystem.Broker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<MqttClientReceiver>();
builder.Services.Configure<BrokerSettings>(
    builder.Configuration.GetSection(BrokerSettings.SectionName));

var app = builder.Build();

// To check if web server is still responsive
app.MapGet("/", () =>
{
    return "Hello World";
});


app.Run();