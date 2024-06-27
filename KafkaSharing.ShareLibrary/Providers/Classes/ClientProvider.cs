using KafkaSharing.ShareLibrary.DatabaseContext;
using KafkaSharing.ShareLibrary.Dto;
using KafkaSharing.ShareLibrary.Entities;
using KafkaSharing.ShareLibrary.Providers.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KafkaSharing.ShareLibrary.Providers.Classes;

public class ClientProvider : BaseEntityProvider<Client, AppDbContext>, IClientProvider
{
    #region [ Ctor ]
    public ClientProvider(ILogger<ClientProvider> logger, IDbContextFactory<AppDbContext> dbContextFactory) : base(logger, dbContextFactory)
    {
    }
    #endregion

    #region [ Method - Create ]
    public async ValueTask<string> AddSingleAsync(ClientModel model, CancellationToken cancellationToken = default)
    {
        var result = string.Empty;

        cancellationToken.ThrowIfCancellationRequested();
        using var dbContext = await this.GetDbContextAsync(cancellationToken);
        result = Ulid.NewUlid().ToString();
        var client = new Client(result);
        client.Name = model.Name;
        client.DateOfBirth = model.DateOfBirth;
        client.Description = model.Description;

        await dbContext.Clients.AddAsync(client, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken); 

        return result;
    }
    #endregion
}
