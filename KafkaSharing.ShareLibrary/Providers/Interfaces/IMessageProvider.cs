using KafkaSharing.ShareLibrary.Dto;
using KafkaSharing.ShareLibrary.Entities;
using KafkaSharing.ShareLibrary.Providers.Base;

namespace KafkaSharing.ShareLibrary.Providers;

public interface IMessageProvider : IBaseEntityProvider<Message>
{
    ValueTask<string> AddSingleAsync(ClientModel model, CancellationToken cancellationToken = default);

    ValueTask<Message?> GetSingleLatestMessageAsync(CancellationToken cancellationToken = default);
}
