using Confluent.Kafka;
using KafkaSharing.ShareLibrary.SettingModels;
using Microsoft.AspNetCore.Mvc;

namespace KafkaSharing.WebApi.Controllers;

[ApiController]
[Route("api/v1/kafka")]
public class TestController : ControllerBase
{
    private IProducer<string, string> _producer;
    private readonly ILogger<TestController> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public TestController(ILogger<TestController> logger, KafkaSettings kafkaSettings)
    {
        this._kafkaSettings = kafkaSettings;
        this._logger = logger;
       

        var config = new ProducerConfig()
        {
            BootstrapServers = this._kafkaSettings.Server,
            AllowAutoCreateTopics = true,
            //Acks = Acks.All,
            SecurityProtocol = SecurityProtocol.SaslPlaintext,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = this._kafkaSettings.UserName,
            SaslPassword = this._kafkaSettings.Password,
        };
        this._producer = new ProducerBuilder<string, string>(config).Build();

    }

    [HttpPost("produce")]
    public async ValueTask<IActionResult> ProduceAsync()
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
                await this._producer.ProduceAsync(this._kafkaSettings.Topic,
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

    private Task DeleteDataAsync()
    {
        this._producer?.Dispose();

        return Task.CompletedTask;
    }
    #endregion
}