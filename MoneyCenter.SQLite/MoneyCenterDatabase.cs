

using Microsoft.VisualBasic;
using MoneyCenter.Schema;
using SQLite;

namespace MoneyCenter.SQLiteWrapper
{
    public class MoneyCenterDatabase
    {
        SQLiteAsyncConnection Database;
        public MoneyCenterDatabase()
        {
            if (Database != null)
                return;
            Database = new SQLiteAsyncConnection(DatabaseConfig.DatabasePath, DatabaseConfig.Flags);
        }
        public async Task InitializeAsync()
        {
            await Database.CreateTableAsync<SingleEntryDataModel>();
        }

        public async Task<List<SingleEntryDataModel>> GetAllEntries() 
        {
            return await Database.Table<SingleEntryDataModel>().ToListAsync();
        }
        public async Task<int> DeleteEntryByID(int id) 
        {
            //remember error handling for not found cases
            return await Database.Table<SingleEntryDataModel>().DeleteAsync(entry=> entry.Id == id);
        }
        public async Task<int> InsertEntry(SingleEntryDataModel entry)
        {
            return await Database.InsertAsync(entry);
        }
        //Right now I will determine all the existing tables at the start but in the future maybe I wnat to give the user 
        //the havility to create their own tables
    }
}
