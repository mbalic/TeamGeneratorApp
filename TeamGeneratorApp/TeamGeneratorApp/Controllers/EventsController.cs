using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.Models;

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

            return View(@event);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
