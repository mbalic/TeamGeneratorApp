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

        public IEnumerable<PositionInCategory> GetPositionsInCategory(int categoryId)
        {
            return context.PositionInCategory.Where(c => c.CategoryId == categoryId);
        }

        public void InsertPositionInCategory(PositionInCategory item)
        {
            context.PositionInCategory.Add(item);
        }

        public void UpdatePositionInCategory(PositionInCategory item)
        {
            context.PositionInCategory.Attach(item);
            context.Entry(item).State = EntityState.Modified;
        }

        public void DeletePositionInCategory(int id)
        {
            PositionInCategory item = context.PositionInCategory.Find(id);
            if (context.Entry(item).State == EntityState.Detached)
            {
                context.PositionInCategory.Attach(item);
            }
            context.PositionInCategory.Remove(item);
        }
        

    }
}
