using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class UserInCategoryRepository : GenericRepository<UserInCategory>
    {
        public UserInCategoryRepository(TeamDb6Entities context) : base(context)
        {
        }

        public IEnumerable<UserInCategory> GetByCategoryId(int categoryId)
        {
            return context.UserInCategory.Where(c => c.CategoryId == categoryId);
        }

        public UserInCategory GetByUserAndCategoryId(string userId, int categoryId)
        {
            return context.UserInCategory.Where(c => c.CategoryId == categoryId && c.UserIId == userId).Single();
        }
    }
}
