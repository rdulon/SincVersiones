using JumpsellerSync.BusinessLogic.Provider.Impl.Mappings;
using JumpsellerSync.Domain.Impl.Linkstore;

using Newtonsoft.Json.Linq;

namespace JumpsellerSync.BusinessLogic.Provider.Linkstore.Mappings
{
    internal class LinkstoreCategoriesProfile : BaseCategoryProfile<LinkstoreCategory>
    {
        public LinkstoreCategoriesProfile()
        {
            CreateMap<JArray, LinkstoreCategory>()
                .ConvertUsing(arr => new LinkstoreCategory
                {
                    Id = arr[0].ToObject<string>(),
                    Description = arr[1].ToObject<string>()
                });

            CreateMap<JArray, LinkstoreSubcategory>()
                .ConstructUsing(arr => new LinkstoreSubcategory
                {
                    Id = arr[0].ToObject<string>(),
                    Description = arr[1].ToObject<string>()
                });
        }
    }
}
