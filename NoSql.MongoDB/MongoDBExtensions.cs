using Autofac;
using NoSql.Abstractions;

namespace NoSql.MongoDB
{
    public static class MongoDBExtensions
    {
        public static void AddMongoDB(this ContainerBuilder builder, string host, int port, string defaultDatabase)
        {
            AddMongoDB(builder, new MongoDBOptions(host, port, defaultDatabase));
        }

        public static void AddMongoDB(this ContainerBuilder builder, MongoDBOptions options)
        {
            builder
                .Register(c => new Connection(options))
                .As<IConnection>();
        }
    }
}
