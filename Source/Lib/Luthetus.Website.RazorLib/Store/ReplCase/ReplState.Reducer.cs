using Fluxor;

namespace Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

public partial class ReplState
{
    private class Reducer
    {
        [ReducerMethod]
        public static ReplState ReduceNextInstanceAction(
            ReplState inReplFileSystemState,
            NextInstanceAction nextInstanceAction)
        {
            return nextInstanceAction.ConstructNextReplFileSystemStateFunc
                .Invoke(inReplFileSystemState);
        }
    }
}
