using KafkaSharing.ShareLibrary.Entities.Base;

namespace KafkaSharing.ShareLibrary.Entities;

public class Message : BaseEntity
{
    public bool IsConsumed { get; set; }   
    
    public DateTime ConsumedAt { get; set; }    

    public string Value { get; set; }
}
