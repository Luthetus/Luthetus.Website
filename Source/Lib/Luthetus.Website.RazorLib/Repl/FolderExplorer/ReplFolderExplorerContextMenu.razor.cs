using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Dropdown;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Website.RazorLib.Repl.FolderExplorer;

public partial class ReplFolderExplorerContextMenu : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IMenuOptionsFactory MenuOptionsFactory { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITreeViewCommandParameter TreeViewCommandParameter { get; set; } = null!;

    public static readonly DropdownKey ContextMenuEventDropdownKey = DropdownKey.NewDropdownKey();

    private MenuRecord GetMenuRecord(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.TargetNode is null)
            return MenuRecord.Empty;

        var menuOptionRecords = new List<MenuOptionRecord>();

        var treeViewModel = treeViewCommandParameter.TargetNode;
        var parentTreeViewModel = treeViewModel.Parent;

        var parentTreeViewAbsoluteFilePath = parentTreeViewModel as TreeViewAbsoluteFilePath;

        if (treeViewModel is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)
        {
            return MenuRecord.Empty;
        }

        if (treeViewAbsoluteFilePath.Item.IsDirectory)
        {
            menuOptionRecords.AddRange(
                GetFileMenuOptions(treeViewAbsoluteFilePath, parentTreeViewAbsoluteFilePath)
                    .Union(GetDirectoryMenuOptions(treeViewAbsoluteFilePath)));
        }

        if (menuOptionRecords.Any())
        {
            return new MenuRecord(menuOptionRecords.ToImmutableArray());
        }
        else
        {
            return MenuRecord.Empty;
        }
    }

    private MenuOptionRecord[] GetDirectoryMenuOptions(TreeViewAbsoluteFilePath treeViewModel)
    {
        return new[]
        {
        MenuOptionsFactory.NewEmptyFile(
            treeViewModel.Item,
            async () => await ReloadTreeViewModel(treeViewModel)),
        MenuOptionsFactory.NewDirectory(
            treeViewModel.Item,
            async () => await ReloadTreeViewModel(treeViewModel)),
    };
    }

    private MenuOptionRecord[] GetFileMenuOptions(
        TreeViewAbsoluteFilePath treeViewModel,
        TreeViewAbsoluteFilePath? parentTreeViewModel)
    {
        return new[]
        {
        MenuOptionsFactory.DeleteFile(
            treeViewModel.Item,
            async () =>
            {
                await ReloadTreeViewModel(parentTreeViewModel);
            }),
        MenuOptionsFactory.RenameFile(
            treeViewModel.Item,
            Dispatcher,
            async ()  =>
            {
                await ReloadTreeViewModel(parentTreeViewModel);
            }),
    };
    }

    /// <summary>
    /// This method I believe is causing bugs
    /// <br/><br/>
    /// For example, when removing a C# Project the
    /// solution is reloaded and a new root is made.
    /// <br/><br/>
    /// Then there is a timing issue where the new root is made and set
    /// as the root. But this method erroneously reloads the old root.
    /// </summary>
    /// <param name="treeViewModel"></param>
    private async Task ReloadTreeViewModel(
        TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildrenAsync();

        TreeViewService.ReRenderNode(
            Facts.ReplFacts.TreeViewStateKeys.FolderExplorer,
            treeViewModel);

        TreeViewService.MoveUp(
            Facts.ReplFacts.TreeViewStateKeys.FolderExplorer,
            false);
    }

    public static string GetContextMenuCssStyleString(ITreeViewCommandParameter? treeViewCommandParameter)
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