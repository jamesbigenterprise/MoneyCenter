using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using MoneyCenter.Views;

namespace MoneyCenter.ViewModel;

public partial class WelcomeViewModel : ObservableObject
{
    public ObservableCollection<WelcomePageModel> Pages { get; } = new()
    {
        new WelcomePageModel { Title = "Welcome!", Description = "This is MoneyCenter." },
        new WelcomePageModel { Title = "Private & Local", Description = "Your data stays on your device." },
        new WelcomePageModel { Title = "Easy to Use", Description = "Track your finances simply." }
    };

    [RelayCommand]
    private async Task Continue()
    {
        await Shell.Current.GoToAsync(nameof(PermissionsPage));
    }
}

public class WelcomePageModel
{
    public string Title { get; set; }
    public string Description { get; set; }
}
