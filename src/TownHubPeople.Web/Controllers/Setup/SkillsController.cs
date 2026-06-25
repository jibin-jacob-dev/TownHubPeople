using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Entities.Workforce;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.ViewModels.Setup;

namespace TownHubPeople.Web.Controllers.Setup;

[AuthorizeTab("Setup", "Skills")]
[Route("Setup/[controller]")]
public class SkillsController : MasterDataController<Skill, SkillViewModel>
{
    public SkillsController(IUnitOfWork uow) : base(uow) { }
    protected override string ViewFolder => "~/Views/Setup/Skills";

    protected override IQueryable<Skill> Search(IQueryable<Skill> q, string t)
        => q.Where(x => x.Name.Contains(t) || (x.Category != null && x.Category.Contains(t)));

    protected override IQueryable<Skill> Order(IQueryable<Skill> q, string? col, bool asc) => col switch
    {
        "category" => asc ? q.OrderBy(x => x.Category) : q.OrderByDescending(x => x.Category),
        "isActive" => asc ? q.OrderBy(x => x.IsActive) : q.OrderByDescending(x => x.IsActive),
        _ => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
    };

    protected override object Project(Skill e) => new
    {
        id = e.Id,
        name = e.Name,
        category = e.Category ?? "",
        isActive = e.IsActive
    };

    protected override SkillViewModel ToViewModel(Skill e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Category = e.Category,
        Description = e.Description,
        IsActive = e.IsActive
    };

    protected override void Apply(SkillViewModel vm, Skill e)
    {
        e.Name = vm.Name;
        e.Category = vm.Category;
        e.Description = vm.Description;
        e.IsActive = vm.IsActive;
    }

    protected override async Task<string?> ValidateAsync(SkillViewModel vm, int id)
        => await Repo.QueryNoTracking().AnyAsync(x => x.Name == vm.Name && x.Id != id)
            ? "A skill with this name already exists."
            : null;
}
