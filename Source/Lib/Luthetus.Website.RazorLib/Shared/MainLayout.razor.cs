using Luthetus.Website.RazorLib.Settings;
using Luthetus.Website.RazorLib.Store.ReplCase.Facts;
using Luthetus.Website.RazorLib.Store.ReplCase;
using Luthetus.Website.RazorLib.Facts;

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
    private TextEditorXmlCompilerService XmlCompilerService { get; set; } = null!;
    [Inject]
    private CSharpCompilerService CSharpCompilerService { get; set; } = null!;
    [Inject]
    private RazorCompilerService RazorCompilerService { get; set; } = null!;
    [Inject]
    private TextEditorCssCompilerService CssCompilerService { get; set; } = null!;
    [Inject]
    private TextEditorJavaScriptCompilerService JavaScriptCompilerService { get; set; } = null!;
    [Inject]
    private TextEditorTypeScriptCompilerService TypeScriptCompilerService { get; set; } = null!;
    [Inject]
    private TextEditorJsonCompilerService JsonCompilerService { get; set; } = null!;
    [Inject]
    private IState<SemanticContextState> SemanticContextStateWrap { get; set; } = null!;

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
        // AppCss
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.APP_CSS_ABSOLUTE_FILE_PATH,
            ReplStateFacts.APP_CSS_CONTENTS);
        
        // AppJs
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.APP_JS_ABSOLUTE_FILE_PATH,
            ReplStateFacts.APP_JS_CONTENTS);

        // AppRazor
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.APP_RAZOR_FILE_ABSOLUTE_FILE_PATH,
            ReplStateFacts.APP_RAZOR_FILE_CONTENTS);
        
        // AppTs
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.APP_TS_ABSOLUTE_FILE_PATH,
            ReplStateFacts.APP_TS_CONTENTS);
        
        // CounterTest
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.COUNTER_TEST_ABSOLUTE_FILE_PATH,
            ReplStateFacts.COUNTER_TEST_CONTENTS);

        // Csproj
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.C_SHARP_PROJECT_ABSOLUTE_FILE_PATH,
            ReplStateFacts.C_SHARP_PROJECT_CONTENTS);

        // Imports
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.IMPORTS_RAZOR_FILE_ABSOLUTE_FILE_PATH,
            ReplStateFacts.IMPORTS_RAZOR_FILE_CONTENTS);

        // IndexHtml
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.INDEX_HTML_FILE_ABSOLUTE_FILE_PATH,
            ReplStateFacts.INDEX_HTML_FILE_CONTENTS);

        // IndexRazor
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.INDEX_RAZOR_FILE_ABSOLUTE_FILE_PATH,
            ReplStateFacts.INDEX_RAZOR_FILE_CONTENTS);

        // IPersonModel
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.IPERSON_MODEL_ABSOLUTE_FILE_PATH,
            ReplStateFacts.IPERSON_MODEL_CONTENTS);

        // IPersonRepository
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.IPERSON_REPOSITORY_ABSOLUTE_FILE_PATH,
            ReplStateFacts.IPERSON_REPOSITORY_CONTENTS);
        
        // LaunchSettingsJson
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.LAUNCH_SETTINGS_JSON_ABSOLUTE_FILE_PATH,
            ReplStateFacts.LAUNCH_SETTINGS_JSON_CONTENTS);

        // MainLayout
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.MAIN_LAYOUT_RAZOR_FILE_ABSOLUTE_FILE_PATH,
            ReplStateFacts.MAIN_LAYOUT_RAZOR_FILE_CONTENTS);

        // PersonDisplayMarkup
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.PERSON_DISPLAY_MARKUP_ABSOLUTE_FILE_PATH,
            ReplStateFacts.PERSON_DISPLAY_MARKUP_CONTENTS);

        // PersonDisplayCodebehind
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.PERSON_DISPLAY_CODEBEHIND_ABSOLUTE_FILE_PATH,
            ReplStateFacts.PERSON_DISPLAY_CODEBEHIND_CONTENTS);

        // PersonModel
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.PERSON_MODEL_ABSOLUTE_FILE_PATH,
            ReplStateFacts.PERSON_MODEL_CONTENTS);

        // PersonRepository
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.PERSON_REPOSITORY_ABSOLUTE_FILE_PATH,
            ReplStateFacts.PERSON_REPOSITORY_CONTENTS);

        // Program
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.PROGRAM_CS_FILE_ABSOLUTE_FILE_PATH,
            ReplStateFacts.PROGRAM_CS_FILE_CONTENTS);

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

        var dotNetSolutionSemanticContext = new DotNetSolutionSemanticContext(
            DotNetSolutionKey.NewSolutionKey(),
            dotNetSolution);

        Dispatcher.Dispatch(
            new SemanticContextState.SetDotNetSolutionSemanticContextAction(
                dotNetSolutionSemanticContext));

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