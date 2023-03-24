using JumpsellerSync.Domain.Impl.Main;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories.Main
{
    public interface IBrandRepository
        : IBaseRepository<Brand>,
          IUpsertRepository<Brand>
    {
        Task<DataAccessResult<int>> CreateAsync(IEnumerable<Brand> brands, bool deferSave = false);

        Task<DataAccessResult<Brand>> FindBrandByNormalizedNameAsync(string normalizedName);
    }
}
