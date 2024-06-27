using KafkaSharing.ShareLibrary.Entities.Base;

namespace KafkaSharing.ShareLibrary.Entities;

public class Message : BaseEntity
{
    #region [ Ctor ]
    public Message() : base()
    {

    }

    public Message(string id) : base(id)
    {

    }
    #endregion

    #region [ Properties ]
    public bool IsConsumed { get; set; }   
    
    public DateTime? ConsumedAt { get; set; }    

    public string Value { get; set; }
    #endregion
}
