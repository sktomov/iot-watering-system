using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using WateringSystem;
using WateringSystem.Broker;
using WateringSystem.Commands;
using WateringSystem.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddMqttReceiver<StatusReceiver>(Topics.Status, Guid.NewGuid().ToString());
builder.Services.Configure<BrokerSettings>(
    builder.Configuration.GetSection(BrokerSettings.SectionName));
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.EnableAnnotations();
});

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.AllowTrailingCommas = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.ReferenceHandler =ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.MapPost("/api/send-to-topic", async (TestCommand request, IMediator mediator) =>
    {
        var result = await mediator.Send(request);
        return Results.Ok(result);
    })
    .WithMetadata(new SwaggerOperationAttribute("summary", "Send message to topic"));;

app.Run();