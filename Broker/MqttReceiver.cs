using System.Text;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using WateringSystem.Broker.Interfaces;

namespace WateringSystem.Broker;

public class MqttReceiver<TReceiver> : BackgroundService
    where TReceiver : IMqttReceiver
{
    private readonly ILogger<TReceiver> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMqttClient _client;
    private readonly MqttClientOptions _clientOptions;
    private readonly MqttClientSubscribeOptions _subscriptionOptions;

    public MqttReceiver(string topic, 
        string clientId,
        IOptions<BrokerSettings> brokerOptions,
        ILogger<TReceiver> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();
        _clientOptions = MqttClientHelper.GetClientOptions(brokerOptions.Value, clientId);
        _subscriptionOptions = factory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f =>
            {
                f.WithTopic(topic);
                f.WithAtLeastOnceQoS();
            })
            .Build();
        _client.ApplicationMessageReceivedAsync += HandleMessageAsync;
    }

    private async Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
        _logger.LogInformation("### RECEIVED APPLICATION MESSAGE ###\n{payload}", payload);
        using var scope = _serviceScopeFactory.CreateScope();
        var receiverInstance = scope.ServiceProvider.GetRequiredService<TReceiver>();
        await receiverInstance.HandleMessageAsync(payload);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _client.ConnectAsync(_clientOptions, stoppingToken);
            _logger.LogInformation("Connected");
        }
        catch (Exception e)
        {
            _logger.LogCritical(e.ToString());
        }

        await _client.SubscribeAsync(_subscriptionOptions, stoppingToken);
        _logger.LogInformation("Subscribed");
    }
}