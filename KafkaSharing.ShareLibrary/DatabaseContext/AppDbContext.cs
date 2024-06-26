using KafkaSharing.ShareLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace KafkaSharing.ShareLibrary.DatabaseContext;

public class AppDbContext : DbContext
{
    #region [ CTor ]
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    #endregion

    #region [ DbSet ]
    public DbSet<Client> Clients { get; set; }  
    public DbSet<Message> Messages { get; set; }  
    #endregion
}
