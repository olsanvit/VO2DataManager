using MercenariesAndBeasts.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorVO2DataManager;

/// <summary>
/// Minimal Identity context for VO2DataManager.
/// Stores ASP.NET Identity tables (AspNetUsers, AspNetRoles, …) in the AIData database
/// alongside the existing data tables managed by AppDbContextAiData.
/// </summary>
public class AppDbContextAiDataIdentity : IdentityDbContext<AppUser>
{
    public AppDbContextAiDataIdentity(DbContextOptions<AppDbContextAiDataIdentity> options)
        : base(options) { }
}
