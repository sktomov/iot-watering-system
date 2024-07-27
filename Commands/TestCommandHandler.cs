using MediatR;
using WateringSystem.Broker.Interfaces;

namespace WateringSystem.Commands;

public class TestCommandHandler(IMqttSender mqttSender) : IRequestHandler<TestCommand, Unit>
{
    public async Task<Unit> Handle(TestCommand request, CancellationToken cancellationToken)
    {
        await mqttSender.SendAsync(request.Topic, request.SomeText, cancellationToken);
        return Unit.Value;
    }
}