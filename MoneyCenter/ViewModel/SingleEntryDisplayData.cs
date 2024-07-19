using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.ViewModel
{
    //This is one of the ways to view the data, there will be pie and bar charts as well
    //The fields from the database need to be formatted to fit into this
    public partial class SingleEntryDisplayData : ObservableObject
    {
        [ObservableProperty]
        private string date;
        [ObservableProperty]
        private string category;
        [ObservableProperty]
        private string amount;
        [ObservableProperty]
        private string paragraph;
        [ObservableProperty]
        private int id;
    }
}
