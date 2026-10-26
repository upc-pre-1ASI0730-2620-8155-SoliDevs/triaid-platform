namespace TriAidPlatform.API.Shared.Domain.Repositoriesn;

public interface IUnitOfWork
{
    Task CompleteAsync(CancellationToken cancellationToken = default);
}