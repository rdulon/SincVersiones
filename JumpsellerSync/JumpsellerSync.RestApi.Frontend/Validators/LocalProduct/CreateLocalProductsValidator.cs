using FluentValidation;

using JumpsellerSync.RestApi.FrontEnd.Models;

namespace JumpsellerSync.RestApi.FrontEnd.Validators.LocalProduct
{
    public class CreateLocalProductsValidator : AbstractValidator<CreateLocalProductsViewModel>
    {
        public CreateLocalProductsValidator()
        {
            RuleFor(vm => vm.Products)
                .NotEmpty();

            RuleForEach(vm => vm.Products)
                .ChildRules(products =>
                {
                    products.RuleFor(p => p.ProductId)
                        .NotEmpty();
                    products.When(
                        p => p.Price != null,
                        () =>
                        {
                            products.RuleFor(p => p.Price.Value)
                                .GreaterThanOrEqualTo(0);
                        });
                    products.When(
                        p => p.Stock != null,
                        () =>
                        {
                            products.RuleFor(p => p.Stock.Value)
                                .GreaterThanOrEqualTo(0);
                        });
                });

        }
    }
}
