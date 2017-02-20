using Microsoft.Azure.Documents.Client;
using System;
namespace Canducci.DocumentDB
{
    public sealed class ConnectionDocumentDB: IDisposable
    {
        public string DatabaseName { get; private set; }
        public DocumentClient Client { get; private set; }

        public ConnectionDocumentDB(string url, string key, string database)
        {
            Client = new DocumentClient(new Uri(url),
                key, 
                new ConnectionPolicy { EnableEndpointDiscovery = false });
            DatabaseName = database;         
        }
        public void Dispose()
        {
            if (Client != null)
            {
                Client.Dispose();
                Client = null;
            }
        }
    }
}
