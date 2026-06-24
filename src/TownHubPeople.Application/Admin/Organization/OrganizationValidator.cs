using FluentValidation;

namespace TownHubPeople.Application.Admin.Organization;

public class OrganizationValidator : AbstractValidator<OrganizationDto>
{
    public OrganizationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.CurrencySymbol).NotEmpty().MaximumLength(10);
    }
}
