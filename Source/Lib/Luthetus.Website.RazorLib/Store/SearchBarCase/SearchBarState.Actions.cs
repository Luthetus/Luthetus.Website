using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Website.RazorLib.Store.SearchBarCase;

public partial class SearchBarState
{
    public record SetSearchQueryAction(string SearchQuery);
}
