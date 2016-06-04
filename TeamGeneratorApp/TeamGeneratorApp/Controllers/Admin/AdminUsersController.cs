using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.SqlServer.Server;
using PagedList;
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.Models;
using TeamGeneratorApp.Models.ViewModels;

namespace TeamGeneratorApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();


        public ActionResult Index()
        {
            return View();
        }

        //// GET: AdminUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = unitOfWork.UserRepository.GetByID(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var result = new AdminUserEditVM();
            result.Id = user.Id;
            result.Name = user.Name;
            result.UserName = user.UserName;
            result.Email = user.Email;
            result.ImageUrl = user.ImageUrl;

            if (user.AspNetRoles.Count > 0)
                result.IsAdmin = true;
            else
                result.IsAdmin = false;

            ViewBag.ImageUrl = result.ImageUrl;

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,UserName,IsAdmin,Name")] AdminUserEditVM userVm, HttpPostedFileBase file)
        {
            //try
            //{
                if (ModelState.IsValid)
                {
                    var serverPath = "";
                    userVm.ImageUrl = ViewBag.ImageUrl;

                    var user = unitOfWork.UserRepository.GetByID(userVm.Id);
                        user.Email = userVm.Email;
                        user.UserName = userVm.UserName;
                        user.Name = userVm.Name;

                    if (file != null && file.ContentLength > 0)
                    {
                        var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
                        var ext = Path.GetExtension(file.FileName);
                        if (allowedExtensions.Contains(ext)) //check what type of extension  
                        {
                            var fileName = Guid.NewGuid() + ext;
                            serverPath = Path.Combine(Server.MapPath("~/Img/"), fileName);
                            user.ImageUrl = "http://localhost:2422" + "/Img/" + fileName;
                        }
                    }

                    if (userVm.IsAdmin)
                        {
                            if (user.AspNetRoles.Count == 0)
                            {
                                var role = unitOfWork.RoleRepository.FindByName("Admin");
                                user.AspNetRoles.Add(role);
                            }
                        }
                        else
                        {
                            user.AspNetRoles.Clear();
                        }

                        unitOfWork.UserRepository.Update(user);
                        unitOfWork.Commit();
                        
                        if(serverPath != "")
                            file.SaveAs(serverPath); 
                        
                        return RedirectToAction("Index");
                }
            //}
            //catch (Exception)
            //{
            //    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            //}

            return View(userVm);
        }


        public ActionResult AdminUsersGrid_Read([DataSourceRequest] DataSourceRequest request)
        {
            var res = unitOfWork.UserRepository.Get().ToList();

            var list = new List<AdminUserIndexVM>();
            foreach (var e in res)
            {
                var newVm = new AdminUserIndexVM();

                newVm.Id = e.Id;
                newVm.Name = e.Name;
                newVm.Email = e.Email;
                newVm.UserName = e.UserName;
                newVm.Image = e.ImageUrl;
                
                if (e.AspNetRoles.Count > 0)
                    newVm.IsAdmin = true;
                else
                    newVm.IsAdmin = false;

                list.Add(newVm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }



        #region OldCode


        // GET: AdminUsers
        //public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string ddlFilter = "Username")
        //{
        //    ViewBag.CurrentSort = sortOrder;
        //    ViewBag.UsernameSortParm = String.IsNullOrEmpty(sortOrder) ? "Username_desc" : "";
        //    ViewBag.EmailSortParm = sortOrder == "Email" ? "Email_desc" : "Email";

        //    var columnList = new List<string>();
        //    columnList.Add("Username");
        //    columnList.Add("Email");

        //    ViewBag.ddlFilter = new SelectList(columnList, ddlFilter);


        //    if (searchString != null)
        //    {
        //        page = 1;
        //    }
        //    else
        //    {
        //        searchString = currentFilter;
        //    }

        //    ViewBag.CurrentFilter = searchString;

        //    var userRepo = unitOfWork.UserRepository.GetAll();

        //    var users = new List<AdminUserIndexVM>();

        //    foreach (var item in userRepo)
        //    {
        //        AdminUserIndexVM user = new AdminUserIndexVM();
        //        user.Id = item.Id;
        //        user.UserName = item.UserName;
        //        user.Email = item.Email;
        //        user.Name = item.Name;

        //        if (item.AspNetRoles.Count > 0)
        //            user.IsAdmin = true;
        //        else
        //            user.IsAdmin = false;

        //        users.Add(user);
        //    }



        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        switch (ddlFilter)
        //        {
        //            case "Username":
        //                users = users.Where(s => s.UserName != null);
        //                users = users.Where(s => s.UserName.Contains(searchString));
        //                break;
        //            case "Email":
        //                users = users.Where(s => s.Email != null);
        //                users = users.Where(s => s.Email.Contains(searchString));
        //                break;
        //            //case "Role":
        //            //    users = users.Where(s => s.AspNetRoles.FirstOrDefault() != null);
        //            //    users = users.Where(s => s.AspNetRoles.FirstOrDefault().Name.Contains(searchString));
        //            //    break;
        //            default:
        //                users = users.Where(s => s.UserName != null);
        //                users = users.Where(s => s.UserName.Contains(searchString));
        //                break;
        //        }
        //    }


        //    switch (sortOrder)
        //    {
        //        case "Username_desc":
        //            users = users.OrderByDescending(s => s.UserName);
        //            break;
        //        case "Email":
        //            users = users.OrderBy(s => s.Email);
        //            break;
        //        case "Email_desc":
        //            users = users.OrderByDescending(s => s.Email);
        //            break;
        //        //case "Role":
        //        //    users = users.OrderBy(s => s.AspNetRoles.FirstOrDefault().Name);
        //        //    break;
        //        //case "Role_desc":
        //        //    users = users.OrderByDescending(s => s.AspNetRoles.FirstOrDefault().Name);
        //        //    break;
        //        default:
        //            users = users.OrderBy(s => s.UserName);
        //            break;
        //    }

        //    int pageSize = 10;
        //    int pageNumber = (page ?? 1);

        //    return View(users.ToPagedList(pageNumber, pageSize));
        //}

        // GET: AdminUsers/Details/5
        //public ActionResult Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AspNetUsers aspNetUsers = unitOfWork.UserRepository.GetByID(id);

        //    if (aspNetUsers == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(aspNetUsers);
        //}

        //// GET: AdminUsers/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: AdminUsers/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Email,UserName,IsAdmin")] AdminUserCreateVM user)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            unitOfWork.UserRepository.InsertUser(user);
        //            unitOfWork.Commit();
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
        //    }


        //    return View(user);
        //}

        //// GET: AdminUsers/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AdminUserCreateVM user = unitOfWork.UserRepository.GetByIDForEdit(id);

        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(user);
        //}

        //// POST: AdminUsers/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Email,UserName,IsAdmin")] AdminUserCreateVM user)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            unitOfWork.UserRepository.Update(user);
        //            unitOfWork.Commit();
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
        //    }

        //    return View(user);
        //}

        //// GET: AdminUsers/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AdminUserCreateVM aspNetUsers = unitOfWork.UserRepository.GetByIDForEdit(id);
        //    if (aspNetUsers == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(aspNetUsers);
        //}

        //// POST: AdminUsers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    AdminUserCreateVM user = unitOfWork.UserRepository.GetByIDForEdit(id);

        //    try
        //    {
        //        unitOfWork.UserRepository.Delete(user);
        //        unitOfWork.Commit();
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.ConstraintError = "Unable to delete this item because it is used in other entities as foreign key.";
        //        return View("Delete", user);
        //    }
        //}


        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
