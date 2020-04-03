using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;

namespace MakersPortal.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                        
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();

                        var keyVaultClient =
                            new KeyVaultClient(
                                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider
                                    .KeyVaultTokenCallback));

                        builder.AddAzureKeyVault(keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                    });

                    webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);

                    webBuilder.UseStartup<Startup>();
                });
    }
}