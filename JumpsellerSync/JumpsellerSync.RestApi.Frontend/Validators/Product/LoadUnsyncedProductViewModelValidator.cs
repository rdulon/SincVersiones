using FluentValidation;

using JumpsellerSync.RestApi.FrontEnd.Models;

namespace JumpsellerSync.RestApi.FrontEnd.Validators.Product
{
    public class LoadUnsyncedProductViewModelValidator
        : AbstractValidator<LoadUnsyncedProductsPageViewModel>
    {
        public LoadUnsyncedProductViewModelValidator()
        {
            RuleFor(m => m.ProviderId).NotEmpty();
            RuleFor(m => m.Page).GreaterThanOrEqualTo(1);
            RuleFor(m => m.Limit).GreaterThanOrEqualTo(1);
        }
    }
}
