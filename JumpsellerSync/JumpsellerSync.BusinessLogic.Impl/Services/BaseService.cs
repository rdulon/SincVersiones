using AutoMapper;

using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Services;
using JumpsellerSync.BusinessLogic.Filter;
using JumpsellerSync.BusinessLogic.Impl.Extensions;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.Repositories;
using JumpsellerSync.Domain.Impl;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Impl.Services
{
    public abstract class BaseService<TCreateDto, TUpdateDto, TDetailsDto, TModel, TLogger>
        : IBaseService<TCreateDto, TUpdateDto, TDetailsDto>
        where TCreateDto : class
        where TUpdateDto : class
        where TDetailsDto : class
        where TLogger : class
        where TModel : DomainModel
    {
        protected readonly IMapper mapper;
        protected readonly ILogger<TLogger> logger;
        protected readonly ICreateRepository<TModel> createRepository;
        protected readonly IUpdateRepository<TModel> updateRepository;
        protected readonly IReadRepository<TModel> readRepository;
        protected readonly IDeleteRepository<TModel> deleteRepository;
        private readonly DomainFilter<TModel>.Factory domainFilterFactory;

        public BaseService(
            IMapper mapper, ILogger<TLogger> logger, ICreateRepository<TModel> createRepository,
            IUpdateRepository<TModel> updateRepository, IReadRepository<TModel> readRepository,
            IDeleteRepository<TModel> deleteRepository,
            DomainFilter<TModel>.Factory domainFilterFactory)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.createRepository = createRepository;
            this.updateRepository = updateRepository;
            this.readRepository = readRepository;
            this.deleteRepository = deleteRepository;
            this.domainFilterFactory = domainFilterFactory;
        }

        public virtual async Task<ServiceResult<TDetailsDto>> CreateAsync(TCreateDto createDto)
        {
            var model = mapper.Map<TCreateDto, TModel>(createDto);
            var createResult = await createRepository.CreateAsync(model);

            return createResult.ToServiceResult<TModel, TDetailsDto>(mapper);
        }

        public virtual async Task<ServiceResult> DeleteAsync(string keyDto)
        {
            var model = await readRepository.ReadAsync(new[] { keyDto });
            if (!model.OperationSucceed)
            { return model.ToServiceResult(); }

            var deleteResult = await deleteRepository.DeleteAsync(model.Data);
            return deleteResult.ToServiceResult();
        }

        public virtual async Task<ServiceResult<TDetailsDto>> ReadAsync(string keyDto)
        {
            var readResult = await readRepository.ReadAsync(new[] { keyDto });
            return readResult.ToServiceResult<TModel, TDetailsDto>(mapper);
        }

        public virtual async Task<PageResultDto<TDetailsDto>> ReadAsync(string filter, int page, int limit)
        {
            try
            {
                var domainFilter = domainFilterFactory.Invoke(filter);
                var conditions = domainFilter.GetConditions();

                var offset = page.GetDbOffset(limit);
                var result = mapper.Map<IEnumerable<TModel>, IEnumerable<TDetailsDto>>(
                    await readRepository
                        .ReadAsync(conditions, offset, limit)
                        .ToListAsync());
                var pages = await readRepository.GetPagesAsync(conditions, limit);

                return new PageResultDto<TDetailsDto>
                {
                    Items = result,
                    Limit = limit,
                    Page = page,
                    TotalPages = pages
                };
            }
            catch (Exception e)
            { logger.LogException(e); }

            return new PageResultDto<TDetailsDto>
            {
                Items = Enumerable.Empty<TDetailsDto>(),
                TotalPages = 0,
                Page = page,
                Limit = limit
            };
        }

        public virtual async Task<ServiceResult<TDetailsDto>> UpdateAsync(TUpdateDto updatelDto)
        {
            var model = mapper.Map<TUpdateDto, TModel>(updatelDto);
            var updateResult = await updateRepository.UpdateAsync(model);

            return updateResult.ToServiceResult<TModel, TDetailsDto>(mapper);
        }
    }
}
