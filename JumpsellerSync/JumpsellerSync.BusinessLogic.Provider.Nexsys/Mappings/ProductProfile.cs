using JumpsellerSync.BusinessLogic.Provider.Impl.Mappings;
using JumpsellerSync.BusinessLogic.Provider.Nexsys.Services;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.Domain.Impl.Nexsys;

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace JumpsellerSync.BusinessLogic.Provider.Nexsys.Mappings
{
    internal class ProductProfile : BaseProductProfile<NexsysProduct>
    {
        public ProductProfile()
        {
            CreateMap<storeProduct, NexsysProduct>()
                .IgnoreUnknownProperties()
                .MapFrom(p => p.Id, wsp => wsp.sku)
                .MapFrom(p => p.ProductCode, wsp => wsp.sku)
                .MapFrom(p => p.Price, wsp => Convert.ToDouble(wsp.price))
                .MapFrom(p => p.Stock, wsp => ToDouble(wsp.inventory))
                .MapFrom(p => p.Description, wsp => wsp.long_description)
                .MapFrom(p => p.Currency, wsp => wsp.currency)
                .MapFrom(p => p.BrandId, wsp => wsp.mark.ToDbId())
                .MapFrom(p => p.Category, wsp => CreateCategory(wsp.category))
                .MapFrom(p => p.ImageUrl, wsp => wsp.image)
                .MapFrom(p => p.Parent, wsp => wsp.parent)
                .MapFrom(p => p.TaxExcluded, wsp => wsp.tax_excluded);
        }

        private static double ToDouble(string n)
        {
            if (string.IsNullOrEmpty(n))
            { return default; }

            return double.TryParse(n, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number)
                ? number
                : default;
        }

        private static NexsysCategory CreateCategory(string str)
        {
            var categories = Regex
               .Split(str ?? "", @"\s*>\s*")
               .Where(s => !string.IsNullOrEmpty(s))
               .ToArray();

            NexsysCategory category = null;
            for (var i = categories.Length - 1; i >= 0; i--)
            {
                var superCategory = new NexsysCategory
                {
                    Id = $"{i}_{categories[i]}".ToDbId(),
                    Description = categories[i],
                    SubCategory = category
                };
                category = superCategory;
            }

            return category;
        }
    }
}
