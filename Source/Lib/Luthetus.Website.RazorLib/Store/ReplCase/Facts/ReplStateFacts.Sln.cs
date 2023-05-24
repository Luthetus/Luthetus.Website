using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Resize;
using Luthetus.Ide.ClassLib.DotNet;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Repl.FileSystem;
using System.Collections.Immutable;

namespace Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

public partial class ReplStateFacts
{
    public static readonly string SLN_ABSOLUTE_FILE_PATH = @"/Repl.sln";

    public const string SLN_CONTENTS = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.5.33627.172
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""BlazorWasmApp"", ""BlazorWasmApp\BlazorWasmApp.csproj"", ""{A41C752D-A976-4337-8865-7EA232E0AE7E}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{A41C752D-A976-4337-8865-7EA232E0AE7E}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{A41C752D-A976-4337-8865-7EA232E0AE7E}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{A41C752D-A976-4337-8865-7EA232E0AE7E}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{A41C752D-A976-4337-8865-7EA232E0AE7E}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {F37C2ECB-ABC6-40C3-B9B7-96D353EA0C26}
	EndGlobalSection
EndGlobal
";
}
