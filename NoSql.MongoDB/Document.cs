using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace NoSql.MongoDB
{
    public class Document
    {
        public Document()
        {
            Date = DateTime.Now;
        }

        [BsonId]
        public object Key { get; set; }
        public DateTime Date { get; set; }
        public string ContentType { get; set; }
        public BsonDocument Content { get; set; }
    }
}
