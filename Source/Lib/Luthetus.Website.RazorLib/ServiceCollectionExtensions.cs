using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;
using Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.TextEditorCase;

namespace Luthetus.Website.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusWebsiteServices(
        this IServiceCollection services,
        bool isServerSide)
    {
        services.AddLuthetusIdeRazorLibServices(options => options with
        {
            IsNativeApplication = false
        });

        if (isServerSide)
            services.AddLuthetusWebsiteServerSideServices();
        else
            services.AddLuthetusWebsiteWasmServices();

        return services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonOptions).Assembly,
            typeof(LuthetusTextEditorOptions).Assembly,
            typeof(Luthetus.Ide.ClassLib.ServiceCollectionExtensions).Assembly));
    }
    
    private static IServiceCollection AddLuthetusWebsiteServerSideServices(
        this IServiceCollection services)
    {
        return services
            .AddSingleton<LuthetusCommonBackgroundTaskServiceWorker>()
            .AddSingleton<LuthetusTextEditorTextEditorBackgroundTaskServiceWorker>()
            .AddSingleton<LuthetusTextEditorCompilerServiceBackgroundTaskServiceWorker>()
            .AddSingleton<LuthetusIdeFileSystemBackgroundTaskServiceWorker>()
            .AddSingleton<LuthetusIdeTerminalBackgroundTaskServiceWorker>();

        //return services
        //    .AddHostedService<LuthetusCommonBackgroundTaskServiceWorker>()
        //    .AddHostedService<LuthetusTextEditorTextEditorBackgroundTaskServiceWorker>()
        //    .AddHostedService<LuthetusTextEditorCompilerServiceBackgroundTaskServiceWorker>()
        //    .AddHostedService<LuthetusIdeFileSystemBackgroundTaskServiceWorker>()
        //    .AddHostedService<LuthetusIdeTerminalBackgroundTaskServiceWorker>();
    }
    
    private static IServiceCollection AddLuthetusWebsiteWasmServices(
        this IServiceCollection services)
    {
        return services
            .AddSingleton<LuthetusCommonBackgroundTaskServiceWorker>()
            .AddSingleton<LuthetusTextEditorTextEditorBackgroundTaskServiceWorker>()
            .AddSingleton<LuthetusTextEditorCompilerServiceBackgroundTaskServiceWorker>()
            .AddSingleton<LuthetusIdeFileSystemBackgroundTaskServiceWorker>()
            .AddSingleton<LuthetusIdeTerminalBackgroundTaskServiceWorker>();
    }
}