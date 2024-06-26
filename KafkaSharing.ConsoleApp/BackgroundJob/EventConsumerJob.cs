using Confluent.Kafka;
using KafkaSharing.ShareLibrary.SettingModels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestLocalConsole.BackgroundJob;

public class EventConsumerJob : BackgroundService
{
    #region [ Fields ]
    private readonly ILogger<EventConsumerJob> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private IConsumer<string, string> _consumer;
    #endregion

    #region [ Ctor ]
    public EventConsumerJob(ILogger<EventConsumerJob> logger, KafkaSettings kafkaSettings)
    {
        this._logger = logger;
        this._kafkaSettings = kafkaSettings;
        var config = new ConsumerConfig()
        {
            BootstrapServers = this._kafkaSettings.Server,
            AllowAutoCreateTopics = true,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            SecurityProtocol = SecurityProtocol.SaslPlaintext,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = this._kafkaSettings.UserName,
            SaslPassword = this._kafkaSettings.Password,
            GroupId = this._kafkaSettings.GroupId,
        };

        this._consumer = new ConsumerBuilder<string, string>(config).Build();
        this._consumer.Subscribe(this._kafkaSettings.Topic);
    }
    #endregion

    #region [ Methods ]
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = this._consumer.Consume(TimeSpan.FromSeconds(5));
                //var messageList = this._consumer.
                if (consumeResult == null) {
                    continue;
                }
                var dateTimeNow = DateTime.Now;
                this._logger.LogInformation($"Message consumed: {consumeResult.Message.Value} \n at {dateTimeNow.Hour}:{dateTimeNow.Minute}:{dateTimeNow.Second}:{dateTimeNow.Ticks}");
                await Task.Delay(TimeSpan.FromSeconds(0.2));
            }
            catch (Exception ex)
            {
                
                this._logger.LogError($"{ex.Message}");
            }
        }

        return;
    }
    #endregion
}
