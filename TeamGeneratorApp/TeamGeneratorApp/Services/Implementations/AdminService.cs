using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Services.Interfaces;
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.DAL.Repositories;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.Services.Implementations
{
    //public class AdminService : IAdminService
    //{
    //    private IUnitOfWork _unitOfWork;
    //    private IUserRepository _userRepository;
    //    private IRoleRepository _roleRepository;
    //    private ICategoryRepository _categoryRepository;

    //    public AdminService(IUnitOfWork unitOfWork, IUserRepository userRepository, IRoleRepository roleRepository,
    //        ICategoryRepository categoryRepository)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _userRepository = userRepository;
    //        _roleRepository = roleRepository;
    //        _categoryRepository = categoryRepository;
    //    }


    //    public IEnumerable<AspNetUsers> GetAllUsers()
    //    {
    //        return _userRepository.GetAll();
    //    }


    //    public AspNetUsers GetUserById(string id)
    //    {
    //        return _userRepository.Get(id);
    //    }

    //    public string CreateUser(AspNetUsers data)
    //    {
    //        data.Id = Guid.NewGuid().ToString();
    //        _userRepository.Add(data);
    //        _unitOfWork.Commit();
    //        return data.Id;
    //    }

    //    public void UpdateUser(AspNetUsers data)
    //    {
    //        _userRepository.Update(data);
    //        _unitOfWork.Commit();
    //    }

    //    public void DeleteUser(string id)
    //    {
    //        _userRepository.Remove(id);
    //        _unitOfWork.Commit();
    //    }



    //    public IEnumerable<AspNetRoles> GetAllRoles()
    //    {
    //        return _roleRepository.GetAll();
    //    }

    //    public AspNetRoles GetRoleById(string id)
    //    {
    //        return _roleRepository.Get(id);
    //    }

    //    public string CreateRole(AspNetRoles data)
    //    {
    //        data.Id = Guid.NewGuid().ToString();
    //        _roleRepository.Add(data);
    //        _unitOfWork.Commit();
    //        return data.Id;
    //    }

    //    public void UpdateRole(AspNetRoles data)
    //    {
    //        _roleRepository.Update(data);
    //        _unitOfWork.Commit();
    //    }

    //    public void DeleteRole(string id)
    //    {
    //        _roleRepository.Remove(id);
    //        _unitOfWork.Commit();
    //    }



    //    public IEnumerable<Category> GetAllCategories()
    //    {
    //        return _categoryRepository.GetAll();
    //    }

    //    public Category GetCategoryById(int id)
    //    {
    //        return _categoryRepository.Get(id);
    //    }

    //    public int CreateCategory(Category data)
    //    {
    //        _categoryRepository.Add(data);
    //        _unitOfWork.Commit();
    //        return data.Id;
    //    }

    //    public void UpdateCategoryDetails(Category data)
    //    {
    //        _categoryRepository.Update(data);
    //        _unitOfWork.Commit();
    //    }

    //    public void DeleteCategory(int id)
    //    {
    //        _categoryRepository.Remove(id);
    //        _unitOfWork.Commit();
    //    }
    //}
}
