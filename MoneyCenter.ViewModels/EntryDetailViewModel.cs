using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.ViewModels
{
    public partial class EntryDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        private SingleEntry entry;
    }
}
