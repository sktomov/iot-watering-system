using WateringSystem.Broker;
using WateringSystem.Broker.Interfaces;

namespace WateringSystem;

public class StatusReceiver : IMqttReceiver
{
    public async Task HandleMessageAsync(string message)
    {
        Console.WriteLine(message);
    }
}