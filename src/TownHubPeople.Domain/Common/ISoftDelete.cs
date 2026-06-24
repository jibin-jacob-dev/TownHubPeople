namespace TownHubPeople.Domain.Common;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    string? DeletedBy { get; set; }
    DateTime? DeletedDate { get; set; }
}
