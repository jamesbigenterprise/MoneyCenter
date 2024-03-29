﻿using MoneyCenter.RealmData;
using Realms;

namespace MoneyCenter.Model
{
    public class Model
    {
        private readonly RealmContext _realmContext;
        public Model() 
        {
            _realmContext = new RealmContext();
        }
        public List<SingleEntryDataModel> GetAllEntries() 
        {
            var allItems = _realmContext.RealmInstance.All<SingleEntryDataModel>().ToList();
            return allItems;
        }
        public RealmContext RealmContext()
        {
            // Replace 'SingleEntryDataModel' with your actual data model type
            
            return _realmContext;
            
        }
        public bool AddEntry(SingleEntryDataModel entry) 
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
