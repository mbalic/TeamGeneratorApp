using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;
using TeamGeneratorApp.Models.ViewModels;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class UserRepository : GenericRepository<AspNetUsers>
    {
        public UserRepository(TeamDb6Entities context) : base(context)
        {
        }
        
        public AspNetUsers SearchByMail(string mail)
        {
            return context.AspNetUsers.Where(p => p.Email == mail).FirstOrDefault();
        }

    }
}
