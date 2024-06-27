using KafkaSharing.ShareLibrary.Dto;
using KafkaSharing.ShareLibrary.Entities;
using KafkaSharing.ShareLibrary.Providers.Base;

namespace KafkaSharing.ShareLibrary.Providers;

public interface IClientProvider : IBaseEntityProvider<Client>
{
    ValueTask<string> AddSingleAsync(ClientModel model, CancellationToken cancellationToken = default);
}
