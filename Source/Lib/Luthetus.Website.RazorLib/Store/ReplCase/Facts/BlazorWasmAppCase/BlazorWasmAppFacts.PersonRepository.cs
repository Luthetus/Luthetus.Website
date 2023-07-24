namespace Luthetus.Website.RazorLib.Store.ReplCase.Facts.BlazorWasmAppCase;

public partial class BlazorWasmAppFacts
{
    public static readonly string PERSON_REPOSITORY_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/PersonCase/PersonRepository.cs";

    public static readonly string PERSON_REPOSITORY_CONTENTS = @"namespace BlazorWasmApp.PersonCase;

public class PersonRepository : IPersonRepository
{
	private List<IPersonModel> _people = new();

	public void AddPerson(IPersonModel personModel)
	{
		_people.Add(personModel);
	}
}";
}