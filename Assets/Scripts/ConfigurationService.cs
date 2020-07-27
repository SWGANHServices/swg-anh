using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

public class ConfigurationService
{
    public ServiceProvider ServiceProvider { get; private set; }

    public static ConfigurationService CreateInstance()
    {
        return CreateInstance((s) =>
        {
        });
    }

    public static ConfigurationService CreateInstance(Action<IServiceCollection> handler)
    {
        var instance = new ConfigurationService();

        var descriptors = CreateDefaultSericeDescriptors();
        handler(descriptors);

        instance.ServiceProvider = descriptors.BuildServiceProvider();
        return instance;
    }

    // Server Config file load
    private static IServiceCollection CreateDefaultSericeDescriptors()
    {
        IServiceCollection serviceDescriptors = new ServiceCollection();

        serviceDescriptors.AddLogging(s => s.AddConsole());

        return serviceDescriptors;
    }
}