using MediatR;

namespace WateringSystem.Commands.Set;

public class SetIrrigationRequest : IRequest<Unit>
{
    public string Field { get; set; }

    public bool Value { get; set; }
}