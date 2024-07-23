namespace WateringSystem.Broker.Interfaces;

public interface IMqttReceiver
{
    Task HandleMessageAsync(string message);
}