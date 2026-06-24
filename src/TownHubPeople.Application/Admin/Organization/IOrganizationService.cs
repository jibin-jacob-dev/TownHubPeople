using TownHubPeople.Application.Common.Models;

namespace TownHubPeople.Application.Admin.Organization;

public interface IOrganizationService
{
    /// <summary>Returns the singleton organization profile (never null; defaults if unset).</summary>
    Task<OrganizationDto> GetAsync(CancellationToken ct = default);
    Task<Result> UpdateAsync(OrganizationDto dto, CancellationToken ct = default);
}
