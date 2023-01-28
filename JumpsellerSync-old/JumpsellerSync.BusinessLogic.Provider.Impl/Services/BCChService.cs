using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services;
using JumpsellerSync.BusinessLogic.Provider.Impl.BCCh;
using JumpsellerSync.BusinessLogic.Provider.Impl.Options;

using Microsoft.Extensions.Options;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Provider.Impl.Services
{
    public class BCChService : ICurrencyConverterService
    {
        private readonly SieteWSSoapClient sieteWS;
        private readonly BCChOptions bCChOptions;

        public BCChService(
            SieteWSSoapClient sieteWS,
            IOptions<CurrencyOptions> currencyOptions)
        {
            this.sieteWS = sieteWS;
            bCChOptions = currencyOptions.Value.BCCh;
        }

        public async Task<ServiceResult<ConvertCurrencyDetailsDto>> ConvertAsync(string from, string to)
        {
            var key = $"{from?.ToUpper()}_{to?.ToUpper()}";
            var (serie, additive) = bCChOptions.GetSerie(key);
            if (string.IsNullOrEmpty(serie))
            {
                return ServiceResult.NotFound<ConvertCurrencyDetailsDto>(
                  $"Conversión desde {from} a {to} no soportada.");
            }

            var firstDate = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
            var lastDate = DateTime.Today.ToString("yyyy-MM-dd");

            var response = await sieteWS.GetSeriesAsync(
                bCChOptions.Username, bCChOptions.Password, firstDate, lastDate, new[] { serie });

            var factor = response.Series
                .SelectMany(serie => serie.obs)
                .Where(obs => obs.statusCode == "OK")
                .Select(obs => new
                {
                    Date = DateTime.ParseExact(obs.indexDateString, "dd-MM-yyyy", default),
                    Value = obs.value
                })
                .OrderByDescending(item => item.Date)
                .Select(item => item.Value)
                .FirstOrDefault();

            return factor != 0
                ? ServiceResult.Succeed(new ConvertCurrencyDetailsDto
                {
                    From = from,
                    To = to,
                    Value = Math.Round(factor + additive)
                })
                : ServiceResult.Error<ConvertCurrencyDetailsDto>(
                    $"No se ha podido obtener información del BCCh.");
        }

        public async Task<ServiceResult<ConvertCurrencyDetailsDto>> ConvertAsync(
            string from, string to, double amount)
        {
            var result = await ConvertAsync(from, to);
            if (result.IsSucceed())
            { result.Data.Value = Math.Round(result.Data.Value * amount, 2); }

            return result;
        }
    }
}
