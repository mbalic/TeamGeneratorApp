using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class UserInGroupRepository : GenericRepository<UserInGroup>
    {
        public UserInGroupRepository(TeamDb4Entities context) : base(context)
        {
        }

        public IEnumerable<UserInGroup> GetByGroupId(int groupId)
        {
            return context.UserInGroup.Where(c => c.GroupId == groupId);
        }


    }
}
