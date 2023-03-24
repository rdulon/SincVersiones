using FluentValidation;

using JumpsellerSync.RestApi.FrontEnd.Models;

namespace JumpsellerSync.RestApi.FrontEnd.Validators.LocalProduct
{
    public class UpdateLocalProductsValidator : AbstractValidator<UpdateLocalProductsViewModel>
    {
        public UpdateLocalProductsValidator()
        {
            RuleFor(vm => vm.Products)
                .NotEmpty();

            RuleForEach(vm => vm.Products)
                .ChildRules(lpValidator =>
                {
                    lpValidator.RuleFor(lp => lp.Id)
                        .NotEmpty();
                    lpValidator.RuleFor(lp => lp.Price)
                        .GreaterThanOrEqualTo(0);
                    lpValidator.RuleFor(lp => lp.Stock)
                        .GreaterThanOrEqualTo(0);
                });
        }
    }
}
