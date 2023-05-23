namespace Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

public partial class ReplState
{
    public record NextInstanceAction(Func<ReplState, ReplState> ConstructNextReplFileSystemStateFunc);
}
