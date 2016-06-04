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
        public RoleRepository(TeamDb6Entities context) : base(context)
        {
        }

        public override void Insert(AspNetRoles role)
        {
            role.Id = Guid.NewGuid().ToString();
            dbSet.Add(role);
        }

        public AspNetRoles FindByName(string name)
        {
            return context.AspNetRoles.Where(p => p.Name == name).Single();
        } 


    }
}