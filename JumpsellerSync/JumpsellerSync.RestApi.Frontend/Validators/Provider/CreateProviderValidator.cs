using FluentValidation;

using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.Common.Util.Extensions;

using System;
using System.Text.RegularExpressions;

namespace JumpsellerSync.RestApi.FrontEnd.Validators.Provider
{
    public class CreateProviderValidator : AbstractValidator<ProviderDto>
    {
        public static readonly Regex tsRegex =
            new Regex(@"^(0?\d|1\d|2[0-3]):(0?\d|[1-5]\d)$");

        public CreateProviderValidator()
        {
            RuleFor(p => p.Url)
                .NotEmpty()
                .WithMessage("La URL del proveedor es obligatoria.")
                .UrlAddress()
                .WithMessage("La URL del proveedor no es correcta.");

            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("El nombre del proveedor es obligatorio.");

            RuleFor(p => p.ProviderType)
                .Must(pt => Enum.IsDefined(typeof(ProviderType), pt))
                .WithMessage("Tipo de proveedor no válido.");

            When(
                p => p.ProviderType == ProviderType.HourlyProvider,
                () =>
                {
                    RuleFor(p => p.Hours)
                        .NotEmpty()
                        .WithMessage("Debe especificar al menos una hora para la sincronización.");
                    RuleForEach(p => p.Hours)
                        .Must(h => tsRegex.IsMatch(h ?? ""));
                });

            When(
                p => p.ProviderType == ProviderType.PeriodicallyProvider,
                () =>
                {
                    RuleFor(p => p.Interval)
                        .NotNull()
                        .WithMessage("Debe especificar el intervalo de actualización.");

                    RuleFor(p => p.StartTime)
                        .NotEmpty()
                        .WithMessage("Debe proveer la hora de inicio.")
                        .Must(s => tsRegex.IsMatch(s ?? ""))
                        .WithMessage("El formato de la hora de inicio es 23:59.");
                });
        }
    }
}
