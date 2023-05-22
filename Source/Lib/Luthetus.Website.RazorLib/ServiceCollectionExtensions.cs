using Luthetus.Common.RazorLib.BackgroundTaskCase;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.WatchWindow;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewDisplays;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Website.RazorLib.Settings;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Website.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusWebsiteServices(this IServiceCollection services)
    {
        var shouldInitializeFluxor = false;

        var watchWindowTreeViewRenderers = new WatchWindowTreeViewRenderers(
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        var commonRendererTypes = new LuthetusCommonComponentRenderers(
            typeof(BackgroundTaskDisplay),
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            watchWindowTreeViewRenderers);

        // TODO: Move registration of "ILuthetusCommonComponentRenderers" to LuthetusCommon
        services.AddSingleton<ILuthetusCommonComponentRenderers>(_ => commonRendererTypes);

        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddSingleton<IBackgroundTaskMonitor, BackgroundTaskMonitor>();

        services.AddHostedService<QueuedHostedService>();

        services.AddLuthetusTextEditor(options => options with
        {
            InitializeFluxor = shouldInitializeFluxor,
            SettingsComponentRendererType = typeof(SettingsDisplay),
            SettingsDialogComponentIsResizable = true,
            LuthetusCommonOptions = (options.LuthetusCommonOptions ?? new()) with
            {
                InitializeFluxor = shouldInitializeFluxor
            },
        });

        return services.AddFluxor(options =>
           options.ScanAssemblies(
               typeof(ServiceCollectionExtensions).Assembly,
               typeof(Luthetus.Common.RazorLib.ServiceCollectionExtensions).Assembly,
               typeof(Luthetus.TextEditor.RazorLib.ServiceCollectionExtensions).Assembly));
    }
}