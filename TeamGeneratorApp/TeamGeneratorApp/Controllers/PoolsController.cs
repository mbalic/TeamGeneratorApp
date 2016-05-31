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
using Microsoft.AspNet.Identity;
using PagedList;
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.Models;
using TeamGeneratorApp.Models.ViewModels;

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

            var columnList = new List<string>();
            columnList.Add("Name");

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
            return View();
        }

        //// POST: Pools/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,OwnerId")] Pool pool)
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

            return View(pool);
        }

        //// POST: Pools/Edit/5
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

            userId = User.Identity.GetUserId();
            ViewBag.UserId = userId;

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


        public PartialViewResult PoolEvents(int poolId = 0)
        {
            //var events = unitOfWork.EventRepository.GetByPoolId(poolId);
            ViewBag.PoolId = poolId;

            return PartialView("_PoolEvents");
        }

        public PartialViewResult PoolUsers(int poolId = 0)
        {         
            ViewBag.PoolId = poolId;

            return PartialView("_PoolUsers");
        }


        //public ActionResult PoolUsers_Read([DataSourceRequest] DataSourceRequest request, int poolId)
        //{
        //    var res = unitOfWork.UserRepository.GetFromPool(poolId).ToList();

        //    var list = new List<UserInPoolVM>();
        //    foreach (var e in res)
        //    {
        //        var usersVm = new UserInPoolVM
        //        {
        //            Id = e.Id,
        //            UserId = e.UserId,
        //            PoolId = e.PoolId,
        //            Weight = e.Weight,
        //            //User = unitOfWork.UserRepository.GetByID(e.UserId)
        //        };
        //        list.Add(usersVm);
        //    }

        //    return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}
    


        ////public ActionResult RedirectToDetails(int id)
        ////{
        ////   return RedirectToAction("Details", "Subjects", new {id = id});
        ////}

        //#region PoolEvents

        //public ActionResult PoolEvents_Read([DataSourceRequest] DataSourceRequest request, int poolId)
        //{
        //    var res = unitOfWork.EventRepository.GetByPoolId(poolId).ToList();

        //    var list = new List<EventVM>();
        //    foreach (var e in res)
        //    {
        //        var eventVm = new EventVM
        //        {
        //            Id = e.Id,
        //            Name = e.Name,
        //            Description = e.Description,
        //            Fullname = e.Fullname,
        //            Start = e.Start,
        //            Finish = e.Finish,
        //            NumberOfTeams = e.NumberOfTeams,
        //            PoolId = e.PoolId
        //        };
        //        list.Add(eventVm);
        //    }
            
        //    return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult PoolEvents_Create([DataSourceRequest] DataSourceRequest request,
        //   [Bind(Prefix = "models")]IEnumerable<EventVM> list, int poolId)
        //{
        //    var results = new List<EventVM>();
        //    if (list != null && ModelState.IsValid)
        //    {
        //        foreach (var e in list)
        //        {
        //            var newEvent = new Event
        //            {
        //                PoolId = poolId,
        //                Name = e.Name,
        //                Fullname = e.Fullname,
        //                Description = e.Description,
        //                Start = e.Start,
        //                Finish = e.Finish,
        //                NumberOfTeams = e.NumberOfTeams
        //            };
        //            try
        //            {
        //                unitOfWork.EventRepository.Insert(newEvent);
        //                unitOfWork.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                ViewBag.ConstraintError = "There was an error while adding rows in grid.";
        //            }
        //            results.Add(e);
        //        }
        //    }

        //    return Json(results.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult PoolEvents_Update([DataSourceRequest] DataSourceRequest request,
        //   [Bind(Prefix = "models")]IEnumerable<EventVM> list)
        //{
        //    var results = new List<EventVM>();
        //    if (list != null && ModelState.IsValid)
        //    {
        //        foreach (var e in list)
        //        {
        //            var newEvent = new Event
        //            {
        //                Id = e.Id,
        //                PoolId = e.PoolId,
        //                Name = e.Name,
        //                Fullname = e.Fullname,
        //                Description = e.Description,
        //                Start = e.Start,
        //                Finish = e.Finish,
        //                NumberOfTeams = e.NumberOfTeams
        //            };
        //            try
        //            {
        //                unitOfWork.EventRepository.Update(newEvent);
        //                unitOfWork.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                ViewBag.ConstraintError = "There was an error while updating rows in grid.";
        //            }
        //            results.Add(e);
        //        }
        //    }

        //    return Json(results.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult PoolEvents_Destroy([DataSourceRequest] DataSourceRequest request,
        //  [Bind(Prefix = "models")]IEnumerable<EventVM> list)
        //{
        //    var results = new List<EventVM>();
        //    if (list != null && ModelState.IsValid)
        //    {
        //        foreach (var e in list)
        //        {
        //            var newEvent = new Event
        //            {
        //                Id = e.Id,
        //                PoolId = e.PoolId,
        //                Name = e.Name,
        //                Fullname = e.Fullname,
        //                Description = e.Description,
        //                Start = e.Start,
        //                Finish = e.Finish,
        //                NumberOfTeams = e.NumberOfTeams
        //            };
        //            try
        //            {
        //                unitOfWork.EventRepository.Delete(newEvent.Id);
        //                unitOfWork.Commit();
        //                results.Add(e);
        //            }
        //            catch (Exception)
        //            {
        //                ViewBag.ConstraintError = "There was an error while deleting rows in grid.";
        //            }
        //        }
        //    }

        //    return Json(results.ToDataSourceResult(request, ModelState));
        //}

//#endregion
    }
}
