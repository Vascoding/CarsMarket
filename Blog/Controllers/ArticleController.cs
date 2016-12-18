using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;

namespace Blog.Controllers
{
    public class ArticleController : Controller
    {
        //
        // GET: Article
        public ActionResult Index()
        {
            
            return RedirectToAction("List");
        }

        //
        // GET: Article/List
        public ActionResult List(int? id)
        {
            using (var database = new BlogDbContext())
            {
                //Get articles from database
                var articles = database.Articles
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
                    .Include(a => a.Files)
                    .ToList();
                    
                return View(articles);
            }

        }

        //
        // GET: Article/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
                    .Include(s => s.Files)
                    .SingleOrDefault(s => s.Id == id);


                if (article == null)    
                {
                    return HttpNotFound();
                }

                return View(article);
            }
        }

        //
        // Get: Article/Create
        [Authorize]
        public ActionResult Create()
        {
            using (var database = new BlogDbContext())
            {
                var model = new ArticleViewModel();
                model.Categories = database.Categories
                    .OrderBy(c => c.Name)
                    .ToList();

                return View(model);
            }
        }

        //
        // POST: Article/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ArticleViewModel model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    // get author id
                    var authorId = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;

                    // set articles author
                    var article = new Article(authorId, model.Title, model.Phone, model.Content, model.Year, 
                        model.CategoryId, model.Price, model.City, model.HorsePower);

                    
                    // save article in DB
                    this.SetArticleTags(article, model, database);

                    if (upload != null && upload.ContentLength > 0)
                    {
                        var avatar = new File
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.Avatar,
                            ContentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            avatar.Content = reader.ReadBytes(upload.ContentLength);
                        }
                        article.Files = new List<File> { avatar };
                    }

                    database.Articles.Add(article);
                    database.SaveChanges();
                    
                    return RedirectToAction("Index");
                }

            }
            return View(model);
        }

        //
        // Get: Article/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                // Get article from database
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(c => c.Category)
                    .Include(c => c.Files)
                    .First();

                if (!IsUserAuthorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                // Chek if article exists
                if (article == null)
                {
                    return HttpNotFound();
                }

                ViewBag.TagsString = string.Join(", ", article.Tags.Select(t => t.Name));

                // Pass article to view
                return View(article);
            }
        }

        //
        // Post: Article/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                // Get article from database 
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(c => c.Category)
                    .Include(c => c.Files)
                    .First();

                // Check if article exists
                if (article == null)
                {
                    return HttpNotFound();
                }

                // Delete article from database
                database.Files.Remove(article.Files.First(f => f.FileType == FileType.Avatar));
                database.Articles.Remove(article);
                database.SaveChanges();

                // Redirect to index page
                return RedirectToAction("Index");
            }
        }

        //
        // Get: Article/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                // Get article from database 
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Files)
                    .First();
                    
                    
                if (!IsUserAuthorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                // Check if article exists
                if (article == null)
                {
                    return HttpNotFound();
                }

                // Create the view model
                var model = new ArticleViewModel();
                model.Id = article.Id;
                model.Title = article.Title;
                model.Phone = article.Phone;
                model.Year = article.Year;
                model.Price = article.Price;
                model.City = article.City;
                model.HorsePower = article.HorsePower;
                model.Content = article.Content;
                model.CategoryId = article.CategoryId;
                model.Categories = database.Categories
                    .OrderBy(c => c.Name)
                    .ToList();
                model.Files = article.Files;
                model.Tags = string.Join(", ", article.Tags.Select(t => t.Name));
                // Pass the view model to view
                return View(model);
            }
        }

        //
        // Post: Article/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ArticleViewModel model, HttpPostedFileBase upload)
        {
            // Check if model state is valid
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    // Get article from database 
                    var article = database.Articles
                        .FirstOrDefault(a => a.Id == model.Id);

                    // Set article properties
                    article.Title = model.Title;
                    article.Phone = model.Phone;
                    article.Year = model.Year;
                    article.Price = model.Price;
                    article.City = model.City;
                    article.HorsePower = model.HorsePower;
                    article.Content = model.Content;
                    article.CategoryId = model.CategoryId;
                    article.Files = model.Files;

                    this.SetArticleTags(article, model, database);


                    if (upload != null && upload.ContentLength > 0)
                    {
                        if (article.Files.Any(f => f.FileType == FileType.Avatar))
                        {
                            database.Files.Remove(article.Files.First(f => f.FileType == FileType.Avatar));
                        }
                        var avatar = new File
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.Avatar,
                            ContentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            avatar.Content = reader.ReadBytes(upload.ContentLength);
                        }
                        article.Files = new List<File> { avatar };
                    }

                    // Save article state in database
                    database.Entry(article).State = EntityState.Modified;
                    database.SaveChanges();
                }
            }
            // Redirect to the index page 
            return RedirectToAction("Index");
        }

        private void SetArticleTags(Article article, ArticleViewModel model, BlogDbContext db)
        {
            // Split tags
            var tagsString = model.Tags
                .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToLower())
                .Distinct();

            // Clear current article tags
            article.Tags.Clear();

            // Set new article tags
            foreach (var tagString in tagsString)
            {
                // Get tags from database by its name
                Tag tag = db.Tags.FirstOrDefault(t => t.Name.Equals(tagString));

                // If the tag is null create new tag
                if (tag == null)
                {
                    tag = new Tag() { Name = tagString };
                    db.Tags.Add(tag);
                }
                // Add tag to article tags
                article.Tags.Add(tag);
            }
        }

        private bool IsUserAuthorizedToEdit(Article article)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = article.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }

        
    }
    
}