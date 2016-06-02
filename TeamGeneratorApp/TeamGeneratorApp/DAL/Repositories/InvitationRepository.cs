using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class InvitationRepository : GenericRepository<Invitaton>
    {
        public InvitationRepository(TeamDb6Entities context) : base(context)
        {
        }

        public IEnumerable<Invitaton> GetByGroupId(int groupId)
        {
            return context.Invitaton.Where(p => p.GroupId == groupId).OrderByDescending(p => p.DateCreated);
        }

    }
}
