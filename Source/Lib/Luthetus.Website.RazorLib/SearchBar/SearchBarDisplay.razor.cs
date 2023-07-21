using Luthetus.Website.RazorLib.Store.SearchBarCase;

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
        DropdownService.RemoveActiveDropdownKey(SearchBarDropdownKey);
    }

    public void Dispose()
    {
        DropdownService.RemoveActiveDropdownKey(SearchBarDropdownKey);
    }
}