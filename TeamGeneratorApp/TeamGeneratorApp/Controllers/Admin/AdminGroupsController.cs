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

namespace TeamGeneratorApp.Controllers.Admin
{
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class AdminGroupsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
      
        #region OldCode
/*
        // GET: AdminGroups
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

            var groups = unitOfWork.GroupRepository.Get();

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (ddlFilter)
                {
                    case "Name":
                        groups = groups.Where(s => s.Name != null);
                        groups = groups.Where(s => s.Name.Contains(searchString));
                        break;
                    case "Owner":
                        groups = groups.Where(s => s.AspNetUsers != null);
                        groups = groups.Where(s => s.AspNetUsers.UserName.Contains(searchString));
                        break;
                    default:
                        groups = groups.Where(s => s.Name != null);
                        groups = groups.Where(s => s.Name.Contains(searchString));
                        break;
                }
            }


            switch (sortOrder)
            {
                case "Name_desc":
                    groups = groups.OrderByDescending(s => s.Name);
                    break;
                case "Owner":
                    groups = groups.OrderBy(s => s.AspNetUsers.UserName);
                    break;
                case "Owner_desc":
                    groups = groups.OrderByDescending(s => s.AspNetUsers.UserName);
                    break;
                default:
                    groups = groups.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(groups.ToPagedList(pageNumber, pageSize));
        }

      
        //// GET: AdminGroups/Create
        public ActionResult Create()
        {
            PopulateOwnerDropDown();
            return View();
        }

        //// POST: AdminGroups/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,OwnerId")] Group group)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.GroupRepository.Insert(group);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            PopulateOwnerDropDown(group.OwnerId);

            return View(group);
        }

        //// GET: AdminGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Group group = unitOfWork.GroupRepository.GetByID(id);

            if (group == null)
            {
                return HttpNotFound();
            }

            PopulateOwnerDropDown(group.OwnerId);

            return View(group);
        }

        //// POST: AdminGroups/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,OwnerId")] Group group)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.GroupRepository.Update(group);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            PopulateOwnerDropDown(group.OwnerId);

            return View(group);
        }

        //// GET: AdminGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = unitOfWork.GroupRepository.GetByID(id);

            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        //// POST: AdminGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = unitOfWork.GroupRepository.GetByID(id);

            try
            {
                unitOfWork.GroupRepository.Delete(group);
                unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.ConstraintError = "Unable to delete this item because it is used in other entities as foreign key.";
                return View("Delete", group);
            }
        }
        */
#endregion


        public ActionResult Index()
        {
            //userId = User.Identity.GetUserId(); 

            ViewData["owners"] =  unitOfWork.UserRepository.Get().AsQueryable()
                     .Select(e => new OwnerVM
                     {
                         Id = e.Id,
                         Name = e.Name
                     })
                     .OrderBy(e => e.Name);

            return View();
        }


        #region IndexGrid

        public ActionResult AdminGroupsGrid_Read([DataSourceRequest] DataSourceRequest request)
        {

            var res = unitOfWork.GroupRepository.Get().ToList();
            
            var list = new List<AdminGroupsIndexVM>();
            foreach (var e in res)
            {
                var newVm = new AdminGroupsIndexVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    OwnerId = e.OwnerId,
                    Owner = new OwnerVM { Id = e.AspNetUsers.Id, Name = e.AspNetUsers.Name}
                };

                list.Add(newVm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdminGroupsGrid_Create([DataSourceRequest] DataSourceRequest request,
           [Bind(Prefix = "models")]IEnumerable<AdminGroupsIndexVM> list)
        {
            var results = new List<AdminGroupsIndexVM>();
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
        public ActionResult AdminGroupsGrid_Update([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<AdminGroupsIndexVM> list)
        {
            var results = new List<AdminGroupsIndexVM>();
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
        public ActionResult AdminGroupsGrid_Destroy([DataSourceRequest] DataSourceRequest request,
         [Bind(Prefix = "models")]IEnumerable<AdminGroupsIndexVM> list)
        {
            var results = new List<AdminGroupsIndexVM>();
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


        //GET: AdminGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = unitOfWork.GroupRepository.GetByID(id);

            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
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

    }
}

