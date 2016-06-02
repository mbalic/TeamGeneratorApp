using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class GroupRepository : GenericRepository<Group>
    {
        public GroupRepository(TeamDb6Entities context) : base(context)
        {
        }

        public IEnumerable<Group> GetByUserId(string id)
        {
            return context.Group.Where(p => p.OwnerId == id);
        }

    }
}