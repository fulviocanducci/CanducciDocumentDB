using Microsoft.Azure.Documents.Client;
using System;
namespace Canducci.DocumentDB
{
    public sealed class ConnectionDocumentDB: IDisposable
    {        
        public string DatabaseName { get; private set; } = "Todo";
        public DocumentClient Client { get; private set; }

        public ConnectionDocumentDB(
            string url = "https://localhost:8081/",
            string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", 
            string database = "Todo")
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
