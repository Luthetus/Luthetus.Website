using Luthetus.Website.RazorLib.Settings;
using Luthetus.Website.RazorLib.Store.ReplCase.Facts;
using Luthetus.Website.RazorLib.Store.ReplCase;
using Luthetus.Website.RazorLib.Facts;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options;
using Luthetus.Common.RazorLib.Dialog;
using Fluxor;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.CompilerServices.Lang.Xml;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Css;
using Luthetus.CompilerServices.Lang.JavaScript;
using Luthetus.CompilerServices.Lang.TypeScript;
using Luthetus.CompilerServices.Lang.Json;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.CompilerServices.Lang.DotNetSolution;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.Website.RazorLib.Store.ReplCase.Facts.BlazorWasmAppCase;
using Luthetus.Website.RazorLib.Store.ReplCase.Facts.ConsoleAppCase;

namespace Luthetus.Website.RazorLib.Shared;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
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
    private JavaScriptCompilerService JavaScriptCompilerService { get; set; } = null!;
    [Inject]
    private TypeScriptCompilerService TypeScriptCompilerService { get; set; } = null!;
    [Inject]
    private JsonCompilerService JsonCompilerService { get; set; } = null!;

    protected override void OnInitialized()
    {
        AppOptionsService.AppOptionsStateWrap.StateChanged += AppOptionsStateWrapOnStateChanged;

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await WriteFileSystemInMemoryAsync();

            await InitializeDotNetSolutionAndExplorerAsync();

            await ParseSolutionAsync();
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

    private async Task InitializeDotNetSolutionAndExplorerAsync()
    {
        var dotNetSolutionAbsoluteFilePath = new AbsoluteFilePath(
                ReplStateFacts.SLN_ABSOLUTE_FILE_PATH,
                false,
                EnvironmentProvider);

        var dotNetSolutionNamespacePath = new NamespacePath(
            string.Empty,
            dotNetSolutionAbsoluteFilePath);

        var dotNetSolution = DotNetSolutionParser.Parse(
            ReplStateFacts.SLN_CONTENTS,
            dotNetSolutionNamespacePath,
            EnvironmentProvider);

        Dispatcher.Dispatch(
            new ReplState.NextInstanceAction(inReplState =>
                new ReplState(
                    inReplState.RootDirectory,
                    dotNetSolution,
                    inReplState.Files,
                    inReplState.ViewExplorerElementDimensions,
                    inReplState.TextEditorGroupElementDimensions)));

        if (!TreeViewService.TryGetTreeViewState(
                    ReplFacts.TreeViewStateKeys.SolutionExplorer,
                    out _))
        {
            var rootTreeViewNode = new TreeViewSolution(
                dotNetSolution,
                LuthetusIdeComponentRenderers,
                FileSystemProvider,
                EnvironmentProvider,
                true,
                true);

            await rootTreeViewNode.LoadChildrenAsync();

            var treeViewState = new TreeViewState(
                ReplFacts.TreeViewStateKeys.SolutionExplorer,
                rootTreeViewNode,
                rootTreeViewNode,
                ImmutableList<TreeViewNoType>.Empty);

            TreeViewService.RegisterTreeViewState(treeViewState);
        }
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

    private async void AppOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private void OpenSettingsDialogOnClick()
    {
        DialogService.RegisterDialogRecord(new DialogRecord(
            SettingsDisplay.SettingsDialogKey,
            "Settings",
            typeof(SettingsDisplay),
            null,
            null)
        {
            IsResizable = true
        });
    }

    public void Dispose()
    {
        AppOptionsService.AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}