using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.Models;
using TeamGeneratorApp.Models.ViewModels;

namespace TeamGeneratorApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private string userId;

        public ActionResult Index()
        {
            userId = User.Identity.GetUserId();
            ViewBag.UserId = userId;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }


        public PartialViewResult InvitationsGrid()
        {
            userId = User.Identity.GetUserId();
            ViewBag.UserId = userId;
            return PartialView("_InvitationsGrid");
        }

        #region Invitations
        public ActionResult InvitationsGrid_Read([DataSourceRequest] DataSourceRequest request, string userId)
        {
            var res = unitOfWork.InvitationRepository.GetByUserId(userId).ToList();

            var list = new List<InvitationRequestVM>();
            foreach (var e in res)
            {
                var owner = unitOfWork.UserRepository.GetByID(e.Group.OwnerId);
                var invitationVm = new InvitationRequestVM();

                invitationVm.Id = e.Id;
                invitationVm.DateCreated = e.DateCreated;
                invitationVm.UserPersonalName = owner.Name;
                invitationVm.UserEmail = owner.Email;
                invitationVm.GroupName = e.Group.Name;

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

        [HttpPost]
        public ActionResult AcceptInvitation(int id)
        {
            var invitation = unitOfWork.InvitationRepository.GetByID(id);

            var userInGroup = new UserInGroup
            {
                GroupId = invitation.GroupId,
                UserId = invitation.UserId,
                Active = true
            };

            //var invitation = new Invitaton
            //{
            //    UserId = userId,
            //    DateCreated = DateTime.Now,
            //    GroupId = groupId,
            //    AspNetUsers = unitOfWork.UserRepository.GetByID(userId),
            //    Group = unitOfWork.GroupRepository.GetByID(groupId)
            //};
            try
            {
                unitOfWork.UserInGroupRepository.Insert(userInGroup);
                unitOfWork.InvitationRepository.Delete(id);
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                return Json(new { status = e.Message });
            }
            return Json(new { status = "OK" });


        }

        #endregion

    }
}