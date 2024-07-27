using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using WateringSystem;
using WateringSystem.Broker;
using WateringSystem.Commands;
using WateringSystem.Commands.Status;
using WateringSystem.Data;

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
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

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy  =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyOrigin();
            policy.WithOrigins("http://localhost:3000",
                "https://www.le6o.net");
            policy.AllowAnyMethod();
        });
});

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

app.ConfigureMinimalApis();

app.UseCors(MyAllowSpecificOrigins);
app.Run();