namespace Luthetus.Website.RazorLib.Store.SearchBarCase;

[FeatureState]
public partial class SearchBarState
{
    private SearchBarState()
    {
        SearchQuery = string.Empty;
    }

    public SearchBarState(string searchQuery)
    {
        SearchQuery = searchQuery;
    }

    public string SearchQuery { get; }
}