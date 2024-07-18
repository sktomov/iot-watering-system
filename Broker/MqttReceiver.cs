using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;

namespace WateringSystem.Broker;

public class MqttClientReceiver : BackgroundService
{
    private readonly ILogger<MqttClientReceiver> _logger;
    private readonly IMqttClient _client;
    private readonly MqttClientOptions _clientOptions;
    private readonly string _topic;
    private readonly MqttClientSubscribeOptions _subscriptionOptions;

    public MqttClientReceiver(IOptions<BrokerSettings> brokerOptions, ILogger<MqttClientReceiver> logger)
    {
        _logger = logger;
        var brokerSettings = brokerOptions.Value;
        _topic = "test/broker";
        var mqttFactory = new MqttFactory();

        var caCrt = new X509Certificate2(File.ReadAllBytes(brokerSettings.CaLocation));
        _client = mqttFactory.CreateMqttClient();
        var tlsOptions = new MqttClientTlsOptions
        {
            UseTls = true,
            SslProtocol = SslProtocols.Tls12,
            CertificateValidationHandler = (certContext) =>
            {
                var chain = new X509Chain();
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                chain.ChainPolicy.VerificationTime = DateTime.Now;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);
                chain.ChainPolicy.CustomTrustStore.Add(caCrt);
                chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                // convert provided X509Certificate to X509Certificate2
                var x5092 = new X509Certificate2(certContext.Certificate);
                return chain.Build(x5092);
            }
        };
        _subscriptionOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f =>
            {
                f.WithTopic(_topic);
                f.WithAtLeastOnceQoS();
            })
            .Build();
        // _clientOptions = new MqttClientOptionsBuilder()
        //     .WithTcpServer(brokerSettings.Host, brokerSettings.Port)
        //     .WithCredentials(brokerSettings.Username, brokerSettings.Password)
        //     //.WithTlsOptions(new MqttClientTlsOptionsBuilder()
        //     // .WithTrustChain(caChain)
        //     // .Build())
        //     .WithTlsOptions(tlsOptions)
        //     .Build();

        _clientOptions = CreateMqttClientOptions(brokerSettings);
        _client.ApplicationMessageReceivedAsync += HandleMessageAsync;
    }

    async Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
        _logger.LogInformation("### RECEIVED APPLICATION MESSAGE ###\n{payload}",payload);
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(_topic)
            .WithPayload("OK")
            .Build();

        // await _client.PublishAsync(applicationMessage, CancellationToken.None);

        //_logger.LogInformation("MQTT application message is published.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _client.ConnectAsync(_clientOptions, CancellationToken.None);
            _logger.LogInformation("Connected");
        }
        catch (Exception e)
        {
            _logger.LogCritical(e.ToString());
        }
        await _client.SubscribeAsync(_subscriptionOptions, CancellationToken.None);
        _logger.LogInformation("Subscribed");
    }
    
    private MqttClientOptions CreateMqttClientOptions(BrokerSettings brokerSettings)
    {
        var mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerSettings.Host, brokerSettings.Port);

        if (!string.IsNullOrEmpty(brokerSettings.ClientCertificateFilePath) 
            && File.Exists(brokerSettings.ClientCertificateFilePath))
        {
            mqttClientOptionsBuilder.WithTlsOptions(
                o =>
                {
                    o.WithAllowUntrustedCertificates(true);
                    IEnumerable<X509Certificate2> CreateCertificates()
                    {
                        var certificates = new List<X509Certificate2>();
                        if (!string.IsNullOrEmpty(brokerSettings.ClientCertificateFilePassword))
                        {
                            if (File.Exists(brokerSettings.ClientCertificateFilePath))
                            {
                                var certificate = new X509Certificate2(brokerSettings.ClientCertificateFilePath, brokerSettings.ClientCertificateFilePassword,
                                    X509KeyStorageFlags.Exportable);
                                certificates.Add(certificate);
                            }
                            else
                            {
                                _logger.LogWarning($"{brokerSettings.ClientCertificateFilePath} not found");
                            }

                        }
                        if (!string.IsNullOrWhiteSpace(brokerSettings.ClientCertificateKeyFilePath))
                        {
                            if (File.Exists(brokerSettings.ClientCertificateFilePath) && File.Exists(brokerSettings.ClientCertificateKeyFilePath))
                            {
                                var certificate = X509Certificate2.CreateFromPemFile(
                                   brokerSettings.ClientCertificateFilePath,
                                   brokerSettings.ClientCertificateKeyFilePath);

                                using (var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                                {
                                    certStore.Open(OpenFlags.ReadWrite);
                                    certStore.Add(certificate);
                                }

                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                {
                                    var certificateExport = new X509Certificate2(certificate.Export(X509ContentType.Pkcs12));
                                    certificates.Add(certificateExport);
                                }
                                else
                                {
                                   // new X509Certificate2(certificate.Export(X509ContentType.Pfx))
                                    certificates.Add(certificate);
                                }
                            }
                            else
                            {
                                _logger.LogWarning($"{brokerSettings.CertificateAuthorityFilePath} or {brokerSettings.ClientCertificateKeyFilePath} not found");
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(brokerSettings.CertificateAuthorityFilePath))
                        {
                            if (File.Exists(brokerSettings.CertificateAuthorityFilePath))
                            {
                                var certificate = new X509Certificate2(brokerSettings.CertificateAuthorityFilePath);
                                certificates.Add(certificate);
                            }
                            else
                            {
                                _logger.LogWarning($"{brokerSettings.CertificateAuthorityFilePath} not found");
                            }

                        }
                        return certificates;
                    }

                    var certificates = CreateCertificates();
                    o.WithClientCertificates(certificates);
                    o.WithIgnoreCertificateChainErrors(true);
                    o.WithIgnoreCertificateRevocationErrors(true);
                    o.WithSslProtocols(SslProtocols.Tls12);
                });
        }

        var mqttClientOptions = mqttClientOptionsBuilder
            .Build();
        return mqttClientOptions;
    }
}