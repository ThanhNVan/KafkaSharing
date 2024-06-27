using KafkaSharing.ShareLibrary.DatabaseContext;
using KafkaSharing.ShareLibrary.Dto;
using KafkaSharing.ShareLibrary.Entities;
using KafkaSharing.ShareLibrary.Providers.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KafkaSharing.ShareLibrary.Extension;

namespace KafkaSharing.ShareLibrary.Providers.Classes;

public class MessageProvider : BaseEntityProvider<Message, AppDbContext>, IMessageProvider
{
    #region [ Ctor ]
    public MessageProvider(ILogger<MessageProvider> logger, IDbContextFactory<AppDbContext> dbContextFactory) : base(logger, dbContextFactory)
    {
    }
    #endregion

    #region [ Public Methods - Create ]
    public async ValueTask<string> AddSingleAsync(ClientModel model, CancellationToken cancellationToken = default)
    {
        var result = default(string);

        cancellationToken.ThrowIfCancellationRequested();

        var value = model.SerializeObject();

        var message = new Message();
        message.Value = value;

        using var dbContext = await this.GetDbContextAsync(cancellationToken);
        await dbContext.Messages.AddAsync(message, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        result = message.Id;
        return result;
    }
    #endregion

    #region [ Public Methods - Get Single ]
    public async ValueTask<Message?> GetSingleLatestMessageAsync(CancellationToken cancellationToken = default)
    {
        var result = default(Message);

        cancellationToken.ThrowIfCancellationRequested();
        using var dbContext = await this.GetDbContextAsync(cancellationToken);

        result = await dbContext.Messages.Where(x => !x.IsConsumed)
                                          .OrderBy(x => x.CreatedAt)
                                          .FirstOrDefaultAsync();
                                            


        return result;
    }
    #endregion
}
