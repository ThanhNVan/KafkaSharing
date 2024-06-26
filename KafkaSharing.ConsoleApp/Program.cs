using KafkaSharing.ShareLibrary.SettingModels;
using Microsoft.Extensions.Configuration;
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

        var kafkaSettings = new KafkaSettings();
        builder.Configuration.GetSection("KafkaSettings").Bind(kafkaSettings);

        builder.Services.AddSingleton(kafkaSettings);
        builder.Services.AddHostedService<EventConsumerJob>();

        builder.Build().Run();
    }
}
