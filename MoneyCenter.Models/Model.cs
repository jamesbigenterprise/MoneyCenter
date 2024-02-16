using MoneyCenter.RealmData;

namespace MoneyCenter.Model
{
    public class Model
    {
        private readonly RealmContext _realmContext;
        public Model() 
        {
            _realmContext = new RealmContext();
        }
        public List<SingleEntry> GetAllEntries() 
        {
            var allItems = _realmContext.RealmInstance.All<SingleEntry>().ToList();
            return allItems;
        }
        public bool AddEntry(SingleEntry entry) 
        {
            try 
            {
                _realmContext.RealmInstance.Write(() =>
                {
                    _realmContext.RealmInstance.Add(entry);
                });
                return true;
            }catch (Exception ex) 
            {
                //error saving the entry
                return false;
            }
        }
    }
}
