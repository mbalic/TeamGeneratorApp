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


        public IEnumerable<Event> GetByUserId(string id)
        {
            var q = from e in context.Event
                    join c in context.Category on e.CategoryId equals c.Id
                    join g in context.Group on c.GroupId equals g.Id
                    join uig in context.UserInGroup on g.Id equals uig.GroupId
                    where uig.UserId == id
                    select e;

            return q;
        }


    }
}

