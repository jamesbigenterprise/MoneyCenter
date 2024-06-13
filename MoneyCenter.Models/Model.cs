using MoneyCenter.RealmData;
using MoneyCenter.Schema;
using MoneyCenter.SQLiteWrapper;
using MongoDB.Bson;
using Realms;
using static Realms.Sync.MongoClient;

namespace MoneyCenter.Model
{
    public class Model
    {
        private readonly RealmContext _realmContext;
        private MoneyCenterDatabase databaseHelper;
        public Model() 
        {
            _realmContext = new RealmContext();
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
        public RealmContext RealmContext()
        {
            // Replace 'SingleEntryDataModel' with your actual data model type
            
            return _realmContext;
            
        }
        public async Task AddEntry(SingleEntryDataModel entry) 
        {
            await InitializeDatabase();
            await databaseHelper.InsertEntry(entry);

        }
    }
}
