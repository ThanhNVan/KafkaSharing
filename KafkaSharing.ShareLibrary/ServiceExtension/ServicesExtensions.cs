using KafkaSharing.ShareLibrary.DatabaseContext;
using KafkaSharing.ShareLibrary.Providers;
using KafkaSharing.ShareLibrary.Providers.Classes;
using KafkaSharing.ShareLibrary.SettingModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaSharing.ShareLibrary.ServiceExtension;

public static class ServicesExtensions
{
    public static void AddServices(this IServiceCollection services,
             IConfiguration configuration,
             string connectionStringKey = "DbConnection")
    {
        var connectionString = configuration.GetConnectionString(connectionStringKey);
        var options = new DbContextOptions<AppDbContext>();
        var builder = new DbContextOptionsBuilder<AppDbContext>(options);
        builder.UseSqlServer(connectionString);
        builder.EnableSensitiveDataLogging();

        services.AddPooledDbContextFactory<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString,
                sqlServerOptionsAction =>
                {
                    sqlServerOptionsAction.EnableRetryOnFailure();
                });
            options.EnableSensitiveDataLogging();
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        });

        services.AddScoped(p => p.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());
        var kafkaSettings = new KafkaSettings();
        configuration.GetSection(nameof(KafkaSettings)).Bind(kafkaSettings);

        services.AddSingleton(kafkaSettings);
        services.AddProvider();
    }

    public static void AddProvider(this IServiceCollection services)
    {
        services.AddTransient<IClientProvider, ClientProvider>();
        services.AddTransient<IMessageProvider, MessageProvider>();
    }
}
