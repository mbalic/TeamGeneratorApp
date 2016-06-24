using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.Models;
using TeamGeneratorApp.Models.ViewModels;

namespace TeamGeneratorApp.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();


        //GET: Users/Profile
        public ActionResult Profile(string id)
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

            var result = new UserEditVM();
            result.Id = user.Id;
            result.Name = user.Name;
            result.UserName = user.UserName;
            result.Email = user.Email;
            result.ImageUrl = user.ImageUrl;

            ViewBag.ImageUrl = result.ImageUrl;

            if (id == User.Identity.GetUserId())
                ViewBag.Owner = true;
            else
                ViewBag.Owner = false; 
            return View(result);
        }


        public ActionResult EditProfile(string message)
        {
            var id = User.Identity.GetUserId();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = unitOfWork.UserRepository.GetByID(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var result = new UserEditVM();
            result.Id = user.Id;
            result.Name = user.Name;
            result.UserName = user.UserName;
            result.Email = user.Email;
            result.ImageUrl = user.ImageUrl;

            ViewBag.StatusMessage = message;
            ViewBag.ImageUrl = result.ImageUrl;
            return View(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Include = "Id,Email,Username, Name")] UserEditVM userVm, HttpPostedFileBase file)
        {

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

                unitOfWork.UserRepository.Update(user);
                unitOfWork.Commit();

                if (serverPath != "")
                    file.SaveAs(serverPath);

                return RedirectToAction("Profile");
            }
           

            ViewBag.Message = "Error";
            return View(userVm);
        }





        public ActionResult GroupsMemberGrid(string userId)
        {
            ViewBag.UserId = userId;
            return PartialView("_GroupsMemberGrid");
        }

        public ActionResult GroupsMemberGrid_Read([DataSourceRequest] DataSourceRequest request, string userId)
        {
            var res = unitOfWork.GroupRepository.GetByUserId(userId).ToList();

            var list = new List<GroupListVM>();
            foreach (var e in res)
            {
                var newVm = new GroupListVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    OwnerName = e.AspNetUsers.Name
                };

                list.Add(newVm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GroupsOwnerGrid(string userId)
        {
            ViewBag.UserId = userId;
            return PartialView("_GroupsOwnerGrid");
        }

        public ActionResult GroupsOwnerGrid_Read([DataSourceRequest] DataSourceRequest request, string userId)
        {
            var res = unitOfWork.GroupRepository.GetByOwnerId(userId).ToList();

            var list = new List<GroupListVM>();
            foreach (var e in res)
            {
                var newVm = new GroupListVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description
                };

                list.Add(newVm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


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