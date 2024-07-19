using MoneyCenter.Schema;
using MoneyCenter.SQLiteWrapper;

namespace MoneyCenter.Model
{
    public class MoneyCenterModel : IModel
    {
        private MoneyCenterDatabase? databaseHelper;
        public MoneyCenterModel() 
        {
        }
        public async Task InitializeDatabase() 
        {
            if (databaseHelper == null) 
            {
                databaseHelper = await MoneyCenterDatabase.CreateAsync();
            }
            //Research the best way to check if the initialization was succesful
        }

        public async Task<List<SingleEntryDataModel>> GetAllEntries() 
        {
            await InitializeDatabase();
            var allItems = await databaseHelper.GetAllEntries();
            return allItems;
        }
        public async Task DeleteSingleEntry(int id) 
        {
            await InitializeDatabase();
            int response = await databaseHelper.DeleteEntryByID(id);
        }

        public async Task AddEntry(SingleEntryDataModel entry) 
        {
            await InitializeDatabase();
            await databaseHelper.InsertEntry(entry);

        }
    }
}
