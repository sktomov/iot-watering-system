using MediatR;
using WateringSystem.Broker;
using WateringSystem.Broker.Interfaces;

namespace WateringSystem.Commands.Schedule;

public class ScheduleHandler(IMqttSender mqttSender) : IRequestHandler<ScheduleRequest, Unit>
{
    public async Task<Unit> Handle(ScheduleRequest request, CancellationToken cancellationToken)
    {
        await mqttSender.SendJsonAsync(Topics.Schedule, request, cancellationToken);
        
        return Unit.Value;
    }
}