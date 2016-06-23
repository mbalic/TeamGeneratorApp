using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
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

            ViewBag.Votings = unitOfWork.UserVotingRepository.GetByUserIdAndActivity(userId, true).ToList().Count;
            ViewBag.Invitations = unitOfWork.InvitationRepository.GetByUserId(userId).ToList().Count;
            ViewBag.MyVotings = unitOfWork.VotingRepository.GetByOwnerIdAndActivity(userId, true).ToList().Count;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }



        #region InvitationsGrid

        public PartialViewResult InvitationsGrid()
        {
            userId = User.Identity.GetUserId();
            ViewBag.UserId = userId;
            return PartialView("_InvitationsGrid");
        }


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


        #region VotingsGrid

        public PartialViewResult VotingsGrid()
        {
            userId = User.Identity.GetUserId();
            ViewBag.UserId = userId;
            return PartialView("_VotingsGrid");
        }


        public ActionResult VotingsGrid_Read([DataSourceRequest] DataSourceRequest request, string userId)
        {
            var userVotings = unitOfWork.UserVotingRepository.GetByUserIdAndActivity(userId, true).ToList();

            var list = new List<VotingIndexVM>();
            foreach (var e in userVotings)
            {
                var votingVM = new VotingIndexVM
                {
                    Id = e.VotingId,
                    Name = e.Voting.Name,
                    EventName = e.Voting.Event.Name,
                    StartVoting = e.Voting.StartVoting,
                    FinishVoting = e.Voting.FinishVoting,
                    VotesLeft = e.Voting.VotesPerUser - e.VoteCounter
                };
                
                list.Add(votingVM);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region MyVotingsGrid

        public PartialViewResult MyVotingsGrid()
        {
            userId = User.Identity.GetUserId();
            ViewBag.UserId = userId;
            return PartialView("_MyVotingsGrid");
        }


        public ActionResult MyVotingsGrid_Read([DataSourceRequest] DataSourceRequest request, string userId)
        {
            var voting = unitOfWork.VotingRepository.GetByOwnerIdAndActivity(userId, null).ToList();

            var list = new List<VotingVM>();
            foreach (var e in voting)
            {
                var votingVM = new VotingVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    EventId = e.EventId,
                    EventName = e.Event.Name,
                    StartVoting = e.StartVoting,
                    FinishVoting = e.FinishVoting,
                    Active = e.Active,
                    VotesPerUser = e.VotesPerUser
                };

                list.Add(votingVM);
            }

            return Json(list.OrderBy(p => p.Active).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MyVotingsGrid_Update([DataSourceRequest] DataSourceRequest request,
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
                        return JavaScript("location.reload(true)");
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