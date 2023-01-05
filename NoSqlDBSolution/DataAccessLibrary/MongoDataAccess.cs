using MongoDB.Bson;
using MongoDB.Driver;
public class MongoDataAccess
{
   private IMongoDatabase db;
   public MongoDataAccess(string dbName, string connString)
   {
      // define a client to the mongodb server
      var client = new MongoClient(connString);

      // open a connection to connect to specific database in the mongodb server
      db = client.GetDatabase(dbName);  
   }

   public void InsertRecord<T>(string table, T record)
   {
      var collection = db.GetCollection<T>(table);
      collection.InsertOne(record);
   }

   public List<T> LoadRecords<T>(string table)
   {
      // establish a connection to the specific collection we need
      var collection = db.GetCollection<T>(table);

      // returns all the matchings of this empty bson document which means all documents
      return collection.Find(new BsonDocument()).ToList();
   }

   public T LoadRecordById<T>(string table, Guid id)
   {
      var collection = db.GetCollection<T>(table);
      // build the filter based on the Id property
      var filter = Builders<T>.Filter.Eq("Id", id);
      
      return collection.Find(filter).First();
   }

   // combination of update and isnert
   // because insert alone will fail if the record is exists before
   public void UpsertRecord<T>(string table, Guid id, T record)
   {
      var collection = db.GetCollection<T>(table);

      // if you find this document, replace it with this new upated record
      // if you don't, just add it as a new record
      var result = collection.ReplaceOne(
         new BsonDocument("_id", id),
         record,
         new UpdateOptions{IsUpsert = true});
   }

   public void DeleteRecord<T> (string table, Guid id)
   {
      var collection = db.GetCollection<T>(table);
      var filter = Builders<T>.Filter.Eq("Id", id);
      collection.DeleteOne(filter);
   }
}