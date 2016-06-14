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
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.Models;
using PagedList;
using TeamGeneratorApp.Models.ViewModels;


namespace TeamGeneratorApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminCategoriesController : Controller
    {
        
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: AdminCategories
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string ddlFilter = "Name")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var category = unitOfWork.CategoryRepository.Get();

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (ddlFilter)
                {
                    case "Name":
                        category = category.Where(s => s.Name != null);
                        category = category.Where(s => s.Name.Contains(searchString));
                        break;
                    default:
                        category = category.Where(s => s.Name != null);
                        category = category.Where(s => s.Name.Contains(searchString));
                        break;
                }
            }


            switch (sortOrder)
            {
                case "Name_desc":
                    category = category.OrderByDescending(s => s.Name);
                    break;
                default:
                    category = category.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(category.ToPagedList(pageNumber, pageSize));
        }

        // GET: AdminCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = unitOfWork.CategoryRepository.GetByID(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //// GET: AdminCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        //// POST: AdminCategories/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.CategoryRepository.Insert(category);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(category);
        }

        //// GET: AdminCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = unitOfWork.CategoryRepository.GetByID(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        //// POST: AdminCategories/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.CategoryRepository.Update(category);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(category);
        }

        //// GET: AdminCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = unitOfWork.CategoryRepository.GetByID(id);

            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //// POST: AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = unitOfWork.CategoryRepository.GetByID(id);

            try
            {
                unitOfWork.CategoryRepository.Delete(category);
                unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.ConstraintError = "Unable to delete this item because it is used in other entities as foreign key.";
                return View("Delete", category);
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

        public PartialViewResult EventsGrid(int categoryId = 0)
        {

            ViewBag.CategoryId = categoryId;
            return PartialView("_EventsGrid");
        }

        public PartialViewResult UsersGrid(int categoryId = 0)
        {

            ViewBag.CategoryId = categoryId;
            return PartialView("_UsersGrid");
        }

        #region EventsGrid


        public ActionResult EventsGrid_Read([DataSourceRequest] DataSourceRequest request, int categoryId)
        {
            var res = unitOfWork.EventRepository.GetByCategoryId(categoryId).ToList();

            var list = new List<EventVM>();
            foreach (var e in res)
            {
                var eventVm = new EventVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Start = e.Start,
                    Finish = e.Finish,
                    CategoryId = e.CategoryId
                };
                list.Add(eventVm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EventsGrid_Create([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<EventVM> res)
        {
            var list = new List<EventVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    var newEvent = new Event
                    {
                        Name = e.Name,
                        Description = e.Description,
                        Start = e.Start,
                        Finish = e.Finish,
                        CategoryId = e.CategoryId
                    };
                    try
                    {
                        unitOfWork.EventRepository.Insert(newEvent);
                        unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        ViewBag.ConstraintError = "There was an error while adding rows in grid.";
                    }
                    list.Add(e);
                }
            }

            return Json(list.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EventsGrid_Update([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<EventVM> res)
        {
            var list = new List<EventVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    var newEvent = new Event
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Description = e.Description,
                        Start = e.Start,
                        Finish = e.Finish,
                        CategoryId = e.CategoryId
                    };
                    try
                    {
                        unitOfWork.EventRepository.Update(newEvent);
                        unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        ViewBag.ConstraintError = "There was an error while updating rows in grid.";
                    }
                    list.Add(e);
                }
            }

            return Json(list.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EventsGrid_Destroy([DataSourceRequest] DataSourceRequest request,
         [Bind(Prefix = "models")]IEnumerable<EventVM> res)
        {
            var list = new List<EventVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    var newEvent = new Event
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Description = e.Description,
                        Start = e.Start,
                        Finish = e.Finish,
                        CategoryId = e.CategoryId
                    };
                    try
                    {
                        unitOfWork.EventRepository.Delete(newEvent.Id);
                        unitOfWork.Commit();
                        list.Add(e);
                    }
                    catch (Exception)
                    {
                        ViewBag.ConstraintError = "There was an error while deleting rows in grid.";
                    }
                }
            }

            return Json(list.ToDataSourceResult(request, ModelState));
        }
        #endregion


        public ActionResult UsersGrid_Read([DataSourceRequest] DataSourceRequest request, int categoryId)
        {
            var res = unitOfWork.UserInCategoryRepository.GetByCategoryId(categoryId).ToList();

            var list = new List<UserCategoryVM>();
            //foreach (var e in res)
            //{
            //    var newItem = new UserCategoryVM
            //    {
            //        Id = e.Id,
            //        UserId = e.UserId,
            //        CategoryId = e.CategoryId,
            //        Username = e.AspNetUsers.UserName,
            //        Score = e.Score

            //    };
            //    list.Add(newItem);
            //}

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}
