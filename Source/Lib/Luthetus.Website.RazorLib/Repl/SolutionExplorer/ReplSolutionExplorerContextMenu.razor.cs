using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Dropdown;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Website.RazorLib.Facts;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Website.RazorLib.Repl.SolutionExplorer;

public partial class ReplSolutionExplorerContextMenu : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private Luthetus.Ide.ClassLib.Menu.ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;
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

        var menuRecords = new List<MenuOptionRecord>();

        var treeViewModel = treeViewCommandParameter.TargetNode;
        var parentTreeViewModel = treeViewModel.Parent;

        var parentTreeViewNamespacePath = parentTreeViewModel as TreeViewNamespacePath;

        if (treeViewModel is TreeViewNamespacePath treeViewNamespacePath)
        {
            if (treeViewNamespacePath.Item.AbsoluteFilePath.IsDirectory)
            {
                menuRecords.AddRange(
                    GetFileMenuOptions(treeViewNamespacePath, parentTreeViewNamespacePath)
                        .Union(GetDirectoryMenuOptions(treeViewNamespacePath)));
            }
            else
            {
                switch (treeViewNamespacePath.Item.AbsoluteFilePath.ExtensionNoPeriod)
                {
                    case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                        menuRecords.AddRange(
                            GetCSharpProjectMenuOptions(treeViewNamespacePath));
                        break;
                    default:
                        menuRecords.AddRange(
                            GetFileMenuOptions(treeViewNamespacePath, parentTreeViewNamespacePath));
                        break;
                }
            }
        }
        else if (treeViewModel is TreeViewSolution treeViewSolution)
        {
            if (treeViewSolution.Item.NamespacePath.AbsoluteFilePath.ExtensionNoPeriod ==
                ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
            {
                if (treeViewSolution.Parent is null ||
                    treeViewSolution.Parent is TreeViewAdhoc)
                {
                    menuRecords.AddRange(
                        GetDotNetSolutionMenuOptions(treeViewSolution));
                }
            }
        }
        else if (treeViewModel is TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference)
        {
            menuRecords.AddRange(
                GetCSharpProjectToProjectReferenceMenuOptions(
                    treeViewCSharpProjectToProjectReference));
        }

        if (!menuRecords.Any())
            return MenuRecord.Empty;

        return new MenuRecord(
            menuRecords.ToImmutableArray());
    }

    private MenuOptionRecord[] GetDotNetSolutionMenuOptions(
        TreeViewSolution treeViewSolution)
    {
        if (treeViewSolution.Item is null)
            return Array.Empty<MenuOptionRecord>();

        return Array.Empty<MenuOptionRecord>();
    }

    private MenuOptionRecord[] GetCSharpProjectMenuOptions(
        TreeViewNamespacePath treeViewModel)
    {
        var parentDirectory = (IAbsoluteFilePath)treeViewModel.Item.AbsoluteFilePath.Directories.Last();

        var treeViewSolution = treeViewModel.Parent as TreeViewSolution;

        if (treeViewSolution is null)
        {
            var ancestorTreeView = treeViewModel.Parent;

            if (ancestorTreeView.Parent is null)
                return Array.Empty<MenuOptionRecord>();

            // Parent could be a could be one or many levels of solution folders
            while (ancestorTreeView.Parent is not null)
                ancestorTreeView = ancestorTreeView.Parent;

            treeViewSolution = ancestorTreeView as TreeViewSolution;

            if (treeViewSolution is null)
                return Array.Empty<MenuOptionRecord>();
        }

        return new[]
        {
        CommonMenuOptionsFactory.NewEmptyFile(
            parentDirectory,
            async () => await ReloadTreeViewModel(treeViewModel)),
        CommonMenuOptionsFactory.NewTemplatedFile(
            new NamespacePath(treeViewModel.Item.Namespace, parentDirectory),
            async () => await ReloadTreeViewModel(treeViewModel)),
        CommonMenuOptionsFactory.NewDirectory(
            parentDirectory,
            async () => await ReloadTreeViewModel(treeViewModel)),
    };
    }

    private MenuOptionRecord[] GetCSharpProjectToProjectReferenceMenuOptions(
        TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference)
    {
        return Array.Empty<MenuOptionRecord>();
    }

    private MenuOptionRecord[] GetDirectoryMenuOptions(
        TreeViewNamespacePath treeViewModel)
    {
        return new[]
        {
        CommonMenuOptionsFactory.NewEmptyFile(
            treeViewModel.Item.AbsoluteFilePath,
            async () => await ReloadTreeViewModel(treeViewModel)),
        CommonMenuOptionsFactory.NewTemplatedFile(
            treeViewModel.Item,
            async () => await ReloadTreeViewModel(treeViewModel)),
        CommonMenuOptionsFactory.NewDirectory(
            treeViewModel.Item.AbsoluteFilePath,
            async () => await ReloadTreeViewModel(treeViewModel)),
    };
    }

    private MenuOptionRecord[] GetFileMenuOptions(
        TreeViewNamespacePath treeViewModel,
        TreeViewNamespacePath? parentTreeViewModel)
    {
        return new[]
        {
        CommonMenuOptionsFactory.DeleteFile(
            treeViewModel.Item.AbsoluteFilePath,
            async () =>
            {
                await ReloadTreeViewModel(parentTreeViewModel);
            }),
        CommonMenuOptionsFactory.RenameFile(
            treeViewModel.Item.AbsoluteFilePath,
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
            ReplFacts.TreeViewStateKeys.SolutionExplorer,
            treeViewModel);

        TreeViewService.MoveUp(
            ReplFacts.TreeViewStateKeys.SolutionExplorer,
            false);
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