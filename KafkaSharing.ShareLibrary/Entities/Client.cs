using KafkaSharing.ShareLibrary.Entities.Base;

namespace KafkaSharing.ShareLibrary.Entities;


public class Client : BaseEntity
{
    #region [ Ctor ]
    public Client() : base()
    {
            
    }

    public Client(string id) : base(id)
    {

    }
    #endregion

    #region [ Properties ]
    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime DateOfBirth { get; set; }
    #endregion
}
