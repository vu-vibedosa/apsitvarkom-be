using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class CleaningEventUpdateRequestValidator : AbstractValidator<CleaningEventUpdateRequest>
{
    private readonly IRepository<CleaningEvent> _cleaningEventRepository;

    public CleaningEventUpdateRequestValidator(IRepository<CleaningEvent> cleaningEventRepository)
    {
        _cleaningEventRepository = cleaningEventRepository;

        RuleFor(l => l.StartTime).NotNull().GreaterThan(DateTime.UtcNow);
        RuleFor(l => l.Id).NotNull().DependentRules(() =>
        {
            RuleFor(l => l.Id).MustAsync(async (req, _, context, _) =>
            {
                CleaningEvent? cleaningEvent;
                try
                {
                    cleaningEvent = await _cleaningEventRepository.GetByPropertyAsync(x => x.Id == req.Id);
                }
                catch (Exception)
                {
                    return true;
                }

                if (cleaningEvent is null)
                    return true;

                if (cleaningEvent.IsFinalized)
                    context.AddFailure("The cleaning event is already finalized.");

                return true;
            });
        });
    }
}