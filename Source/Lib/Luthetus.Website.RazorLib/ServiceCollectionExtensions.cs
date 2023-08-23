using Luthetus.Website.RazorLib.Settings;
using Luthetus.Website.RazorLib.Repl.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.TextEditor.RazorLib;
using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Ide.RazorLib;
using Luthetus.Common.RazorLib;

namespace Luthetus.Website.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusWebsiteServices(
        this IServiceCollection services)
    {
        services.AddLuthetusIdeRazorLibServices(
            false,
            options =>
            {
                var heightOfNavbarInPixels = 64;

                var luthetusCommonOptions = options.LuthetusCommonOptions ?? new();

                luthetusCommonOptions = luthetusCommonOptions with
                {
                    DialogServiceOptions = luthetusCommonOptions.DialogServiceOptions with
                    {
                        IsMaximizedStyleCssString = $"width: 100vw; height: calc(100vh - {heightOfNavbarInPixels}px); left: 0; top: {heightOfNavbarInPixels}px;"
                    }
                };

                return options with
                {
                    SettingsComponentRendererType = typeof(SettingsDisplay),
                    SettingsDialogComponentIsResizable = true,
                    LuthetusCommonOptions = luthetusCommonOptions
                };
            });

        services.AddScoped<IEnvironmentProvider, ReplEnvironmentProvider>();
        services.AddScoped<IFileSystemProvider, ReplFileSystemProvider>();

        return services.AddFluxor(options =>
            options.ScanAssemblies(
                typeof(ServiceCollectionExtensions).Assembly,
                typeof(LuthetusCommonOptions).Assembly,
                typeof(LuthetusTextEditorOptions).Assembly,
                typeof(Ide.ClassLib.ServiceCollectionExtensions).Assembly));
    }
}