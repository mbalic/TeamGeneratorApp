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
    public class EventsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private string userId;

        // GET: Events
        //    public ActionResult Index(int poolId, string sortOrder, string currentFilter, string searchString, int? page)
        //    {
        //        ViewBag.CurrentSort = sortOrder;
        //        ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
        //        ViewBag.StartSortParm = sortOrder == "Start" ? "Start_desc" : "Start";
        //        ViewBag.FinishSortParm = sortOrder == "Finish" ? "Finish_desc" : "Finish";
        //        ViewBag.TeamsSortParm = sortOrder == "Teams" ? "Teams_desc" : "Teams";


        //        if (searchString != null)
        //        {
        //            page = 1;
        //        }
        //        else
        //        {
        //            searchString = currentFilter;
        //        }

        //        ViewBag.CurrentFilter = searchString;

        //        var events = unitOfWork.EventRepository.GetByID(poolId);

        //        if (!String.IsNullOrEmpty(searchString))
        //        {            
        //            events = events.Where(s => s.Name != null);
        //            events = events.Where(s => s.Name.Contains(searchString));
        //        }

        //        switch (sortOrder)
        //        {
        //            case "Name_desc":
        //                events = events.OrderByDescending(s => s.Name);
        //                break;
        //            case "Start":
        //                events = events.OrderBy(s => s.Start);
        //                break;
        //            case "Start_desc":
        //                events = events.OrderByDescending(s => s.Start);
        //                break;
        //            case "Finish":
        //                events = events.OrderBy(s => s.Finish);
        //                break;
        //            case "Finish_desc":
        //                events = events.OrderByDescending(s => s.Finish);
        //                break;
        //            default:
        //                events = events.OrderBy(s => s.Name);
        //                break;
        //        }

        //        ViewBag.PoolId = poolId;
        //        int pageSize = 10;
        //        int pageNumber = (page ?? 1);

        //        return View(events.ToPagedList(pageNumber, pageSize));
        //    }

        //    // GET: Events/Details/5
        //    public ActionResult Details(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        Event @event = unitOfWork.EventRepository.GetByID(id);

        //        if (@event == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        return View(@event);
        //    }

        //    //// GET: Events/Create
        //    public ActionResult Create()
        //    {
        //        PopulatePoolDropDown();
        //        return View();
        //    }

        //    //// POST: Events/Create
        //    //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //    //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult Create([Bind(Include = "Id,Name,Fullname,Description,Start,Finish,NumberOfTeams,PoolId")] Event @event)
        //    {
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                unitOfWork.EventRepository.Insert(@event);
        //                unitOfWork.Commit();
        //                return RedirectToAction("Index");
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
        //        }

        //        //PopulatePoolDropDown(@event.PoolId);

        //        return View(@event);
        //    }

        //    //// GET: Events/Edit/5
        //    public ActionResult Edit(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        Event @event = unitOfWork.EventRepository.GetByID(id);
        //        if (@event == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        //PopulatePoolDropDown(@event.PoolId);

        //        return View(@event);
        //    }

        //    //// POST: Events/Edit/5
        //    //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //    //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult Edit([Bind(Include = "Id,Name,Fullname,Description,Start,Finish,NumberOfTeams,PoolId")] Event @event)
        //    {
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                unitOfWork.EventRepository.Update(@event);
        //                unitOfWork.Commit();
        //                return RedirectToAction("Index");
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
        //        }

        //        //PopulatePoolDropDown(@event.PoolId);

        //        return View(@event);
        //    }

        //    //// GET: Events/Delete/5
        //    public ActionResult Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }

        //        Event @event = unitOfWork.EventRepository.GetByID(id);

        //        if (@event == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(@event);
        //    }

        //    //// POST: Events/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult DeleteConfirmed(int id)
        //    {
        //        Event @event = unitOfWork.EventRepository.GetByID(id);

        //        try
        //        {
        //            unitOfWork.EventRepository.Delete(@event);
        //            unitOfWork.Commit();
        //            return RedirectToAction("Index");
        //        }
        //        catch (Exception)
        //        {
        //            ViewBag.ConstraintError = "Unable to delete this item because it is used in other entities as foreign key.";
        //            return View("Delete", @event);
        //        }
        //    }

        //    protected override void Dispose(bool disposing)
        //    {
        //        if (disposing)
        //        {
        //            unitOfWork.Dispose();
        //        }
        //        base.Dispose(disposing);
        //    }

        //    private void PopulatePoolDropDown(object selectedItem = null)
        //    {
        //        userId = User.Identity.GetUserId();
        //        var items = unitOfWork.PoolRepository.GetByUserId(userId).ToList();
        //        ViewBag.PoolId = new SelectList(items, "Id", "Name", selectedItem);
        //    }
    }
}
