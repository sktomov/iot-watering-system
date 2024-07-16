using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;

public class MqttClientReceiver : BackgroundService
{
    private readonly BrokerSettings _brokerSettings;
    private readonly IMqttClient _client;
    private readonly MqttClientOptions _clientOptions;
    private readonly string _topic;

    public MqttClientReceiver(IOptions<BrokerSettings> brokerOptions, string topic)
    {
        _brokerSettings = brokerOptions.Value;
        _topic = topic;
        var mqttFactory = new MqttFactory();

        X509Certificate2Collection caChain = new X509Certificate2Collection();
        caChain.ImportFromPem(_brokerSettings.CaLocation); // from https://test.mosquitto.org/ssl/mosquitto.org.crt

        _client = mqttFactory.CreateMqttClient();

        _clientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("test.mosquitto.org", 8883)
                .WithCredentials(_brokerSettings.Username, _brokerSettings.Password)
                .WithTlsOptions(new MqttClientTlsOptionsBuilder()
                    .WithTrustChain(caChain)
                    .Build())
                .Build();
        _client.ApplicationMessageReceivedAsync += HandleMessageAsync;
    }

    async Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
        // _logger.LogInformation("### RECEIVED APPLICATION MESSAGE ###\n{payload}",payload);
        var applicationMessage = new MqttApplicationMessageBuilder()
                        .WithTopic(_topic)
                        .WithPayload("OK")
                        .Build();

        await _client.PublishAsync(applicationMessage, CancellationToken.None);

        //_logger.LogInformation("MQTT application message is published.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _client.ConnectAsync(_clientOptions, CancellationToken.None);
        //_logger.LogInformation("Connected");

        // await _client.SubscribeAsync(_subscriptionOptions, CancellationToken.None);
        //_logger.LogInformation("Subscribed");
    }
}