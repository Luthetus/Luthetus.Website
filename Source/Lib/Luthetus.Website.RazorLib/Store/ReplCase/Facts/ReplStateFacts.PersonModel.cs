using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Website.RazorLib.Store.ReplCase.Facts;

public partial class ReplStateFacts
{
    public static readonly string PERSON_MODEL_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/PersonCase/PersonModel.cs";

    public static readonly string PERSON_MODEL_CONTENTS = @"namespace BlazorWasmApp.PersonCase;

public class PersonModel
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

