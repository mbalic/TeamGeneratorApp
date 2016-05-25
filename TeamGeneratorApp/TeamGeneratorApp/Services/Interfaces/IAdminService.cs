using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.Services.Interfaces
{
    public interface IAdminService
    {
        //users
        IEnumerable<AspNetUsers> GetAllUsers();

        AspNetUsers GetUserById(string id);

        string CreateUser(AspNetUsers data);

        void UpdateUser(AspNetUsers data);

        void DeleteUser(string id);


        //roles
        IEnumerable<AspNetRoles> GetAllRoles();

        AspNetRoles GetRoleById(string id);

        string CreateRole(AspNetRoles data);

        void UpdateRole(AspNetRoles data);

        void DeleteRole(string id);


        //categories
        IEnumerable<Category> GetAllCategories();

        Category GetCategoryById(int id);

        int CreateCategory(Category data);

        void UpdateCategoryDetails(Category data);

        void DeleteCategory(int id);
    }
}
