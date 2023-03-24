using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Services;

using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Provider.Impl.Services
{
    public class MockCurrencyConverter : ICurrencyConverterService
    {
        private readonly double staticFactor;

        public MockCurrencyConverter(double staticFactor)
        {
            this.staticFactor = staticFactor;
        }

        public Task<ServiceResult<ConvertCurrencyDetailsDto>> ConvertAsync(string from, string to)
        {
            return Task.FromResult(ServiceResult.Succeed(new ConvertCurrencyDetailsDto
            {
                From = from,
                To = to,
                Value = staticFactor
            }));
        }

        public Task<ServiceResult<ConvertCurrencyDetailsDto>> ConvertAsync(string from, string to, double amount)
        {
            return Task.FromResult(ServiceResult.Succeed(new ConvertCurrencyDetailsDto
            {
                From = from,
                To = to,
                Value = staticFactor * amount
            }));
        }
    }
}
