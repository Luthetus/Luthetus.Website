using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Resize;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Repl;
using System.Collections.Immutable;

namespace Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

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

        // Initialize FolderExplorerElementDimensions
        {
            FolderExplorerElementDimensions = new();

            var folderExplorerWidth = FolderExplorerElementDimensions.DimensionAttributes
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

        // Initialize TextEditorGroupElementDimensions
        {
            TextEditorGroupElementDimensions = new();

            var textEditorGroupWidth = TextEditorGroupElementDimensions.DimensionAttributes
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
    }

    public ReplState(
        IAbsoluteFilePath rootDirectory,
        ImmutableList<ReplFile> files,
        ElementDimensions folderExplorerElementDimensions,
        ElementDimensions textEditorGroupElementDimensions)
    {
        RootDirectory = rootDirectory;
        Files = files;
        FolderExplorerElementDimensions = folderExplorerElementDimensions;
        TextEditorGroupElementDimensions = textEditorGroupElementDimensions;
    }

    public IAbsoluteFilePath? RootDirectory { get; }
    public ImmutableList<ReplFile> Files { get; }
    public ElementDimensions FolderExplorerElementDimensions { get; }
    public ElementDimensions TextEditorGroupElementDimensions { get; }
}
