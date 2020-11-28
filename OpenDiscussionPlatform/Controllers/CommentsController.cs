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
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }

        //NEW

        /*[HttpPost]
        public ActionResult New(Comment comment)
        {
            comment.Date = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Comments.Add(comment);
                    db.SaveChanges();
                    TempData["message"] = "The comment has been added!";
                    return Redirect("/Subjects/Show/" + comment.SubjectId);
                }
                else
                {
                    return Redirect("/Subjects/Show/" + comment.SubjectId);
                }
            }
            catch (Exception e)
            {
                return Redirect("/Subjects/Show/" + comment.SubjectId);
            }
        }*/

        //EDIT
        [Authorize(Roles = "User, Moderator")]
        public ActionResult Edit(int id)
        {
            Comment comment = db.Comments.Find(id);

            if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator"))
            {
                return View(comment);
            }
            else
            {
                TempData["message"] = "You are not allowed to edit this comment";
                return Redirect("/Subjects/Show/" + comment.SubjectId);
            }
        }

        [HttpPut]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Comment comment = db.Comments.Find(id);

                    if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator"))
                    {
                        if (TryUpdateModel(comment))
                        {
                            comment.CommContent = requestComment.CommContent;
                            db.SaveChanges();
                            TempData["message"] = "The comment has been edited!";

                        }
                        return Redirect("/Subjects/Show/" + comment.SubjectId);
                    }
                    else
                    {
                        TempData["message"] = "You are not allowed to edit this comment";
                        return Redirect("/Subjects/Show/" + comment.SubjectId);
                    }
                }
                else
                {
                    return View(requestComment);
                }
            }
            catch (Exception e)
            {
                return View(requestComment);
            }
        }

        //DELETE
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Comment comment = db.Comments.Find(id);

            if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator"))
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
                TempData["message"] = "The comment has been deleted!";
                return Redirect("/Subjects/Show/" + comment.SubjectId);
            }
            else
            {
                TempData["message"] = "You are not allowed to delete this comment";
                return Redirect("/Subjects/Show/" + comment.SubjectId);
            }
        }
    }
}