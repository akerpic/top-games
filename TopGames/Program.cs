using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using TopGames.Helpers;

namespace TopGames
{
    public class Program
    {
        private const string DefaultListenPort = "8080";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(Int32.Parse(Environment.GetEnvironmentVariable("PORT") ??
                                                          DefaultListenPort));
                    }).ConfigureServices(services =>
                    {
                        services.AddHostedService<TimedHostedService>();
                    });
                });
    }
}
