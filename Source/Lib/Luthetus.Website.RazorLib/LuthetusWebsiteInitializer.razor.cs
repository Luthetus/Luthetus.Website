using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.TreeView;
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
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;
using Luthetus.Ide.Wasm.Facts;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib;
using Microsoft.AspNetCore.Components;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using System.Threading;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.TextEditorCase;
using Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;
using Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;
using Luthetus.Common.RazorLib;

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
    private ILuthetusCommonBackgroundTaskService LuthetusCommonBackgroundTaskService { get; set; } = null!;
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var backgroundTask = new BackgroundTask(
                async cancellationToken =>
                {
                    await WriteFileSystemInMemoryAsync();

                    InitializeDotNetSolutionAndExplorer();

                    await ParseSolutionAsync();

                    // Display a file from the get-go so the user is less confused on what the website is.
                    var absoluteFilePath = new AbsoluteFilePath(
                        InitialSolutionFacts.PROGRAM_ABSOLUTE_FILE_PATH,
                        false,
                        EnvironmentProvider);

                    Dispatcher.Dispatch(new EditorState.OpenInEditorAction(
                        absoluteFilePath,
                        false));

                    // This code block is hacky. I want the Solution Explorer to from the get-go be fully expanded, so the user can see 'Program.cs'
                    {
                        TreeViewService.MoveRight(DotNetSolutionState.TreeViewSolutionExplorerStateKey, false);
                        TreeViewService.MoveRight(DotNetSolutionState.TreeViewSolutionExplorerStateKey, false);
                        TreeViewService.MoveRight(DotNetSolutionState.TreeViewSolutionExplorerStateKey, false);
                    }
                },
                "Parsing Solution",
                string.Empty,
                true,
                _ => Task.CompletedTask,
                Dispatcher,
                CancellationToken.None);

            LuthetusCommonBackgroundTaskService.QueueBackgroundWorkItem(backgroundTask);
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
        var solutionAbsoluteFilePath = new AbsoluteFilePath(
            InitialSolutionFacts.SLN_ABSOLUTE_FILE_PATH,
            false,
            EnvironmentProvider);

        Dispatcher.Dispatch(new DotNetSolutionState.SetDotNetSolutionAction(
            solutionAbsoluteFilePath));
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
            var absoluteFilePath = new AbsoluteFilePath(file, false, EnvironmentProvider);

            var resourceUri = new ResourceUri(file);

            var fileLastWriteTime = await FileSystemProvider.File.GetLastWriteTimeAsync(
                file);

            var content = await FileSystemProvider.File.ReadAllTextAsync(
                file);

            var compilerService = ExtensionNoPeriodFacts.GetCompilerService(
                absoluteFilePath.ExtensionNoPeriod,
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
                absoluteFilePath.ExtensionNoPeriod);

            var textEditorModel = new TextEditorModel(
                resourceUri,
                fileLastWriteTime,
                absoluteFilePath.ExtensionNoPeriod,
                content,
                compilerService,
                decorationMapper,
                null,
                new(),
                TextEditorModelKey.NewTextEditorModelKey()
            );

            textEditorModel.CompilerService.RegisterModel(textEditorModel);

            TextEditorService.Model.RegisterCustom(textEditorModel);

            await textEditorModel.ApplySyntaxHighlightingAsync();
        }
    }
}