using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TheatreBlogAssessment.Models;
using System.Data;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TheatreBlogAssessment.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public TheatreDbContext db = new TheatreDbContext();

        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryId, Name")] Category category)
        {
            if(ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("ViewAllCategories");
            }
            return View(category);
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryId, Name")] Category category)
        {
           if(ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewAllCategories");
            }
            return View(category);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if(category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("ViewAllCategories");
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing); 
        }

        public ActionResult ViewAllCategories()
        {
            return View(db.Categories.ToList());
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = db.Categories.Find(id);
            if(category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        public ActionResult DetailsPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post= db.Posts.Find(id);
            if (post== null)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Details", "Home", new { id = post.PostId });
        }

        //******************************Posts********************************
        [Authorize(Roles = "Admin")]
        public ActionResult ViewAllPosts()
        {
            List<Post> posts = db.Posts.Include(p => p.Category).Include(p => p.User).ToList();
            return View(posts);
        }
        [HttpPost]
        public ActionResult ViewAllPosts(string SearchString)
        {
            var posts = db.Posts.Include(p => p.Category).Include(p => p.User).Where(p => p.Category.Name.Equals(SearchString.Trim())).OrderByDescending(p => p.DatePosted);
            ViewBag.Categories = db.Categories.ToList();
            return View(posts.ToList());
        }

        //GET: Posts/Delete/5
        public ActionResult DeletePost(int? id)
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

        [HttpPost, ActionName("DeletePost")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePostConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("ViewAllPosts");
        }

        public ActionResult DeleteComment(int? id)
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

            return RedirectToAction("DeleteComment", "Home", new { id = comment.CommentId});
        }

        //************************USERS****************************
        [Authorize(Roles = "Admin")]
        public ActionResult ViewUsers()
        {
            List<User> users = db.Users.Include(u => u.Roles).OrderBy(u => u.LastName).ToList();

            return View(users);
        }

        //HttpGET ChangeRole Action
        public async Task<ActionResult> ChangeRole(string id)
        {
            UserManager<User> userManager = new UserManager<User>(new UserStore<User>(db));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(id == User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }
            
            User user = await userManager.FindByIdAsync(id);
            string oldRole = (await userManager.GetRolesAsync(id)).Single();

            var items = db.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == oldRole
            }).ToList();

            return View(new ChangeRoleViewModel
            {
                UserName = user.UserName,
                Roles = items,
                OldRole = oldRole,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangeRole")]
        public async Task<ActionResult> ChangeRoleConfirmed(string id, [Bind(Include ="Role")] ChangeRoleViewModel model)
        {
            UserManager<User> userManager = new UserManager<User>(new UserStore<User>(db));

            if (id == User.Identity.GetUserId())
            {
                //Flash.Instance.Error("Error", "You cannot change your own role");
                return RedirectToAction("Index");
            }
            if(ModelState.IsValid)
            {
                User user = await userManager.FindByIdAsync(id);
                string oldRole = (await userManager.GetRolesAsync(id)).Single();

                if (oldRole == model.Role)
                {
                    //[Flash error]
                    return RedirectToAction("Index");
                }

                await userManager.RemoveFromRoleAsync(id, oldRole);
                await userManager.AddToRoleAsync(id, model.Role);

                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
