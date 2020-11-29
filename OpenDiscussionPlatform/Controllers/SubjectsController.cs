using Microsoft.AspNet.Identity;
using OpenDiscussion.Models;
using OpenDiscussionPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenDiscussion.Controllers
{
    public class SubjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Subjects
        public ActionResult Index(int id)
        {
            SetAccessRightsSubjects();

            Category category = db.Categories.Find(id);
            var subjects = category.Subjects;
            ViewBag.Subjects = subjects;
            ViewBag.Category = category;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }

            return View();
        }

        // SHOW
        public ActionResult Show(int id)
        {
            SetAccessRightsSubjects();
            Subject subject = db.Subjects.Find(id);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }
            return View(subject);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult Show(Comment comm)
        {
            comm.Date = DateTime.Now;
            comm.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Comments.Add(comm);
                    db.SaveChanges();
                    TempData["message"] = "The comment has been added!";
                    return Redirect("/Subjects/Show/" + comm.SubjectId);
                }

                else
                {
                    Subject a = db.Subjects.Find(comm.SubjectId);
                    SetAccessRightsSubjects();
                    return View(a);
                }

            }

            catch (Exception e)
            {
                Subject a = db.Subjects.Find(comm.SubjectId);
                SetAccessRightsSubjects();
                return View(a);
            }

        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {

            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Categories select cat;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            return selectList;
        }


        // NEW cu select category
        [Authorize(Roles = "User")]
        public ActionResult NewSel()
        {
            Subject subject = new Subject();
            subject.Categ = GetAllCategories();
            subject.UserId = User.Identity.GetUserId();

            return View(subject);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult NewSel(Subject subject)
        {
            subject.Date = DateTime.Now;
            subject.Categ = GetAllCategories();
            subject.UserId = User.Identity.GetUserId();
        

            try
            {
                if (ModelState.IsValid)
                {
                    db.Subjects.Add(subject);
                    db.SaveChanges();
                    TempData["message"] = "The subject has been added!";
                    return Redirect("/Subjects/Index/" + subject.CategoryId);
                }
                else
                {
                    return View(subject);
                }
            }
            catch (Exception e)
            {
                return View(subject);
            }
        }


        //NEW
        [Authorize(Roles = "User")]
        public ActionResult New(int id)
        {
            ViewBag.id = id;
            var cat = db.Categories.Find(id);
            ViewBag.UserId = User.Identity.GetUserId();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult New(Subject subject)
        {
            subject.Date = DateTime.Now;
            ViewBag.id = subject.CategoryId;
            ViewBag.UserId = User.Identity.GetUserId();

            try
            {
                if (ModelState.IsValid)
                {
                    db.Subjects.Add(subject);
                    db.SaveChanges();
                    TempData["message"] = "The subject has been added!";
                    return Redirect("/Subjects/Index/" + subject.CategoryId);
                }
                else
                {
                    return View(subject);
                }
            }
            catch (Exception e)
            {
                return View(subject);
            }
        }

        //EDIT
        [Authorize(Roles = "User,Moderator")]
        public ActionResult Edit(int id)
        {
            SetAccessRightsSubjects();
            Subject subject = db.Subjects.Find(id);
            subject.Categ = GetAllCategories();

            if (subject.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator"))
            {
                return View(subject);
            }
            else
            {
                TempData["message"] = "You are not allowed to edit this subject";
                return Redirect("/Subjects/Index/" + subject.CategoryId);
            }
        }

        [HttpPut]
        [Authorize(Roles = "User,Moderator")]
        public ActionResult Edit(int id, Subject requestSubject)
        {
            requestSubject.Categ = GetAllCategories();
            SetAccessRightsSubjects();
            try
            {
                if (ModelState.IsValid)
                {
                    Subject subject = db.Subjects.Find(id);

                    if (subject.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator"))
                    {
                        if (TryUpdateModel(subject))
                        {
                            subject.Title = requestSubject.Title;
                            subject.Content = requestSubject.Content;
                            subject.Date = requestSubject.Date;
                            subject.CategoryId = requestSubject.CategoryId;
                            db.SaveChanges();
                            TempData["message"] = "The subject has been edited!";

                        }
                        return Redirect("/Subjects/Index/" + subject.CategoryId);
                    }
                    else
                    {
                        TempData["message"] = "You are not allowed to edit this subject";
                        return Redirect("/Subjects/Index/" + subject.CategoryId);
                    }
                }
                else
                {
                    return View(requestSubject);
                }
            }
            catch (Exception e)
            {
                return View(requestSubject);
            }
        }

        //DELETE
        [HttpDelete]
        [Authorize(Roles = "User,Moderator")]
        public ActionResult Delete(int id)
        {
            Subject subject = db.Subjects.Find(id);

            if (subject.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator"))
            {
                db.Subjects.Remove(subject);
                db.SaveChanges();
                TempData["message"] = "The subject has been deleted!";
                return Redirect("/Subjects/Index/" + subject.CategoryId);
            }
            else
            {
                TempData["message"] = "You are not allowed to delete this subject";
                return Redirect("/Subjects/Index/" + subject.CategoryId);
            }
        }

        private void SetAccessRightsSubjects()
        {
            ViewBag.esteModerator = User.IsInRole("Moderator");
            ViewBag.esteUser = User.IsInRole("User");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
        }


    }
}