using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TeamGeneratorApp.DAL;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.Controllers
{
    [Authorize]
    public class SubjectsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Subjects
        public ActionResult Index(int poolId)
        {
            var subjects = unitOfWork.SubjectRepository.GetByPoolId(poolId);

            return View(subjects.ToList());
        }

        // GET: Subjects/Details/5
        public ActionResult Details(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = unitOfWork.SubjectRepository.GetByID(id);

            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        //// GET: Subjects/Create
        public ActionResult Create()
        {
            return View();
        }

        //// POST: Subjects/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email")] Subject subject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.SubjectRepository.Insert(subject);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(subject);
        }

        //// GET: Subjects/Edit/5
        public ActionResult Edit(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            Subject subject = unitOfWork.SubjectRepository.GetByID(id);

            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        //// POST: Subjects/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email")] Subject subject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.SubjectRepository.Update(subject);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(subject);
        }

        //// GET: Subjects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Subject subject = unitOfWork.SubjectRepository.GetByID(id);

            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        //// POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Subject subject = unitOfWork.SubjectRepository.GetByID(id);

            try
            {
                unitOfWork.SubjectRepository.Delete(subject);
                unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.ConstraintError = "Unable to delete this item because it is used in other entities as foreign key.";
                return View("Delete", subject);
            }
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
