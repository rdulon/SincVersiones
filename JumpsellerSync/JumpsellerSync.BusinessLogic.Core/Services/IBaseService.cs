namespace JumpsellerSync.BusinessLogic.Core.Services
{
    public interface IBaseService<TCreateDto, TUpdateDto, TDetailsDto>
        : ICreateService<TCreateDto, TDetailsDto>,
          IReadService<TDetailsDto>,
          IUpdateService<TUpdateDto, TDetailsDto>,
          IDeleteService
        where TCreateDto : class
        where TUpdateDto : class
        where TDetailsDto : class
    { }
}
