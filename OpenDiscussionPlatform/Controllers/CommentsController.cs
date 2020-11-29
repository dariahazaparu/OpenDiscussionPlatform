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


        //EDIT
        [Authorize(Roles = "User")]
        public ActionResult Edit(int id)
        {
            Comment comment = db.Comments.Find(id);

            if (comment.UserId == User.Identity.GetUserId())
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
        [Authorize(Roles = "User")]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Comment comment = db.Comments.Find(id);

                    if (comment.UserId == User.Identity.GetUserId())
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
        [Authorize(Roles = "User,Moderator")]
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