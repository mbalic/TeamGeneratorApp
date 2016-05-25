using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class RoleRepository : GenericRepository<AspNetRoles>
    {
        public RoleRepository(TeamDb3Entities context) : base(context)
        {
        }

        public override void Insert(AspNetRoles role)
        {
            role.Id = Guid.NewGuid().ToString();
            dbSet.Add(role);
        }


    }
}