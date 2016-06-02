using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class InvitationRepository : GenericRepository<UserInGroupInvitation>
    {
        public InvitationRepository(TeamDb4Entities context) : base(context)
        {
        }

        public IEnumerable<UserInGroupInvitation> GetByGroupId(int groupId)
        {
            return context.UserInGroupInvitation.Where(p => p.GroupId == groupId).OrderByDescending(p => p.DateCreated);
        }

    }
}
