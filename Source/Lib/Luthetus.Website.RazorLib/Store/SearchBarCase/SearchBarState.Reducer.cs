using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
