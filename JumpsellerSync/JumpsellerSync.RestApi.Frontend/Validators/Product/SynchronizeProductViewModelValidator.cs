using FluentValidation;

using JumpsellerSync.BusinessLogic.Core.Dtos.Main;

namespace JumpsellerSync.RestApi.FrontEnd.Validators.Product
{
    public class SynchronizeProductViewModelValidator : AbstractValidator<CreateProductDto>
    {
        public SynchronizeProductViewModelValidator()
        {
            RuleFor(dto => dto.ProviderId)
                .NotEmpty();

            RuleFor(dto => dto.ProviderProductId)
                .NotEmpty();

            RuleFor(dto => dto.Margin)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100);

            RuleFor(dto => dto.Sku)
                .NotEmpty();
        }
    }
}
