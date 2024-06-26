namespace KafkaSharing.ShareLibrary.Entities;


public class Client
{
    public string Id { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime LastUpdatedAt { get; set; }
    
    public bool IsActive { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string DateOfBirth { get; set; }
}
