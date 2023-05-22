using Luthetus.TextEditor.RazorLib.Model;
using Fluxor;

namespace Luthetus.Website.RazorLib.Store.WellKnownModelKindCase;

[FeatureState]
public partial class WellKnownModelKindState
{
    private WellKnownModelKindState()
    {
        WellKnownModelKind = WellKnownModelKind.CSharp;
    }
    
    private WellKnownModelKindState(WellKnownModelKind wellKnownModelKind)
    {
        WellKnownModelKind = wellKnownModelKind;
    }
    
    public WellKnownModelKind WellKnownModelKind { get; }
}