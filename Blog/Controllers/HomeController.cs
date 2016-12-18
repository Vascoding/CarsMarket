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
    public class HomeController : Controller
    {
        public ActionResult Index(string searchString)
        {
            using (var database = new BlogDbContext())
            {

                var article = database.Articles
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
                    .Include(s => s.Files)
                    .ToList();
                if (!String.IsNullOrEmpty(searchString))
                {
                    article = article.Where(s => s.Title.Contains(searchString)
                                                 || s.Content.Contains(searchString)).ToList();
                    return View(article);
                }

            }

            return RedirectToAction("ListCategories");
        }

        public ActionResult ListCategories()
        {
            using (var database = new BlogDbContext())
            {
                var categories = database.Categories
                    .Include(c => c.Articles)
                    .OrderBy(c => c.Name)
                    .ToList();

                return View(categories);
            }
        }

        public ActionResult ListArticles(int? categoryId)
        {
            if (categoryId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var articles = database.Articles
                    .Where(a => a.CategoryId == categoryId)
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
                    .Include(a => a.Files)
                    .ToList();

                return View(articles);
            }
        }

        public ActionResult ListCars(int? carsId)
        {
            if (carsId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var cars = database.Carses
                    .Where(a => a.CarsId == carsId)
                    .ToList();

                return View(cars);
            }
        }


    }
}