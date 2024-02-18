using MongoDB.Bson;
using Realms;

namespace MoneyCenter.Model
{
    public partial class SingleEntryDataModel : IRealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        [MapTo("date")]
        public DateTimeOffset Date { get; set; }

        [MapTo("store")]
        public string Store { get; set; }
        [MapTo("details")]
        public string Details { get; set; }
        [MapTo("amount")]
        public string Amount { get; set; }
        [MapTo("category")]
        public string Category { get; set; }
        [MapTo("pmtmethod")]
        public string PmtMethod { get; set; }
        [MapTo("applyto")]
        public string ApplyTo { get; set; }
    }
}
