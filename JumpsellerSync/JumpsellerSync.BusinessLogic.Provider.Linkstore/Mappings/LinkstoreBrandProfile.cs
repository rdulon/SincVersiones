using JumpsellerSync.BusinessLogic.Provider.Impl.Mappings;
using JumpsellerSync.Domain.Impl.Linkstore;

using Newtonsoft.Json.Linq;

namespace JumpsellerSync.BusinessLogic.Provider.Linkstore.Mappings
{
    internal class LinkstoreBrandProfile : BaseBrandProfile<LinkstoreBrand>
    {
        public LinkstoreBrandProfile()
        {
            CreateMap<JArray, LinkstoreBrand>()
                .ConvertUsing(arr => new LinkstoreBrand
                {
                    Id = arr[0].ToObject<string>(),
                    Description = arr[1].ToObject<string>(),
                });
        }
    }
}
