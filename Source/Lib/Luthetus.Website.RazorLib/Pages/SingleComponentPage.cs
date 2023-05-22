using Fluxor.Blazor.Web.Components;

namespace Luthetus.Website.RazorLib.Pages;

public class SingleComponentPage : FluxorComponent
{
    protected const string LINE_HEIGHT_OF_H3 = "1.2";
    protected const string HEIGHT_OF_H3 = $"calc(calc(1.3rem + .6vw) * {LINE_HEIGHT_OF_H3})";
    protected const string HEIGHT_OF_HR = "1px";
    protected const string TOTAL_VERTICAL_MARGIN_OF_HR = "2rem";

    protected string CssStyleTextEditorViewModelDisplay =>
        $"height: calc(100% - {HEIGHT_OF_H3} - {HEIGHT_OF_HR} - {TOTAL_VERTICAL_MARGIN_OF_HR});";
}