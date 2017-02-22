using Microsoft.Azure.Documents.Client;
using System;

namespace Canducci.DocumentDBCore
{
    public sealed class ConnectionDocumentDB: IDisposable
    {
        public string DatabaseName { get; private set; }
        public DocumentClient Client { get; private set; }        
        public ConnectionPolicy ConnectionPolicy { get; private set; }

        public ConnectionDocumentDB(string url, string key, string database)
        {
            ConnectionPolicy ConnectionPolicy = new ConnectionPolicy { EnableEndpointDiscovery = false };            
            Client = new DocumentClient(new Uri(url), key, ConnectionPolicy);
            DatabaseName = database;         
        }        

        //public async Task<Database> GetOrCreateDatabaseIfNotExists()
        //{
        //    return await Client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
        //}

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
