using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Repositories;

namespace TriAidPlatform.API.SpecialtyAssignment.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>Infrastructure implementation of the Voucher repository.</summary>
/// <param name="context">Database context provided via dependency injection.</param>
public class VoucherRepository(AppDbContext context) : BaseRepository<Voucher>(context), IVoucherRepository
{
}
