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

        /// <summary>
        /// HttpGet which loads the home page with a list of posts
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var posts = context.Posts.Include(p => p.Category).Include(p => p.User).OrderByDescending(p => p.DatePosted);
            ViewBag.Categories = context.Categories.ToList();
            return View(posts.ToList());
        }

        /// <summary>
        /// HttpPost action which takes in the search string containing the category
        /// and returns a list of posts in that category to the view
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string SearchString)
        {
            var posts = context.Posts.Include(p => p.Category).Include(p => p.User).Where(p => p.Category.Name.Equals(SearchString.Trim())).OrderByDescending(p => p.DatePosted);
            ViewBag.Categories = context.Categories.ToList();
            return View(posts.ToList());
        }

        /// <summary>
        /// HttpGet action which displays the details of a selected post,
        /// including the user who posted it, it's category and comments
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// HttpGet action to make a comment, all users but anonymous can access it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Member, Staff, Admin")]
        public ActionResult CreateComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = context.Posts.Find(id); //finds the post the comment is going to be displayed under
            Comment comment = new Comment();//creates a new empty 

            comment.PostId = post.PostId; //assigns the empty comment to the post

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }

        /// <summary>
        /// HttpPost action adds the comment to the database
        /// as approved if it was made by an admin/staff but as 
        /// not yet approved if it was made by a member
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles =("Member, Staff, Admin"))]
        public ActionResult CreateComment(Comment comment, int id) 
        {
            if (ModelState.IsValid)
            {
                comment.CommentDate = DateTime.Now;
                comment.UserId = User.Identity.GetUserId(); //sets the comments userId to the userId of the currently logged in user
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
        
        /// <summary>
        /// HttpGet action redirects to the Edit Post in the HomeController
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// HttpGet action which takes in the id of the comment to be approved
        /// Only staff and admins have access to this action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Staff, Admin")]
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = context.Comments.Find(id); //finds the comment to be approved
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        /// <summary>
        /// HttpPost action which sets the comment to approved after it has been confirmed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveConfirmed(int id)
        {
            Comment comment = context.Comments.Find(id);
            comment.IsAproved = true;
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edit comment action which takes in the comment to be edited
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

            return View(comment);
        }

        /// <summary>
        /// HttpPost action which updates the comment in the database,
        /// sets its state to modified and unapproves it as it's content has changed
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
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
            return View(comment);

        }

        /// <summary>
        /// HttpGet action to change the details of the currently logged in user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditDetails()
        {
           string id = User.Identity.GetUserId().ToString(); //finds the id of the currently logged in user
            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = context.Users.Find(id); //finds the currently logged in user by id

            if (user== null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        /// <summary>
        /// HttpPost action which saves the changes to the users details to the database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDetails(User user)
        {

            if (ModelState.IsValid)
            {
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);

        }

        /// <summary>
        /// HttpGet action which redirects to the StaffController's delete post 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// HttpGet action which takes in the id of the comment to be deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// HttpPost actoin which removes the commenet from the database after its deletion has been confirmed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCommentConfirmed(int id)
        {
            Comment comment = context.Comments.Find(id);
            context.Comments.Remove(comment);

            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}