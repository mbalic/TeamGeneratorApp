using System;
using System.Collections.Generic;
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


    }
}
