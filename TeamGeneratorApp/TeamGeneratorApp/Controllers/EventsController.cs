using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
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

            var @event = unitOfWork.EventRepository.GetByID(id);

            if (@event == null)
            {
                return HttpNotFound();
            }

            string userId = User.Identity.GetUserId();

            if (@event.Category.Group.OwnerId == userId)
                ViewBag.IsOwner = true;
            else
            {
                if (unitOfWork.EventRepository.GetByUserId(userId).Contains(@event))
                {
                    ViewBag.IsOwner = false;
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            var eventVm = new EventVM
            {
                Id = @event.Id,
                CategoryId = @event.CategoryId,
                CategoryName = @event.Category.Name,
                Description = @event.Description,
                Start = @event.Start,
                Finish = @event.Finish,
                Name = @event.Name
            };
            
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

        public PartialViewResult UsersGrid(int eventId = 0, bool isOwner = false) 
        {
            var @event = unitOfWork.EventRepository.GetByID(eventId);

            ViewData["users"] = unitOfWork.UserInCategoryRepository.GetByCategoryIdAndActivity(@event.CategoryId, true).AsQueryable()
                  .Select(e => new UserddlVM
                  {
                      Id = e.Id,
                      Name = e.UserInGroup.AspNetUsers.Name
                  })
                  .OrderBy(e => e.Name);

            ViewData["positions"] = unitOfWork.CategoryRepository.GetPositionsInCategory(@event.CategoryId).AsQueryable()
                   .Select(e => new PositionddlVM()
                   {
                       Id = e.Id,
                       Name = e.Name
                   })
                   .OrderBy(e => e.Name);

            ViewBag.IsOwner = isOwner;
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
                    UserInCategoryId = e.UserInCategoryId,
                    EventId = e.EventId,
                    UserPersonalName = e.UserInCategory.UserInGroup.AspNetUsers.Name,
                    PositionId = e.PositionId,
                    Rating = unitOfWork.UserInCategoryRepository.GetByID(e.UserInCategoryId).Rating,
                    UserId = e.UserInCategory.UserInGroup.UserId
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
                        UserInCategoryId = e.UserInCategoryId,
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
                            UserOnEventId = newUser.Id,
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



        public PartialViewResult VotingsGrid(int eventId = 0, bool isOwner = false)
        {
            ViewBag.IsOwner = isOwner;
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
                            UserOnEventId = user.Id,
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

            var voting = unitOfWork.VotingRepository.GetByID(id);

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

            //check if user has access permission

            if (voting.Event.Category.Group.OwnerId == User.Identity.GetUserId())
            {
                ViewBag.UserIsOwner = true;
            }
            else
            {
                ViewBag.UserIsOwner = false;
            }


            var userOnEvent = unitOfWork.UserOnEventRepository.GetByUserId(User.Identity.GetUserId());
            if (userOnEvent != null)
            {
                var userVoting = unitOfWork.UserVotingRepository.GetByUserOnEventAndVotingId(userOnEvent.Id, voting.Id);
                if (userVoting == null)
                    ViewBag.UserCanVote = false;
                else
                    ViewBag.UserCanVote = true;
            }
            else
                ViewBag.UserCanVote = false;


            if (ViewBag.UserIsOwner == false && ViewBag.UserCanVote == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }


            ViewBag.EventId = voting.EventId;
            return View(votingVm);
        }


        #region UserVotingsGrid

        public PartialViewResult UserVotingsGrid(string votingId)
        {

            ViewBag.VotingId = votingId;
            return PartialView("_UserVotingsGrid");
        }



        public ActionResult UserVotingsGrid_Read([DataSourceRequest] DataSourceRequest request, string votingId)
        {
            var res = unitOfWork.UserVotingRepository.GetByVotingId(votingId);

            var list = new List<UserVotingVM>();
            foreach (var e in res)
            {
                var newItem = new UserVotingVM
                {
                    Id = e.Id,
                    UserPersonalName = e.UserOnEvent.UserInCategory.UserInGroup.AspNetUsers.Name,
                    VoteCounter = e.VoteCounter,
                    Wins = e.Wins,
                    Draws = e.Draws,
                    Loses = e.Loses,
                    UserOnEventId = e.UserOnEventId,
                    VotingId = e.VotingId
                };
                list.Add(newItem);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        
        #endregion



        // GET: Events/Voting/5
        public ActionResult Voting(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //check if user is on this event
            var userId = User.Identity.GetUserId();
            var userOnEvent = unitOfWork.UserOnEventRepository.GetByUserId(userId);

            if (userOnEvent == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);


            //check if voting is active
            var voting = unitOfWork.VotingRepository.GetByID(id);
            if (!voting.Active)
            {
                ViewBag.Message = "This voting is not active.";
                return View("ErrorVoting");
            }

                
            var userVoting = unitOfWork.UserVotingRepository.GetByUserOnEventAndVotingId(userOnEvent.Id, voting.Id);
            if (userVoting.VoteCounter > voting.VotesPerUser)
            {
                ViewBag.Message = "You have voted maximum times on this voting.";
                return View("ErrorVoting");
            }

            //fetch all users on this event
            var userList = unitOfWork.UserOnEventRepository.GetByEventId(userOnEvent.EventId).ToList();

            //remove current user from list
            var userToRemove = unitOfWork.UserOnEventRepository.GetByUserId(userId);
            userList.Remove(userToRemove);

            //random pick 2 users
            var random = new Random();
            var user1 = userList.OrderBy(x => random.Next()).Take(1).Single();
            userList.Remove(user1);
            var user2 = userList.OrderBy(x => random.Next()).Take(1).Single();

            //voting process
            var votingProcess = new VotingProcessVM
            {
                VotingId = id,
                UserVotingId = userVoting.Id,
                UserOnEvent1_Id =  user1.Id,
                User1_PersonalName = user1.UserInCategory.UserInGroup.AspNetUsers.Name,
                User1_Image = user1.UserInCategory.UserInGroup.AspNetUsers.ImageUrl,
                UserOnEvent2_Id = user2.Id,
                User2_PersonalName = user2.UserInCategory.UserInGroup.AspNetUsers.Name,
                User2_Image = user2.UserInCategory.UserInGroup.AspNetUsers.ImageUrl
            };

            ViewBag.CategoryId = userOnEvent.Event.CategoryId;
            ViewBag.CategoryName = userOnEvent.Event.Category.Name;
            ViewBag.EventId = userOnEvent.EventId;
            ViewBag.EventName = userOnEvent.Event.Name;

            return View(votingProcess);
        }

        public ActionResult Rate(int categoryId, string votingId, int userVotingId, int userOnEvent1_Id, int userOnEvent2_Id, bool draw, bool firstWins)
        {
            //check access right
            var userId = User.Identity.GetUserId();
            var userOnEvent = unitOfWork.UserOnEventRepository.GetByUserId(userId);

            if (userOnEvent == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            //+1 vote for this user
            var userVoting = unitOfWork.UserVotingRepository.GetByUserOnEventAndVotingId(userOnEvent.Id, votingId);
            userVoting.VoteCounter++;
            unitOfWork.UserVotingRepository.Update(userVoting);

            var userVoting1 = unitOfWork.UserVotingRepository.GetByUserOnEventAndVotingId(userOnEvent1_Id, votingId);
            var userVoting2 = unitOfWork.UserVotingRepository.GetByUserOnEventAndVotingId(userOnEvent2_Id, votingId);

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

            var user1 = unitOfWork.UserInCategoryRepository.GetByID(userVoting1.UserOnEvent.UserInCategory.Id);
            var user2 = unitOfWork.UserInCategoryRepository.GetByID(userVoting2.UserOnEvent.UserInCategory.Id);

            //elo algorithm
            var eloRating = new EloRating(Convert.ToDouble(user1.Rating), Convert.ToDouble(user2.Rating), draw, firstWins);

            //update rating
            user1.Rating = Convert.ToInt32(eloRating.FinalRating1);
            user2.Rating = Convert.ToInt32(eloRating.FinalRating2);

            unitOfWork.UserInCategoryRepository.Update(user1);
            unitOfWork.UserInCategoryRepository.Update(user1);
            unitOfWork.Commit();

            return RedirectToAction("Voting", new {id = votingId});

        }


        public ActionResult GeneratorsGrid(int eventId = 0, bool isOwner = false)
        {
            ViewData["criterias"] = unitOfWork.GeneratorRepository.GetCriterias().AsQueryable()
                  .Select(e => new CriteriaddlVM()
                  {
                      Id = e.Id,
                      Name = e.Name
                  })
                  .OrderBy(e => e.Name);

            ViewBag.IsOwner = isOwner;
            ViewBag.EventId = eventId;
            return PartialView("_GeneratorsGrid");
        }


        #region GeneratorsGrid

        public ActionResult GeneratorsGrid_Read([DataSourceRequest] DataSourceRequest request, int eventId)
        {
            var res = unitOfWork.GeneratorRepository.GetByEventId(eventId).ToList();

            var list = new List<GeneratorVM>();
            foreach (var e in res)
            {
                var newItem = new GeneratorVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    EventId = e.EventId,
                    CriteriaId = e.CriteriaId,
                    NumberOfTeams = e.NumberOfTeams,
                    IsGenerated = e.Team.Count != 0? true : false,
                };
                list.Add(newItem);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GeneratorsGrid_Create([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<GeneratorVM> res)
        {
            var list = new List<GeneratorVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    var newItem = new Generator
                    {
                        Id = e.Id,
                        Name = e.Name,
                        EventId = e.EventId,
                        CriteriaId = e.CriteriaId,
                        NumberOfTeams = e.NumberOfTeams,
                    };
                 

                    try
                    {
                        unitOfWork.GeneratorRepository.Insert(newItem);
                        for(var i = 1; i<= e.NumberOfTeams; i++)
                        {
                            var newTeam = new Team
                            {
                                GeneratorId = e.Id,
                                Name = "Team_" + i,
                                Strength = 0
                            };
                            unitOfWork.TeamRepository.Insert(newTeam);
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
        public ActionResult GeneratorsGrid_Update([DataSourceRequest] DataSourceRequest request,
        [Bind(Prefix = "models")]IEnumerable<GeneratorVM> res)
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
        public ActionResult GeneratorsGrid_Destroy([DataSourceRequest] DataSourceRequest request,
         [Bind(Prefix = "models")]IEnumerable<GeneratorVM> res)
        {
            var list = new List<GeneratorVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    try
                    {
                        unitOfWork.GeneratorRepository.Delete(e.Id);
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



        // GET: Events/GeneratorDetails/5
        public ActionResult GeneratorDetails(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        

            var generator = unitOfWork.GeneratorRepository.GetByID(id);

            if (generator == null)
            {
                return HttpNotFound();
            }

            ////check access rights

            string userId = User.Identity.GetUserId();

            if (generator.Event.Category.Group.OwnerId == userId)
                ViewBag.IsOwner = true;
            else
            {
                if (unitOfWork.GeneratorRepository.GetByUserId(userId).Contains(generator))
                {
                    ViewBag.IsOwner = false;
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
            

            var generatorVm = new GeneratorDetailsVM();

            generatorVm.Id = generator.Id;
            generatorVm.Name = generator.Name;
            generatorVm.EventId = generator.EventId;
            generatorVm.EventName = generator.Event.Name;
            generatorVm.NumberOfTeams = generator.NumberOfTeams;
            generatorVm.CriteriaName = generator.Criteria.Name;
            generatorVm.IsGenerated = generator.Team.Count != 0? true : false;         
            

            var allUsers = unitOfWork.UserOnEventRepository.GetByEventId(generator.EventId).ToList();
            if (allUsers.Any())
            {
                ViewBag.AllUsersEmpty = false;
            }
            else
            {
                ViewBag.AllUsersEmpty = true;
            }


            return View(generatorVm);
        }



        #region AllUsersGenerateGrid
        public PartialViewResult AllUsersGenerateGrid(int generatorId = 0, bool isOwner = false)
        {
            var generator = unitOfWork.GeneratorRepository.GetByID(generatorId);
            var teams = unitOfWork.TeamRepository.GetByGeneratorId(generator.Id).ToList();
            teams.Insert(0, new Team {Id = -1, Name = "", GeneratorId = -1});

            ViewData["teams"] = teams.AsQueryable()
                   .Select(e => new TeamddlVM
                   {
                       Id = e.Id,
                       Name = e.Name
                   })
                   .OrderBy(e => e.Name);


            ViewBag.IsOwner = isOwner;
            ViewBag.IsGenerated = generator.Team.Count != 0 ? true: false;
            ViewBag.EventId = generator.EventId;
            ViewBag.GeneratorId = generator.Id;
            return PartialView("_AllUsersGenerateGrid");
        }

        public ActionResult AllUsersGenerateGrid_Read([DataSourceRequest] DataSourceRequest request, int eventId, int generatorId)
        {
            var res = unitOfWork.UserOnEventRepository.GetByEventId(eventId).ToList();

            var list = new List<UserOnEventVM>();
            foreach (var e in res)
            {
                var newItem = new UserOnEventVM
                {
                    Id = e.Id,
                    EventId = e.EventId,
                    UserPersonalName = e.UserInCategory.UserInGroup.AspNetUsers.Name,
                    PositionId = e.PositionId,
                    PositionName = e.Position.Name,
                    Rating = unitOfWork.UserInCategoryRepository.GetByID(e.UserInCategory.Id).Rating,
                    TeamId = (unitOfWork.UserInTeamRepository.GetByUserOnEventAndGeneratorId(e.Id, generatorId) == null? -1 : unitOfWork.UserInTeamRepository.GetByUserOnEventAndGeneratorId(e.Id, generatorId).TeamId)
                };
                list.Add(newItem);
            }

            return Json(list.OrderByDescending(p => p.Rating).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllUsersGenerateGrid_Update([DataSourceRequest] DataSourceRequest request,
       [Bind(Prefix = "models")]IEnumerable<UserOnEventVM> res, int generatorId)
        {
            var list = new List<UserOnEventVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    try
                    {  
                        //if null
                        if (e.TeamId == -1)
                        {
                            //find if exists in userInTeam and delete it
                            var existingItem = unitOfWork.UserInTeamRepository.GetByUserOnEventAndGeneratorId(e.Id,
                                generatorId);
                            if (existingItem != null)
                            {
                                var team = unitOfWork.TeamRepository.GetByID(existingItem.TeamId);
                                team.Strength -= existingItem.UserOnEvent.UserInCategory.Rating;
                                if (team.Strength < 0)  //for insurance, if user rating changes
                                    team.Strength = 0;

                                unitOfWork.TeamRepository.Update(team);
                                unitOfWork.UserInTeamRepository.Delete(existingItem);
                                unitOfWork.Commit();
                            }
                        }
                        else
                        {

                            //automaticaly insert in userInTeam
                            var newUserInTeam = new UserInTeam
                            {
                                UserOnEventId = e.Id,
                                TeamId = (int)e.TeamId
                            };

                            var existingItem = unitOfWork.UserInTeamRepository.GetByUserOnEventAndGeneratorId(e.Id,
                                generatorId);
                            if (existingItem != null)
                            {
                                if (existingItem.TeamId != e.TeamId)
                                {

                                    var team1 = unitOfWork.TeamRepository.GetByID(existingItem.TeamId);
                                    var team2 = unitOfWork.TeamRepository.GetByID(e.TeamId);
                                    team1.Strength -= existingItem.UserOnEvent.UserInCategory.Rating;
                                    team2.Strength += e.Rating;
                                    if (team1.Strength < 0)  //for insurance, if user rating changes
                                        team1.Strength = 0;

                                    existingItem.TeamId = (int)e.TeamId;

                                    unitOfWork.TeamRepository.Update(team1);
                                    unitOfWork.TeamRepository.Update(team2);
                                    unitOfWork.UserInTeamRepository.Update(existingItem);
                                    unitOfWork.Commit();
                                }
                            }
                            else
                            {
                                var team1 = unitOfWork.TeamRepository.GetByID(newUserInTeam.TeamId);
                                team1.Strength += e.Rating;

                                unitOfWork.TeamRepository.Update(team1);
                                unitOfWork.UserInTeamRepository.Insert(newUserInTeam);
                                unitOfWork.Commit();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ViewBag.ConstraintError = "There was an error while updating rows in grid.";
                    }
                    list.Add(e);
                }
                
            }

            return Json(res.OrderByDescending(p => p.Rating).ToDataSourceResult(request, ModelState));
        }

       



        #endregion





        public ActionResult GenerateTeams(int generatorId = 0)
        {
            
            if (generatorId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //check access rights
            if (!unitOfWork.GeneratorRepository.IsOwner(User.Identity.GetUserId(), generatorId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }


            var generator = unitOfWork.GeneratorRepository.GetByID(generatorId);

            if (generator == null)
            {
                return HttpNotFound();
            }


            //deactivate votings on this event
            var votings = unitOfWork.VotingRepository.GetByEventId(generator.EventId);
            foreach (var voting in votings)
            {
                voting.Active = false;
                unitOfWork.VotingRepository.Update(voting);
            }


            //get all users on event 
            var allUsersOnEventList = unitOfWork.UserOnEventRepository.GetByEventId(generator.EventId);
            
            //remove users that are preordered in teams
            var usersWithoutTeam = new List<UserOnEvent>();
            var usersInTeam = new List<UserOnEvent>();

            foreach (var userOnEvent in allUsersOnEventList)
            {
                var userInTeam = unitOfWork.UserInTeamRepository.GetByUserOnEventAndGeneratorId(userOnEvent.Id,
                    generatorId);
                if (userInTeam == null)
                {
                    usersWithoutTeam.Add(userOnEvent);
                }
                else
                {
                    usersInTeam.Add(userOnEvent);
                }

            }
            //need them ordered for generating loop
            usersWithoutTeam = usersWithoutTeam.OrderByDescending(p => p.UserInCategory.Rating).ToList();
            



            //create teamList which has teamHelpers with ratingSum and list of users inside it

            var teams = unitOfWork.TeamRepository.GetByGeneratorId(generatorId);
            var teamList = new List<TeamHelper>();

            foreach (var t in teams)
            {
                var team = new TeamHelper();
                team.Id = t.Id;
                team.Strength = 0;

                var list1 = new List<UserOnEvent>();
                foreach (var u in t.UserInTeam)
                {
                    var userOnEvent = unitOfWork.UserOnEventRepository.GetByID(u.UserOnEventId);
                    list1.Add(userOnEvent);
                    team.Strength += userOnEvent.UserInCategory.Rating;
                }

                team.Users = list1;

                teamList.Add(team);
            }


            //parameters for generating
            var criteria = generator.Criteria;
            int smallestTeam = 0;

            //balanced teams
            if (criteria.Name == "Balanced")
            {
                while (usersWithoutTeam.Any())
                {
                    teamList = teamList.OrderBy(p => p.Strength).OrderBy(p => p.Users.Count).ToList();
                    smallestTeam = teamList.First().Users.Count;

                    foreach (var team in teamList)
                    {
                        if (team.Users.Count == smallestTeam)                                                     //skipping teams with more users
                        {
                            team.Users.Add(usersWithoutTeam.First());
                            team.Strength += usersWithoutTeam.First().UserInCategory.Rating;

                            usersWithoutTeam.Remove(usersWithoutTeam.First());
                            break;

                        }
                       
                    }
                }
            }

            //criteria: strongest
            else
            {
                var usersPerTeam = allUsersOnEventList.Count()/generator.NumberOfTeams;
                teamList = teamList.OrderByDescending(p => p.Strength).ToList();

                foreach (var team in teamList)
                {

                    while (team.Users.Count < usersPerTeam)                                         //every team should have same number of users
                    {
                        team.Users.Add(usersWithoutTeam.First());
                        team.Strength += usersWithoutTeam.First().UserInCategory.Rating;

                        usersWithoutTeam.Remove(usersWithoutTeam.First());
                    }
                }

            }

            //save userInTeam and teams to db
            foreach (var team in teamList)
            {
                var teamDb = unitOfWork.TeamRepository.GetByID(team.Id);
                teamDb.Strength = team.Strength;

                foreach (var user in team.Users)
                {
                    var userInTeam = new UserInTeam
                    {
                        UserOnEventId = user.Id,
                        TeamId = team.Id
                    };
                    //check if userInTeam already exist
                    if(unitOfWork.UserInTeamRepository.GetByUserOnEventAndGeneratorId(user.Id, generatorId) == null)
                        unitOfWork.UserInTeamRepository.Insert(userInTeam);
                }             
            }
            unitOfWork.GeneratorRepository.Update(generator);
            

            unitOfWork.Commit();
            

            return RedirectToAction("GeneratorDetails", new {id = generator.Id });




        }


        //public ActionResult LockGenerator(int generatorId)
        //{
        //    if (generatorId == 0)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    //check access rights
        //    if (!unitOfWork.GeneratorRepository.IsOwner(User.Identity.GetUserId(), generatorId))
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        //    }

        //    var generator = unitOfWork.GeneratorRepository.GetByID(generatorId);

        //    if (generator == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    generator.IsLocked = true;
        //    unitOfWork.GeneratorRepository.Update(generator);
        //    unitOfWork.Commit();

        //    return RedirectToAction("GeneratorDetails", new {id = generatorId});
        //}


        #region TeamsGrid


        public PartialViewResult TeamsGrid(int generatorId = 0, bool isOwner = false)
        {
            var generator = unitOfWork.GeneratorRepository.GetByID(generatorId);

            ViewBag.IsOwner = isOwner;
            ViewBag.GeneratorId = generatorId;
            return PartialView("_TeamsGrid");
        }


        public ActionResult TeamsGrid_Read([DataSourceRequest] DataSourceRequest request, int generatorId)
        {
            var res = unitOfWork.TeamRepository.GetByGeneratorId(generatorId).ToList();

            var list = new List<TeamVM>();
            foreach (var e in res)
            {
                var newItem = new TeamVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    GeneratorId = e.GeneratorId,
                    Strength = e.Strength,
                    SuccessPercentage = e.SuccessPercentage
                };
                list.Add(newItem);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TeamsGrid_Update([DataSourceRequest] DataSourceRequest request,
        [Bind(Prefix = "models")]IEnumerable<TeamVM> res)
        {
            var list = new List<TeamVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    var team = unitOfWork.TeamRepository.GetByID(e.Id);
                    team.Name = e.Name;
                    team.SuccessPercentage = e.SuccessPercentage;

                    try
                    {
                        unitOfWork.TeamRepository.Update(team);
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


        #endregion


    }
}



