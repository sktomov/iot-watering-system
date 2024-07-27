namespace WateringSystem.Broker.Interfaces;

public interface IMqttSender
{
    Task SendAsync(string topic, string message, CancellationToken cancellationToken);

    Task SendJsonAsync(string topic, object message, CancellationToken cancellationToken);
}