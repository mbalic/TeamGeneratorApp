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
        
        // GET: Pools
        public ActionResult Index()
        {
            ViewBag.UserId = User.Identity.GetUserId();
            return View();
        }

        #region GroupsGrid

        public ActionResult GroupsGrid_Read([DataSourceRequest] DataSourceRequest request, string userId)
        {
            var res = unitOfWork.GroupRepository.GetByUserId(userId).ToList();

            var list = new List<GroupListVM>();
            foreach (var e in res)
            {
                var newVm = new GroupListVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    OwnerId = e.OwnerId
                };

                list.Add(newVm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GroupsGrid_Create([DataSourceRequest] DataSourceRequest request,
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
                        unitOfWork.GroupRepository.Insert(newModel);
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
        public ActionResult GroupsGrid_Update([DataSourceRequest] DataSourceRequest request,
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
                        unitOfWork.GroupRepository.Update(newModel);
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
        
            ViewBag.GroupName = group.Name;
            return View(group);
        }

        //// GET: Pools/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    Group group = unitOfWork.GroupRepository.GetByID(id);

        //    if (group == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    userId = User.Identity.GetUserId();
        //    ViewBag.UserId = userId;

        //    return View(group);
        //}

        //// POST: Pools/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Name,Description,OwnerId")] Group group)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            unitOfWork.GroupRepository.Update(group);
        //            unitOfWork.Commit();
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
        //    }

        //    userId = User.Identity.GetUserId();
        //    ViewBag.UserId = userId;

        //    return View(group);
        //}

        ////// GET: Pools/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    Group group = unitOfWork.GroupRepository.GetByID(id);

        //    if (group == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(group);
        //}

        ////// POST: Pools/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Group group = unitOfWork.GroupRepository.GetByID(id);

        //    try
        //    {
        //        unitOfWork.GroupRepository.Delete(group);
        //        unitOfWork.Commit();
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.ConstraintError = "Unable to delete this item because it is used in other entities as foreign key.";
        //        return View("Delete", group);
        //    }
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }


        public PartialViewResult CategoriesGrid(int groupId = 0)
        {

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



        public PartialViewResult UsersInGroupGrid(int groupId = 0)
        {

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
                var userInGroupVm = new UserInGroupVM
                {
                    Id = e.Id,
                    Name = e.AspNetUsers.UserName,
                    UserId = e.UserId,
                    Email = e.AspNetUsers.Email,
                    GroupId = e.GroupId,
                    Active = e.Active
                };
                list.Add(userInGroupVm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult UsersInGroupGrid_Create([DataSourceRequest] DataSourceRequest request,
        //   [Bind(Prefix = "models")]IEnumerable<UserInGroupVM> list)
        //{
        //    var results = new List<UserInGroupVM>();
        //    if (list != null && ModelState.IsValid)
        //    {
        //        foreach (var e in list)
        //        {
        //            var newUserInGroup = new UserInGroup
        //            {
        //                UserId = e.UserId,
        //                GroupId = e.GroupId,
        //                Active = e.Active
        //            };
        //            try
        //            {
        //                unitOfWork.UserInGroupRepository.Insert(newUserInGroup);
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UsersInGroupGrid_Update([DataSourceRequest] DataSourceRequest request,
           [Bind(Prefix = "models")]IEnumerable<UserInGroupVM> list)
        {
            var results = new List<UserInGroupVM>();
            if (list != null && ModelState.IsValid)
            {
                foreach (var e in list)
                {
                    var newUserInGroup = new UserInGroup
                    {
                        Id = e.Id,
                        UserId = e.UserId,
                        GroupId = e.GroupId,
                        Active = e.Active
                    };
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

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult UsersInGroupGrid_Destroy([DataSourceRequest] DataSourceRequest request,
        // [Bind(Prefix = "models")]IEnumerable<UserInGroupVM> list)
        //{
        //    var results = new List<UserInGroupVM>();
        //    if (list != null && ModelState.IsValid)
        //    {
        //        foreach (var e in list)
        //        {
        //            var newUserInGroup = new UserInGroup
        //            {
        //                Id = e.Id,
        //                UserId = e.UserId,
        //                GroupId = e.GroupId,
        //                Active = e.Active
        //            };
        //            try
        //            {
        //                unitOfWork.UserInGroupRepository.Delete(newUserInGroup);
        //                unitOfWork.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                ViewBag.ConstraintError = "There was an error while deleting rows in grid.";
        //            }
        //        }
        //    }

        //    return Json(results.ToDataSourceResult(request, ModelState));
        //}

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
                    //var newInvitation = new Invitaton
                    //{
                    //    Id = e.Id,
                    //    UserId = e.UserId,
                    //    GroupId = e.GroupId,
                    //    DateCreated = e.DateCreated
                    //};
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








        #region Old
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

        #endregion
    }
}
