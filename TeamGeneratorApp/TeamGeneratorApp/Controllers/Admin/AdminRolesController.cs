using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminRolesController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: AspNetRoles
        public ActionResult Index(string sortOrder)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";

            var roles = unitOfWork.RoleRepository.Get();

            switch (sortOrder)
            {
                case "Name_desc":
                    roles = roles.OrderByDescending(s => s.Name);
                    break;
                default:
                    roles = roles.OrderBy(s => s.Name);
                    break;
            }

            return View(roles.ToList());
        }
        

        // GET: AspNetRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AspNetRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] AspNetRoles aspNetRoles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.RoleRepository.Insert(aspNetRoles);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(aspNetRoles);
        }

        // GET: AspNetRoles/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRoles aspNetRoles = unitOfWork.RoleRepository.GetByID(id);

            if (aspNetRoles == null)
            {
                return HttpNotFound();
            }

            return View(aspNetRoles);
        }

        // POST: AspNetRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] AspNetRoles aspNetRoles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.RoleRepository.Update(aspNetRoles);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(aspNetRoles);
        }

        // GET: AspNetRoles/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AspNetRoles aspNetRoles = unitOfWork.RoleRepository.GetByID(id);

            if (aspNetRoles == null)
            {
                return HttpNotFound();
            }

            return View(aspNetRoles);
        }

        // POST: AspNetRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetRoles aspNetRoles = unitOfWork.RoleRepository.GetByID(id);

            try
            {
                unitOfWork.RoleRepository.Delete(aspNetRoles);
                unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.ConstraintError = "Unable to delete this item because it is used in other entities as foreign key.";
                return View("Delete", aspNetRoles);
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
