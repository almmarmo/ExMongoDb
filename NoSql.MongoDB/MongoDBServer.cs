using System;

namespace NoSql.MongoDB
{
    public class MongoDBServer
    {
        public MongoDBServer(string host, int port)
        {
            if (String.IsNullOrEmpty(host))
                throw new ArgumentNullException("host", "Host cannot be null or empty.");
            if (port <= 0)
                throw new ArgumentException("Port cannot be less or equal to zero.");

            Host = host;
            Port = port;
        }

        public string Host { get; private set; }
        public int Port { get; private set; }

        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
    }
}
