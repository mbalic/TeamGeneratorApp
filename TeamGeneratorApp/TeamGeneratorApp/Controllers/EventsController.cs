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
    public class EventsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = unitOfWork.EventRepository.GetByID(id);

            if (@event == null)
            {
                return HttpNotFound();
            }

            var eventVm = new EventVM
            {
                Id = @event.Id,
                CategoryId = @event.CategoryId,
                CategoryName = @event.Category.Name,
                Description = @event.Description,
                Start = @event.Start,
                Finish = @event.Finish,
                Name = @event.Name,
                NumberOfTeams = @event.NumberOfTeams
            };

            ViewBag.CategoryId = @event.CategoryId;
            return View(eventVm);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        public PartialViewResult UsersGrid(int eventId = 0)
        {
            var @event = unitOfWork.EventRepository.GetByID(eventId);
            var votings = @event.Voting;

            if (votings.Count == 0)
            {
                ViewBag.VotingStarted = false;
            }
            else
            {
                foreach (var voting in @event.Voting)
                {
                    if (voting.StartVoting < DateTime.Now)
                        ViewBag.VotingStarted = true;
                    else
                        ViewBag.VotingStarted = false;
                }
            }

            ViewBag.EventId = eventId;
            return PartialView("_UsersGrid");
        }

        #region UsersGrid

        public ActionResult UsersGrid_Read([DataSourceRequest] DataSourceRequest request, int eventId)
        {
            var res = unitOfWork.UserOnEventRepository.GetByEventId(eventId).ToList();

            var list = new List<UserOnEventVM>();
            foreach (var e in res)
            {
                var newItem = new UserOnEventVM
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    EventId = e.EventId,
                    UserPersonalName = e.AspNetUsers.Name,
                    TeamId = e.TeamId,
                    TeamName = e.Team.Name
                };
                list.Add(newItem);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UsersGrid_Create([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<UserOnEventVM> res)
        {
            var list = new List<UserOnEventVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    var newUser = new UserOnEvent
                    {
                        Id = e.Id,
                        UserId = e.UserId,
                        EventId = e.EventId,
                    };
                    try
                    {
                        unitOfWork.UserOnEventRepository.Insert(newUser);
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
        public ActionResult UsersGrid_Update([DataSourceRequest] DataSourceRequest request,
        [Bind(Prefix = "models")]IEnumerable<UserOnEventVM> res)
        {
            //var list = new List<UserOnEventVM>();
            //if (res != null && ModelState.IsValid)
            //{
            //    foreach (var e in res)
            //    {
            //        var newUser = new UserOnEvent
            //        {
            //            Id = e.Id,
            //            UserId = e.UserId,
            //            EventId = e.EventId
            //        };
            //        try
            //        {
            //            unitOfWork.UserOnEventRepository.Update(newUser);
            //            unitOfWork.Commit();
            //        }
            //        catch (Exception)
            //        {
            //            ViewBag.ConstraintError = "There was an error while updating rows in grid.";
            //        }
            //        list.Add(e);
            //    }
            //}

            return Json(res.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UsersGrid_Destroy([DataSourceRequest] DataSourceRequest request,
         [Bind(Prefix = "models")]IEnumerable<UserOnEventVM> res)
        {
            var list = new List<UserOnEventVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                { 
                    try
                    {
                        unitOfWork.UserOnEventRepository.Delete(e.Id);
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



        public PartialViewResult VotingsGrid(int eventId = 0)
        {          

            ViewBag.EventId = eventId;
            return PartialView("_VotingsGrid");
        }

        #region VotingsGrid

        public ActionResult VotingsGrid_Read([DataSourceRequest] DataSourceRequest request, int eventId)
        {
            var res = unitOfWork.VotingRepository.GetByEventId(eventId).ToList();

            var list = new List<VotingVM>();
            foreach (var e in res)
            {
                var newItem = new VotingVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    EventId = e.EventId,
                    StartVoting = e.StartVoting,
                    FinishVoting = e.FinishVoting,
                    Active = e.Active
                };
                list.Add(newItem);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VotingsGrid_Create([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<VotingVM> res)
        {
            var list = new List<VotingVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    var newVoting = new Voting
                    {
                        Id = e.Id,
                        Name = e.Name,
                        EventId = e.EventId,
                        StartVoting = e.StartVoting,
                        FinishVoting = e.FinishVoting,
                        Active = e.Active

                    };
                    try
                    {
                        unitOfWork.VotingRepository.Insert(newVoting);
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
        public ActionResult VotingsGrid_Update([DataSourceRequest] DataSourceRequest request,
       [Bind(Prefix = "models")]IEnumerable<UserOnEventVM> res)
        {
            //var list = new List<UserOnEventVM>();
            //if (res != null && ModelState.IsValid)
            //{
            //    foreach (var e in res)
            //    {
            //        var newUser = new UserOnEvent
            //        {
            //            Id = e.Id,
            //            UserId = e.UserId,
            //            EventId = e.EventId
            //        };
            //        try
            //        {
            //            unitOfWork.UserOnEventRepository.Update(newUser);
            //            unitOfWork.Commit();
            //        }
            //        catch (Exception)
            //        {
            //            ViewBag.ConstraintError = "There was an error while updating rows in grid.";
            //        }
            //        list.Add(e);
            //    }
            //}

            return Json(res.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VotingsGrid_Destroy([DataSourceRequest] DataSourceRequest request,
         [Bind(Prefix = "models")]IEnumerable<VotingVM> res)
        {
            var list = new List<VotingVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    try
                    {
                        unitOfWork.VotingRepository.Delete(e.Id);
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
    }
}
