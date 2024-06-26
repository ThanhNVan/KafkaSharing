using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestLocalConsole.BackgroundJob;

namespace KafkaSharing.ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Start consuming events ...");

        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddHostedService<EventConsumerJob>();

        builder.Build().Run();
    }
}
