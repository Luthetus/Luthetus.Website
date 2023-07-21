namespace Luthetus.Website.RazorLib.Store.ReplCase.Facts;

public partial class ReplStateFacts
{
    public static readonly string PERSON_DISPLAY_CODEBEHIND_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/PersonCase/PersonDisplay.razor.cs";

    public static readonly string PERSON_DISPLAY_CODEBEHIND_CONTENTS = @"using Microsoft.AspNetCore.Components;

namespace BlazorWasmApp.Components;

public partial class PersonDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public IPersonModel PersonModel { get; set; }
}
";
}