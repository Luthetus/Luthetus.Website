namespace Luthetus.Website.RazorLib.Store.ReplCase.Facts;

public partial class ReplStateFacts
{
    public static readonly string COUNTER_TEST_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/Components/CounterTest.razor";

    public static readonly string COUNTER_TEST_CONTENTS = @"<button class=""bwa_counter""
     @onclick=""IncrementCountOnClick"">

	Count: @_count
</button>

@code {
	private int _count;

	private void IncrementCountOnClick()
	{
		_count++;
	}
	
	public class MyClass
	{
		
	}
}";
}