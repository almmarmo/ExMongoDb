using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using NoSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NoSql.MongoDB
{
    public class Connection : IConnection
    {
        private MongoClient _client;
        private IMongoDatabase _db;
        private readonly MongoCollectionSettings _mongoSettings;
        public Connection(MongoDBOptions options)
        {
            var settings = GetSettings(options);
            _client = new MongoClient(settings);
            _db = _client.GetDatabase(options.DefaultDatabase);

            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

            _mongoSettings = new MongoCollectionSettings();
            _mongoSettings.GuidRepresentation = GuidRepresentation.Standard;
        }

        private MongoClientSettings GetSettings(MongoDBOptions options)
        {
            MongoClientSettings settings = new MongoClientSettings();
            settings.UseSsl = options.EnableSsl;
            if (options.Servers.Count() > 0)
                settings.Servers = options.Servers.Select(x => new MongoServerAddress(x.Host, x.Port));
            else
            {
                var server = settings.Servers.FirstOrDefault();
                settings.Server = new MongoServerAddress(server.Host, server.Port);
            }

            if (!String.IsNullOrEmpty(options.Username) && !String.IsNullOrEmpty(options.Password))
                settings.Credential = MongoCredential.CreateCredential(options.DefaultDatabase, options.Username, options.Password);

            return settings;
        }

        public Document Get(object key, string table)
        {
            var collection = _db.GetCollection<Document>(table);

            return collection
                .AsQueryable()
                .FirstOrDefault(x => x.Key == key);
        }

        public TEntity Get<TEntity>(object key, string table)
        {
            var collection = _db.GetCollection<Document>(table);
            return collection
                .AsQueryable()
                .Where(x => x.Key == key)
                .ToList()
                .Select(x => BsonSerializer.Deserialize<TEntity>(x.Content, null))
                .FirstOrDefault();
        }

        public IEnumerable<Document> List(string table)
        {
            return _db.GetCollection<Document>(table)
                .AsQueryable()
                .ToList();
        }

        public IEnumerable<TEntity> List<TEntity>(string table, Expression<Func<TEntity, bool>> expression)
        {
            var collection = _db.GetCollection<Document>(table);
            var contentType = typeof(TEntity).FullName;
            var docs = collection
                .AsQueryable()
                .Where(x => x.ContentType == contentType);

            return docs
                .ToList()
                .Select(x => BsonSerializer.Deserialize<TEntity>(x.Content, null))
                .Where(expression.Compile());
        }

        public IEnumerable<TEntity> List<TEntity>(string table)
        {
            var collection = _db.GetCollection<Document>(table);
            return collection
                .AsQueryable()
                .Where(x => x.ContentType == typeof(TEntity).FullName)
                .Select(x => BsonSerializer.Deserialize<TEntity>(x.Content, null));
        }

        public IEnumerable<TEntity> List<TEntity>(string table, string contentFilter)
        {
            var pos = contentFilter.IndexOf("'");
            if (pos != 0)
                throw new ArgumentException("Content filter is invalid.");
            else
                contentFilter = contentFilter.Insert(pos + 1, "Content.");

            var collection = _db.GetCollection<Document>(table);
            return collection.Find($"{{ {contentFilter} }}")
                .ToList()
                .Select(x => BsonSerializer.Deserialize<TEntity>(x.Content, null));
        }

        public async Task<object> AddAsync<TEntity>(TEntity entity, string table, Func<TEntity, object> keyExpression)
        {
            var collection = _db.GetCollection<Document>(table);

            Document doc = new Document();
            doc.ContentType = typeof(TEntity).FullName;
            doc.Content = entity.ToBsonDocument(typeof(TEntity));
            doc.Key = keyExpression.Invoke(entity) ?? Guid.NewGuid().ToString();

            await collection.InsertOneAsync(doc);

            return doc.Key;
        }

        public async Task AddAsync<TEntity>(IEnumerable<TEntity> entities, string table, Func<TEntity, string> keyExpression)
        {
            var collection = _db.GetCollection<Document>(table);
            var docs = entities.Select(x => new Document()
            {
                Content = x.ToBsonDocument(),
                ContentType = typeof(TEntity).FullName,
                Key = keyExpression.Invoke(x)
            });

            await collection.InsertManyAsync(docs);
        }

        //public async Task UpdateAsync<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> expression)
        //{
        //    await UpdateAsync(entity, GetTableName<TEntity>(), expression);
        //}

        //public async Task UpdateAsync<TEntity>(TEntity entity, string table, Expression<Func<TEntity, bool>> expression)
        //{
        //    var collection = _db.GetCollection<TEntity>(table);
        //    var docs = List(table, expression);
        //    if (docs.Count() > 1)
        //        throw new Exception("Expression returns more than one document.");

        //    await DeleteAsync(table, expression);
        //    //await AddAsync(entity);
        //}

        public async Task DeleteContentAsync(string table, string contentFilter)
        {
            var pos = contentFilter.IndexOf("'");
            if (pos != 0)
                throw new ArgumentException("Content filter is invalid.");
            else
                contentFilter = contentFilter.Insert(pos + 1, "Content.");

            await DeleteAsync(table, $"{{ {contentFilter} }}");
        }

        public async Task DeleteAsync(string table, string filter)
        {
            var collection = _db.GetCollection<Document>(table);
            await collection.DeleteManyAsync(filter);
        }

        public async Task DeleteAllAsnc<TEntity>(string table)
        {
            var collection = _db.GetCollection<TEntity>(table);
            await collection.DeleteManyAsync("{}");
        }

        private IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            return _db.GetCollection<TEntity>(GetTableName<TEntity>());
        }

        private string GetTableName<TEntity>()
        {
            return typeof(TEntity).Name;
        }
    }
}
