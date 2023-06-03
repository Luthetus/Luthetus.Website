using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Dropdown;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Fluxor;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;

namespace Luthetus.Website.RazorLib.Repl.SemanticExplorer;

public partial class ReplSemanticExplorerContextMenu : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private Luthetus.Ide.ClassLib.Menu.ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

    [CascadingParameter(Name="ReplSemanticExplorerTreeViewStateKey"), EditorRequired]
    public TreeViewStateKey ReplSemanticExplorerTreeViewStateKey { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITreeViewCommandParameter TreeViewCommandParameter { get; set; } = null!;

    public static readonly DropdownKey ContextMenuEventDropdownKey = DropdownKey.NewDropdownKey();

    private MenuRecord GetMenuRecord(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        return MenuRecord.Empty;
    }

    public static string GetContextMenuCssStyleString(
        ITreeViewCommandParameter? treeViewCommandParameter)
    {
        if (treeViewCommandParameter?.ContextMenuFixedPosition is null)
            return "display: none;";

        var left =
            $"left: {treeViewCommandParameter.ContextMenuFixedPosition.LeftPositionInPixels.ToCssValue()}px;";

        var top =
            $"top: {treeViewCommandParameter.ContextMenuFixedPosition.TopPositionInPixels.ToCssValue()}px;";

        return $"{left} {top}";
    }
}
