using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;

namespace Blog.Controllers
{
    public class TagController : Controller
    {
        // GET: Tag
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int? id)
        {
            if (id == null)
            {
                throw new System.ArgumentException("Parameter cannot be null", "original");
            }

            using (var database = new BlogDbContext())
            {
                // Get articles from database
                var artices = database.Tags
                    .Include(t => t.Articles.Select(a => a.Tags))
                    .Include(t => t.Articles.Select(a => a.Files))
                    .Include(t => t.Articles.Select(a => a.Author))
                    .FirstOrDefault(t => t.Id == id)
                    .Articles
                    .ToList();
                // Return the view
                return View(artices);
            }
        }
    }
}