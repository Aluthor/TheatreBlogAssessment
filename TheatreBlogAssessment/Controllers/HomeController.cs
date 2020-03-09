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

        [HttpPost]
        public ActionResult Index(string SearchString)
        {
            var posts = context.Posts.Include(p => p.Category).Include(p => p.User).Where(p => p.Category.Name.Equals(SearchString.Trim())).OrderByDescending(p => p.DatePosted);
            ViewBag.Categories = context.Categories.ToList();
            return View(posts.ToList());
        }

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

        [Authorize(Roles = "Member, Staff, Admin")]
        public ActionResult CreateComment(int? id)
        {
            //ViewBag.PostId = new SelectList(db.Posts, "PostId", "Title");
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

        //redirects to staff controller
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
    }
    
}