using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MakersPortal.WebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(@"  __  __       _                   ____            _        _ ");
            Console.WriteLine(@" |  \/  | __ _| | _____ _ __ ___  |  _ \ ___  _ __| |_ __ _| |");
            Console.WriteLine(@" | |\/| |/ _` | |/ / _ \ '__/ __| | |_) / _ \| '__| __/ _` | |");
            Console.WriteLine(@" | |  | | (_| |   <  __/ |  \__ \ |  __/ (_) | |  | || (_| | |");
            Console.WriteLine(@" |_|  |_|\__,_|_|\_\___|_|  |___/ |_|   \___/|_|   \__\__,_|_|");
            Console.ResetColor();
            
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                });
    }
}