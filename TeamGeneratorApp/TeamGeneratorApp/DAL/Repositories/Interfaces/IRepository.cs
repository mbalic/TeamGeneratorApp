using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.DAL.Repositories.Interfaces
{
    public interface IRepository<TEntity, TKey>
       where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        TEntity Get(TKey id);
        TEntity Add(TEntity entity);
        void Remove(TKey id);
        void Update(TEntity entity);
    }
}
