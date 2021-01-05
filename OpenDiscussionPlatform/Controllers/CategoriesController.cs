using Microsoft.AspNet.Identity;
using OpenDiscussion.Models;
using OpenDiscussionPlatform.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenDiscussion.Controllers
{
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories
        public ActionResult Index()
        {
            SetAccessRightsCategories();

            var categories = db.Categories;
            ViewBag.Categories = categories;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }

            IDictionary<Category, int> nrSubjects = new Dictionary<Category, int>();
            foreach (var i in categories)
            {
                List<int> subjectIds = db.Subjects.Where(s => s.CategoryId == i.CategoryId).Select(a => a.SubjectId).ToList(); 
                    nrSubjects.Add(i, subjectIds.Count());
            }
            ViewBag.nrSubjects = nrSubjects;

            return View();
        }

        //NEW
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Category category, HttpPostedFileBase uploadedFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (uploadedFile != null)
                    {
                        string uploadedFileName = uploadedFile.FileName;
                        string uploadedFileExtension = Path.GetExtension(uploadedFileName);

                        if (uploadedFileExtension == ".png" || uploadedFileExtension == ".jpg")
                        {
                            // Se stocheaza fisierul in folderul Files

                            string uploadFolderPath = Server.MapPath("~//Files//");

                            uploadedFile.SaveAs(uploadFolderPath + uploadedFileName);

                            FileUpload file = new FileUpload();
                            file.Extension = uploadedFileExtension;
                            file.FileName = uploadedFileName;
                            file.FilePath = uploadFolderPath + uploadedFileName;

                            db.FileUploads.Add(file);
                            db.SaveChanges();

                            category.CategoryPicture = file.FileId;
                            category.File = file;
                        }
                    }
                    db.Categories.Add(category);
                    db.SaveChanges();
                    TempData["message"] = "A new country has been added!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(category);
                }
            }
            catch (Exception e)
            {
                return View(category);
            }
        }

        //EDIT
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Category requestCategory, HttpPostedFileBase reqUploadedFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category category = db.Categories.Find(id);
                    if (TryUpdateModel(category))
                    {
                        if (reqUploadedFile != null)
                        {
                            string uploadedFileName = reqUploadedFile.FileName;
                            string uploadedFileExtension = Path.GetExtension(uploadedFileName);

                            if (uploadedFileExtension == ".png" || uploadedFileExtension == ".jpg")
                            {
                                string uploadFolderPath = Server.MapPath("~//Files//");

                                reqUploadedFile.SaveAs(uploadFolderPath + uploadedFileName);

                                if (category.File != null)
                                {
                                    var file = db.FileUploads.Find(category.CategoryPicture);
                                    file.FileName = uploadedFileName;
                                    file.Extension = uploadedFileExtension;
                                    file.FilePath = uploadFolderPath + uploadedFileName;

                                    category.File = file;
                                    db.SaveChanges();

                                }
                                else
                                {
                                    FileUpload file = new FileUpload();
                                    file.Extension = uploadedFileExtension;
                                    file.FileName = uploadedFileName;
                                    file.FilePath = uploadFolderPath + uploadedFileName;

                                    db.FileUploads.Add(file);
                                    db.SaveChanges();

                                    category.CategoryPicture = file.FileId;
                                    category.File = file;
                                }
                            }
                        }

                        category.CategoryName = requestCategory.CategoryName;
                        db.SaveChanges();
                        TempData["message"] = "The country has been edited!";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestCategory);
                }
            }
            catch (Exception e)
            {
                return View(requestCategory);
            }
        }

        //DELETE
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            FileUpload file = db.FileUploads.Find(category.CategoryPicture);
            db.Categories.Remove(category);
            db.FileUploads.Remove(file);
            db.SaveChanges();
            TempData["message"] = "The country has been deleted!";
            return RedirectToAction("Index");
        }

        private void SetAccessRightsCategories()
        {
            ViewBag.afisareButoane = false;
            if (User.IsInRole("User"))
            {
                ViewBag.afisareButoane = true;
            }
            ViewBag.esteUser = User.IsInRole("User");
            ViewBag.esteAdmin = User.IsInRole("Admin");
        }
    }
}