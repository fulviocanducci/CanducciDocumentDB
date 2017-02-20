using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Canducci.DocumentDB
{
    public abstract class Repository<T> : IRepository<T>
        where T : class, new()
    {
        protected ConnectionDocumentDB _connection { get; private set; }        
        protected DocumentClient _doc { get; private set; }
        protected string _collectionName { get; private set; } = "";

        public Repository(ConnectionDocumentDB connection)
        {
            Initialize(connection);
        }

        public Repository(ConnectionDocumentDB connection, string collectionName)
        {
            Initialize(connection);
            _collectionName = collectionName;            
        }

        public async Task<T> InsertAsync(T document)
        {            
           ResourceResponse<Document> result = 
                await _doc.CreateDocumentAsync(GetDocumentUri(),document);
            return (T)(dynamic)result.Resource;
        }

        public async Task<ResourceResponse<Document>> UpdateAsync(T document, string id)
        {
            return await _doc.ReplaceDocumentAsync(GetDocumentCreateUri(id), document);
        }

        public async Task<ResourceResponse<Document>> DeleteAsync(string id)
        {
            return await _doc.DeleteDocumentAsync(GetDocumentCreateUri(id));
        }

        public async Task<T> FindAsync(string id)
        {
            Document doc = await _doc.ReadDocumentAsync(GetDocumentCreateUri(id));
            return (T)((dynamic)doc);
        }

        public async Task<IEnumerable<T>> AllAsync(Expression<Func<T, bool>> where)
        {
            IDocumentQuery<T> docQuery = GetOrderedQueryableAsync()
                .Where(where)
                .AsDocumentQuery();
            return await GetAllListAsync(docQuery);
        }

        public async Task<IEnumerable<T>> AllAsync<TKey>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy)
        {
            IDocumentQuery<T> docQuery = GetOrderedQueryableAsync()
                .Where(where)
                .OrderBy(orderBy)
                .AsDocumentQuery();
            return await GetAllListAsync(docQuery);
        }

        public async Task<IEnumerable<TDocument>> AllAsync<TKey, TDocument>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy, Expression<Func<T, TDocument>> select)
        {
            IDocumentQuery<TDocument> docQuery = GetOrderedQueryableAsync()
                .Where(where)
                .OrderBy(orderBy)
                .Select(select)
                .AsDocumentQuery();
            return await GetAllListAsync(docQuery);
        }

        public IOrderedQueryable Query()
        {
            return GetOrderedQueryableAsync();
        }

        #region _private
        private void Initialize(ConnectionDocumentDB connection)
        {
            _connection = connection;
            _doc = _connection.Client;
        }
        private IOrderedQueryable<T> GetOrderedQueryableAsync()
        {
            return _doc
                 .CreateDocumentQuery<T>(GetDocumentUri(),
                 new FeedOptions { MaxItemCount = -1 });
        }
        private async Task<IEnumerable<TDocument>> GetAllListAsync<TDocument>(IDocumentQuery<TDocument> docQuery)
        {
            List<TDocument> _list = new List<TDocument>();
            while (docQuery.HasMoreResults)
            {
                _list.AddRange(await docQuery.ExecuteNextAsync<TDocument>());
            }
            return _list.Count == 0 ? null : _list;
        }
        private Uri GetDocumentCreateUri(string id)
        {
            return UriFactory
                .CreateDocumentUri(_connection.DatabaseName, _collectionName, id);
        }
        private Uri GetDocumentUri()
        {
            return UriFactory
                 .CreateDocumentCollectionUri(_connection.DatabaseName, _collectionName);
        }

        #endregion

        #region dispose 
        public void Dispose()
        {
            if (_doc != null)
            {
                _doc.Dispose();
                _doc = null;
            }
        }
        #endregion

    }
}
