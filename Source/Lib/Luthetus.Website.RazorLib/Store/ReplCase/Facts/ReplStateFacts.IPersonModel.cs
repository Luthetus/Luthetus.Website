namespace Luthetus.Website.RazorLib.Store.ReplCase.Facts;

public partial class ReplStateFacts
{
    public static readonly string IPERSON_MODEL_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/PersonCase/IPersonModel.cs";

    public static readonly string IPERSON_MODEL_CONTENTS = @"namespace BlazorWasmApp.PersonCase;

public interface IPersonModel
{
    public string FirstName { get; set; }
	public string LastName { get; set; }

	public string DisplayName { get; }
}
";
}

