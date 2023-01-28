using FluentValidation;

using JumpsellerSync.Common.Util.Extensions;

namespace JumpsellerSync.Common.Util.Validators
{
    public class UrlValidator : AbstractValidator<string>
    {
        public UrlValidator()
        {
            RuleFor(s => s)
                .UrlAddress();
        }
    }
}
