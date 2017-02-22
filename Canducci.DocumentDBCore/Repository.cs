using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace Canducci.DocumentDBCore
{
    public abstract class Repository<T> : IRepository<T>
        where T : class, new()
    {

        private ConnectionDocumentDB connection { get; set; }
        private DocumentClient documentClient { get; set; }
        private DocumentCollection documentCollection { get; set; }
        private string collectionName { get; set; }

        public Repository(ConnectionDocumentDB connection)
        {
            Initialize(connection);            
        }

        public Repository(ConnectionDocumentDB connection, string collectionName)
        {
            this.collectionName = collectionName;
            Initialize(connection);
        }

        public async Task<T> InsertAsync(T document)
        {
            ResourceResponse<Document> result =
                 await documentClient.CreateDocumentAsync(GetDocumentUri(), document);

            return (T)(dynamic)result.Resource;
        }

        public async Task<ResourceResponse<Document>> UpdateAsync(T document, string id)
        {
            return await documentClient.ReplaceDocumentAsync(GetDocumentCreateUri(id), document);
        }

        public async Task<ResourceResponse<Document>> DeleteAsync(string id)
        {
            return await documentClient.DeleteDocumentAsync(GetDocumentCreateUri(id));
        }

        public async Task<T> FindAsync(string id)
        {
            Document doc = await documentClient.ReadDocumentAsync(GetDocumentCreateUri(id));

            return (T)((dynamic)doc);
        }

        public async Task<IEnumerable<T>> AllAsync()
        {
            return await GetAllListAsync(GetOrderedQueryable()
                .AsDocumentQuery());
        }

        public async Task<IEnumerable<T>> AllAsync(Expression<Func<T, bool>> where)
        {
            IDocumentQuery<T> docQuery = GetOrderedQueryable()
                .Where(where)
                .AsDocumentQuery();

            return await GetAllListAsync(docQuery);
        }

        public async Task<IEnumerable<T>> AllAsync<TKey>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy)
        {
            IDocumentQuery<T> docQuery = GetOrderedQueryable()
                .Where(where)
                .OrderBy(orderBy)
                .AsDocumentQuery();

            return await GetAllListAsync(docQuery);
        }

        public async Task<IEnumerable<TDocument>> AllAsync<TKey, TDocument>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy, Expression<Func<T, TDocument>> select)
        {
            IDocumentQuery<TDocument> docQuery = GetOrderedQueryable()
                .Where(where)
                .OrderBy(orderBy)
                .Select(select)
                .AsDocumentQuery();

            return await GetAllListAsync(docQuery);
        }

        public IOrderedQueryable<T> Query()
        {
            return GetOrderedQueryable();
        }

        public async Task<DocumentCollection> GetOrCreateDocumentCollectionIfNotExists()
        {
            Database database = await connection.GetOrCreateDatabaseIfNotExists();
            documentCollection = await documentClient
                .CreateDocumentCollectionIfNotExistsAsync(database.SelfLink,
                new DocumentCollection { Id = collectionName });
            return documentCollection;
        }

        #region _private        

        private void Initialize(ConnectionDocumentDB conn)
        {
            connection = conn;
            documentClient = connection.Client;
        }

        private IOrderedQueryable<T> GetOrderedQueryable()
        {
            return documentClient.CreateDocumentQuery<T>(GetDocumentUri(), new FeedOptions { MaxItemCount = -1 });                 
        }

        private async Task<IEnumerable<TDocument>> GetAllListAsync<TDocument>(IDocumentQuery<TDocument> docQuery)
        {
            List<TDocument> _list = new List<TDocument>();
            while (docQuery.HasMoreResults)
            {
                _list.AddRange(await docQuery.ExecuteNextAsync<TDocument>());
            }
            return _list.ToList();
        }

        private Uri GetDatabaseUri()
        {
            return UriFactory.CreateDatabaseUri(connection.DatabaseName);
        }

        private Uri GetDocumentUri()
        {
            return UriFactory.CreateDocumentCollectionUri(connection.DatabaseName, collectionName);
        }

        private Uri GetDocumentCreateUri(string id)
        {
            return UriFactory.CreateDocumentUri(connection.DatabaseName, collectionName, id);
        }        

        #endregion

        #region dispose 
        public void Dispose()
        {
            if (documentClient != null)
            {
                documentClient.Dispose();
                documentClient = null;
            }
        }
        
        #endregion dispose

    }
}
