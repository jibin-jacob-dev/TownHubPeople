using TownHubPeople.Domain.Common;
using TownHubPeople.Domain.Entities.Identity;
using TownHubPeople.Domain.Entities.Workforce;
using TownHubPeople.Domain.Enums;

namespace TownHubPeople.Domain.Entities.People;

public class Employee : AuditableEntity
{
    // Identity
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FullName => string.IsNullOrWhiteSpace(MiddleName)
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";

    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? PersonalEmail { get; set; }
    public string? Phone { get; set; }
    public string? WorkEmail { get; set; }
    public string? ProfileImageUrl { get; set; }

    // Job
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int DesignationId { get; set; }
    public Designation? Designation { get; set; }

    public int? GradeId { get; set; }
    public Grade? Grade { get; set; }

    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public int? EmploymentTypeId { get; set; }
    public EmploymentType? EmploymentType { get; set; }

    public int? CostCenterId { get; set; }
    public CostCenter? CostCenter { get; set; }

    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }

    public DateOnly DateOfJoining { get; set; }
    public DateOnly? DateOfConfirmation { get; set; }
    public DateOnly? ProbationEndDate { get; set; }

    public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;

    // Optional link to a login account
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
