using System.Collections.Generic;

namespace NoSql.MongoDB
{
    public class MongoDBOptions
    {
        private List<MongoDBServer> _servers;

        private MongoDBOptions()
        {
            _servers = new List<MongoDBServer>();
        }
        public MongoDBOptions(string host, int port, string defaultDatabase) : this()
        {
            AddServer(host, port);
            DefaultDatabase = defaultDatabase;
        }

        public IEnumerable<MongoDBServer> Servers { get { return _servers; } }
        public string DefaultDatabase { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }

        public MongoDBOptions AddServer(string host, int port)
        {
            var server = new MongoDBServer(host, port);
            if (!_servers.Contains(server))
                _servers.Add(server);

            return this;
        }
    }
}
