using MediatR;
using WateringSystem.Data;

namespace WateringSystem.Commands.Status;

public class StatusHandler(IStatusRepository statusRepository) : IRequestHandler<StatusRequest, StatusResponse>
{
    public async Task<StatusResponse> Handle(StatusRequest request, CancellationToken cancellationToken)
    {
        var lastStatus = await statusRepository.GetLastStatusAsync();

        if (lastStatus == null)
            return new StatusResponse();

        var result =  new StatusResponse
        {
            Distance = lastStatus.Distance,
            Pump = lastStatus.Pump,
            Strawberries = lastStatus.Strawberries,
            Tomatoes = lastStatus.Tomatoes,
            StrawberriesHours = lastStatus.StrawberriesHours,
            PumpingHours = lastStatus.PumpingHours,
            TomatoesHours = lastStatus.TomatoesHours
        };
        
        // TODO: Pass last date + time of watering

        return result;
    }
}