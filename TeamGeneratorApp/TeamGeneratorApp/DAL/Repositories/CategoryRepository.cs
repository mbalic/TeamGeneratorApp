using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.DAL.Repositories;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class CategoryRepository : GenericRepository<Category>
    {
        public CategoryRepository(
            TeamDb6Entities context) : base(context)
        {
        }

        public IEnumerable<Category> GetByGroupId(int groupId)
        {
            return context.Category.Where(c => c.GroupId == groupId);
        }

        public IEnumerable<Category> GetByUserId(string id)
        {
            var q = from c in context.Category
                    join g in context.Group on c.GroupId equals g.Id
                    join uig in context.UserInGroup on g.Id equals uig.GroupId
                    where uig.UserId == id
                    select c;
            
            return q;
        }



        public IEnumerable<Position> GetPositionsInCategory(int categoryId)
        {
            return context.Position.Where(c => c.CategoryId == categoryId);
        }

        public void InsertPositionInCategory(Position item)
        {
            context.Position.Add(item);
        }

        public void UpdatePositionInCategory(Position item)
        {
            context.Position.Attach(item);
            context.Entry(item).State = EntityState.Modified;
        }

        public void DeletePositionInCategory(int id)
        {
            Position item = context.Position.Find(id);
            if (context.Entry(item).State == EntityState.Detached)
            {
                context.Position.Attach(item);
            }
            context.Position.Remove(item);
        }
        

    }
}
