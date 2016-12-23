using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using Blog.Models;

namespace Blog.Controllers.Admin
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //
        // Get Category/List
        public ActionResult List()
        {
            using (var database = new BlogDbContext())
            {
                var categories = database.Categories.ToList();

                return View(categories);
            }
        }

        //
        // Get: Category/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // Post: Category/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Category category, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
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
                        category.Files = new List<File> { avatar };
                    }

                    database.Categories.Add(category);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(category);
        }

        //
        // Get: Category/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var category = database.Categories
                    .Include(a => a.Files)
                    .FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
            }
        }

        //
        // Post: Category/Edit
        [HttpPost]
        public ActionResult Edit(Category category, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        if (category.Files.Any(f => f.FileType == FileType.Avatar))
                        {
                            database.Files.Remove(category.Files.First(f => f.FileType == FileType.Avatar));
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
                        category.Files = new List<File> { avatar };
                    }
                    database.Entry(category).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(category);
        }

        //
        // Get: Category/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var category = database.Categories
                    .Where(c => c.Id == id)
                    .Include(c => c.Files)
                    .FirstOrDefault(c => c.Id == id);


                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
            }
        }

        //
        // Post: Category/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            using (var database = new BlogDbContext())
            {
                var category = database.Categories
                    .FirstOrDefault(c => c.Id == id);
                var categoryArticles = category.Articles
                    .ToList();

                foreach (var article in categoryArticles)
                {
                    database.Articles.Remove(article);
                }
                database.Files.Remove(category.Files.First(f => f.FileType == FileType.Avatar));
                database.Categories.Remove(category);
                database.SaveChanges();

                return RedirectToAction("Index");
            }
        }
    }
}