using System.ComponentModel.DataAnnotations;

namespace KafkaSharing.ShareLibrary.Entities.Base;

public class BaseEntity
{
    #region [ Properties ]
    [Key]
    [Required]
    [StringLength(36)]
    [DataType(DataType.Text)]
    public string Id { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime LastUpdatedAt { get; set; }
    #endregion

    #region [ CTor ]
    public BaseEntity()
    {
        Id = Ulid.NewUlid().ToString();
        //Id = Guid.NewGuid().ToString();
        IsActive = true;
        CreatedAt = DateTime.Now;
        LastUpdatedAt = DateTime.Now;
    }
    
    public BaseEntity(string id)
    {
        Id = id;
        IsActive = true;
        CreatedAt = DateTime.Now;
        LastUpdatedAt = DateTime.Now;
    }
    #endregion
}
