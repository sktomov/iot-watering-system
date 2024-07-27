using MediatR;

namespace WateringSystem.Commands.Schedule;

public class ScheduleRequest : IRequest<Unit>
{
    public List<int> StrawberriesHours { get; set; }
    
    public List<int> TomatoesHours { get; set; }
   
    public List<int> PumpingHours { get; set; }
}