using Fluxor;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Css;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.FSharp;
using Luthetus.CompilerServices.Lang.JavaScript;
using Luthetus.CompilerServices.Lang.Json;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.TypeScript;
using Luthetus.CompilerServices.Lang.Xml;
using Luthetus.Ide.Wasm.Facts;
using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Website.RazorLib;

public partial class LuthetusWebsiteInitializer : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    private XmlCompilerService XmlCompilerService { get; set; } = null!;
    [Inject]
    private DotNetSolutionCompilerService DotNetCompilerService { get; set; } = null!;
    [Inject]
    private CSharpProjectCompilerService CSharpProjectCompilerService { get; set; } = null!;
    [Inject]
    private CSharpCompilerService CSharpCompilerService { get; set; } = null!;
    [Inject]
    private RazorCompilerService RazorCompilerService { get; set; } = null!;
    [Inject]
    private CssCompilerService CssCompilerService { get; set; } = null!;
    [Inject]
    private FSharpCompilerService FSharpCompilerService { get; set; } = null!;
    [Inject]
    private JavaScriptCompilerService JavaScriptCompilerService { get; set; } = null!;
    [Inject]
    private TypeScriptCompilerService TypeScriptCompilerService { get; set; } = null!;
    [Inject]
    private JsonCompilerService JsonCompilerService { get; set; } = null!;
    [Inject]
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;
    [Inject]
    private EditorSync EditorSync { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
                "Initialize Website",
                async () =>
                {
                    await WriteFileSystemInMemoryAsync();

                    InitializeDotNetSolutionAndExplorer();

                    await ParseSolutionAsync();

                    // Display a file from the get-go so the user is less confused on what the website is.
                    var absolutePath = new AbsolutePath(
                        InitialSolutionFacts.PROGRAM_ABSOLUTE_FILE_PATH,
                        false,
                        EnvironmentProvider);

                    EditorSync.OpenInEditor(absolutePath, false);

                    // This code block is hacky. I want the Solution Explorer to from the get-go be fully expanded, so the user can see 'Program.cs'
                    {
                        TreeViewService.MoveRight(DotNetSolutionState.TreeViewSolutionExplorerStateKey, false);
                        TreeViewService.MoveRight(DotNetSolutionState.TreeViewSolutionExplorerStateKey, false);
                        TreeViewService.MoveRight(DotNetSolutionState.TreeViewSolutionExplorerStateKey, false);
                    }
                });
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task WriteFileSystemInMemoryAsync()
    {
        // Program.cs
        await FileSystemProvider.File.WriteAllTextAsync(
            InitialSolutionFacts.PROGRAM_ABSOLUTE_FILE_PATH,
            InitialSolutionFacts.PROGRAM_CONTENTS);

        // ConsoleApp1.csproj
        await FileSystemProvider.File.WriteAllTextAsync(
            InitialSolutionFacts.CSPROJ_ABSOLUTE_FILE_PATH,
            InitialSolutionFacts.CSPROJ_CONTENTS);

        // ConsoleApp1.sln
        await FileSystemProvider.File.WriteAllTextAsync(
            InitialSolutionFacts.SLN_ABSOLUTE_FILE_PATH,
            InitialSolutionFacts.SLN_CONTENTS);
    }

    private void InitializeDotNetSolutionAndExplorer()
    {
        var solutionAbsolutePath = new AbsolutePath(
            InitialSolutionFacts.SLN_ABSOLUTE_FILE_PATH,
            false,
            EnvironmentProvider);

        DotNetSolutionSync.SetDotNetSolution(solutionAbsolutePath);
    }

    private async Task ParseSolutionAsync()
    {
        var allFiles = new List<string>();

        await RecursiveStep(
            new List<string> { "/" },
            allFiles);

        async Task RecursiveStep(IEnumerable<string> directories, List<string> allFiles)
        {
            foreach (var directory in directories)
            {
                var childDirectories = await FileSystemProvider.Directory
                    .GetDirectoriesAsync(directory);

                allFiles.AddRange(await FileSystemProvider.Directory
                    .GetFilesAsync(directory));

                await RecursiveStep(childDirectories, allFiles);
            }
        }

        foreach (var file in allFiles)
        {
            var absolutePath = new AbsolutePath(file, false, EnvironmentProvider);

            var resourceUri = new ResourceUri(file);

            var fileLastWriteTime = await FileSystemProvider.File.GetLastWriteTimeAsync(
                file);

            var content = await FileSystemProvider.File.ReadAllTextAsync(
                file);

            var compilerService = ExtensionNoPeriodFacts.GetCompilerService(
                absolutePath.ExtensionNoPeriod,
                XmlCompilerService,
                DotNetCompilerService,
                CSharpProjectCompilerService,
                CSharpCompilerService,
                RazorCompilerService,
                CssCompilerService,
                FSharpCompilerService,
                JavaScriptCompilerService,
                TypeScriptCompilerService,
                JsonCompilerService);

            var decorationMapper = ExtensionNoPeriodFacts.GetDecorationMapper(
                absolutePath.ExtensionNoPeriod);

            var textEditorModel = new TextEditorModel(
                resourceUri,
                fileLastWriteTime,
                absolutePath.ExtensionNoPeriod,
                content,
                compilerService,
                decorationMapper,
                null,
                new(),
                Key<TextEditorModel>.NewKey()
            );

            textEditorModel.CompilerService.RegisterModel(textEditorModel);

            TextEditorService.Model.RegisterCustom(textEditorModel);

            TextEditorService.Model.RegisterPresentationModel(
                    textEditorModel.ModelKey,
                    CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);

            await textEditorModel.ApplySyntaxHighlightingAsync();
        }
    }
}