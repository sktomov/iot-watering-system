using MediatR;

namespace WateringSystem.Commands;

public class TestCommand : IRequest<Unit>
{
    public string Topic { get; set; }
    public string SomeText { get; set; }
}