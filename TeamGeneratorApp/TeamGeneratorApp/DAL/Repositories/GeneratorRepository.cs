using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class GeneratorRepository : GenericRepository<Generator>
    {
        public GeneratorRepository(TeamDb6Entities context) : base(context)
        {
        }
        public IEnumerable<Criteria> GetCriterias()
        {
            return context.Criteria.ToList();
        }

        public IEnumerable<Generator> GetByEventId(int eventId)
        {
            return context.Generator.Where(p => p.EventId == eventId);
        }

        public bool IsOwner(string userId, int generatorId)
        {
            var res = context.Generator.Where(p => p.Id == generatorId && p.Event.Category.Group.OwnerId == userId);
            if (res.Any())
                return true;
            else
                return false;
        }

    }
}
