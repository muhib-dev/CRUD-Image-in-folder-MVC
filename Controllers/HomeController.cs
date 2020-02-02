using CRUD_Image.Models;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CRUD_Image.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private readonly MVC_TestEntities _db = new MVC_TestEntities();

        //Index
        public ActionResult Index()
        {
            var model = _db.Customers.ToList();
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

            _db.Customers.Add(newCustomer);
            var added = _db.SaveChanges();

            if (added <= 0) return RedirectToAction($"Index");

            file.SaveAs(path);
            ViewBag.message = "Image added";
            ModelState.Clear();

            return RedirectToAction($"Index");
        }

        //Edit
        public ActionResult Edit(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            var model = _db.Customers.Find(id);
            var c = new CustomerVM
            {
                CustomerID = model.CustomerID,
                Country = model.Country,
                Name = model.Name,
                ImageUrl = model.ImageUrl
            };


            Session["ImageUrl"] = model.ImageUrl;

            return View(c);
        }

        [HttpPost]
        public ActionResult Edit(CustomerVM customers)
        {
            if (!ModelState.IsValid) RedirectToAction("Edit");
            var model = _db.Customers.Find(customers.CustomerID);

            if (customers.Files != null)
            {
                var file = new CustomPostedFile(customers.Files, customers.FileName);
                var path = Path.Combine(Server.MapPath("~/images/"), file.FileName);

                customers.ImageUrl = "~/images/" + file.FileName;

                if (model != null)
                {
                    model.Country = customers.Country;
                    model.Name = customers.Name;
                    model.ImageUrl = customers.ImageUrl;

                    _db.Entry(model).State = EntityState.Modified;
                }

                var added = _db.SaveChanges();
                var oldImage = Request.MapPath(Session["ImageUrl"].ToString());

                if (added <= 0) return RedirectToAction("Index");


                var imageExists = System.IO.File.Exists(oldImage);
                if (imageExists)
                    System.IO.File.Delete(oldImage);


                file.SaveAs(path);
            }
            else
            {
                if (model != null)
                {
                    model.Country = customers.Country;
                    model.Name = customers.Name;
                    _db.Entry(model).State = EntityState.Modified;
                }

                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //Delete
        public ActionResult Delete(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            var model = _db.Customers.Find(id);

            if (model == null) return RedirectToAction("Index");

            var currentImage = Request.MapPath(model.ImageUrl);
            _db.Entry(model).State = EntityState.Deleted;

            var added = _db.SaveChanges();
            if (added <= 0) return RedirectToAction("Index");

            var imageExists = System.IO.File.Exists(currentImage);
            if (imageExists)
                System.IO.File.Delete(currentImage);


            return RedirectToAction("Index");
        }
    }
}