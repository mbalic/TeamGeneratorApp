using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class EventRepository : GenericRepository<Event>
    {
        public EventRepository(TeamDb6Entities context) : base(context)
        {
        }

        public IEnumerable<Event> GetByCategoryId(int categoryId)
        {
            return context.Event.Where(c => c.CategoryId == categoryId);
        }

        public bool IsOwner(string userId, int eventid)
        {
            var res = context.Event.Where(p => p.Id == eventid && p.Category.Group.OwnerId == userId);
            if (res.Any())
                return true;
            else
                return false;
        }


    }
}

