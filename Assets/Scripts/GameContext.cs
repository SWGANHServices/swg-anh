using Microsoft.Extensions.DependencyInjection;
using SWGANH_Core;
using SWGANH_Core.Client;
using SWGANH_Core.PackageParser;
using System;


namespace Assets.Scripts
{
    internal static class GameContext
    {
        static GameContext()
        {
            ConfigService = ConfigurationService.CreateInstance(s =>
            {
                s.AddSingleton<IPackageParser, PackageParser>();
                //s.AddSingleton<NetworkConnection>();
                s.AddSingleton<ClientConnectionDispatcher>();
                s.AddSingleton<NetworkConnection>();
                //s.AddSingleton<ClientManager>();
                //s.AddSingleton<MenuManager>();
            });
        }

        public static ConfigurationService ConfigService { get; }

        public static IServiceProvider ServicesProvider => ConfigService.ServiceProvider;
    }
}