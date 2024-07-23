using System.Security.Cryptography.X509Certificates;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace WateringSystem.Broker;

public static class MqttClientHelper
{
    public static MqttClientOptions GetClientOptions(BrokerSettings brokerSettings, string clientId)
    {
        var caCertPath = brokerSettings.CaLocation;
        var clientCertPath = brokerSettings.ClientCertificateFilePath;
        var clientKeyPath = brokerSettings.ClientCertificateKeyFilePath;
        var caCert = new X509Certificate2(caCertPath);
        var clientCert = X509Certificate2.CreateFromPemFile(clientCertPath, clientKeyPath);
        
        return new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(brokerSettings.Host, brokerSettings.Port)
            .WithCredentials(brokerSettings.Username, brokerSettings.Password)
            .WithTls(new MqttClientOptionsBuilderTlsParameters
            {
                UseTls = true,
                IgnoreCertificateChainErrors = true,
                IgnoreCertificateRevocationErrors = true,
                Certificates = new List<X509Certificate>
                {
                    clientCert,
                    caCert
                },
                CertificateValidationHandler = context => { return true; }
            })
            .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
    }
}