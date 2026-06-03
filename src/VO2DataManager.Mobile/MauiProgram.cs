using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharedServices;

namespace VO2DataManager.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Konfigurace z embedded appsettings.json
        var assembly = typeof(MauiProgram).Assembly;
        using var stream = assembly.GetManifestResourceStream("VO2DataManager.Mobile.appsettings.json");
        if (stream is not null)
        {
            var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
            builder.Configuration.AddConfiguration(config);
        }

        // EF Core — přímé připojení k PostgreSQL
        var connStr = builder.Configuration.GetConnectionString("DefaultConnection1")
            ?? throw new InvalidOperationException("Chybí DefaultConnection1 v appsettings.json");

        builder.Services.AddDbContextFactory<AppDbContextAiData>(opt =>
        {
            opt.UseNpgsql(connStr);
#if DEBUG
            opt.EnableDetailedErrors();
#endif
        });

        return builder.Build();
    }
}
