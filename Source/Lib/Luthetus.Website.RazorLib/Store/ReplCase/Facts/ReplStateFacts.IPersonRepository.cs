namespace Luthetus.Website.RazorLib.Store.ReplCase.Facts;

public partial class ReplStateFacts
{
    public static readonly string IPERSON_REPOSITORY_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/PersonCase/IPersonRepository.cs";

    public static readonly string IPERSON_REPOSITORY_CONTENTS = @"namespace BlazorWasmApp.PersonCase;

public interface IPersonRepository
{
	public void AddPerson(IPersonModel personModel);
}
";
}

