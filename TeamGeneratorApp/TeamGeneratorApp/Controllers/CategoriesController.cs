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
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.Helpers;
using TeamGeneratorApp.Models;
using TeamGeneratorApp.Models.ViewModels;

namespace TeamGeneratorApp.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

       

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var category = unitOfWork.CategoryRepository.GetByID(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            string userId = User.Identity.GetUserId();

            if (category.Group.OwnerId == userId)
                ViewBag.IsOwner = true;
            else
            {
                if (unitOfWork.CategoryRepository.GetByUserId(userId).Contains(category))
                {
                    ViewBag.IsOwner = false;
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
            var categoryVm = new CategoryVM
            {
                Id = category.Id,
                GroupId = category.GroupId,
                Description = category.Description,
                GroupName = category.Group.Name,
                Name = category.Name
            };

            return View(categoryVm);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }




        public PartialViewResult EventsGrid(int categoryId = 0, bool isOwner = false)
        {
            ViewBag.IsOwner = isOwner;
            ViewBag.CategoryId = categoryId;
            return PartialView("_EventsGrid");
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



        public PartialViewResult UsersGrid(int categoryId = 0, bool isOwner = false)
        {
            var category = unitOfWork.CategoryRepository.GetByID(categoryId);

            ViewData["users"] = unitOfWork.UserInGroupRepository.GetByGroupIdAndActivity(category.GroupId, true).AsQueryable()
                  .Select(e => new UserddlVM
                  {
                      Id = e.Id,
                      Name = e.AspNetUsers.Name
                  })
                  .OrderBy(e => e.Name);

            ViewBag.IsOwner = isOwner;
            ViewBag.CategoryId = categoryId;
            return PartialView("_UsersGrid");
        }


        #region UsersGrid


        public ActionResult UsersGrid_Read([DataSourceRequest] DataSourceRequest request, int categoryId)
        {
            var res = unitOfWork.UserInCategoryRepository.GetByCategoryId(categoryId).ToList();

            var list = new List<UserCategoryVM>();
            foreach (var e in res)
            {
                var newItem = new UserCategoryVM();
                newItem.Id = e.Id;
                newItem.UserInGroupId = e.UserInGroupId;
                newItem.CategoryId = e.CategoryId;
                newItem.UserPersonalName = e.UserInGroup.AspNetUsers.Name;
                newItem.Rating = e.Rating;
                newItem.Active = e.Active;
                newItem.UserId = e.UserInGroup.UserId;

                var pair = CountSuccessPercentageForUser(e);
                newItem.SuccessPercentage = pair.Value;
                newItem.Appereances = pair.Key;           

                list.Add(newItem);
            }

            return Json(list.OrderByDescending(p => p.UserPersonalName).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UsersGrid_Create([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<UserCategoryVM> res)
        {
            var list = new List<UserCategoryVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    var newUser = new UserInCategory
                    {
                        Id = e.Id,
                        UserInGroupId = e.UserInGroupId,
                        CategoryId = e.CategoryId,
                        Rating = e.Rating,
                        Active = e.Active                        
                    };
                    try
                    {
                        unitOfWork.UserInCategoryRepository.Insert(newUser);
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
        [Bind(Prefix = "models")]IEnumerable<UserCategoryVM> res)
        {
            var list = new List<UserCategoryVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    var newUser = unitOfWork.UserInCategoryRepository.GetByID(e.Id);
                    newUser.Rating = e.Rating;
                    newUser.Active = e.Active;
                    try
                    {
                        unitOfWork.UserInCategoryRepository.Update(newUser);
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
        public ActionResult UsersGrid_Destroy([DataSourceRequest] DataSourceRequest request,
         [Bind(Prefix = "models")]IEnumerable<UserCategoryVM> res)
        {
            var list = new List<UserCategoryVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    try
                    {
                        unitOfWork.UserInCategoryRepository.Delete(e.Id);
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



        public PartialViewResult PositionsGrid(int categoryId = 0, bool isOwner = false)
        {
            ViewBag.IsOwner = isOwner;
            ViewBag.CategoryId = categoryId;
            return PartialView("_PositionsGrid");
        }


        #region PositionsGrid


        public ActionResult PositionsGrid_Read([DataSourceRequest] DataSourceRequest request, int categoryId)
        {
            var res = unitOfWork.CategoryRepository.GetPositionsInCategory(categoryId).ToList();

            var list = new List<PositionVM>();
            foreach (var e in res)
            {
                var newItem = new PositionVM
                {
                    Id = e.Id,
                    CategoryId = e.CategoryId,
                    Name = e.Name,
                    Value = e.Value

                };
                list.Add(newItem);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PositionsGrid_Create([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<PositionVM> res)
        {
            var list = new List<PositionVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    var newPosition = new Position
                    {
                        Id = e.Id,
                        CategoryId = e.CategoryId,
                        Name = e.Name,
                        Value = e.Value
                    };

                    try
                    {
                        unitOfWork.CategoryRepository.InsertPositionInCategory(newPosition);
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
        public ActionResult PositionsGrid_Update([DataSourceRequest] DataSourceRequest request,
        [Bind(Prefix = "models")]IEnumerable<PositionVM> res)
        {
            var list = new List<PositionVM>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    var newItem = new Position
                    {
                        Id = e.Id,
                        CategoryId = e.CategoryId,
                        Name = e.Name,
                        Value = e.Value
                    };

                    try
                    {
                        unitOfWork.CategoryRepository.UpdatePositionInCategory(newItem);
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
        public ActionResult PositionsGrid_Destroy([DataSourceRequest] DataSourceRequest request,
         [Bind(Prefix = "models")]IEnumerable<Position> res)
        {
            var list = new List<Position>();
            if (res != null && ModelState.IsValid)
            {
                foreach (var e in res)
                {
                    try
                    {
                        unitOfWork.CategoryRepository.DeletePositionInCategory(e.Id);
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


        private KeyValuePair<int, double?> CountSuccessPercentageForUser(UserInCategory user)
        {
            double? succesPercentage = 0;
            var teamCounter = 0;
            foreach (var userOnEvent in user.UserOnEvent)
            {
                foreach (var userInTeam in userOnEvent.UserInTeam)
                {
                    if (userInTeam.Team.SuccessPercentage != null)
                    {
                        succesPercentage += userInTeam.Team.SuccessPercentage;
                        teamCounter++;
                    }
                }
                if (teamCounter > 0)
                    succesPercentage = succesPercentage/teamCounter;
               
            }

            return new KeyValuePair<int, double?>(teamCounter, succesPercentage); ;
        }


        public ActionResult Cooperation(int userInCategoryId)
        {
            string userId = User.Identity.GetUserId();
            var user = unitOfWork.UserInCategoryRepository.GetByID(userInCategoryId);

            var group = user.Category.Group;

            if (group.OwnerId == userId)
                ViewBag.IsOwner = true;
            else
            {
                if (unitOfWork.CategoryRepository.GetByUserId(userId).Contains(user.Category))
                {
                    ViewBag.IsOwner = false;
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            var userInCategoryVm = new UserCategoryVM();

            userInCategoryVm.Id = userInCategoryId;
            userInCategoryVm.CategoryId = user.CategoryId;
            userInCategoryVm.UserPersonalName = user.UserInGroup.AspNetUsers.Name;
            userInCategoryVm.GroupName = user.UserInGroup.Group.Name;
            userInCategoryVm.CategoryName = user.Category.Name;
            userInCategoryVm.Rating = user.Rating;

            var pair = CountSuccessPercentageForUser(user);
            userInCategoryVm.SuccessPercentage = pair.Value;
            userInCategoryVm.Appereances = pair.Key;

            ViewBag.UserInCategoryId = userInCategoryId;

            return View(userInCategoryVm);
        }


        public PartialViewResult CooperationGrid(int userInCategoryId = 0)
        {

            ViewBag.UserInCategoryId = userInCategoryId;
            return PartialView("_CooperationGrid");
        }


        public ActionResult CooperationGrid_Read([DataSourceRequest] DataSourceRequest request, int userInCategoryId)
        {
            var user = unitOfWork.UserInCategoryRepository.GetByID(userInCategoryId);
            var userInTeamList = unitOfWork.UserInTeamRepository.GetByUserInCategoryId(userInCategoryId);
            var userTeams = new List<Team>();
            foreach (var userInTeam in userInTeamList)
            {
                if(userInTeam.Team.SuccessPercentage != null)
                    userTeams.Add(userInTeam.Team);
            }

            //get all users from category
            var allUsers = unitOfWork.UserInCategoryRepository.Get().ToList();
            allUsers.Remove(user);
            
            var cooperations = new List<CooperationVM>();
            //count cooperation factor for every user
            foreach (var userInCategory in allUsers)
            {
                var newUser = new CooperationVM
                {
                    UserId = userInCategory.Id,
                    UserPersonalName = userInCategory.UserInGroup.AspNetUsers.Name,
                    SuccessPercentage = 0
                };
                var teamCount = 0; 
                foreach (var userOnEvent in userInCategory.UserOnEvent)
                {
                    foreach (var userInTeam in userOnEvent.UserInTeam)
                    {   
                        //check if users were in same team
                        if (userTeams.Contains(userInTeam.Team))
                        {
                           newUser.SuccessPercentage += userInTeam.Team.SuccessPercentage;
                            teamCount++;
                        }
                    }
                }
                if (teamCount > 0)
                    newUser.SuccessPercentage = newUser.SuccessPercentage/teamCount;
               

                newUser.Appereances = teamCount;

                cooperations.Add(newUser);
            }
            
            return Json(cooperations.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

      


    }
}
