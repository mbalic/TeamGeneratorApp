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
using PagedList;


namespace TeamGeneratorApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminCategoriesController : Controller
    {
        
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: AdminCategories
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string ddlFilter = "Name")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.ParentSortParm = sortOrder == "Parent" ? "Parent_desc" : "Parent";

            var columnList = new List<string>();
            columnList.Add("Name");
            columnList.Add("Parent");

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

            var category = unitOfWork.CategoryRepository.Get();

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (ddlFilter)
                {
                    case "Name":
                        category = category.Where(s => s.Name != null);
                        category = category.Where(s => s.Name.Contains(searchString));
                        break;
                    case "Parent":
                        category = category.Where(s => s.Category2 != null);
                        category = category.Where(s => s.Category2.Name.Contains(searchString));
                        break;
                    default:
                        category = category.Where(s => s.Name != null);
                        category = category.Where(s => s.Name.Contains(searchString));
                        break;
                }
            }


            switch (sortOrder)
            {
                case "Name_desc":
                    category = category.OrderByDescending(s => s.Name);
                    break;
                case "Parent":
                    category = category.OrderBy(s => s.Category2.Name);
                    break;
                case "Parent_desc":
                    category = category.OrderByDescending(s => s.Category2.Name);
                    break;
                default:
                    category = category.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(category.ToPagedList(pageNumber, pageSize));
        }

        // GET: AdminCategories/Details/5
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
            return View(category);
        }

        //// GET: AdminCategories/Create
        public ActionResult Create()
        {
            PopulateParentDropDown();
            return View();
        }

        //// POST: AdminCategories/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,ParentId")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.CategoryRepository.Insert(category);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            PopulateParentDropDown(category.ParentId);
            return View(category);
        }

        //// GET: AdminCategories/Edit/5
        public ActionResult Edit(int? id)
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

            PopulateParentDropDown(category.ParentId);
            return View(category);
        }

        //// POST: AdminCategories/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,ParentId")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.CategoryRepository.Update(category);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            PopulateParentDropDown(category.ParentId);
            return View(category);
        }

        //// GET: AdminCategories/Delete/5
        public ActionResult Delete(int? id)
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
            return View(category);
        }

        //// POST: AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = unitOfWork.CategoryRepository.GetByID(id);

            try
            {
                unitOfWork.CategoryRepository.Delete(category);
                unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.ConstraintError = "Unable to delete this item because it is used in other entities as foreign key.";
                return View("Delete", category);
            }
        }

        private void PopulateParentDropDown(object selectedItem = null)
        {
            var items = unitOfWork.CategoryRepository.Get().ToList();
            items.Insert(0, null);
            ViewBag.ParentId = new SelectList(items, "Id", "Name", selectedItem);
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
