using CRUD_Image.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CRUD_Image.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        MVC_TestEntities db = new MVC_TestEntities();
        
        //Index
        public ActionResult Index()
        {
            var model = db.Customers.ToList();
            return View(model);
        }

        //Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CustomerVM customers)
        {
            var file = new CustomPostedFile(customers.Files, customers.FileName);
            var path = Path.Combine(Server.MapPath("~/images/"), file.FileName);

            customers.ImageUrl = "~/images/" + file.FileName;
            var newCustomer = new Customer
            {
                Country = customers.Country,
                ImageUrl = customers.ImageUrl,
                Name = customers.Name
            };
         
            db.Customers.Add(newCustomer);
            var addedd = db.SaveChanges();

            if (addedd > 0)
            {
                file.SaveAs(path);
                ViewBag.message = "Image addedd";
                ModelState.Clear();
            }

           return RedirectToAction("Index");
        }

        //Edit
        public ActionResult Edit(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            var model = db.Customers.Find(id);
            var c = new CustomerVM
            {
                CustomerID=model.CustomerID,
                Country = model.Country,
                Name = model.Name,
                ImageUrl = model.ImageUrl
            };
            

            Session["ImageUrl"] = model.ImageUrl;

            if (model == null) return RedirectToAction("Index");

            return View(c);
        }

        [HttpPost]
        public ActionResult Edit(CustomerVM customers)
        {
            if(!ModelState.IsValid) RedirectToAction("Edit");
            var model = db.Customers.Find(customers.CustomerID);

            if (customers.Files != null)
            {
                var file = new CustomPostedFile(customers.Files, customers.FileName);
                var path = Path.Combine(Server.MapPath("~/images/"), file.FileName);

                customers.ImageUrl = "~/images/" + file.FileName;
                model.Country = customers.Country;
                model.Name  = customers.Name;
                model.ImageUrl = customers.ImageUrl;


                db.Entry(model).State = EntityState.Modified;
                var addedd = db.SaveChanges();
                var oldImage = Request.MapPath(Session["ImageUrl"].ToString());

                if (addedd > 0)
                {
                    file.SaveAs(path);

                    var imageExsits = System.IO.File.Exists(oldImage);
                    if (imageExsits)
                    {
                        System.IO.File.Delete(oldImage);
                    }
                }
            }
            else
            {
                model.Country = customers.Country;
                model.Name = customers.Name;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //Delete
        public ActionResult Delete(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            var model = db.Customers.Find(id);

            if (model == null) return RedirectToAction("Index");

            var currentImage = Request.MapPath(model.ImageUrl);
            db.Entry(model).State = EntityState.Deleted;
           
            var addedd = db.SaveChanges();
            if (addedd > 0)
            {
                var imageExsits = System.IO.File.Exists(currentImage);
                if (imageExsits)
                {
                    System.IO.File.Delete(currentImage);
                }
            }

            return RedirectToAction("Index");
        }
    }
}