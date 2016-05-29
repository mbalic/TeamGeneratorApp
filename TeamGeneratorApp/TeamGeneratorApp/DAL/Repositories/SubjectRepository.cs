using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>
    {
        public SubjectRepository(TeamDb3Entities context) : base(context)
        {
        }

        public IEnumerable<Subject> GetByPoolId(int poolId)
        {
            var result =
                context.SubjectInPool.Where(s => s.PoolId == poolId).Include("Subject").Select(s => s.Subject);
            return result;
        }

    }
}