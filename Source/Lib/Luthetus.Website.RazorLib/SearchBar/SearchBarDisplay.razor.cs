using Fluxor;
using Luthetus.Common.RazorLib.Dropdown;
using Luthetus.Website.RazorLib.Store.SearchBarCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.SearchBar;

public partial class SearchBarDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<SearchBarState> SearchBarStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IDropdownService DropdownService { get; set; } = null!;

    private readonly DropdownKey SearchBarDropdownKey = DropdownKey.NewDropdownKey();

    private string SearchQuery 
    {
        get => SearchBarStateWrap.Value.SearchQuery;
        set
        {
            value ??= string.Empty;

            Dispatcher.Dispatch(
                new SearchBarState.SetSearchQueryAction(
                    value));
        }
    }

    private void HandleInputOnFocusIn()
    {
        DropdownService.AddActiveDropdownKey(SearchBarDropdownKey);
    }

    private void HandleInputOnFocusOut()
    {
        // TODO: Uncomment "RemoveActiveDropdownKey". Temporaily commenting it out so I can work on the CSS
        // DropdownService.RemoveActiveDropdownKey(SearchBarDropdownKey);
    }

    public void Dispose()
    {
        DropdownService.RemoveActiveDropdownKey(SearchBarDropdownKey);
    }
}
