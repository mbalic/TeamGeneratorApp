using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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

        // GET: AdminUsers
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string ddlFilter = "Username")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.UsernameSortParm = String.IsNullOrEmpty(sortOrder) ? "Username_desc" : "";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "Email_desc" : "Email";

            var columnList = new List<string>();
            columnList.Add("Username");
            columnList.Add("Email");

            ViewBag.ddlFilter = new SelectList(columnList, ddlFilter);


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var users = unitOfWork.UserRepository.GetAllForIndex();

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (ddlFilter)
                {
                    case "Username":
                        users = users.Where(s => s.UserName != null);
                        users = users.Where(s => s.UserName.Contains(searchString));
                        break;
                    case "Email":
                        users = users.Where(s => s.Email != null);
                        users = users.Where(s => s.Email.Contains(searchString));
                        break;
                    //case "Role":
                    //    users = users.Where(s => s.AspNetRoles.FirstOrDefault() != null);
                    //    users = users.Where(s => s.AspNetRoles.FirstOrDefault().Name.Contains(searchString));
                    //    break;
                    default:
                        users = users.Where(s => s.UserName != null);
                        users = users.Where(s => s.UserName.Contains(searchString));
                        break;
                }
            }


            switch (sortOrder)
            {
                case "Username_desc":
                    users = users.OrderByDescending(s => s.UserName);
                    break;
                case "Email":
                    users = users.OrderBy(s => s.Email);
                    break;
                case "Email_desc":
                    users = users.OrderByDescending(s => s.Email);
                    break;
                //case "Role":
                //    users = users.OrderBy(s => s.AspNetRoles.FirstOrDefault().Name);
                //    break;
                //case "Role_desc":
                //    users = users.OrderByDescending(s => s.AspNetRoles.FirstOrDefault().Name);
                //    break;
                default:
                    users = users.OrderBy(s => s.UserName);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(users.ToPagedList(pageNumber, pageSize));
        }

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
        public ActionResult Create()
        {
            return View();
        }

        //// POST: AdminUsers/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Email,UserName,IsAdmin")] AdminUserCreateVM user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.UserRepository.InsertUser(user);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }


            return View(user);
        }

        //// GET: AdminUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUserCreateVM user = unitOfWork.UserRepository.GetByIDForEdit(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        //// POST: AdminUsers/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Email,UserName,IsAdmin")] AdminUserCreateVM user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.UserRepository.Update(user);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(user);
        }

        //// GET: AdminUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUserCreateVM aspNetUsers = unitOfWork.UserRepository.GetByIDForEdit(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        //// POST: AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AdminUserCreateVM user = unitOfWork.UserRepository.GetByIDForEdit(id);

            try
            {
                unitOfWork.UserRepository.Delete(user);
                unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.ConstraintError = "Unable to delete this item because it is used in other entities as foreign key.";
                return View("Delete", user);
            }
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
