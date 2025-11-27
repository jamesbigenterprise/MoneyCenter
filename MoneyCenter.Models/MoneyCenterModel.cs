using MoneyCenter.Schema;
using MoneyCenter.SQLiteWrapper;

namespace MoneyCenter.Model
{
    public class MoneyCenterModel : IModel
    {
        private readonly MoneyCenterDatabase _database;

        public MoneyCenterModel(MoneyCenterDatabase database)
        {
            if (database == null) 
            {
                //TODO have the permissions page handle this instead of throwing an exception

                throw new ArgumentNullException(nameof(database));
            }

            _database = database;
        }

        public async Task InitializeDatabase()
        {
            await _database.InitializeAsync();
        }

        public async Task<List<SingleEntryDataModel>> GetAllEntries()
        {
            await InitializeDatabase();
            return await _database.GetAllEntries();
        }

        public async Task DeleteSingleEntry(int id)
        {
            await InitializeDatabase();
            await _database.DeleteEntryByID(id);
        }

        public async Task AddEntry(SingleEntryDataModel entry)
        {
            await InitializeDatabase();
            await _database.InsertEntry(entry);
        }
    }
}