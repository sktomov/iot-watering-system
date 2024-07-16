var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<MqttClientReceiver>();

var app = builder.Build();

// To check if web server is still responsive
app.MapGet("/", () =>
{
    return "Hello World";
});


app.Run();