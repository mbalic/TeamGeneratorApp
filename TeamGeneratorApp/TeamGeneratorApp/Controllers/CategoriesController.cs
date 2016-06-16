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

            Category category = unitOfWork.CategoryRepository.GetByID(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            ViewBag.GroupId = category.GroupId;
            return View(category);
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



        public PartialViewResult UsersGrid(int categoryId = 0)
        {
            var category = unitOfWork.CategoryRepository.GetByID(categoryId);

            ViewData["users"] = unitOfWork.UserInGroupRepository.GetByGroupIdAndActivity(category.GroupId, true).AsQueryable()
                  .Select(e => new UserddlVM
                  {
                      Id = e.Id,
                      Name = e.AspNetUsers.Name
                  })
                  .OrderBy(e => e.Name);


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
                var newItem = new UserCategoryVM
                {
                    Id = e.Id,
                    UserInGroupId = e.UserInGroupId,
                    CategoryId = e.CategoryId,
                    UserPersonalName = e.UserInGroup.AspNetUsers.Name,
                    Rating = e.Rating,
                    Active = e.Active                 

                    //PositionName = e.PositionInCategory == null? null : e.PositionInCategory.Name

                };
                list.Add(newItem);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
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



        public PartialViewResult PositionsGrid(int categoryId = 0)
        {

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


    }
}
