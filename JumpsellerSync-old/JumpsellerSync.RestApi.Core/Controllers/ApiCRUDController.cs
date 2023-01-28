using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Services;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json;
using System.Threading.Tasks;

namespace JumpsellerSync.RestApi.Core.Controllers
{
    public abstract class ApiCRUDController<TCreateDto, TUpdateDto, TDetailsDto>
        : ApiBaseController
        where TCreateDto : class
        where TUpdateDto : class
        where TDetailsDto : class
    {
        public ApiCRUDController(
            IBaseService<TCreateDto, TUpdateDto, TDetailsDto> service,
            JsonSerializerOptions jsonSerializerOptions)
            : base(jsonSerializerOptions)
        {
            Service = service;
        }


        protected IBaseService<TCreateDto, TUpdateDto, TDetailsDto> Service { get; }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult> Get(string id)
        {
            var details = await Service.ReadAsync(id);
            return Json(details.Data, true);
        }

        [HttpGet]
        public virtual async Task<ActionResult> Get([FromQuery] ReadPageFilterDto input)
        {
            if (ModelState.IsValid)
            {
                var page = await Service.ReadAsync(input.Filter, input.Page, input.Limit);
                return Json(page);
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await Service.CreateAsync(dto);
                return FromServiceResult(result);
            }
            return BadRequest();
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] TUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await Service.UpdateAsync(dto);
                return FromServiceResult(result);
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var result = await Service.DeleteAsync(id);
            return FromServiceResult(result);
        }
    }
}