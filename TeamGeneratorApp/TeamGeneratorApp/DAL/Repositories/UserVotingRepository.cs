using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class UserVotingRepository : GenericRepository<UserVoting>
    {
        public UserVotingRepository(TeamDb6Entities context) : base(context)
        {
        }

         public UserVoting GetByUserOnEventAndVotingId(int userOnEventId, string votingId)
        {
             var rez = context.UserVoting.Where(p => p.UserOnEventId == userOnEventId && p.VotingId == votingId);
             if (rez.Any())
             {
                 return rez.Single();
             }
             else
             {
                 return null;
             }
        }

        public IEnumerable<UserVoting> GetByVotingId(string votingId)
        {
            return context.UserVoting.Where(p => p.VotingId == votingId);
        }

        public IEnumerable<UserVoting> GetByUserIdAndActivity(string userId, bool active)
        {
            return context.UserVoting.Where(p => p.UserOnEvent.UserInCategory.UserInGroup.UserId == userId && p.Voting.Active == active && p.VoteCounter < p.Voting.VotesPerUser);
        }

       

    }
}
