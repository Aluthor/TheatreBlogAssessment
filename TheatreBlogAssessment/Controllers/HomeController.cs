using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheatreBlogAssessment.Models;
using System.Data.Entity;

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
            return View(posts);
        }

        [HttpPost]
        public ActionResult Index(string SearchString)
        {
            var posts = context.Posts.Include(p => p.Category).Include(p => p.User).Where(p => p.Category.Name.Equals(SearchString.Trim())).OrderByDescending(p => p.DatePosted);
            return View(posts.ToList());
        }
        public ActionResult Details(int id)
        {
            Post post = context.Posts.Find(id);

            var user = context.Users.Find(post.UserId);
            var category = context.Categories.Find(post.CategoryId);
            var comments = context.Comments.Include(c=>c.User);

            post.User = user;
            post.Category = category;
            post.Comments = comments.ToList();

            return View(post);
        }

    }
}