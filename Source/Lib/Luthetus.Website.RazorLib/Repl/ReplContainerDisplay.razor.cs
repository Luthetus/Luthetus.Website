using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Resize;
using Luthetus.TextEditor.RazorLib.Group;
using Luthetus.TextEditor.RazorLib;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;

namespace Luthetus.Website.RazorLib.Repl;

public partial class ReplContainerDisplay : FluxorComponent
{
    [Inject]
    private IState<ReplState> ReplStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorGroupKey ReplTextEditorGroupKey = TextEditorGroupKey.NewTextEditorGroupKey();
    private static readonly TreeViewStateKey ReplTreeViewStateKey = TreeViewStateKey.NewTreeViewStateKey();

    private ElementDimensions _folderExplorerElementDimensions = new();
    private ElementDimensions _textEditorGroupElementDimensions = new();

    protected override void OnInitialized()
    {
        TextEditorService.Group.Register(ReplTextEditorGroupKey);

        // Initialize _folderExplorerElementDimensions
        {
            var folderExplorerWidth = _folderExplorerElementDimensions.DimensionAttributes
                .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

            folderExplorerWidth.DimensionUnits.AddRange(new[]
            {
                new DimensionUnit
                {
                    Value = 50,
                    DimensionUnitKind = DimensionUnitKind.Percentage
                },
                new DimensionUnit
                {
                    Value = ResizableRow.RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
                    DimensionUnitKind = DimensionUnitKind.Pixels,
                    DimensionOperatorKind = DimensionOperatorKind.Subtract
                }
            });
        }

        // Initialize _textEditorGroupElementDimensions
        {
            var textEditorGroupWidth = _textEditorGroupElementDimensions.DimensionAttributes
                .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

            textEditorGroupWidth.DimensionUnits.AddRange(new[]
            {
                new DimensionUnit
                {
                    Value = 50,
                    DimensionUnitKind = DimensionUnitKind.Percentage
                },
                new DimensionUnit
                {
                    Value = ResizableRow.RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
                    DimensionUnitKind = DimensionUnitKind.Pixels,
                    DimensionOperatorKind = DimensionOperatorKind.Subtract
                }
            });
        }


        base.OnInitialized();
    }
}