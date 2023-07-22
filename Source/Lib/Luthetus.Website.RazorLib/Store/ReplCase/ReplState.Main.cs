using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Resize;
using Luthetus.CompilerServices.Lang.DotNet;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Repl.FileSystem;
using System.Collections.Immutable;

namespace Luthetus.Website.RazorLib.Store.ReplCase;

[FeatureState]
public partial class ReplState
{
    private ReplState()
    {
        Files = new ReplFile[]
        {
        new ReplFile(
            string.Empty,
            "/",
            DateTime.UtcNow)
        }
        .ToImmutableList();

        // Initialize ViewExplorerElementDimensions
        {
            ViewExplorerElementDimensions = new();

            var folderExplorerWidth = ViewExplorerElementDimensions.DimensionAttributes
                .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

            folderExplorerWidth.DimensionUnits.AddRange(new[]
            {
            new DimensionUnit
            {
                Value = 35,
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

        // Initialize TextEditorGroupElementDimensions
        {
            TextEditorGroupElementDimensions = new();

            var textEditorGroupWidth = TextEditorGroupElementDimensions.DimensionAttributes
                .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

            textEditorGroupWidth.DimensionUnits.AddRange(new[]
            {
            new DimensionUnit
            {
                Value = 65,
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
    }

    public ReplState(
        IAbsoluteFilePath rootDirectory,
        DotNetSolution dotNetSolution,
        ImmutableList<ReplFile> files,
        ElementDimensions viewExplorerElementDimensions,
        ElementDimensions textEditorGroupElementDimensions)
    {
        RootDirectory = rootDirectory;
        DotNetSolution = dotNetSolution;
        Files = files;
        ViewExplorerElementDimensions = viewExplorerElementDimensions;
        TextEditorGroupElementDimensions = textEditorGroupElementDimensions;
    }

    public IAbsoluteFilePath? RootDirectory { get; }
    public DotNetSolution? DotNetSolution { get; }
    public ImmutableList<ReplFile> Files { get; }
    public ElementDimensions ViewExplorerElementDimensions { get; }
    public ElementDimensions TextEditorGroupElementDimensions { get; }
}