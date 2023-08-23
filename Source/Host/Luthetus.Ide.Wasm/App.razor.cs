using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Lexing;
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
using Luthetus.CompilerServices.Lang.DotNetSolution;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Website.RazorLib.Store.ReplCase.Facts.ConsoleAppCase;
using Luthetus.Website.RazorLib.Store.ReplCase.Facts.BlazorWasmAppCase;
using Luthetus.Website.RazorLib.Store.ReplCase.Facts;
using Luthetus.Website.RazorLib.Store.ReplCase;
using Luthetus.Website.RazorLib.Facts;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;
using Luthetus.Ide.ClassLib.Store.EditorCase;

namespace Luthetus.Ide.Wasm;

public partial class App : ComponentBase
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
    private ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ICommonBackgroundTaskQueue CommonBackgroundTaskQueue { get; set; } = null!;
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
                        BlazorWasmAppFacts.PROGRAM_CS_FILE_ABSOLUTE_FILE_PATH,
                        false,
                        EnvironmentProvider);

                    Dispatcher.Dispatch(new EditorState.OpenInEditorAction(
                        absoluteFilePath,
                        false));
                },
                "Parsing Solution",
                string.Empty,
                true,
                _ => Task.CompletedTask,
                Dispatcher,
                CancellationToken.None);

            CommonBackgroundTaskQueue.QueueBackgroundWorkItem(backgroundTask);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task WriteFileSystemInMemoryAsync()
    {
        // BlazorWasmAppFacts
        {
            // AppCss
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.APP_CSS_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.APP_CSS_CONTENTS);

            // AppJs
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.APP_JS_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.APP_JS_CONTENTS);

            // AppRazor
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.APP_RAZOR_FILE_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.APP_RAZOR_FILE_CONTENTS);

            // AppTs
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.APP_TS_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.APP_TS_CONTENTS);

            // CounterTest
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.COUNTER_TEST_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.COUNTER_TEST_CONTENTS);

            // Csproj
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.BLAZOR_WASM_APP_C_SHARP_PROJECT_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.BLAZOR_WASM_APP_C_SHARP_PROJECT_CONTENTS);

            // Imports
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.IMPORTS_RAZOR_FILE_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.IMPORTS_RAZOR_FILE_CONTENTS);

            // IndexHtml
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.INDEX_HTML_FILE_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.INDEX_HTML_FILE_CONTENTS);

            // IndexRazor
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.INDEX_RAZOR_FILE_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.INDEX_RAZOR_FILE_CONTENTS);

            // IPersonModel
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.IPERSON_MODEL_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.IPERSON_MODEL_CONTENTS);

            // IPersonRepository
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.IPERSON_REPOSITORY_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.IPERSON_REPOSITORY_CONTENTS);

            // LaunchSettingsJson
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.LAUNCH_SETTINGS_JSON_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.LAUNCH_SETTINGS_JSON_CONTENTS);

            // MainLayout
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.MAIN_LAYOUT_RAZOR_FILE_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.MAIN_LAYOUT_RAZOR_FILE_CONTENTS);

            // PersonDisplayMarkup
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.PERSON_DISPLAY_MARKUP_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.PERSON_DISPLAY_MARKUP_CONTENTS);

            // PersonDisplayCodebehind
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.PERSON_DISPLAY_CODEBEHIND_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.PERSON_DISPLAY_CODEBEHIND_CONTENTS);

            // PersonModel
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.PERSON_MODEL_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.PERSON_MODEL_CONTENTS);

            // PersonRepository
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.PERSON_REPOSITORY_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.PERSON_REPOSITORY_CONTENTS);

            // Program
            await FileSystemProvider.File.WriteAllTextAsync(
                BlazorWasmAppFacts.PROGRAM_CS_FILE_ABSOLUTE_FILE_PATH,
                BlazorWasmAppFacts.PROGRAM_CS_FILE_CONTENTS);
        }

        // ConsoleAppFacts
        {
            // Csproj
            await FileSystemProvider.File.WriteAllTextAsync(
                ConsoleAppFacts.CONSOLE_APP_C_SHARP_PROJECT_ABSOLUTE_FILE_PATH,
                ConsoleAppFacts.CONSOLE_APP_C_SHARP_PROJECT_CONTENTS);

            // Program
            await FileSystemProvider.File.WriteAllTextAsync(
                ConsoleAppFacts.PROGRAM_CS_FILE_ABSOLUTE_FILE_PATH,
                ConsoleAppFacts.PROGRAM_CS_FILE_CONTENTS);
        }

        // Sln
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.SLN_ABSOLUTE_FILE_PATH,
            ReplStateFacts.SLN_CONTENTS);
    }

    private void InitializeDotNetSolutionAndExplorer()
    {
        var solutionAbsoluteFilePath = new AbsoluteFilePath(
            ReplStateFacts.SLN_ABSOLUTE_FILE_PATH,
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