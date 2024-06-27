using KafkaSharing.ShareLibrary.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KafkaSharing.ShareLibrary.Providers.Base;

public abstract class BaseEntityProvider<TEntity, TDbContext> : IBaseEntityProvider<TEntity>
    where TEntity : BaseEntity
    where TDbContext : DbContext
{
    #region [ Fields ]
    protected readonly ILogger<BaseEntityProvider<TEntity, TDbContext>> _logger;

    protected readonly IDbContextFactory<TDbContext> _dbContextFactory;
    #endregion

    #region [ CTor ]
    public BaseEntityProvider(ILogger<BaseEntityProvider<TEntity, TDbContext>> logger,
                            IDbContextFactory<TDbContext> dbContextFactory)
    {
        this._logger = logger;
        this._dbContextFactory = dbContextFactory;
    }
    #endregion

    #region [ Public Methods - CRUD ]
    public virtual async ValueTask<bool> AddSingleAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var context = await this.GetDbContextAsync(cancellationToken);
        if (await context.Set<TEntity>().FindAsync(entity.Id, cancellationToken) != null)
        {
            return false;
        }

        await context.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public virtual async ValueTask<bool> AddManyAsync(IEnumerable<TEntity> entityList, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var context = await this.GetDbContextAsync(cancellationToken);

        await context.AddRangeAsync(entityList, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public virtual async ValueTask<bool> IsExistAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var context = await this.GetDbContextAsync(cancellationToken);

        if (await context.Set<TEntity>().FindAsync(id, cancellationToken) == null)
        {
            return false;
        }

        return true;

    }

    public virtual async ValueTask<TEntity> GetSingleByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = default(TEntity);

        using var context = await this.GetDbContextAsync(cancellationToken);
        result = await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return result;
    }

    public virtual async ValueTask<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var context = await this.GetDbContextAsync(cancellationToken);
        entity.LastUpdatedAt = DateTime.Now;
        context.Set<TEntity>().Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;

    }

    public virtual async ValueTask<bool> SoftDeleteAsync(string entityId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var context = await this.GetDbContextAsync(cancellationToken);
        var dbResult = await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == entityId, cancellationToken);
        if (dbResult == null)
        {

            return false;
        }
        dbResult.IsActive = false;
        dbResult.LastUpdatedAt = DateTime.Now;
        context.Set<TEntity>().Update(dbResult);
        await context.SaveChangesAsync(cancellationToken);
        return true;

    }

    public virtual async ValueTask<bool> RecoverAsync(string entityId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var context = await this.GetDbContextAsync(cancellationToken);
        var dbResult = await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == entityId, cancellationToken);
        if (dbResult == null)
        {

            return false;
        }
        dbResult.IsActive = true;
        dbResult.LastUpdatedAt = DateTime.Now;
        context.Set<TEntity>().Update(dbResult);
        await context.SaveChangesAsync(cancellationToken);
        return true;

    }

    public virtual async ValueTask<bool> DestroyAsync(string entityId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var context = await this.GetDbContextAsync(cancellationToken);
        var dbResult = await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == entityId, cancellationToken);
        if (dbResult == null)
        {

            return false;
        }
        context.Set<TEntity>().Remove(dbResult);
        await context.SaveChangesAsync(cancellationToken);
        return true;

    }

    public virtual async ValueTask<IEnumerable<TEntity>> GetManyAsync(int take, int skip, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = default(IEnumerable<TEntity>);

        using var context = await this.GetDbContextAsync(cancellationToken);
        result = await context.Set<TEntity>().AsNoTracking()
                        .OrderByDescending(x => x.IsActive)
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync(cancellationToken);
        return result;

    }

    public virtual async ValueTask<IEnumerable<TEntity>> GetManyActiveAsync(int take, int skip, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = default(IEnumerable<TEntity>);

        using var context = await this.GetDbContextAsync(cancellationToken);
        result = await context.Set<TEntity>().AsNoTracking().Where(x => x.IsActive)
                            .Skip(skip)
                            .Take(take)
                            .ToListAsync(cancellationToken);
        return result;

    }

    public virtual async ValueTask<IEnumerable<TEntity>> GetManyInactiveAsync(int take, int skip, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = default(IEnumerable<TEntity>);

        using var context = await this.GetDbContextAsync(cancellationToken);
        result = await context.Set<TEntity>().AsNoTracking().Where(x => !x.IsActive)
                            .Skip(skip)
                            .Take(take)
                            .ToListAsync(cancellationToken);
        return result;

    }

    public virtual async ValueTask<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = default(int);

        using var context = await this.GetDbContextAsync(cancellationToken);
        result = await context.Set<TEntity>().AsNoTracking().CountAsync(cancellationToken);
        return result;

    }

    public virtual async ValueTask<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = default(int);

        using var context = await this.GetDbContextAsync(cancellationToken);
        result = await context.Set<TEntity>().AsNoTracking().CountAsync(x => x.IsActive, cancellationToken);
        return result;

    }

    public virtual async ValueTask<int> CountInactiveAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = default(int);

        using var context = await this.GetDbContextAsync(cancellationToken);
        result = await context.Set<TEntity>().AsNoTracking().CountAsync(x => !x.IsActive, cancellationToken);
        return result;

    }
    #endregion

    #region [ Private Methods - TContext ]
    protected async ValueTask<TDbContext> GetDbContextAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this._dbContextFactory.CreateDbContextAsync(cancellationToken);
    }
    #endregion
}
