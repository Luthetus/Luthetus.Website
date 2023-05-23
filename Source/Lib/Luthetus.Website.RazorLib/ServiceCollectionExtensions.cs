using Luthetus.Common.RazorLib.BackgroundTaskCase;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.WatchWindow;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewDisplays;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Website.RazorLib.Settings;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Repl;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.RazorLib.FormsGeneric;
using Luthetus.Ide.RazorLib.File;
using Luthetus.Ide.RazorLib.TreeViewImplementations;
using Luthetus.Ide.RazorLib.NuGet;
using Luthetus.Ide.RazorLib.Git;
using Luthetus.Ide.RazorLib.CSharpProjectForm;
using Luthetus.Ide.RazorLib.InputFile;

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
            typeof(Common.RazorLib.WatchWindow.TreeViewDisplays.TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        var commonRendererTypes = new LuthetusCommonComponentRenderers(
            typeof(BackgroundTaskDisplay),
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(Common.RazorLib.WatchWindow.TreeViewDisplays.TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            watchWindowTreeViewRenderers);

        // TODO: Move registration of "ILuthetusCommonComponentRenderers" to LuthetusCommon
        services.AddSingleton<ILuthetusCommonComponentRenderers>(_ => commonRendererTypes);

        services.AddScoped<IEnvironmentProvider, ReplEnvironmentProvider>();
        services.AddScoped<IFileSystemProvider, ReplFileSystemProvider>();

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

        services.AddScoped<ILuthetusIdeComponentRenderers>(serviceProvider =>
        {
            var blazorCommonComponentRenderers = serviceProvider
                .GetRequiredService<ILuthetusCommonComponentRenderers>();

            return new LuthetusIdeComponentRenderers(
                blazorCommonComponentRenderers,
                typeof(BooleanPromptOrCancelDisplay),
                typeof(FileFormDisplay),
                typeof(DeleteFileFormDisplay),
                typeof(TreeViewNamespacePathDisplay),
                typeof(TreeViewAbsoluteFilePathDisplay),
                typeof(TreeViewGitFileDisplay),
                typeof(NuGetPackageManager),
                typeof(GitChangesDisplay),
                typeof(RemoveCSharpProjectFromSolutionDisplay),
                typeof(InputFileDisplay),
                typeof(TreeViewCSharpProjectDependenciesDisplay),
                typeof(TreeViewCSharpProjectNugetPackageReferencesDisplay),
                typeof(TreeViewCSharpProjectToProjectReferencesDisplay),
                typeof(TreeViewLightWeightNugetPackageRecordDisplay),
                typeof(TreeViewCSharpProjectToProjectReferenceDisplay),
                typeof(TreeViewSolutionFolderDisplay));
        });

        return services.AddFluxor(options =>
           options.ScanAssemblies(
               typeof(ServiceCollectionExtensions).Assembly,
               typeof(Luthetus.Common.RazorLib.ServiceCollectionExtensions).Assembly,
               typeof(Luthetus.TextEditor.RazorLib.ServiceCollectionExtensions).Assembly));
    }
}