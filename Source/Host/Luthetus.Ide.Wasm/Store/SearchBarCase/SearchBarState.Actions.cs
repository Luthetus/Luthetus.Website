namespace Luthetus.Website.RazorLib.Store.SearchBarCase;

public partial class SearchBarState
{
    public record SetSearchQueryAction(string SearchQuery);
}