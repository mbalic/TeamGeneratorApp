using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using PagedList;
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.Models;
using TeamGeneratorApp.Models.ViewModels;

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

            var columnList = new List<string>();
            columnList.Add("Name");
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

        //GET: AdminPools/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Pool pool = unitOfWork.PoolRepository.GetByID(id);

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
            return View();
        }

        //// POST: AdminPools/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,OwnerId")] Pool pool)
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

            PopulateOwnerDropDown(pool.OwnerId);

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

            PopulateOwnerDropDown(pool.OwnerId);

            return View(pool);
        }

        //// POST: AdminPools/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,OwnerId")] Pool pool)
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

            PopulateOwnerDropDown(pool.OwnerId);

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
            ViewBag.OwnerId = new SelectList(items, "Id", "Username", selectedItem);
        }

        public PartialViewResult CategoriesGrid(int poolId = 0)
        {

            ViewBag.PoolId = poolId;
            return PartialView("_CategoriesGrid");
        }

        public ActionResult CategoriesGrid_Read([DataSourceRequest] DataSourceRequest request, int poolId)
        {
            var res = unitOfWork.CategoryRepository.GetByPoolId(poolId).ToList();

            var list = new List<CategoryVM>();
            foreach (var e in res)
            {
                var categoryVm = new CategoryVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    PoolId = e.PoolId,
                    Description = e.Description,
                };
                list.Add(categoryVm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CategoriesGrid_Create([DataSourceRequest] DataSourceRequest request,
           [Bind(Prefix = "models")]IEnumerable<CategoryVM> list, int poolId)
        {
            var results = new List<CategoryVM>();
            if (list != null && ModelState.IsValid)
            {
                foreach (var e in list)
                {
                    var newCategory = new Category
                    {
                        Name = e.Name,
                        PoolId = poolId,
                        Description = e.Description
                    };
                    try
                    {
                        unitOfWork.CategoryRepository.Insert(newCategory);
                        unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        ViewBag.ConstraintError = "There was an error while adding rows in grid.";
                    }
                    results.Add(e);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CategoriesGrid_Update([DataSourceRequest] DataSourceRequest request,
           [Bind(Prefix = "models")]IEnumerable<CategoryVM> list)
        {
            var results = new List<CategoryVM>();
            if (list != null && ModelState.IsValid)
            {
                foreach (var e in list)
                {
                    var newCategory = new Category
                    {
                        Id = e.Id,
                        PoolId = e.PoolId,
                        Name = e.Name,
                        Description = e.Description
                    };
                    try
                    {
                        unitOfWork.CategoryRepository.Update(newCategory);
                        unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        ViewBag.ConstraintError = "There was an error while updating rows in grid.";
                    }
                    results.Add(e);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CategoriesGrid_Destroy([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<CategoryVM> list)
        {
            var results = new List<CategoryVM>();
            if (list != null && ModelState.IsValid)
            {
                foreach (var e in list)
                {
                    var newCategory = new Category
                    {
                        Id = e.Id,
                        PoolId = e.PoolId,
                        Name = e.Name,
                        Description = e.Description                       
                    };
                    try
                    {
                        unitOfWork.CategoryRepository.Delete(newCategory.Id);
                        unitOfWork.Commit();
                        results.Add(e);
                    }
                    catch (Exception)
                    {
                        ViewBag.ConstraintError = "There was an error while deleting rows in grid.";
                    }
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
    }
}
