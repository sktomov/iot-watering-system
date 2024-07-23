using Microsoft.Extensions.Options;
using WateringSystem.Broker;
using WateringSystem.Broker.Interfaces;

namespace WateringSystem;

public static class ServiceExtensions
{
    public static void AddMqttReceiver<TReceiver>(this IServiceCollection services, string topic, string clientId)
        where TReceiver : class, IMqttReceiver
    {
        services.AddScoped<IMqttSender, MqttSender>();
        services.AddScoped<TReceiver>();
        services.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<BrokerSettings>>();
            var logger = provider.GetRequiredService<ILogger<TReceiver>>();
            var serviceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            return (IHostedService)new MqttReceiver<TReceiver>(topic, clientId,options, logger, serviceScopeFactory);
        });
    }
}