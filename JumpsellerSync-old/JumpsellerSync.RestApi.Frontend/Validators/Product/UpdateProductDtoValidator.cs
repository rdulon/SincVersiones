using FluentValidation;

using JumpsellerSync.BusinessLogic.Core.Dtos.Main;

namespace JumpsellerSync.RestApi.FrontEnd.Validators.Product
{
    public class UpdateProductDtoValidator : AbstractValidator<ProductDetailsDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty()
                .WithMessage("Producto incorrecto.");

            RuleFor(p => p.Margin)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100)
                .WithMessage("El margen debe ser un valor entre 0 y 100.");
        }
    }
}
