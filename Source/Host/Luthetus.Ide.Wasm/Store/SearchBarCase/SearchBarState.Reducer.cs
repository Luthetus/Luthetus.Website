using Fluxor;

namespace Luthetus.Website.RazorLib.Store.SearchBarCase;

public partial class SearchBarState
{
    private class Reducer
    {
        [ReducerMethod]
        public static SearchBarState ReduceSetSearchQueryAction(
            SearchBarState inSearchBarState,
            SetSearchQueryAction setSearchQueryAction)
        {
            return new SearchBarState(setSearchQueryAction.SearchQuery);
        }
    }
}