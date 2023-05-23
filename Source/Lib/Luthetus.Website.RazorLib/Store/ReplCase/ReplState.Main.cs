using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Resize;
using Luthetus.Ide.ClassLib.DotNet;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Repl.FileSystem;
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

    public static readonly string INITAL_DOT_NET_SOLUTION_ABSOLUTE_FILE_PATH = @"/DotNetSolution.sln";

    public const string INITIAL_DOT_NET_SOLUTION_CONTENTS = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""BlazorCrudApp.ServerSide"", ""BlazorCrudApp.ServerSide\BlazorCrudApp.ServerSide.csproj"", ""{189E0052-7139-4E31-8728-EFAA6A7F10E3}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{189E0052-7139-4E31-8728-EFAA6A7F10E3}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{189E0052-7139-4E31-8728-EFAA6A7F10E3}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{189E0052-7139-4E31-8728-EFAA6A7F10E3}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{189E0052-7139-4E31-8728-EFAA6A7F10E3}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
EndGlobal
";
}
