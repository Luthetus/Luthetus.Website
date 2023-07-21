namespace Luthetus.Website.RazorLib.Store.ReplCase.Facts;

public partial class ReplStateFacts
{
    public static readonly string PERSON_MODEL_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/PersonCase/PersonModel.cs";

    public static readonly string PERSON_MODEL_CONTENTS = @"namespace BlazorWasmApp.PersonCase;

public class PersonModel : IPersonModel
{
	public PersonModel()
	{
	}

	public string FirstName { get; set; }
	public string LastName { get; set; }

	public string DisplayName => $""{FirstName} + {LastName}"";
}
";
}