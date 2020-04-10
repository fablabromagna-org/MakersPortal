using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MakersPortal.WebApi
{
    public class Program
    {
        private static KeyVaultClient _keyVaultClient;

        public static void Main(string[] args)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            _keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider
                    .KeyVaultTokenCallback));
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((context, builder) =>
                    {
                        var keyVaultEndpoint = Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");

                        if (keyVaultEndpoint == null)
                            return;

                        builder.AddAzureKeyVault(keyVaultEndpoint, _keyVaultClient, new DefaultKeyVaultSecretManager());
                    });

                    webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);

                    webBuilder.UseStartup<Startup>().ConfigureServices(collection =>
                        {
                            collection.AddSingleton<IKeyVaultClient>(_keyVaultClient);
                        });
                });
    }
}