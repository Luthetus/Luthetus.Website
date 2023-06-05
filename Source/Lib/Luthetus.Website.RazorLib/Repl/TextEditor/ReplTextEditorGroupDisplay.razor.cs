using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Group;
using Luthetus.TextEditor.RazorLib.Semantics;
using Luthetus.Website.RazorLib.Facts;
using Luthetus.Website.RazorLib.Store.ReplCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.Repl.TextEditor;

public partial class ReplTextEditorGroupDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [CascadingParameter, EditorRequired]
    public ReplState ReplState { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public AppOptionsState AppOptionsState { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;

    private void HandleGotoDefinitionWithinDifferentFileAction(TextEditorSymbolDefinition textEditorSymbolDefinition)
    {
        var model = TextEditorService.Model.FindOrDefaultByResourceUri(
            textEditorSymbolDefinition.ResourceUri);

        if (model is null)
            return;

        var viewModels = TextEditorService.Model.GetViewModelsOrEmpty(model.ModelKey);

        if (!viewModels.Any())
            return;

        var viewModel = viewModels[0];

        var rowInformation = model.FindRowInformation(
            textEditorSymbolDefinition.PositionIndex);

        viewModel.PrimaryCursor.IndexCoordinates =
            (rowInformation.rowIndex,
                textEditorSymbolDefinition.PositionIndex - rowInformation.rowStartPositionIndex);

        Dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            new AbsoluteFilePath(
                textEditorSymbolDefinition.ResourceUri.Value,
                false,
                EnvironmentProvider),
            true,
            ReplFacts.TextEditorGroupKeys.GroupKey));
    }
}