using AutoMapper;

using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.DataAccess.Core;

namespace JumpsellerSync.BusinessLogic.Impl.Extensions
{
    internal static class ServiceExtensions
    {
        public static ServiceResult<TDto> ToServiceResult<TModel, TDto>(
            this DataAccessResult<TModel> dataAccessResult, IMapper mapper)
        {
            if (dataAccessResult == null)
            { return null; }

            if (dataAccessResult.OperationSucceed && dataAccessResult.Data != null)
            {
                var dto = mapper.Map<TModel, TDto>(dataAccessResult.Data);
                return ServiceResult.Succeed(dto);
            }

            return dataAccessResult.Data == null
                ? ServiceResult.NotFound<TDto>()
                : ServiceResult.Error<TDto>(dataAccessResult.Errors);
        }

        public static ServiceResult ToServiceResult(this DataAccessResult dataAccessResult)
        {
            if (dataAccessResult == null)
            { return null; }

            return dataAccessResult.OperationSucceed
                ? ServiceResult.Succeed()
                : ServiceResult.Error(dataAccessResult.Errors);
        }

        public static string YesNo(this DataAccessResult dataAccessResult)
         => dataAccessResult != null && dataAccessResult.OperationSucceed ? "Yes" : "No";
    }
}
