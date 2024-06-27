using Confluent.Kafka;
using KafkaSharing.ShareLibrary.Dto;
using KafkaSharing.ShareLibrary.Extension;
using KafkaSharing.ShareLibrary.Providers;
using KafkaSharing.ShareLibrary.SettingModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KafkaSharing.WebApi.Controllers;

[ApiController]
[Route("api/v1/kafka")]
public class KafkaController : ControllerBase
{
    #region [ Fields ]
    private IProducer<string, string> _producer;
    private readonly ILogger<KafkaController> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageProvider _messageProvider;
    #endregion

    #region [ Ctor ]
    public KafkaController(ILogger<KafkaController> logger, KafkaSettings kafkaSettings, IMessageProvider messageProvider)
    {
        this._kafkaSettings = kafkaSettings;
        this._messageProvider = messageProvider;
        this._logger = logger;
       
        this._producer = new ProducerBuilder<string, string>(this.GetProducerConfiguration()).Build();

    }
    #endregion

    #region [ Public Methods - POST ]
    [HttpPost("produce-many")]
    public async ValueTask<IActionResult> ProduceManyAsync()
    {
        try
        {
            await this.AddDataAsync();
        }
        catch (Exception ex)
        {
            return Ok($"Error: {ex.Message}");
        }

        return Ok("Ok");
    }

    [HttpPost("produce")]
    public async ValueTask<IActionResult> ProduceSingleAsync([FromBody]ClientModel model, CancellationToken cancellationToken = default)
    {
        try
        {
           var id = await this._messageProvider.AddSingleAsync(model, cancellationToken);
           await this._producer.ProduceAsync(this._kafkaSettings.Topic,
                                                   new Message<string, string> { Key = id, Value = model.SerializeObject() });
            this._logger.LogInformation($"Added a message with Id: {id} at {DateTime.Now.ToLongTimeString()}");
            this._producer.Flush();
        }
        catch (Exception ex)
        {
            return Ok($"Error: {ex.Message}");
        }

        return Ok("Message Added");
    }
    #endregion

    #region [ Private Methods ]
    private async Task AddDataAsync()
    {
        try
        {
            var hello = "Hello World";
            for (var i = 0; i < 100; i++)
            {
                var dateTimeNow = DateTime.Now;
                var value = hello + $"\n {i} at {dateTimeNow.Hour}:{dateTimeNow.Minute}:{dateTimeNow.Second}:{dateTimeNow.Ticks}";
                await this._producer.ProduceAsync("test-topic",
                                                    new Message<string, string> {Key = Ulid.NewUlid().ToString(), Value = value });

                this._logger.LogInformation(value);
            }

            this._producer.Flush();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex.Message);
            throw;
        }

    }

    private ProducerConfig GetProducerConfiguration()
    {
        var config = new ProducerConfig()
        {
            BootstrapServers = this._kafkaSettings.Server,
            AllowAutoCreateTopics = true,
            Acks = Acks.All,
            SecurityProtocol = SecurityProtocol.SaslPlaintext,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = this._kafkaSettings.UserName,
            SaslPassword = this._kafkaSettings.Password,
        };

        return config;
    }
    #endregion
}