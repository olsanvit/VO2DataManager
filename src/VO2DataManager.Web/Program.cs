using BlazorVO2DataManager;
using BlazorVO2DataManager.Components;
using MercenariesAndBeasts.Infrastructure;
using MercenariesAndBeasts.Infrastructure.Auth;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using SharedServices;
using SharedServices.Services;
using Services;
using VO2DataManager.Web.Achievements;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.SessionStorage;
using VO2DataManager.Services;
using ApexCharts;
using MudBlazor.Services;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Logs"));
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console(
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        shared: true,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .WriteTo.PostgreSQL(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection1") ?? "",
        tableName: "Logs",
        columnOptions: (IDictionary<string, ColumnWriterBase>?)null,
        needAutoCreateTable: true,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Logging.AddDebug();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.Configure<CircuitOptions>(o =>
{
    o.DisconnectedCircuitRetentionPeriod = TimeSpan.FromHours(8);
    o.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(2);
});
builder.Services.Configure<HubOptions>(o =>
{
    o.KeepAliveInterval = TimeSpan.FromSeconds(15);
    o.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    o.HandshakeTimeout = TimeSpan.FromSeconds(30);
});

var cs = "";
#if DEBUG
cs = builder.Configuration.GetConnectionString("DefaultConnection1QNAP");
#else
cs = builder.Configuration.GetConnectionString("DefaultConnection1");
#endif
var dsb = new NpgsqlDataSourceBuilder(cs);
dsb.EnableDynamicJson();
var dataSource = dsb.Build();

builder.Services.AddDbContextFactory<AppDbContextAiData>(options =>
    options.UseNpgsql(dataSource, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(180);
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    })
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors());

builder.Services.AddScoped<AppDbContextAiData>(sp =>
    sp.GetRequiredService<IDbContextFactory<AppDbContextAiData>>().CreateDbContext());

// ── Identity context (same DB as AiData, separate EF context for Identity tables) ──
builder.Services.AddDbContext<AppDbContextAiDataIdentity>(options =>
    options.UseNpgsql(cs!));
builder.Services.AddMabAuth<AppDbContextAiDataIdentity>(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors();
builder.Services.AddHealthChecks();
builder.Services.AddMudServices();
builder.Services.AddSharedUI(builder.Configuration);
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<UiLibraryService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<AchievementService>(sp =>
    new AchievementService(
        sp.GetRequiredService<ToastService>(),
        sp.GetRequiredService<IWebHostEnvironment>())
    {
        Definitions = VO2Achievements.All
    });
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<LoadingService>();
builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddApexCharts();
builder.Services.AddScoped<ErrorService<AppDbContextAiData>>();
builder.Services.AddScoped<EfCoreService<AppDbContextAiData>>();
builder.Services.AddSingleton<SharedServices.Services.ThemeService>(_ => new SharedServices.Services.ThemeService(builder.Configuration));
builder.Services.AddScoped<AiDataSyncService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.WebHost.ConfigureKestrel(k =>
{
    k.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(3);
    k.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
});

AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
    Log.Fatal(e.ExceptionObject as Exception, "UNHANDLED AppDomain exception");
TaskScheduler.UnobservedTaskException += (sender, e) =>
{
    Log.Fatal(e.Exception, "UNOBSERVED task exception");
    e.SetObserved();
};

var app = builder.Build();
app.MapHealthChecks("/health");
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

var pathBase = builder.Configuration["PathBase"];
if (!string.IsNullOrWhiteSpace(pathBase))
    app.UsePathBase(pathBase);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

// UseStaticFiles() odstraněno — MapStaticAssets() dole pokrývá static files s .NET 10 optimalizacemi
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// ── Google OAuth endpoints ────────────────────────────────────────────────
app.MapPost("/auth/google/start", (string? returnUrl, Microsoft.AspNetCore.Authentication.IAuthenticationService _, HttpContext ctx) =>
{
    var redirectUrl = $"/auth/google/callback?returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}";
    var props = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = redirectUrl };
    return Results.Challenge(props, ["Google"]);
});

app.MapGet("/auth/google/callback", async (
    HttpContext ctx,
    Microsoft.AspNetCore.Identity.SignInManager<AppUser> signInManager,
    Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager,
    IWebHostEnvironment env,
    IConfiguration config,
    string? returnUrl) =>
{
    var info = await signInManager.GetExternalLoginInfoAsync();
    if (info is null) return Results.Redirect("/login?error=external");

    var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
    if (result.Succeeded)
    {
        var signedInUser = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (signedInUser is not null)
        {
            var denied = await MercenariesAndBeasts.Infrastructure.Auth.AccessGate.CheckAsync(signedInUser, signInManager, env, config);
            if (denied is not null) return Results.Redirect(denied);
        }
        return Results.Redirect(returnUrl ?? "/");
    }

    var email = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
    var user  = await userManager.FindByEmailAsync(email);
    if (user is null)
    {
        user = new AppUser { UserName = email, Email = email, EmailConfirmed = true };
        await userManager.CreateAsync(user);
    }
    await userManager.AddLoginAsync(user, info);
    await signInManager.SignInAsync(user, false);
    var deniedFinal = await MercenariesAndBeasts.Infrastructure.Auth.AccessGate.CheckAsync(user, signInManager, env, config);
    if (deniedFinal is not null) return Results.Redirect(deniedFinal);
    return Results.Redirect(returnUrl ?? "/");
});

app.MapGet("/logout", async (HttpContext ctx, Microsoft.AspNetCore.Identity.SignInManager<AppUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ── Migrate Identity DB + Seed admin ─────────────────────────────────────
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db          = scope.ServiceProvider.GetRequiredService<AppDbContextAiDataIdentity>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await db.Database.MigrateAsync();
        await SeedAdminAsync(userManager, roleManager);
    }
}
catch (Exception ex) { Log.Warning(ex, "DB migration/seed skipped — DB not available"); }


// Seed role a admin účet
await AdminUserSeeder.SeedAsync(app.Services, app.Configuration);

app.Lifetime.ApplicationStopping.Register(() =>
    Log.Warning("Application stopping — flushing logs..."));

try { app.Run(); }
catch (Exception ex) { Log.Fatal(ex, "Host terminated unexpectedly"); }
finally { Log.CloseAndFlush(); }

// ── Seed helpers ──────────────────────────────────────────────────────────
static async Task SeedAdminAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
{
    const string adminRole = "Admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
        await roleManager.CreateAsync(new IdentityRole(adminRole));

    await EnsureAdminAsync(userManager, adminRole, "admin@local",             "admin", "Admin123.");
    await EnsureAdminAsync(userManager, adminRole, "olsanskyvitek@gmail.com", "vitek", "Vitek575");
}

static async Task EnsureAdminAsync(UserManager<AppUser> userManager, string adminRole,
    string email, string username, string password)
{
    var user = await userManager.FindByEmailAsync(email);
    if (user is null)
    {
        user = new AppUser { UserName = username, Email = email, EmailConfirmed = true, IsAdmin = true };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception($"Failed to create {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }
    if (!await userManager.IsInRoleAsync(user, adminRole))
        await userManager.AddToRoleAsync(user, adminRole);
}

public partial class Program { }
