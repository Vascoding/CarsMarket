using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;

namespace Blog.Controllers.Admin
{
    public class CarsController : Controller
    {
        // GET: Cars
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //
        // Get: Cars/List
        public ActionResult List()
        {
            using (var database = new BlogDbContext())
            {
                var carses = database.Carses.ToList();

                return View(carses);
            }
        }

        // Get: Cars/Create
        public ActionResult Create()
        {
            return View();
        }

        // Post: Cars/Create
        [HttpPost]
        public ActionResult Create(Cars cars)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    database.Carses.Add(cars);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(cars);

        }

        //
        // Get: Cars/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var cars = database.Carses
                    .FirstOrDefault(c => c.CarsId == id);

                if (cars == null)
                {
                    return HttpNotFound();
                }
                return View(cars);
            }
        }

        // Post: Cars/Edit
        [HttpPost]
        public ActionResult Edit(Cars cars)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    database.Entry(cars).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(cars);
        }

        // Get: Cars/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var cars = database.Carses.FirstOrDefault(c => c.CarsId == id);

                if (cars == null)
                {
                    return HttpNotFound();
                }

                return View(cars);
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
                var cars = database.Carses
                    .FirstOrDefault(c => c.CarsId == id);
                var carsCategory = cars.Categories.ToList();

                foreach (var category in carsCategory)
                {
                    database.Categories.Remove(category);
                }
                database.Carses.Remove(cars);
                database.SaveChanges();

                return RedirectToAction("Index");
            }
        }
    }
}