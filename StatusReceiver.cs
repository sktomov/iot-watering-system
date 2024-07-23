using WateringSystem.Broker;
using WateringSystem.Broker.Interfaces;
using WateringSystem.Data;
using WateringSystem.Data.Entities;
using System.Text.Json;

namespace WateringSystem;

public class StatusReceiver(IStatusRepository statusRepository) : IMqttReceiver
{
    public async Task HandleMessageAsync(string message)
    {
        var statusMessage = JsonSerializer.Deserialize<Status>(message, new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        await statusRepository.InsertStatusAsync(statusMessage);
    }
}