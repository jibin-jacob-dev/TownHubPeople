using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TownHubPeople.Domain.Enums;

namespace TownHubPeople.Web.ViewModels.People;

public class EmployeeFormViewModel
{
    public int Id { get; set; }

    // Auto-generated on create; shown read-only.
    public string? EmployeeCode { get; set; }

    [Required, MaxLength(100), Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100), Display(Name = "Middle Name")]
    public string? MiddleName { get; set; }

    [Required, MaxLength(100), Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    public string? Gender { get; set; }

    [DataType(DataType.Date), Display(Name = "Date of Birth")]
    public DateOnly? DateOfBirth { get; set; }

    [EmailAddress, MaxLength(256), Display(Name = "Personal Email")]
    public string? PersonalEmail { get; set; }

    [MaxLength(40)]
    public string? Phone { get; set; }

    [EmailAddress, MaxLength(256), Display(Name = "Work Email")]
    public string? WorkEmail { get; set; }

    [Required, Display(Name = "Department")]
    public int DepartmentId { get; set; }

    [Required, Display(Name = "Designation")]
    public int DesignationId { get; set; }

    [Display(Name = "Grade")]
    public int? GradeId { get; set; }

    [Display(Name = "Location")]
    public int? LocationId { get; set; }

    [Display(Name = "Employment Type")]
    public int? EmploymentTypeId { get; set; }

    [Display(Name = "Cost Center")]
    public int? CostCenterId { get; set; }

    [Display(Name = "Reporting Manager")]
    public int? ManagerId { get; set; }

    [Required, DataType(DataType.Date), Display(Name = "Date of Joining")]
    public DateOnly DateOfJoining { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [DataType(DataType.Date), Display(Name = "Date of Confirmation")]
    public DateOnly? DateOfConfirmation { get; set; }

    [DataType(DataType.Date), Display(Name = "Probation End Date")]
    public DateOnly? ProbationEndDate { get; set; }

    public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;

    [Display(Name = "Login Account")]
    public string? UserId { get; set; }

    // Lookups
    public List<SelectListItem> AvailableDepartments { get; set; } = [];
    public List<SelectListItem> AvailableDesignations { get; set; } = [];
    public List<SelectListItem> AvailableGrades { get; set; } = [];
    public List<SelectListItem> AvailableLocations { get; set; } = [];
    public List<SelectListItem> AvailableEmploymentTypes { get; set; } = [];
    public List<SelectListItem> AvailableCostCenters { get; set; } = [];
    public List<SelectListItem> AvailableManagers { get; set; } = [];
    public List<SelectListItem> AvailableUsers { get; set; } = [];
}
