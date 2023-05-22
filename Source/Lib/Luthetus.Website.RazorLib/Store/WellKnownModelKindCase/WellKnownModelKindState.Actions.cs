using Luthetus.TextEditor.RazorLib.Model;

namespace Luthetus.Website.RazorLib.Store.WellKnownModelKindCase;

public partial class WellKnownModelKindState
{
    public record SetWellKnownModelKindAction(WellKnownModelKind WellKnownModelKind);
}