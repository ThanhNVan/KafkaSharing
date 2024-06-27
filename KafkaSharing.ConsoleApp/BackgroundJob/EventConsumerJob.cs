using Confluent.Kafka;
using KafkaSharing.ShareLibrary.Dto;
using KafkaSharing.ShareLibrary.Extension;
using KafkaSharing.ShareLibrary.Providers;
using KafkaSharing.ShareLibrary.SettingModels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TestLocalConsole.BackgroundJob;

public class EventConsumerJob : BackgroundService
{
    #region [ Fields ]
    private readonly ILogger<EventConsumerJob> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageProvider _messageProvider;
    private readonly IClientProvider _clientProvider;
    private IConsumer<string, string> _consumer;
    #endregion

    #region [ Ctor ]
    public EventConsumerJob(ILogger<EventConsumerJob> logger,
                            KafkaSettings kafkaSettings,
                            IMessageProvider messageProvider,
                            IClientProvider clientProvider)
    {
        this._logger = logger;
        this._kafkaSettings = kafkaSettings;
        this._messageProvider = messageProvider;
        this._clientProvider = clientProvider;

        this._consumer = new ConsumerBuilder<string, string>(this.GetComsumerConfiguration()).Build();
    }
    #endregion

    #region [ Methods ]
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                //this.ConsumeTestTopic();
                await this.ConsumeClientTopicAsync();
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

    #region [ Private Methods ]
    private void ConsumeTestTopic()
    {
        this._consumer.Subscribe("test-topic");
        var consumeResult = this._consumer.Consume(TimeSpan.FromSeconds(5));
        if (consumeResult == null)
        {
            return;
        }
        var dateTimeNow = DateTime.Now;
        this._logger.LogInformation($"Message consumed: {consumeResult.Message.Value} \n at {dateTimeNow.Hour}:{dateTimeNow.Minute}:{dateTimeNow.Second}:{dateTimeNow.Ticks}");

        return;

    }

    private async Task ConsumeClientTopicAsync(CancellationToken cancellationToken = default)
    {

        cancellationToken.ThrowIfCancellationRequested();
        this._consumer.Subscribe(this._kafkaSettings.Topic);
        var consumeResult = this._consumer.Consume(cancellationToken);

        if (consumeResult == null)
        {
            return;
        }

        // read from DB
        var message = await this._messageProvider.GetSingleByIdAsync(consumeResult.Key, cancellationToken);

        //var latestMessage = await this._messageProvider.GetSingleLatestMessageAsync(cancellationToken);

        if (message is null || message.IsConsumed)
        {
            return;
        }

        var model = message.Value.DeserializeObject<ClientModel>();

        await this._clientProvider.AddSingleAsync(model, cancellationToken);
        message.IsActive = false;
        message.IsConsumed = true;
        message.ConsumedAt = DateTime.Now;
        await this._messageProvider.UpdateAsync(message,cancellationToken);

        return;
    }

    private ConsumerConfig GetComsumerConfiguration()
    {
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

        return config;
    }
    #endregion
}
