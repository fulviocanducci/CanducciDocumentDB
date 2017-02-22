using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace Canducci.DocumentDB
{
    public interface IRepository<T> : IDisposable
        where T : class, new()
    {
        Task<T> InsertAsync(T document);
        Task<ResourceResponse<Document>> UpdateAsync(T document, string id);
        Task<ResourceResponse<Document>> DeleteAsync(string id);
        Task<T> FindAsync(string id);

        IEnumerable<T> All();
        IEnumerable<T> All(Expression<Func<T, bool>> where);
        IEnumerable<T> All<TKey>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy);
        IEnumerable<TDocument> All<TKey, TDocument>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy, Expression<Func<T, TDocument>> select);

        Task<IEnumerable<T>> AllAsync();
        Task<IEnumerable<T>> AllAsync(Expression<Func<T, bool>> where);
        Task<IEnumerable<T>> AllAsync<TKey>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy);
        Task<IEnumerable<TDocument>> AllAsync<TKey, TDocument>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy, Expression<Func<T, TDocument>> select);

        IOrderedQueryable<T> Query();
    }
}
