using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheatreBlogAssessment.Models;
using System.Data.Entity;
using System.Net;
using Microsoft.AspNet.Identity;

namespace TheatreBlogAssessment.Controllers
{
    public class HomeController : Controller
    {
        private TheatreDbContext context = new TheatreDbContext();

        // GET: Home
        public ActionResult Index()
        {
            var posts = context.Posts.Include(p => p.Category).Include(p => p.User).OrderByDescending(p => p.DatePosted);
            ViewBag.Categories = context.Categories.ToList();
            return View(posts.ToList());
        }

        //POST: Home
        [HttpPost]
        public ActionResult Index(string SearchString)
        {
            var posts = context.Posts.Include(p => p.Category).Include(p => p.User).Where(p => p.Category.Name.Equals(SearchString.Trim())).OrderByDescending(p => p.DatePosted);
            ViewBag.Categories = context.Categories.ToList();
            return View(posts.ToList());
        }

        //GET: Home/Details
        public ActionResult Details(int id)
        {
            Post post = context.Posts.Find(id);

            var user = context.Users.Find(post.UserId);
            var category = context.Categories.Find(post.CategoryId);
            var comments = context.Comments.Include(c => c.User);

            post.User = user;
            post.Category = category;
            post.Comments = comments.ToList();

            return View(post);
        }

        //GET create comment action, all users except anonymous are authorized
        [Authorize(Roles = "Member, Staff, Admin")]
        public ActionResult CreateComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = context.Posts.Find(id);
            Comment comment = new Comment();

            comment.PostId = post.PostId;

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }

        // POST: Member/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles =("Member, Staff, Admin"))]
        public ActionResult CreateComment(Comment comment, int id) 
        {
            if (ModelState.IsValid)
            {
                comment.CommentDate = DateTime.Now;
                comment.UserId = User.Identity.GetUserId();
                if (User.IsInRole("Admin"))
                {
                    comment.IsAproved = true;
                }
                else
                {
                    comment.IsAproved = false;
                }

                comment.HasBeenEdited = false;

                Post post = context.Posts.Find(id);
                comment.Post = post;

                context.Comments.Add(comment);
                context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(comment);
        }
        //CHANGES 
        //redirects to staff controller(edit post)
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = context.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Edit", "Staff", new { id = post.PostId });
        }

        [Authorize(Roles = "Staff, Admin")]
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = context.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveConfirmed(int id)
        {
            Comment comment = context.Comments.Find(id);
            comment.IsAproved = true;
            context.SaveChanges();
            return RedirectToAction("Index");
            

        }
        [HttpGet]
        public ActionResult EditComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = context.Comments.Find(id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            //ViewBag.CategoryId = new SelectList(context.Categories, "CategoryId", "Name", post.CategoryId);
            return View(comment);
        }

        // POST: Staff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment(Comment comment)
        {

            if (ModelState.IsValid)
            {
                comment.CommentDate = DateTime.Now;
                comment.HasBeenEdited = true;
                comment.IsAproved = false;

                comment.UserId = User.Identity.GetUserId();

                Post post = context.Posts.Find(comment.PostId);
                User user = context.Users.Find(comment.UserId);
                comment.Post = post;
                comment.User = user;

                context.Entry(comment).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId);
            return View(comment);

        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = context.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Delete", "Staff", new { id = post.PostId });
        }

        [HttpGet]
        public ActionResult DeleteComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = context.Comments.Find(id);
            var post = context.Posts.Find(comment.PostId);
            comment.Post = post;

            if (comment== null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }

        // POST: Staff/Delete/5
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCommentConfirmed(int id)
        {
            Comment comment = context.Comments.Find(id);
            context.Comments.Remove(comment);
            

            //probably going to have to loop here to delete all comments

            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}