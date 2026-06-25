using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Application.Common.Models;
using TownHubPeople.Domain.Common;
using TownHubPeople.Domain.Interfaces;

namespace TownHubPeople.Web.Controllers.Setup;

/// <summary>
/// Generic CRUD controller for simple Workforce-Setup master data. Implements the shared
/// DataTables list + popup Create/Edit/Delete plumbing (AJAX-aware, mirroring the Admin
/// controllers); each entity only supplies its search/order/projection/mapping via the
/// abstract hooks. A single <c>_Form.cshtml</c> partial serves both Create and Edit.
/// </summary>
public abstract class MasterDataController<TEntity, TVm> : BaseController
    where TEntity : AuditableEntity, new()
    where TVm : class, new()
{
    protected readonly IUnitOfWork Uow;
    protected MasterDataController(IUnitOfWork uow) => Uow = uow;

    protected IRepository<TEntity> Repo => Uow.Repository<TEntity>();

    /// <summary>Razor folder holding Index.cshtml + _Form.cshtml, e.g. "~/Views/Setup/Departments".</summary>
    protected abstract string ViewFolder { get; }

    // --- per-entity hooks ---
    protected virtual IQueryable<TEntity> Includes(IQueryable<TEntity> q) => q;
    protected abstract IQueryable<TEntity> Search(IQueryable<TEntity> q, string term);
    protected abstract IQueryable<TEntity> Order(IQueryable<TEntity> q, string? column, bool asc);
    protected abstract object Project(TEntity e);
    protected abstract TVm ToViewModel(TEntity e);
    protected abstract void Apply(TVm vm, TEntity e);
    protected virtual Task PopulateLookupsAsync(TVm vm) => Task.CompletedTask;
    protected virtual Task<string?> ValidateAsync(TVm vm, int id) => Task.FromResult<string?>(null);
    protected virtual Task<string?> CanDeleteAsync(TEntity e) => Task.FromResult<string?>(null);

    [Route("")]
    public virtual IActionResult Index() => View($"{ViewFolder}/Index.cshtml");

    [HttpPost]
    [Route("GetData")]
    public async Task<IActionResult> GetData([FromBody] DataTablesRequest request)
    {
        var total = await Repo.QueryNoTracking().CountAsync();

        var query = Includes(Repo.QueryNoTracking());
        var term = request.Search?.Value?.Trim();
        if (!string.IsNullOrEmpty(term)) query = Search(query, term);

        var filtered = await query.CountAsync();

        var asc = request.Order.Count == 0 || request.Order[0].Dir == "asc";
        var col = request.Order.Count > 0 ? request.Columns[request.Order[0].Column].Data : null;
        query = Order(query, col, asc);

        var rows = await query.Skip(request.Start).Take(request.Length).ToListAsync();

        return Json(new DataTablesResponse<object>
        {
            Draw = request.Draw,
            RecordsTotal = total,
            RecordsFiltered = filtered,
            Data = rows.Select(Project).ToList()
        });
    }

    [Route("Create")]
    public async Task<IActionResult> Create()
    {
        var vm = new TVm();
        await PopulateLookupsAsync(vm);
        return PartialView($"{ViewFolder}/_Form.cshtml", vm);
    }

    [HttpPost]
    [Route("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TVm vm)
    {
        var error = ModelState.IsValid ? await ValidateAsync(vm, 0) : null;
        if (!ModelState.IsValid || error != null)
        {
            if (error != null) ModelState.AddModelError(string.Empty, error);
            return await Invalid(vm);
        }

        var entity = new TEntity();
        Apply(vm, entity);
        Repo.Add(entity);
        await Uow.SaveChangesAsync();
        return Saved("Saved successfully.");
    }

    [Route("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await Includes(Repo.QueryNoTracking()).FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null) return RecordMissing("Record not found.");

        var vm = ToViewModel(entity);
        await PopulateLookupsAsync(vm);
        return PartialView($"{ViewFolder}/_Form.cshtml", vm);
    }

    [HttpPost]
    [Route("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TVm vm)
    {
        var error = ModelState.IsValid ? await ValidateAsync(vm, id) : null;
        if (!ModelState.IsValid || error != null)
        {
            if (error != null) ModelState.AddModelError(string.Empty, error);
            return await Invalid(vm);
        }

        var entity = await Repo.Query().FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null) return RecordMissing("Record not found.");

        Apply(vm, entity);
        Repo.Update(entity);
        await Uow.SaveChangesAsync();
        return Saved("Saved successfully.");
    }

    [HttpPost]
    [Route("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Repo.Query().FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null) return Json(new { success = false, message = "Record not found." });

        var blocked = await CanDeleteAsync(entity);
        if (blocked != null) return Json(new { success = false, message = blocked });

        Repo.Remove(entity);
        await Uow.SaveChangesAsync();
        return Json(new { success = true, message = "Record deleted successfully." });
    }

    // --- shared AJAX envelope (same convention as the Admin controllers) ---
    private async Task<IActionResult> Invalid(TVm vm)
    {
        await PopulateLookupsAsync(vm);
        Response.StatusCode = StatusCodes.Status400BadRequest;
        return PartialView($"{ViewFolder}/_Form.cshtml", vm);
    }

    private IActionResult Saved(string message)
    {
        if (IsAjax()) return Json(new { success = true, message });
        SuccessNotification(message);
        return RedirectToAction(nameof(Index));
    }

    private IActionResult RecordMissing(string message)
    {
        if (IsAjax()) return Json(new { success = false, message });
        ErrorNotification(message);
        return RedirectToAction(nameof(Index));
    }
}
