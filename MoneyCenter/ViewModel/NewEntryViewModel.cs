using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Schema;
using MoneyCenter.Model;
using CommunityToolkit.Maui.Alerts;
using MoneyCenter.ViewModel.Extensions;
using System.Collections.ObjectModel;
using VmMasterCategory = MoneyCenter.ViewModel.Objects.MasterCategory;
using VmPaymentAccount = MoneyCenter.ViewModel.Objects.PaymentAccount;

namespace MoneyCenter.ViewModel
{
    public partial class NewEntryViewModel : ObservableObject
    {

        private NewEntryInputData newEntryModel = new();
        public NewEntryInputData NewEntryModel
        {
            get => newEntryModel;
            set => SetProperty(ref newEntryModel, value);
        }
        private readonly IModel model;
        private MainViewModel _home;

        [ObservableProperty]
        private ObservableCollection<VmMasterCategory> masterCategories = new();

        [ObservableProperty]
        private ObservableCollection<VmPaymentAccount> paymentAccounts = new();

        [ObservableProperty]
        private VmMasterCategory? selectedCategory;

        [ObservableProperty]
        private VmPaymentAccount? selectedPaymentAccount;

        public NewEntryViewModel(MainViewModel vm, IModel model) 
        {

            _home = vm;
            this.model = model;
            _ = LoadLookupsAsync();

        }

        partial void OnSelectedCategoryChanged(VmMasterCategory? value)
        {
            if (value != null)
                NewEntryModel.MasterCategoryId = value.Id;
        }

        partial void OnSelectedPaymentAccountChanged(VmPaymentAccount? value)
        {
            if (value != null)
                NewEntryModel.PaymentAccountId = value.Id;
        }

        [RelayCommand]
        async Task Save()
        {
            await saveEntry();
            await Close();
        }

        [RelayCommand]
        async Task Close()
        {
            await Application.Current!.MainPage!.Navigation.PopModalAsync();
        }
        private async Task saveEntry() 
        {
            if (newEntryModel.MonthId <= 0) return;
            Expense singleEntry = new Expense
            {
                Id = Guid.NewGuid().ToString(),
                Date = newEntryModel.Date,
                Destination = newEntryModel.Store,
                MasterCategoryId = newEntryModel.MasterCategoryId,
                Amount = newEntryModel.Amount,
                PaymentAccountId = newEntryModel.PaymentAccountId,
                Details = newEntryModel.Details,
                MonthId = newEntryModel.MonthId
            };
            await model.AddExpense(newEntryModel.MonthId, singleEntry);
            await Toast.Make("Expense added successfully.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }

        private async Task LoadLookupsAsync()
        {
            try
            {
                var categories = await model.GetMasterCategories();
                var accounts = await model.GetPaymentAccounts();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MasterCategories.Clear();
                    foreach (var category in categories)
                        MasterCategories.Add(category.ToViewModel());

                    PaymentAccounts.Clear();
                    foreach (var account in accounts)
                        PaymentAccounts.Add(account.ToViewModel());

                    SelectedCategory = MasterCategories.FirstOrDefault();
                    SelectedPaymentAccount = PaymentAccounts.FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadLookupsAsync error: {ex.Message}");
            }
        }
    }
}
