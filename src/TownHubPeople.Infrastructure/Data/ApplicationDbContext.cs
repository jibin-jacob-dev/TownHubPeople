using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TownHubPeople.Domain.Common;
using TownHubPeople.Domain.Entities.Identity;
using TownHubPeople.Domain.Entities.Admin;
using TownHubPeople.Domain.Entities.Workforce;

namespace TownHubPeople.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<AuditTrailEntry> AuditTrailEntries => Set<AuditTrailEntry>();
    public DbSet<DocumentSequence> DocumentSequences => Set<DocumentSequence>();
    public DbSet<RoleMenuAccess> RoleMenuAccess => Set<RoleMenuAccess>();
    public DbSet<RoleTabAccess> RoleTabAccess => Set<RoleTabAccess>();
    public DbSet<TabDisplayConfig> TabDisplayConfigs => Set<TabDisplayConfig>();
    public DbSet<ModuleDisplayConfig> ModuleDisplayConfigs => Set<ModuleDisplayConfig>();
    public DbSet<LoginPageSlide> LoginPageSlides => Set<LoginPageSlide>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<EmailOutboxMessage> EmailOutbox => Set<EmailOutboxMessage>();

    // Workforce Setup (master data)
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Designation> Designations => Set<Designation>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<CostCenter> CostCenters => Set<CostCenter>();
    public DbSet<EmploymentType> EmploymentTypes => Set<EmploymentType>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<HolidayCalendar> HolidayCalendars => Set<HolidayCalendar>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<Skill> Skills => Set<Skill>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
                var falseConstant = Expression.Constant(false);
                var lambda = Expression.Lambda(Expression.Equal(property, falseConstant), parameter);
                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
