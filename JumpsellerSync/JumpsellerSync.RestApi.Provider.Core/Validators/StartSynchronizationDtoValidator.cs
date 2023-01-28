using FluentValidation;

using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;

namespace JumpsellerSync.RestApi.Provider.Core.Validators
{
    public class StartSynchronizationDtoValidator : AbstractValidator<StartSynchronizationDto>
    {
        public StartSynchronizationDtoValidator()
        {
            RuleFor(syncInfo => syncInfo.SyncSessionId)
                .NotEmpty()
                .WithMessage("The synchronization session id cannot be empty");
        }
    }
}
