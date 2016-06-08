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

         public UserVoting GetUserVoting(string userId, string votingId)
        {
            return context.UserVoting.Where(p => p.UserId == userId && p.VotingId == votingId).Single();
        }
    }
}
