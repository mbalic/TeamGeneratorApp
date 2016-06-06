using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class PositionRepository : GenericRepository<PositionInCategory>
    {
        public PositionRepository(TeamDb6Entities context) : base(context)
        {
        }

        public IEnumerable<PositionInCategory> GetByCategoryId(int categoryId)
        {
            return context.PositionInCategory.Where(c => c.CategoryId == categoryId);
        }


    }
}
