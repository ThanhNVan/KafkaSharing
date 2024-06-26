using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace KafkaSharing.WebApi.Controllers;

[ApiController]
[Route("api/v1/kafka")]
public class TestController : ControllerBase
{
    private IProducer<Null, string> _producer;
    private readonly ILogger<TestController> _logger;
    private string _topics;
    private string _server;
    private string _userName;
    private string _password;

    public TestController(ILogger<TestController> logger)
    {
        this._logger = logger;
        this._topics = "test-demo-1";
        this._server = "localhost:9092";

        this._userName = "admin";
        this._password = "galliker";

        var config = new ProducerConfig()
        {
            BootstrapServers = this._server,
            AllowAutoCreateTopics = true,
            //Acks = Acks.All,
            SecurityProtocol = SecurityProtocol.SaslPlaintext,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = this._userName,
            SaslPassword = this._password,
        };
        this._producer = new ProducerBuilder<Null, string>(config).Build();

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
                await this._producer.ProduceAsync(this._topics,
                                                    new Message<Null, string> { Value = value });

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