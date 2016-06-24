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
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using PagedList;
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.Models;
using TeamGeneratorApp.Models.ViewModels;

namespace TeamGeneratorApp.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        
        // GET: Groups
        public ActionResult Index()
        {
            ViewBag.UserId = User.Identity.GetUserId();
            return View("Index");
        }

        #region GroupsGrid

        public ActionResult GroupsGrid_Read([DataSourceRequest] DataSourceRequest request, string userId)
        {
            var res = unitOfWork.GroupRepository.GetByOwnerId(userId).ToList();

            var list = new List<GroupListVM>();
            foreach (var e in res)
            {
                var newVm = new GroupListVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    OwnerName = e.AspNetUsers.Name
                };

                list.Add(newVm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GroupsGrid_Update([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<GroupListVM> list)
        {
            var results = new List<GroupListVM>();
            if (list != null && ModelState.IsValid)
            {
                foreach (var e in list)
                {
                    var group = unitOfWork.GroupRepository.GetByID(e.Id);
                    group.Name = e.Name;
                    group.Description = e.Description;

                    try
                    {
                        unitOfWork.GroupRepository.Update(group);
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
        public ActionResult GroupsGrid_Destroy([DataSourceRequest] DataSourceRequest request,
         [Bind(Prefix = "models")]IEnumerable<GroupListVM> list)
        {
            var results = new List<GroupListVM>();
            if (list != null && ModelState.IsValid)
            {
                foreach (var e in list)
                {
                    var newModel = new Group
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Description = e.Description,
                        OwnerId = e.OwnerId
                    };
                    try
                    {
                        unitOfWork.GroupRepository.Delete(newModel);
                        unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        ViewBag.ConstraintError = "There was an error while deleting rows in grid.";
                    }
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        #endregion



        // GET: Pools/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var group = unitOfWork.GroupRepository.GetByID(id);

            if (group == null)
            {
                return HttpNotFound();
            }

            string userId = User.Identity.GetUserId();

            if (group.OwnerId == userId)
                ViewBag.IsOwner = true;
            else
            {
                if (unitOfWork.GroupRepository.GetByUserId(userId).Contains(group))
                {
                    ViewBag.IsOwner = false;
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
            var groupVm = new GroupListVM
            {
                Id = group.Id,
                OwnerId = group.OwnerId,
                Description = group.Description,
                Name = group.Name,
                OwnerName = group.AspNetUsers.Name
            };

            return View(groupVm);
        }

      

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }


        public PartialViewResult CategoriesGrid(int groupId = 0, bool isOwner = false)
        {
            ViewBag.IsOwner = isOwner;
            ViewBag.GroupId = groupId;
            return PartialView("_CategoriesGrid");
        }

        #region Categories

        public ActionResult CategoriesGrid_Read([DataSourceRequest] DataSourceRequest request, int groupId)
        {
            var res = unitOfWork.CategoryRepository.GetByGroupId(groupId).ToList();

            var list = new List<CategoryVM>();
            foreach (var e in res)
            {
                var categoryVm = new CategoryVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    GroupId = e.GroupId,
                    Description = e.Description,
                };
                list.Add(categoryVm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CategoriesGrid_Create([DataSourceRequest] DataSourceRequest request,
           [Bind(Prefix = "models")]IEnumerable<CategoryVM> list)
        {
            var results = new List<CategoryVM>();
            if (list != null && ModelState.IsValid)
            {
                foreach (var e in list)
                {
                    var newCategory = new Category
                    {
                        Name = e.Name,
                        GroupId = e.GroupId,
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
                        GroupId = e.GroupId,
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
                        GroupId = e.GroupId,
                        Name = e.Name,
                        Description = e.Description
                    };
                    try
                    {
                        unitOfWork.CategoryRepository.Delete(newCategory.Id);
                        unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        ViewBag.ConstraintError = "There was an error while deleting rows in grid.";
                    }
                    results.Add(e);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        #endregion



        public PartialViewResult UsersInGroupGrid(int groupId = 0, bool isOwner = false)
        {
            ViewBag.IsOwner = isOwner;
            ViewBag.GroupId = groupId;
            return PartialView("_UsersInGroupGrid");
        }

        #region UsersInGroup

        public ActionResult UsersInGroupGrid_Read([DataSourceRequest] DataSourceRequest request, int groupId)
        {
            var userInGroupRepo = unitOfWork.UserInGroupRepository.GetByGroupId(groupId).ToList();

            var list = new List<UserInGroupVM>();
            foreach (var e in userInGroupRepo)
            {
                var userInGroupVm = new UserInGroupVM();

                userInGroupVm.Id = e.Id;
                userInGroupVm.Name = e.AspNetUsers.Name;
                userInGroupVm.UserId = e.UserId;
                userInGroupVm.Email = e.AspNetUsers.Email;
                userInGroupVm.GroupId = e.GroupId;
                userInGroupVm.Active = e.Active;
                
                var pair = CountSuccessPercentageForUser(e);
                userInGroupVm.Appereances = pair.Key;
                userInGroupVm.SuccessPercentage = pair.Value;

                list.Add(userInGroupVm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UsersInGroupGrid_Update([DataSourceRequest] DataSourceRequest request,
           [Bind(Prefix = "models")]IEnumerable<UserInGroupVM> list)
        {
            var results = new List<UserInGroupVM>();
            if (list != null && ModelState.IsValid)
            {
                foreach (var e in list)
                {
                    var newUserInGroup = unitOfWork.UserInGroupRepository.GetByID(e.Id);
                    newUserInGroup.Active = e.Active;
                    try
                    {
                        unitOfWork.UserInGroupRepository.Update(newUserInGroup);
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


        #endregion



        public PartialViewResult InvitationsGrid(int groupId = 0)
        {

            ViewBag.GroupId = groupId;
            return PartialView("_InvitationsGrid");
        }

        #region Invitations
        public ActionResult InvitationsGrid_Read([DataSourceRequest] DataSourceRequest request, int groupId)
        {
            var res = unitOfWork.InvitationRepository.GetByGroupId(groupId).ToList();

            var list = new List<InvitationVM>();
            foreach (var e in res)
            {
                var invitationVm = new InvitationVM
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    GroupId = e.GroupId,
                    DateCreated = e.DateCreated,
                    Name = e.AspNetUsers.Name,
                    Email = e.AspNetUsers.Email
                };
                list.Add(invitationVm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvitationsGrid_Destroy([DataSourceRequest] DataSourceRequest request,
        [Bind(Prefix = "models")]IEnumerable<InvitationVM> list)
        {
            var results = new List<InvitationVM>();
            if (list != null && ModelState.IsValid)
            {
                foreach (var e in list)
                {
                    try
                    {
                        unitOfWork.InvitationRepository.Delete(e.Id);
                        unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        ViewBag.ConstraintError = "There was an error while deleting rows in grid.";
                    }
                    results.Add(e);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        #endregion


        [HttpPost]
        public ActionResult SearchUser(string q)
        {
            var res = unitOfWork.UserRepository.SearchByMail(q);
            var newVm = new UserIndexVM();
            if (res != null)
            {
                newVm.Id = res.Id;
                newVm.Email = res.Email;
                newVm.Name = res.Name;
                newVm.UserName = res.UserName;
            }
            else
            {
                newVm.Id = "NULL";
            }

            return Json(newVm);
        }

        [HttpPost]
        public ActionResult CreateInvitation(string userId, int groupId)
        {
            var invitation = new Invitaton
            {
                UserId = userId,
                DateCreated = DateTime.Now,
                GroupId = groupId,
                AspNetUsers = unitOfWork.UserRepository.GetByID(userId),
                Group = unitOfWork.GroupRepository.GetByID(groupId)
            };
            try
            {
                unitOfWork.InvitationRepository.Insert(invitation);
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                return Json(new {status=e.Message});
            }
            return Json(new {status="OK"});


        }


        private KeyValuePair<int, double?> CountSuccessPercentageForUser(UserInGroup user)
        {
            double? succesPercentage = 0;
            var teamCounter = 0;
            foreach (var user1 in user.UserInCategory)
            {
                foreach (var userOnEvent in user1.UserOnEvent)
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
                        succesPercentage = succesPercentage / teamCounter;
                }
            }

            return new KeyValuePair<int, double?>(teamCounter, succesPercentage); ;
        }

        public ActionResult Cooperation(int userInGroupId)
        {
            string userId = User.Identity.GetUserId();
            var user = unitOfWork.UserInGroupRepository.GetByID(userInGroupId);
            var group = user.Group;

            if (group.OwnerId == userId)
                ViewBag.IsOwner = true;
            else
            {
                if (unitOfWork.GroupRepository.GetByUserId(userId).Contains(group))
                {
                    ViewBag.IsOwner = false;
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            var userInGroupVm = new UserInGroupVM();

            userInGroupVm.Id = userInGroupId;
            userInGroupVm.GroupId = user.GroupId;
            userInGroupVm.GroupName = user.Group.Name;
            userInGroupVm.Name = user.AspNetUsers.Name;

            var pair = CountSuccessPercentageForUser(user);
            userInGroupVm.SuccessPercentage = pair.Value;
            userInGroupVm.Appereances = pair.Key;

            ViewBag.UserInCategoryId = userInGroupId;

            return View(userInGroupVm);
        }



        public PartialViewResult CooperationGrid(int userInGroupId = 0)
        {

            ViewBag.UserInGroupId = userInGroupId;
            return PartialView("_CooperationGrid");
        }


        public ActionResult CooperationGrid_Read([DataSourceRequest] DataSourceRequest request, int userInGroupId)
        {
            var user = unitOfWork.UserInGroupRepository.GetByID(userInGroupId);
            var userInTeamList = unitOfWork.UserInTeamRepository.GetByUserInGroupId(userInGroupId);
            var userTeams = new List<Team>();
            foreach (var userInTeam in userInTeamList)
            {
                if (userInTeam.Team.SuccessPercentage != null)
                    userTeams.Add(userInTeam.Team);
            }

            //get all users from category
            var allUsers = unitOfWork.UserInGroupRepository.Get().ToList();
            allUsers.Remove(user);

            var cooperations = new List<CooperationVM>();
            //count cooperation factor for every user
            foreach (var userInGroup in allUsers)
            {
                var newUser = new CooperationVM
                {
                    UserId = userInGroup.Id,
                    UserPersonalName = userInGroup.AspNetUsers.Name,
                    SuccessPercentage = 0
                };
                var teamCount = 0;

                foreach (var userInCategory in userInGroup.UserInCategory)
                {
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
            }

            return Json(cooperations.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }


    }
}
