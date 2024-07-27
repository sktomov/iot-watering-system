using System.Text.Json;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using WateringSystem.Broker.Interfaces;

namespace WateringSystem.Broker;

public class MqttSender : IMqttSender
{
    private readonly ILogger<MqttSender> _logger;
    private readonly IMqttClient _client;
    private readonly MqttClientOptions _clientOptions;

    public MqttSender(IOptions<BrokerSettings> brokerOptions,
        ILogger<MqttSender> logger)
    {
        _logger = logger;
        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();
        _clientOptions = MqttClientHelper.GetClientOptions(brokerOptions.Value, Guid.NewGuid().ToString());

    }

    public async Task SendAsync(string topic, string message, CancellationToken cancellationToken)
        => await SendInternalAsync(topic, message, cancellationToken);

    public async Task SendJsonAsync(string topic, object message, CancellationToken cancellationToken)
    {
        var messageTosend = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await SendInternalAsync(topic, messageTosend, cancellationToken);
    }

    private async Task SendInternalAsync(string topic, string message, CancellationToken cancellationToken)
    {
        if (!_client.IsConnected)
            await Connect(cancellationToken);
        
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(message)
            .Build();
    
        await _client.PublishAsync(applicationMessage, cancellationToken);
        _logger.LogInformation("### SEND APPLICATION MESSAGE TO TOPIC {topic} ###\n{payload}", topic, message);
    }

    private async Task Connect(CancellationToken cancellationToken)
    {
        try
        {
            await _client.ConnectAsync(_clientOptions, cancellationToken);
            _logger.LogInformation("Connected");
        }
        catch (Exception e)
        {
            _logger.LogCritical(e.ToString());
        }
    }
}