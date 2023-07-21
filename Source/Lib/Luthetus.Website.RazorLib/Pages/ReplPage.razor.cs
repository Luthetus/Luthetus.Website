using Luthetus.Website.RazorLib.ViewCase;

namespace Luthetus.Website.RazorLib.Pages;

public partial class ReplPage : ComponentBase
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    /// <summary>TODO: Measure the true height of the title div? FontSize doesn't necessary result in the same height value.<br/><br/>The height insurance in pixels is to reduce likely hood that the height for the text node is larger than that of the div itself. If a more accurate measurement of the div's height is taken then perhaps this constant would not be necessary.</summary>
    private const int HEIGHT_INSURANCE_IN_PIXELS = 20;
    
    private const int HEIGHT_OF_TITLE_DIV_BORDER_BOTTOM_IN_PIXELS = 4;

    private ViewKind ActiveViewKind = 0;

    /// <summary>TODO: Measure the true height of the title div? FontSize doesn't necessary result in the same height value.</summary>
    private int GetHeightOfTitleDivWithoutBorder(AppOptionsState appOptionsState)
    {
        var fontSizeInPixels = appOptionsState.Options.FontSizeInPixels ??
            AppOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS;

        return fontSizeInPixels + HEIGHT_INSURANCE_IN_PIXELS;
    }

    private string GetTitleCssStyleString(AppOptionsState appOptionsState)
    {
        var heightOfTitleDivWithoutBorderCssValue = GetHeightOfTitleDivWithoutBorder(appOptionsState)
            .ToCssValue();

        return $"height: {heightOfTitleDivWithoutBorderCssValue}px;" +
            $" border-bottom: {HEIGHT_OF_TITLE_DIV_BORDER_BOTTOM_IN_PIXELS}px solid var(--luth_primary-border-color);";
    }
    
    private string GetBodyCssStyleString(AppOptionsState appOptionsState)
    {
        var totalHeightOfTitleDivCssValue = (GetHeightOfTitleDivWithoutBorder(appOptionsState) +
            HEIGHT_OF_TITLE_DIV_BORDER_BOTTOM_IN_PIXELS)
            .ToCssValue();

        return $"height: calc(100% - {totalHeightOfTitleDivCssValue}px);";
    }

    private string GetIsActiveCssClass(
        ViewKind viewKind,
        ViewKind renderBatchActiveViewKind)
    {
        return viewKind == renderBatchActiveViewKind
            ? "luth_active"
            : string.Empty;
    }

    private void SetActiveviewKindOnClick(
        ViewKind viewKind)
    {
        ActiveViewKind = viewKind;
    }
}