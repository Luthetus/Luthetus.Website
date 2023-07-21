namespace Luthetus.Website.RazorLib.Store.ReplCase;

public partial class ReplState
{
    public record NextInstanceAction(Func<ReplState, ReplState> ConstructNextReplFileSystemStateFunc);
}