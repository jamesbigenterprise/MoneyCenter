using Realms;

namespace MoneyCenter.RealmData
{
    public class RealmContext
    {
        public Realm RealmInstance { get; }

        public RealmContext()
        {
            RealmInstance = Realm.GetInstance();
        }
    }
}
