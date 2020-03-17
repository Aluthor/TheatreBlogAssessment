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
    //only admin users can access AdminController actions
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public TheatreDbContext db = new TheatreDbContext();

        /// <summary>
        /// HttpGet action which returns the Admin controls page
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// HttpGet action which returns the view to create a category
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// HttpPost action which takes in the users input and adds it to the database as a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryId, Name")] Category category)
        {
            if(category.Name == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    return RedirectToAction("ViewAllCategories");
                }
                return View(category);
            }
        }

        /// <summary>
        /// HttpGet action to edit an existing category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id); //finds the category in the db by id
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        /// <summary>
        /// HttpPost action that changes the category's state to modified and saves the changes to it to the database
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
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

        /// <summary>
        /// HttpGet action which takes in the id of the category to be deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);//finds the category to be deleted
            if(category == null)
            {
                return HttpNotFound();
            }
            return View(category); //returns the category to be deleted to the confirmation page
        }

        /// <summary>
        /// HttpPost action that removes the category from the database after confirmation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("ViewAllCategories");
        }

        /// <summary>
        /// overrides the default dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing); 
        }

        /// <summary>
        /// HttpGet which returns the view all categories page with a list of categories
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAllCategories()
        {
            return View(db.Categories.ToList());
        }

        /// <summary>
        /// HttpGet action which returns the details of the selected category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// HttpGet action which redirects an admins request for the post details page to the HomeController
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// HttpGet action which returns the view all posts page with a list of posts
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult ViewAllPosts()
        {
            List<Post> posts = db.Posts.Include(p => p.Category).Include(p => p.User).ToList();
            return View(posts);
        }

        /// <summary>
        /// HttpPost action which takes in the search string containing the category
        /// and returns a list of posts in that category to the view
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ViewAllPosts(string SearchString)
        {
            //finds all posts where the category matches the value in the search string
            var posts = db.Posts.Include(p => p.Category).Include(p => p.User).Where(p => p.Category.Name.Equals(SearchString.Trim())).OrderByDescending(p => p.DatePosted);
            ViewBag.Categories = db.Categories.ToList();
            return View(posts.ToList()); //returns the sorted list of posts within the searched category
        }

        /// <summary>
        /// HttpGet action which takes in the id of the post to be deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// HttpPost action which removes the post from the db after the deletion has been confirmed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeletePost")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePostConfirmed(int id)
        {
            Post post = db.Posts.Find(id); //Finds the post to be deleted
            db.Posts.Remove(post); //removes it from the db
            db.SaveChanges();
            return RedirectToAction("ViewAllPosts");
        }

        /// <summary>
        /// HttpGet action which redirects to the HomeController's DeleteComment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// HttpGet method which returns a list of all users to the view
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult ViewUsers()
        {
            //creates a list of all users
            List<User> users = db.Users.Include(u => u.Roles).OrderBy(u => u.LastName).ToList();

            return View(users); 
        }

        /// <summary>
        /// HttpGet action which takes in the id of the user who's role is to be changed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
            
            User user = await userManager.FindByIdAsync(id); //finds the user whose role is to be changed
            string oldRole = (await userManager.GetRolesAsync(id)).Single(); //finds their old role

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

        /// <summary>
        /// HttpPost action which changes the users role in the database after its been confirmed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangeRole")]
        public async Task<ActionResult> ChangeRoleConfirmed(string id, [Bind(Include ="Role")] ChangeRoleViewModel model)
        {
            UserManager<User> userManager = new UserManager<User>(new UserStore<User>(db));

            if (id == User.Identity.GetUserId())
            {
                //Users cannot change their own role
                return RedirectToAction("Index");
            }
            if(ModelState.IsValid)
            {
                User user = await userManager.FindByIdAsync(id);
                string oldRole = (await userManager.GetRolesAsync(id)).Single();

                //if the new role is the same as the old role
                if (oldRole == model.Role)
                {
                    return RedirectToAction("Index");//redirect to the index
                }

                await userManager.RemoveFromRoleAsync(id, oldRole); //remove the user from the old role
                await userManager.AddToRoleAsync(id, model.Role);//add the user to the new role

                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
