using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class UserInTeamRepository : GenericRepository<UserInTeam>
    {
        public UserInTeamRepository(TeamDb6Entities context) : base(context)
        {
        }

        public IEnumerable<UserInTeam> GetByTeamId(int teamId)
        {
            return context.UserInTeam.Where(p => p.TeamId == teamId);
        }

        public UserInTeam GetByUserOnEventAndGeneratorId(int userOnEventId, int generatorId)
        {
            var rez =
                context.UserInTeam.Where(p => p.UserOnEventId == userOnEventId && p.Team.GeneratorId == generatorId);
            if (rez.Any())
            {
                return rez.Single();
            }
            else
            {
                return null;
            }
            
        }

        public IEnumerable<UserInTeam> GetByGeneratorId(int generatorId)
        {
            return context.UserInTeam.Where(p => p.Team.GeneratorId == generatorId);
        }

        public IEnumerable<UserInTeam> GetByUserInCategoryId(int userInCategoryId)
        {
            return context.UserInTeam.Where(p => p.UserOnEvent.UserInCategoryId == userInCategoryId);
        }

        public IEnumerable<UserInTeam> GetByUserInGroupId(int userInGroupId)
        {
            return context.UserInTeam.Where(p => p.UserOnEvent.UserInCategory.UserInGroupId == userInGroupId);
        }



    }
}
