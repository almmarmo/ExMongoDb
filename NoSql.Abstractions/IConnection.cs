using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoSql.Abstractions
{
    public interface IConnection
    {
        Task<object> AddAsync<TEntity>(TEntity entity, string table, Func<TEntity, object> keyExpression);
        Task AddAsync<TEntity>(IEnumerable<TEntity> entities, string table, Func<TEntity, string> keyExpression);
        TEntity Get<TEntity>(object key, string table);
        IEnumerable<TEntity> List<TEntity>(string table);
        IEnumerable<TEntity> List<TEntity>(string table, Expression<Func<TEntity, bool>> expression);
        Task DeleteAsync(string table, string filter);
        Task DeleteContentAsync(string table, string contentFilter);
    }
}
