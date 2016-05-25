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

namespace TeamGeneratorApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminPoolsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: AdminPools
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string ddlFilter = "Name")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.OwnerSortParm = sortOrder == "Owner" ? "Owner_desc" : "Owner";
            ViewBag.CategorySortParm = sortOrder == "Category" ? "Category_desc" : "Category";

            var columnList = new List<string>();
            columnList.Add("Name");
            columnList.Add("Category");
            columnList.Add("Owner");            

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

            var pools = unitOfWork.PoolRepository.Get();

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (ddlFilter)
                {
                    case "Name":
                        pools = pools.Where(s => s.Name != null);
                        pools = pools.Where(s => s.Name.Contains(searchString));
                        break;
                    case "Category":
                        pools = pools.Where(s => s.Category != null);
                        pools = pools.Where(s => s.Category.Name.Contains(searchString));
                        break;
                    case "Owner":
                        pools = pools.Where(s => s.AspNetUsers != null);
                        pools = pools.Where(s => s.AspNetUsers.UserName.Contains(searchString));
                        break;
                    default:
                        pools = pools.Where(s => s.Name != null);
                        pools = pools.Where(s => s.Name.Contains(searchString));
                        break;
                }
            }


            switch (sortOrder)
            {
                case "Name_desc":
                    pools = pools.OrderByDescending(s => s.Name);
                    break;
                case "Category":
                    pools = pools.OrderBy(s => s.Category.Name);
                    break;
                case "Category_desc":
                    pools = pools.OrderByDescending(s => s.Category.Name);
                    break;
                case "Owner":
                    pools = pools.OrderBy(s => s.AspNetUsers.UserName);
                    break;
                case "Owner_desc":
                    pools = pools.OrderByDescending(s => s.AspNetUsers.UserName);
                    break;
                default:
                    pools = pools.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(pools.ToPagedList(pageNumber, pageSize));
        }

        // GET: AdminPools/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Pool pool = db.Pool.Find(id);
        //    if (pool == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(pool);
        //}

        //// GET: AdminPools/Create
        public ActionResult Create()
        {
            PopulateOwnerDropDown();
            PopulateCategoryDropDown();
            return View();
        }

        //// POST: AdminPools/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,UserId,CategoryId")] Pool pool)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.PoolRepository.Insert(pool);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            PopulateOwnerDropDown(pool.UserId);
            PopulateCategoryDropDown(pool.CategoryId);

            return View(pool);
        }

        //// GET: AdminPools/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Pool pool = unitOfWork.PoolRepository.GetByID(id);

            if (pool == null)
            {
                return HttpNotFound();
            }

            PopulateOwnerDropDown(pool.UserId);
            PopulateCategoryDropDown(pool.CategoryId);

            return View(pool);
        }

        //// POST: AdminPools/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,UserId,CategoryId")] Pool pool)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.PoolRepository.Update(pool);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            PopulateOwnerDropDown(pool.UserId);
            PopulateCategoryDropDown(pool.CategoryId);

            return View(pool);
        }

        //// GET: AdminPools/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pool pool = unitOfWork.PoolRepository.GetByID(id);

            if (pool == null)
            {
                return HttpNotFound();
            }
            return View(pool);
        }

        //// POST: AdminPools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pool pool = unitOfWork.PoolRepository.GetByID(id);

            try
            {
                unitOfWork.PoolRepository.Delete(pool);
                unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.ConstraintError = "Unable to delete this item because it is used in other entities as foreign key.";
                return View("Delete", pool);
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

        private void PopulateOwnerDropDown(object selectedItem = null)
        {
            var items = unitOfWork.UserRepository.Get().ToList();
            ViewBag.UserId = new SelectList(items, "Id", "Username", selectedItem);
        }

        private void PopulateCategoryDropDown(object selectedItem = null)
        {
            var items = unitOfWork.CategoryRepository.Get().ToList();
            ViewBag.CategoryId = new SelectList(items, "Id", "Name", selectedItem);
        }
    }
}
