using Serilog;
using TownHubPeople.Application.Common.Mappings;
using TownHubPeople.Application.Admin.AuditTrail;
using TownHubPeople.Application.Admin.MenuConfiguration;
using TownHubPeople.Application.Admin.Organization;
using TownHubPeople.Application.Admin.Services;
using TownHubPeople.Infrastructure;
using TownHubPeople.Infrastructure.Data.Seeding;
using TownHubPeople.Infrastructure.Services;
using TownHubPeople.Domain.Interfaces;
using TownHubPeople.Web.Authorization;
using TownHubPeople.Web.Middleware;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddValidatorsFromAssemblyContaining<TownHubPeople.Application.Common.Models.Result>();

MappingConfig.RegisterMappings();

// Application services
builder.Services.AddScoped<IMenuConfigurationService, MenuConfigurationService>();
builder.Services.AddScoped<IAuditTrailService, AuditTrailService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddSingleton<TownHubPeople.Application.Common.Hints.IFormHintProvider,
    TownHubPeople.Application.Common.Hints.FormHintProvider>();

// Role/tab access authorization handler
builder.Services.AddScoped<IAuthorizationHandler, TabAccessHandler>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var seeder = new ApplicationDbContextSeed(scope.ServiceProvider);
    await seeder.SeedAsync();
}

app.Run();
