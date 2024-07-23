using System.ComponentModel.DataAnnotations;

public class BrokerSettings
{
    public const string SectionName = "Broker";

    [Required]
    public int Port { get; set; }

    public string Host { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string CaLocation { get; set; }

    public string ClientCertificateKeyFilePath { get; set; }

    public string ClientCertificateFilePath { get; set; }
}