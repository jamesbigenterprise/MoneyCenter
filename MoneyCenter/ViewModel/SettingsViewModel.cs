using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.ViewModel
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IFinancialService _financialService;
        private readonly IToastService _toastService;

        public SettingsViewModel(IFinancialService financialService, IToastService toastService)
        {
            _financialService = financialService;
            _toastService = toastService;
        }

        [RelayCommand]
        private async Task ExportData()
        {
            // Placeholder: implement actual export logic
            await Task.Delay(500);
            _toastService.Show("Exported data (not really, this is a placeholder).");
        }

        [RelayCommand]
        private async Task ImportData()
        {
            // Placeholder: implement actual import logic
            await Task.Delay(500);
            _toastService.Show("Imported data (not really, this is a placeholder).");
        }
    }

}
