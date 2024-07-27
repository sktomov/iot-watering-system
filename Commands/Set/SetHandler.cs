using MediatR;
using WateringSystem.Broker;
using WateringSystem.Broker.Interfaces;

namespace WateringSystem.Commands.Set;

public class SetHandler(IMqttSender mqttSender) : IRequestHandler<SetIrrigationRequest, Unit>
{
    private readonly List<string> _validFields = ["irrigationOn", "irrigationStrawberries", "irrigationTomatoes", "pumpOn"];
    
    public async Task<Unit> Handle(SetIrrigationRequest request, CancellationToken cancellationToken)
    {
        if(!_validFields.Contains(request.Field))
            return Unit.Value;
        
        await mqttSender.SendJsonAsync(Topics.SetIrrigation, request, cancellationToken);
        
        return Unit.Value;
    }
}