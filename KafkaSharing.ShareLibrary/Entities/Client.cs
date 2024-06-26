using KafkaSharing.ShareLibrary.Entities.Base;

namespace KafkaSharing.ShareLibrary.Entities;


public class Client : BaseEntity
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string DateOfBirth { get; set; }
}
