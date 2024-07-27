using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using WateringSystem.Commands;
using WateringSystem.Commands.Schedule;
using WateringSystem.Commands.Set;
using WateringSystem.Commands.Status;

namespace WateringSystem;

public static class WebApplicationExtensions
{
    public static void ConfigureMinimalApis(this WebApplication app)
    {
        // api/irrigation/status
        // api/watering-schedule
        app.MapGet("api/irrigation/status", async (IMediator mediator) =>
            {
                var request = new StatusRequest();
                var result = await mediator.Send(request);
                return Results.Ok(result);
            })
            .WithMetadata(new SwaggerOperationAttribute("summary", "Get irrigation status"));
        
        app.MapPost("api/watering-schedule", async (ScheduleRequest scheduleRequest,IMediator mediator) =>
            {
                var result = await mediator.Send(scheduleRequest);
                return Results.Ok(result);
            })
            .WithMetadata(new SwaggerOperationAttribute("summary", "Set schedule"));
        
        app.MapPost("api/irrigation/set", async (SetIrrigationRequest request,IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                return Results.Ok(result);
            })
            .WithMetadata(new SwaggerOperationAttribute("summary", "Set irrigation status"));

        app.MapPost("/api/send-to-topic", async (TestCommand request, IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                return Results.Ok(result);
            })
            .WithMetadata(new SwaggerOperationAttribute("summary", "Send message to topic"));
    }
}