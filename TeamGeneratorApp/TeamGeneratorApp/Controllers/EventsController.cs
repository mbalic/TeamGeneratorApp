﻿using System;
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
using TeamGeneratorApp.Helpers;

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
                    if (voting.StartVoting < DateTime.Now || voting.Active)
                        ViewBag.VotingStarted = true;
                    else
                        ViewBag.VotingStarted = false;
                }
            }

            ViewData["users"] = unitOfWork.UserInCategoryRepository.GetByCategoryId(@event.CategoryId).AsQueryable()
                  .Select(e => new UserddlVM
                  {
                      Id = e.AspNetUsers.Id,
                      Name = e.AspNetUsers.Name
                  })
                  .OrderBy(e => e.Name);

            ViewData["positions"] = unitOfWork.CategoryRepository.GetPositionsInCategory(@event.CategoryId).AsQueryable()
                   .Select(e => new PositionddlVM()
                   {
                       Id = e.Id,
                       Name = e.Name
                   })
                   .OrderBy(e => e.Name);

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
                    PositionId = e.PositionId
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
                        PositionId = e.PositionId
                    };
                    //insert users into UserVoting
                    var votingList = unitOfWork.VotingRepository.GetByEventId(e.EventId);
                    var listVotings = new List<UserVoting>();

                    foreach (var voting in votingList)
                    {
                        var userVoting = new UserVoting
                        {
                            VotingId = voting.Id,
                            UserId = newUser.UserId,
                            Wins = 0,
                            Loses = 0,
                            VoteCounter = 0
                        };
                        listVotings.Add(userVoting);

                    }

                    try
                    {
                        unitOfWork.UserOnEventRepository.Insert(newUser);
                        foreach (var item in listVotings)
                        {
                            unitOfWork.VotingRepository.InsertUserVoting(item);
                        }
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
            //            EventId = e.EventId,
            //            PositionId = e.PositionId
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
                    Active = e.Active,
                    VotesPerUser = e.VotesPerUser
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
                        Id = Guid.NewGuid().ToString(),
                        Name = e.Name,
                        EventId = e.EventId,
                        StartVoting = e.StartVoting,
                        FinishVoting = e.FinishVoting,
                        Active = e.Active,
                        VotesPerUser = e.VotesPerUser

                    };
                    
                    //insert users into UserVoting
                    var userOnEvent = unitOfWork.UserOnEventRepository.GetByEventId(e.EventId);
                    var listUsers = new List<UserVoting>();

                    foreach (var user in userOnEvent)
                    {
                        var userVoting = new UserVoting
                        {
                            VotingId = newVoting.Id,
                            UserId = user.UserId,
                            Wins = 0,
                            Loses = 0,
                            Draws = 0,
                            VoteCounter = 0                           
                        };
                        listUsers.Add(userVoting);

                    }

                    
                    try
                    {
                        unitOfWork.VotingRepository.Insert(newVoting);
                        foreach (var item in listUsers)
                        {
                            unitOfWork.VotingRepository.InsertUserVoting(item);
                        }
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
                        EventId = e.EventId,
                        Name = e.Name,
                        StartVoting = e.StartVoting,
                        FinishVoting = e.FinishVoting,
                        Active = e.Active,
                        VotesPerUser = e.VotesPerUser
                    };
                    try
                    {
                        unitOfWork.VotingRepository.Update(newVoting);
                        unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        ViewBag.ConstraintError = "There was an error while updating rows in grid.";
                    }
                    list.Add(e);
                }
            }

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



        // GET: Events/VotingDetails/5
        public ActionResult VotingDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voting voting = unitOfWork.VotingRepository.GetByID(id);

            if (voting == null)
            {
                return HttpNotFound();
            }

            var votingVm = new VotingVM
            {
                Id = voting.Id,
                StartVoting = voting.StartVoting,
                FinishVoting = voting.FinishVoting,
                Name = voting.Name,
                Active = voting.Active,
                EventId = voting.EventId,
                EventName = voting.Event.Name,
                VotesPerUser = voting.VotesPerUser
            };

            var userVoting = unitOfWork.UserVotingRepository.GetUserVoting(User.Identity.GetUserId(), voting.Id);
            if (userVoting == null)
                ViewBag.UserCanVote = false;
            else
                ViewBag.UserCanVote = true;


            ViewBag.EventId = voting.EventId;
            return View(votingVm);
        }



        // GET: Events/Voting/5
        public ActionResult Voting(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //check if voting is active
            var voting = unitOfWork.VotingRepository.GetByID(id);
            if (!voting.Active)
            {
                ViewBag.Message = "This voting is not active.";
                return View("ErrorVoting");
            }

            //check if user is on this event
            var userId = User.Identity.GetUserId();
            var userOnEvent = unitOfWork.UserOnEventRepository.GetByUserId(userId);

            if (userOnEvent != null)
            {
                var userVoting = unitOfWork.UserVotingRepository.GetUserVoting(userId, id);
                if (userVoting.VoteCounter > voting.VotesPerUser)
                {
                    ViewBag.Message = "You have voted maximum times on this voting.";
                    return View("ErrorVoting");
                }

                //fetche all users on this event
                var userList = unitOfWork.UserOnEventRepository.GetByEventId(userOnEvent.EventId).ToList();

                //remove current user from list
                var userToRemove = unitOfWork.UserOnEventRepository.GetByUserId(userId);
                userList.Remove(userToRemove);

                //random pick 2 users
                var random = new Random();
                var choosenOnes = userList.OrderBy(x => random.Next()).Take(2);
                var user1 = choosenOnes.ElementAt(0);
                var user2 = choosenOnes.ElementAt(1);

                //voting process
                var votingProcess = new VotingProcessVM
                {
                    VotingId = id,
                    UserVotingId = userVoting.Id,
                    User1_Id = user1.UserId,
                    User1_PersonalName = user1.AspNetUsers.Name,
                    User1_Image = user1.AspNetUsers.ImageUrl,
                    User2_Id = user2.UserId,
                    User2_PersonalName = user2.AspNetUsers.Name,
                    User2_Image = user2.AspNetUsers.ImageUrl
                };

                ViewBag.CategoryId = userOnEvent.Event.CategoryId;
                ViewBag.CategoryName = userOnEvent.Event.Category.Name;
                ViewBag.EventId = userOnEvent.EventId;
                ViewBag.EventName = userOnEvent.Event.Name;

                return View(votingProcess);
            }
            else
            {
                return new HttpUnauthorizedResult();
            }

           
        }

        public ActionResult Rate(int categoryId, string votingId, int userVotingId, string user1_Id, string user2_Id, bool draw, bool firstWins)
        {
            var userId = User.Identity.GetUserId();

            //+1 vote for this user
            var userVoting = unitOfWork.UserVotingRepository.GetUserVoting(userId, votingId);
            userVoting.VoteCounter++;
            unitOfWork.UserVotingRepository.Update(userVoting);

            var userVoting1 = unitOfWork.UserVotingRepository.GetUserVoting(user1_Id, votingId);
            var userVoting2 = unitOfWork.UserVotingRepository.GetUserVoting(user2_Id, votingId);

            if (!draw)
            {
                if (firstWins)
                {
                    userVoting1.Wins++;
                    userVoting2.Loses++;
                }
                else
                {
                    userVoting2.Wins++;
                    userVoting1.Loses++;
                }
            }
            else
            {
                userVoting1.Draws++;
                userVoting2.Draws++;
            }

            var user1 = unitOfWork.UserInCategoryRepository.GetByUserAndCategoryId(user1_Id, categoryId);
            var user2 = unitOfWork.UserInCategoryRepository.GetByUserAndCategoryId(user2_Id, categoryId);

            //elo algorithm
            var eloRating = new EloRating(Convert.ToDouble(user1.Score), Convert.ToDouble(user2.Score), draw, firstWins);

            //update score
            user1.Score = Convert.ToInt32(eloRating.FinalRating1);
            user2.Score = Convert.ToInt32(eloRating.FinalRating2);

            unitOfWork.UserInCategoryRepository.Update(user1);
            unitOfWork.UserInCategoryRepository.Update(user1);
            unitOfWork.Commit();

            return RedirectToAction("Voting", new {id = votingId});

        }


    }
}
