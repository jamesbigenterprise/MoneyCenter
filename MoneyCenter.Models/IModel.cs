using MoneyCenter.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.Model
{
    public interface IModel
    {
        Task InitializeDatabase();
        Task<List<SingleEntryDataModel>> GetAllEntries();
        Task DeleteSingleEntry(int id);
        Task AddEntry(SingleEntryDataModel entry);
    }
}
