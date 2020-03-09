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

        // GET: Staff
        [Authorize(Roles = "Staff, Admin")]
        public ActionResult Index()
        {
            var posts = db.Posts.Include(p => p.Category).Include(p => p.User);
            var userId = User.Identity.GetUserId();
            posts = posts.Where(p => p.UserId == userId);
            return View(posts.ToList());
        }

        // GET: Staff/Details/5
        public ActionResult Details(int? id) //? Creates a nullable variable
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = db.Posts.Find(id);

            if(post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Staff/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId, Title, Content, CategoryId")] Post post)
        {
            if(ModelState.IsValid)
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

        // GET: Staff/Edit/5
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

        // POST: Staff/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "PostId, Title, Content, CategoryId")] Post post)
        {

            if (ModelState.IsValid)
            {
                post.DatePosted = DateTime.Now;
                post.UserId = User.Identity.GetUserId();
                db.Entry(post).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId);
            return View(post);
        }

        // GET: Staff/Delete/5
        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = db.Posts.Find(id);
            var category = db.Categories.Find(post.CategoryId);
            post.Category = category;

            if(post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // POST: Staff/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);

            //probably going to have to loop here to delete all comments

            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
