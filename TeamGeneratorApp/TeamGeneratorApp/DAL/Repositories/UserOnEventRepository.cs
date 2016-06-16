using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class UserOnEventRepository : GenericRepository<UserOnEvent>
    {
        public UserOnEventRepository(TeamDb6Entities context) : base(context)
        {
        }

        public IEnumerable<UserOnEvent> GetByEventId(int eventId)
        {
            return context.UserOnEvent.Where(c => c.EventId == eventId);
        }

        public UserOnEvent GetByUserId(string userId)
        {
            var rez = context.UserOnEvent.Where(p => p.UserInCategory.UserInGroup.AspNetUsers.Id == userId);
            if (rez.Any())
            {
                return rez.Single();
            }
            else
            {
                return null;
            }
        }
    }
}
