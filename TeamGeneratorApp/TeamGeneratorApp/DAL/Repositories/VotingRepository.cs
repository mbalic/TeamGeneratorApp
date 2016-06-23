using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class VotingRepository : GenericRepository<Voting>
    {
        public VotingRepository(TeamDb6Entities context) : base(context)
        {
        }

        public IEnumerable<Voting> GetByEventId(int eventId)
        {
            return context.Voting.Where(c => c.EventId == eventId);
        }

        public void InsertUserVoting(UserVoting item)
        {
            context.UserVoting.Add(item);
        }

        public IEnumerable<Voting> GetByOwnerIdAndActivity(string userId, bool? active)
        {
            if(active != null)
                return context.Voting.Where(p => p.Event.Category.Group.OwnerId == userId && p.Active == active);
            else
                return context.Voting.Where(p => p.Event.Category.Group.OwnerId == userId);
        }


    }
}
