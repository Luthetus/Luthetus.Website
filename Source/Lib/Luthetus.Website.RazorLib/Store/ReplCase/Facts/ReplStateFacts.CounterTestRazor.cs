namespace Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

public partial class ReplStateFacts
{
    public static readonly string COUNTER_TEST_RAZOR_FILE_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/Components/CounterTest.razor";
    
    public static readonly string COUNTER_TEST_RAZOR_FILE_CONTENTS = @"<div class=""bwa_counter""
     @onclick=""IncrementCountOnClick"">

	Count: @_count
</div>

@code {
	private int _count;

	private void IncrementCountOnClick()
	{
		_count++;
	}
}";
}
