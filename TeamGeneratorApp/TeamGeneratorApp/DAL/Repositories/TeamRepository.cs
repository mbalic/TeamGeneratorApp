using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class TeamRepository : GenericRepository<Team>
    {
        public TeamRepository(TeamDb4Entities context) : base(context)
        {
        }


    }
}
