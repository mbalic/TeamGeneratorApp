﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public AdminUserCreateVM GetByIDForEdit(string id)
        {
            AspNetUsers user = GetByID(id);

            AdminUserCreateVM result = new AdminUserCreateVM();
            result.Id = user.Id;
            result.UserName = user.UserName;
            result.Email = user.Email;
            if (user.AspNetRoles.Count > 0)
                result.IsAdmin = true;
            else
                result.IsAdmin = false;

            return result;
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
 

    }
}
