namespace WateringSystem.Broker.Interfaces;

public interface IMqttSender
{
    Task Send(string topic, string message, CancellationToken cancellationToken);
}