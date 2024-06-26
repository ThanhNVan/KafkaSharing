using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestLocalConsole.BackgroundJob;

public class EventConsumerJob : BackgroundService
{
    #region [ Fields ]
    private readonly ILogger<EventConsumerJob> _logger;

    private string _server;

    private string _topics;
    private string _userName;
    private string _password;
    private IConsumer<Ignore, string> _consumer;
    #endregion

    #region [ Ctor ]
    public EventConsumerJob(ILogger<EventConsumerJob> logger)
    {
        this._logger = logger;
        this._topics = "test-demo-1";
        //this._topics = "customers";
        this._server = "localhost:9092";
        this._userName = "admin";
        this._password = "galliker";
        var config = new ConsumerConfig()
        {
            BootstrapServers = this._server,
            AllowAutoCreateTopics = true,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            SecurityProtocol = SecurityProtocol.SaslPlaintext,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = this._userName,
            SaslPassword = this._password,
            GroupId = Guid.NewGuid().ToString(),
        };

        this._consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        this._consumer.Subscribe(this._topics);
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
