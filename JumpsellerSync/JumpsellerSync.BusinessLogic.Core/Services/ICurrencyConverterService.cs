using JumpsellerSync.BusinessLogic.Core.Dtos;

using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services
{
    public interface ICurrencyConverterService : IService
    {
        Task<ServiceResult<ConvertCurrencyDetailsDto>> ConvertAsync(string @from, string to);

        Task<ServiceResult<ConvertCurrencyDetailsDto>> ConvertAsync(string @from, string to, double amount);
    }
}
