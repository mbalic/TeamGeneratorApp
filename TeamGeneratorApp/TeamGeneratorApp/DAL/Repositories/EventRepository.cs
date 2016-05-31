using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class EventRepository : GenericRepository<Event>
    {
        public EventRepository(TeamDb4Entities context) : base(context)
        {
        }

        public IEnumerable<Event> GetByPoolId(int poolId)
        {
            return context.Event.Where(e => e.PoolId == poolId);
        } 


    }
}

