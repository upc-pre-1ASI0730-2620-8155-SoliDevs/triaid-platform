namespace TriAidPlatform.API.Shared.Domain.Repositoriesn;

public interface IBaseRepository <TEntity>
{
    Task AddSync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<IEnumerable<TEntity>> ListAsync(CancellationToken cancellationToken = default);
}