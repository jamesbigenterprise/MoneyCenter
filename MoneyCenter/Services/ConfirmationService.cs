using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using MoneyCenter.Views.Controls;

namespace MoneyCenter.Services;

public class ConfirmationService : IConfirmationService
{
    public async Task<bool> ConfirmAsync(string title, string message, string confirmText = "Delete")
    {
        var page = Application.Current?.Windows.FirstOrDefault()?.Page ?? Application.Current?.MainPage;
        if (page == null)
        {
            return false;
        }

        var result = await page.ShowPopupAsync<bool>(
            new ConfirmationPopup(title, message, confirmText),
            new PopupOptions
            {
                PageOverlayColor = Color.FromRgba(23, 28, 23, 0.48),
                CanBeDismissedByTappingOutsideOfPopup = true
            });

        return !result.WasDismissedByTappingOutsideOfPopup && result.Result;
    }
}
