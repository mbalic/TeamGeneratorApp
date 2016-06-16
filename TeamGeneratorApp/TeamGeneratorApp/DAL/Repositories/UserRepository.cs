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


        public void InsertUser(AdminUserCreateVM user)
        {
            AspNetUsers newUser = new AspNetUsers();
            newUser.Id = Guid.NewGuid().ToString();
            newUser.Email = user.Email;
            newUser.UserName = user.UserName;
            if (user.IsAdmin)
            {
                AspNetRoles role = context.AspNetRoles.FirstOrDefault(r => r.Name == "Admin");
                newUser.AspNetRoles.Add(role);
            }
            //newUser.EmailConfirmed = false;
            //newUser.PhoneNumberConfirmed = false;
            //newUser.TwoFactorEnabled = false;
            //newUser.LockoutEnabled = false;
            //newUser.AccessFailedCount 

            dbSet.Add(newUser);
            
        }


        public void Update(AdminUserCreateVM data)
        {
            AspNetUsers user = GetByID(data.Id);
            user.Email = data.Email;
            user.UserName = data.UserName;
            if (data.IsAdmin)
            {
                if (user.AspNetRoles.Count == 0)
                {
                    AspNetRoles role = context.AspNetRoles.FirstOrDefault(r => r.Name == "Admin");
                    user.AspNetRoles.Add(role);
                }
            }
            else
            {
                user.AspNetRoles.Clear();
            }
        }

        public AspNetUsers SearchByMail(string mail)
        {
            return context.AspNetUsers.Where(p => p.Email == mail).FirstOrDefault();
        }

        //public IEnumerable<AspNetUsers> GetByCategoryId(int categoryId)
        //{
        //    var u =
        //        from users in context.AspNetUsers
        //        join userInCategory in context.UserInCategory on users.Id equals userInCategory.UserIId
        //        select users;

        //    return u;
        //}

    }
}
