using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class UserCategoryRepository : GenericRepository<UserCategory>
    {
        public UserCategoryRepository(TeamDb6Entities context) : base(context)
        {
        }

        public IEnumerable<UserCategory> GetByCategoryId(int categoryId)
        {
            return context.UserCategory.Where(c => c.CategoryId == categoryId);
        }


    }
}
