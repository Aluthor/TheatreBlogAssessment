using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheatreBlogAssessment.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Net;

namespace TheatreBlogAssessment.Controllers
{
    public class StaffController : Controller
    {
        private TheatreDbContext db = new TheatreDbContext();

        /// <summary>
        /// HttpGet action which returns a view with a list of posts made by the currently logged in user
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Staff, Admin")]
        public ActionResult Index()
        {
            var posts = db.Posts.Include(p => p.Category).Include(p => p.User);
            var userId = User.Identity.GetUserId(); //gets the currently logged in users userid
            posts = posts.Where(p => p.UserId == userId); //searches posts by userid to find all made by the current user
            return View(posts.ToList());
        }

        /// <summary>
        /// HttpGet action which returns the details of a selected post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id) //? Creates a nullable variable
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = db.Posts.Find(id);

            if (post == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(post.UserId);
            var category = db.Categories.Find(post.CategoryId);
            var comments = db.Comments.Include(c => c.User);

            post.User = user;
            post.Category = category;
            post.Comments = comments.ToList();

            return View(post);
        }

        /// <summary>
        /// HttpGet action to create a post
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View();
        }

        /// <summary>
        /// HttpPost action which saves the new post to the database
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId, Title, Content, CategoryId")] Post post)
        {
            if (post.Content == null || post.Title==null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    post.DatePosted = DateTime.Now;
                    post.UserId = User.Identity.GetUserId();

                    db.Posts.Add(post);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId);
                return View(post);
            }
        }

        /// <summary>
        /// HttpGet action to edit an existing post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = db.Posts.Find(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId);
            return View(post);
        }

        /// <summary>
        /// HttpPost action which saves the changes to a post to the database
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit([Bind(Include = "PostId, Title, Content, CategoryId")] Post post)
        {

            if (ModelState.IsValid)
            {
                post.DatePosted = DateTime.Now;//updates the time the post was made
                post.UserId = User.Identity.GetUserId();
                db.Entry(post).State = EntityState.Modified;//sets the entry state to modified

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId);
            return View(post);
        }

        /// <summary>
        /// HttpGet action to delete a post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = db.Posts.Find(id);
            var category = db.Categories.Find(post.CategoryId);
            post.Category = category;

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        /// <summary>
        /// HttpPost action which removes the post from the database after its deletion has been confirmed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);

            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// HttpGet action which redirects to the HomeControllers Approve comment 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Edit", "Home", new { id = comment.CommentId });
        }

        
    }
    }

