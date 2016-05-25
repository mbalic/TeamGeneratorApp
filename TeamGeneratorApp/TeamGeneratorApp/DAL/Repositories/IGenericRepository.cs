using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.DAL.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetWithRawSql(string query, params object[] parameters);

        IEnumerable<T> Get(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeProperties);

        T GetByID(object id);

        void Insert(T entity);

        void Delete(object id);

        void Delete(T entityToDelete);

        void Update(T entityToUpdate);

    }
}
