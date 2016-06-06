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
    }
}
