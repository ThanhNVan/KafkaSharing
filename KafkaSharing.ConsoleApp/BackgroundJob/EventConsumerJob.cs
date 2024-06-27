using Confluent.Kafka;
using KafkaSharing.ShareLibrary.Dto;
using KafkaSharing.ShareLibrary.Extension;
using KafkaSharing.ShareLibrary.Providers;
using KafkaSharing.ShareLibrary.SettingModels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        //this._consumer = new ConsumerBuilder<string, string>(this.GetComsumerConfiguration()).Build();
    }
    #endregion

    #region [ Methods ]
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                this.ConsumeTestTopic();
                //await this.ConsumeClientTopicAsync(cancellationToken);
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
        //using var consumer = new ConsumerBuilder<string, string>(this.GetComsumerConfiguration()).Build();
        _consumer.Subscribe("test-topic");
        var consumeResult = _consumer.Consume(3000);

        if (consumeResult is null)
        {
            var dateTimeNow = DateTime.Now;
            this._logger.LogWarning($"No message at {dateTimeNow.Hour}:{dateTimeNow.Minute}:{dateTimeNow.Second}:{dateTimeNow.Ticks}");   
        }
        else
        { 
            var dateTimeNow = DateTime.Now;
            this._logger.LogInformation($"Message consumed: {consumeResult.Message.Value} \n at {dateTimeNow.Hour}:{dateTimeNow.Minute}:{dateTimeNow.Second}:{dateTimeNow.Ticks}");
        }
        //consumer.Dispose();
        return;
    }

    private async Task ConsumeClientTopicAsync(CancellationToken cancellationToken = default)
    {

        //cancellationToken.ThrowIfCancellationRequested();
        using var consumer = new ConsumerBuilder<string, string>(this.GetComsumerConfiguration()).Build();
        consumer.Subscribe(this._kafkaSettings.Topic);
        var consumeResult = consumer.Consume(cancellationToken);

        var dateTimeNow = DateTime.Now;
        if (consumeResult == null)
        {
            this._logger.LogInformation($"No message at {dateTimeNow.Hour}:{dateTimeNow.Minute}:{dateTimeNow.Second}:{dateTimeNow.Ticks}");
            //await Task.Delay(5000);
            return;
        }

        // read from DB
        var message = await this._messageProvider.GetSingleByIdAsync(consumeResult.Key, cancellationToken);

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
        this._logger.LogInformation($"Message {consumeResult.Key} is consumed at {dateTimeNow.Hour}:{dateTimeNow.Minute}:{dateTimeNow.Second}:{dateTimeNow.Ticks}");
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
