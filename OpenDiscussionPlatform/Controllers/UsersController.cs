using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OpenDiscussionPlatform.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenDiscussionPlatform.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        [Authorize(Roles = "Admin")]
        public ActionResult All()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;
            ViewBag.UsersList = users;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View();
        }

        //Get
        public ActionResult Info(string id)
        {
            SetAccessRightsUsers();
            if (id == User.Identity.GetUserId())
            {
                return Redirect("/Users/Info/Profile");
            }
            if (id == "Profile")
            {
                id = User.Identity.GetUserId();
            } // genial ???????
            ApplicationUser currUser = db.Users.Find(id);
            ViewBag.user = currUser;

            currUser.AllRoles = GetAllRoles();
            var userRole = currUser.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;

            var subjects = db.Subjects.Where(s => s.UserId == id);
            ViewBag.Subjects = subjects;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View(currUser);

        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public ActionResult Info(string id, ApplicationUser newData)
        {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;

            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


                if (TryUpdateModel(user))
                {
                    var roles = from role in db.Roles select role;
                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(id, role.Name);
                    }

                    var selectedRole = db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
                    UserManager.AddToRole(id, selectedRole.Name);

                    db.SaveChanges();

                    TempData["message"] = "The role has been edited!";
                }
                return RedirectToAction("Info");
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                newData.Id = id;
                return View(newData);
            }
        }

        private void SetAccessRightsUsers()
        {
            ViewBag.esteModerator = User.IsInRole("Moderator");
            ViewBag.esteUser = User.IsInRole("User");
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
        }

        [Authorize(Roles = "User, Moderator, Admin")]
        public ActionResult Edit(string id)
        {
            SetAccessRightsUsers();
            if (id == "Profile")
            {
                id = User.Identity.GetUserId();
            }
            var user = db.Users.Find(id);

            if (user.Id == User.Identity.GetUserId())
            {
                return View(user);
            }
            else
            {
                TempData["message"] = "You are not allowed to edit this profile";
                return Redirect("/Users/Info/Profile");
            }
        }

        [HttpPut]
        [Authorize(Roles = "User, Moderator, Admin")]
        [ValidateInput(false)]
        public ActionResult Edit (string id, ApplicationUser reqUser, HttpPostedFileBase reqUploadedFile)
        {
            SetAccessRightsUsers();
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == "Profile")
                    {
                        id = User.Identity.GetUserId();
                    }
                    var user = db.Users.Find(id);

                    if(user.Id == User.Identity.GetUserId() )
                    {
                        if (TryValidateModel(user))
                        {
                            //Debug.WriteLine(reqUploadedFile.FileName);
                            //if (reqUploadedFile != null)
                            //{
                                //Debug.WriteLine(user.PhotoId);
                                //string uploadedFileName = reqUploadedFile.FileName;
                                //string uploadedFileExtension = Path.GetExtension(uploadedFileName);

                                //if (uploadedFileExtension == ".png" || uploadedFileExtension == ".jpg")
                                //{
                                //    if (user.PhotoId != 0)
                                //    {
                                //        // edit
                                //        var file = db.FileUploads.Find(user.PhotoId);
                                //        file.FileName = uploadedFileName;
                                //        file.Extension = uploadedFileExtension;
                                //        file.FilePath = Server.MapPath("~//Files//") + uploadedFileName;

                                //        user.Photo = file;
                                //        db.SaveChanges();
                                //    }
                                //    else
                                //    {
                                //        //add
                                //        string uploadedFolderPath = Server.MapPath("~//Files//");
                                //        reqUploadedFile.SaveAs(uploadedFolderPath + uploadedFileName);

                                //        FileUpload file = new FileUpload();
                                //        file.Extension = uploadedFileExtension;
                                //        file.FileName = uploadedFileName;
                                //        file.FileName = uploadedFolderPath + uploadedFileName;
                                        
                                //        db.FileUploads.Add(file);
                                //        db.SaveChanges();

                                //        user.PhotoId = file.FileId;
                                //        user.Photo = file;
                                //    }
                                //}
                            //}
                            user.School = reqUser.School;
                            user.Description = reqUser.Description;
                            user.City = reqUser.City;
                            user.FavFood = reqUser.FavFood;
                            user.FullName = reqUser.FullName;
                            db.SaveChanges();
                            TempData["message"] = "The profile has been edited!";
                        }
                        return Redirect("/Users/Info/Profile");
                    }
                    else
                    {
                        TempData["message"] = "You are not allowed to edit this profile.";
                        return Redirect("/Users/Info/Profile");
                    }
                }
                else
                {
                    return View(reqUser);
                }
            }
            catch (Exception e)
            {
                return View(reqUser);
            }
        }

        [HttpDelete] 
        public ActionResult Delete(string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var user = UserManager.Users.FirstOrDefault(u => u.Id == id);

            var subjects = db.Subjects.Where(a => a.UserId == id);
            foreach (var subject in subjects)
            {
                db.Subjects.Remove(subject);
            }

            var comments = db.Comments.Where(comm => comm.UserId == id);
            foreach (var comment in comments)
            {
                db.Comments.Remove(comment);
            }

            db.SaveChanges();
            UserManager.Delete(user);

            
            TempData["message"] = "The user has been deleted!";

            return Redirect("/Users/All");
            
        }
    }
}