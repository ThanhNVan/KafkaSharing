using KafkaSharing.ShareLibrary.Entities.Base;

namespace KafkaSharing.ShareLibrary.Providers.Base;

public interface IBaseEntityProvider<TEntity>
    where TEntity : BaseEntity
{
    #region [ Public Methods - CRUD ]
    ValueTask<bool> AddSingleAsync(TEntity entity, CancellationToken cancellationToken = default);

    ValueTask<bool> AddManyAsync(IEnumerable<TEntity> entityList, CancellationToken cancellationToken = default);


    ValueTask<bool> IsExistAsync(string id, CancellationToken cancellationToken = default);

    ValueTask<TEntity> GetSingleByIdAsync(string id, CancellationToken cancellationToken = default);

    ValueTask<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    ValueTask<bool> SoftDeleteAsync(string entityId, CancellationToken cancellationToken = default);

    ValueTask<bool> RecoverAsync(string entityId, CancellationToken cancellationToken = default);

    ValueTask<bool> DestroyAsync(string entityId, CancellationToken cancellationToken = default);

    ValueTask<IEnumerable<TEntity>> GetManyAsync(int take, int skip, CancellationToken cancellationToken = default);

    ValueTask<IEnumerable<TEntity>> GetManyActiveAsync(int take, int skip, CancellationToken cancellationToken = default);

    ValueTask<IEnumerable<TEntity>> GetManyInactiveAsync(int take, int skip, CancellationToken cancellationToken = default);

    ValueTask<int> CountAllAsync(CancellationToken cancellationToken = default);

    ValueTask<int> CountActiveAsync(CancellationToken cancellationToken = default);

    ValueTask<int> CountInactiveAsync(CancellationToken cancellationToken = default);
    #endregion
}
