namespace KafkaSharing.ShareLibrary.SettingModels;

public class KafkaSettings
{
    public string Server { get; set; }

    public string Topic { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string GroupId { get; set; }

    //"Server": "localhost:9092",
    //"Topic": "kafka-sharing",
    //"UserName": "admin",
    //"Password": "galliker",
    //"GroupId": "user-group-1"
}
