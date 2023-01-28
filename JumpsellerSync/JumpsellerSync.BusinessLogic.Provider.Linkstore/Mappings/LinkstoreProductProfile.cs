using AutoMapper;

using JumpsellerSync.BusinessLogic.Provider.Impl.Mappings;
using JumpsellerSync.Domain.Impl.Linkstore;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;

namespace JumpsellerSync.BusinessLogic.Provider.Linkstore.Mappings
{
    internal class LinkstoreProductProfile : BaseProductProfile<LinkstoreProduct>
    {
        public LinkstoreProductProfile()
        {
            CreateMap<JArray, LinkstoreProduct>()
                .ConvertUsing(new JArrayLinkstoreProductTypeConverter());
        }
    }

    internal sealed class JArrayLinkstoreProductTypeConverter : ITypeConverter<JArray, LinkstoreProduct>
    {
        public LinkstoreProduct Convert(
            JArray source, LinkstoreProduct destination, ResolutionContext context)
        {
            try
            {
                var hasInTransitArrival =
                    DateTime.TryParse(source[6].Value<string>(), out var inTransitArrival);

                return new LinkstoreProduct
                {
                    BrandId = source[0].ToObject<string>(),
                    Id = source[1].ToObject<string>(),
                    ProductCode = source[2].ToObject<string>().TrimEnd('.'),
                    Description = source[3].ToObject<string>(),
                    Stock = Math.Max(source[4].ToObject<double>(), 0),
                    AmountInTransit = (int.TryParse(source[5].ToString(), out _) ? source[5].ToObject<int>() : 0),
                    InTransitArrival = hasInTransitArrival ? inTransitArrival : (DateTime?)null,
                    Price = source[8].ToObject<double>(),
                    ProviderPrice = source[8].ToObject<double>(),
                    CategoryId = source[9].ToObject<string>(),
                    SubCategoryId = source[10].ToObject<string>(),
                    Weight = source[11].ToObject<double>(),
                    ImageUrls = source[12].ToObject<ICollection<string>>()
                };
            } catch (Exception e)
            {
                throw e;
            }
        }
    }
}
