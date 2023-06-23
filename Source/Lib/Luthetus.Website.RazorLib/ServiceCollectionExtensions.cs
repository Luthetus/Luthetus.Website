using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.WatchWindow;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewDisplays;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Website.RazorLib.Settings;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.RazorLib.FormsGeneric;
using Luthetus.Ide.RazorLib.File;
using Luthetus.Ide.RazorLib.TreeViewImplementations;
using Luthetus.Ide.RazorLib.NuGet;
using Luthetus.Ide.RazorLib.Git;
using Luthetus.Ide.RazorLib.CSharpProjectForm;
using Luthetus.Ide.RazorLib.InputFile;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Website.RazorLib.Repl.FileSystem;
using Luthetus.Ide.ClassLib.FileTemplates;
using Luthetus.Website.RazorLib.Repl.Run;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Ide.ClassLib.CompilerServices.HostedServiceCase;
using Luthetus.Ide.ClassLib.FileSystem.HostedServiceCase;
using Luthetus.Ide.RazorLib.HostedServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase;

namespace Luthetus.Website.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusWebsiteServices(
        this IServiceCollection services)
    {
        var watchWindowTreeViewRenderers = new WatchWindowTreeViewRenderers(
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(Common.RazorLib.WatchWindow.TreeViewDisplays.TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        var commonRendererTypes = new LuthetusCommonComponentRenderers(
            typeof(CommonBackgroundTaskDisplay),
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(Common.RazorLib.WatchWindow.TreeViewDisplays.TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            watchWindowTreeViewRenderers,
            typeof(RunFileDisplay));

        // TODO: Move registration of "ILuthetusCommonComponentRenderers" to LuthetusCommon
        services.AddSingleton<ILuthetusCommonComponentRenderers>(_ => commonRendererTypes);

        services.AddScoped<IEnvironmentProvider, ReplEnvironmentProvider>();
        services.AddScoped<IFileSystemProvider, ReplFileSystemProvider>();

        services.AddSingleton<ICommonBackgroundTaskQueue, CommonBackgroundTaskQueue>();
        services.AddSingleton<ICommonBackgroundTaskMonitor, CommonBackgroundTaskMonitor>();
        
        services.AddSingleton<ITextEditorBackgroundTaskQueue, TextEditorBackgroundTaskQueue>();
        services.AddSingleton<ITextEditorBackgroundTaskMonitor, TextEditorBackgroundTaskMonitor>();

        services.AddScoped<IFileSystemBackgroundTaskQueue, FileSystemBackgroundTaskQueue>();
        services.AddScoped<IFileSystemBackgroundTaskMonitor, FileSystemBackgroundTaskMonitor>();

        services.AddScoped<ICompilerServiceBackgroundTaskQueue, CompilerServiceBackgroundTaskQueue>();
        services.AddScoped<ICompilerServiceBackgroundTaskMonitor, CompilerServiceBackgroundTaskMonitor>();

        services.AddLuthetusTextEditor(options => 
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

        services.AddScoped<ILuthetusIdeComponentRenderers>(serviceProvider =>
        {
            var luthetusCommonComponentRenderers = serviceProvider
                .GetRequiredService<ILuthetusCommonComponentRenderers>();

            return new LuthetusIdeComponentRenderers(
                luthetusCommonComponentRenderers,
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
                typeof(CompilerServiceBackgroundTaskDisplay),
                typeof(FileSystemBackgroundTaskDisplay),
                typeof(TreeViewCSharpProjectDependenciesDisplay),
                typeof(TreeViewCSharpProjectNugetPackageReferencesDisplay),
                typeof(TreeViewCSharpProjectToProjectReferencesDisplay),
                typeof(TreeViewLightWeightNugetPackageRecordDisplay),
                typeof(TreeViewCSharpProjectToProjectReferenceDisplay),
                typeof(TreeViewSolutionFolderDisplay));
        });

        services.AddScoped<ICommonMenuOptionsFactory, CommonMenuOptionsFactory>();
        services.AddScoped<IFileTemplateProvider, FileTemplateProvider>();

        return services.AddFluxor(options =>
           options.ScanAssemblies(
               typeof(ServiceCollectionExtensions).Assembly,
               typeof(Luthetus.Common.RazorLib.LuthetusCommonOptions).Assembly,
               typeof(Luthetus.TextEditor.RazorLib.LuthetusTextEditorOptions).Assembly,
               typeof(Luthetus.Ide.ClassLib.ServiceCollectionExtensions).Assembly));
    }
}