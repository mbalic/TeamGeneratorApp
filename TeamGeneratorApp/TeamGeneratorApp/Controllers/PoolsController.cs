using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.Controllers
{
    [Authorize]
    public class PoolsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private string userId;
        
        // GET: Pools
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string ddlFilter = "Name")
        {

            userId = User.Identity.GetUserId();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.CategorySortParm = sortOrder == "Category" ? "Category_desc" : "Category";

            var columnList = new List<string>();
            columnList.Add("Name");
            columnList.Add("Category");

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

            var pools = unitOfWork.PoolRepository.GetByUserId(userId);

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
                default:
                    pools = pools.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(pools.ToPagedList(pageNumber, pageSize));
        }

        // GET: Pools/Details/5
        public ActionResult Details(int? id)
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

        //// GET: Pools/Create
        public ActionResult Create()
        {
            userId = User.Identity.GetUserId();

            ViewBag.UserId = userId;
            PopulateCategoryDropDown();
            return View();
        }

        //// POST: Pools/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,UserId,CategoryId")] Pool pool)
        {
            userId = User.Identity.GetUserId();

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

            ViewBag.UserId = userId;
            PopulateCategoryDropDown(pool.CategoryId);

            return View(pool);
        }

        //// GET: Pools/Edit/5
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

            userId = User.Identity.GetUserId();
            ViewBag.UserId = userId;
            PopulateCategoryDropDown(pool.UserId);

            return View(pool);
        }

        //// POST: Pools/Edit/5
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

            userId = User.Identity.GetUserId();
            ViewBag.UserId = userId;
            PopulateCategoryDropDown(pool.CategoryId);

            return View(pool);
        }

        //// GET: Pools/Delete/5
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

        //// POST: Pools/Delete/5
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

        private void PopulateCategoryDropDown(object selectedItem = null)
        {
            var items = unitOfWork.CategoryRepository.Get().ToList();
            ViewBag.CategoryId = new SelectList(items, "Id", "Name", selectedItem);
        }

        public PartialViewResult PoolEvents(int poolId = 0)
        {
            var events = unitOfWork.EventRepository.GetByPoolId(poolId);

            return PartialView("_PoolEvents", events.ToList());
        }

        public PartialViewResult PoolUsers(int poolId = 0)
        {
            var users = unitOfWork.UserRepository.GetFromPool(poolId);

            return PartialView("_PoolUsers", users.ToList());
        }

        //public ActionResult RedirectToDetails(int id)
        //{
        //   return RedirectToAction("Details", "Subjects", new {id = id});
        //}
    }
}
