using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.TextEditor.RazorLib.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Website.RazorLib.Facts;

public static partial class ReplFacts
{
    public static class TreeViewStateKeys
    {
        public static readonly TreeViewStateKey FolderExplorer = TreeViewStateKey.NewTreeViewStateKey();
        public static readonly TreeViewStateKey SolutionExplorer = TreeViewStateKey.NewTreeViewStateKey();
        public static readonly TreeViewStateKey SemanticExplorer = TreeViewStateKey.NewTreeViewStateKey();
    }
}