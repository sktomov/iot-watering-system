using System.Reflection;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using WateringSystem;
using WateringSystem.Commands;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMqttReceiver<StatusReceiver>("test/status", Guid.NewGuid().ToString());
builder.Services.Configure<BrokerSettings>(
    builder.Configuration.GetSection(BrokerSettings.SectionName));
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.EnableAnnotations();
});

builder.Services.AddHttpContextAccessor();

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