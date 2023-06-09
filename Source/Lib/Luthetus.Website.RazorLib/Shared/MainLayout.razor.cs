﻿using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.Options;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Implementations;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Keys;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.DotNet;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.Store.SemanticContextCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Website.RazorLib.Settings;
using Luthetus.Website.RazorLib.Store.ReplCase.Facts;
using Luthetus.Website.RazorLib.Store.ReplCase;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Fluxor;
using Luthetus.Website.RazorLib.Facts;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib;

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

        // AppRazor
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplStateFacts.APP_RAZOR_FILE_ABSOLUTE_FILE_PATH,
            ReplStateFacts.APP_RAZOR_FILE_CONTENTS);

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

            var lexer = ExtensionNoPeriodFacts.GetLexer(
                resourceUri,
                absoluteFilePath.ExtensionNoPeriod);

            var decorationMapper = ExtensionNoPeriodFacts.GetDecorationMapper(
                absoluteFilePath.ExtensionNoPeriod);

            var semanticModel = ExtensionNoPeriodFacts.GetSemanticModel(
                absoluteFilePath.ExtensionNoPeriod,
                EditorState.SharedBinder);

            SemanticContextStateWrap.Value.DotNetSolutionSemanticContext.SemanticModelMap
                .Add(
                    resourceUri,
                    semanticModel);

            var textEditorModel = new TextEditorModel(
                resourceUri,
                fileLastWriteTime,
                absoluteFilePath.ExtensionNoPeriod,
                content,
                lexer,
                decorationMapper,
                semanticModel,
                null,
                new(),
                TextEditorModelKey.NewTextEditorModelKey()
            );

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