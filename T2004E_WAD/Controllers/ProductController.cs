using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using T2004E_WAD.Context;
using T2004E_WAD.Models;
using System.Dynamic;

namespace T2004E_WAD.Controllers
{
    public class ProductController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Product
        public ActionResult Index(string search, string sortOrder, string categoryId)
        {
            ViewBag.CategoryId = 0;
            string sort = !String.IsNullOrEmpty(sortOrder) ? sortOrder : "asc";
            /*// cach 1: dung thang dbSet -- su dung duoc relationship
            if (!String.IsNullOrEmpty(search))
            {
                var products = db.Products.Where(p => p.Name.Contains(search)).OrderBy(p=>p.Name).ToList(); // OrderBy : asc  OrderByDescending: desc
                return View(products);
            }
            return View(db.Products.OrderBy(p=>p.Name).ToList());

*/
            // cach 2 dung db Raw - tim trong 1 bang
            var products = from p in db.Products select p;
            if (!String.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search));
            }
            switch (sort)
            {
                case "asc": products = products.OrderBy(p => p.Name); break;
                case "desc": products = products.OrderByDescending(p => p.Name); break;
            }
            // loc theo category
            if (!String.IsNullOrEmpty(categoryId))
            {
                var catId = Convert.ToInt32(categoryId);
                products = products.Where(p => p.CategoryID == catId);
                ViewBag.CategoryId = catId;
            }


            var categories = db.Categories.ToList();
            dynamic data = new ExpandoObject();
            data.Products = products;
            data.Categories = categories;
            return View(data);

        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id); // lay 1 doi tuong Product theo id
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult AddToCart(int? id, int? qty)
        {
            try
            {
                Product product = db.Products.Find(id);
                if(product == null)
                {
                    return HttpNotFound();
                }
                // them vao gio hang
                CartItem item = new CartItem(product, (int)qty);
                // lay gio hang tu Session
                Cart cart = (Cart)Session["Cart"];
                if(cart == null)
                {
                    Customer customer = new Customer("Nguyễn Văn An", "0987654321", "Số 8 Tôn Thất Thuyết");
                    cart = new Cart();
                    cart.Customer = customer;
                }
                cart.AddToCart(item);
            }catch(Exception e)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Cart");
        }

        public ActionResult Cart()
        {
            return View();
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Image,Price,Description,CategoryID")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product); // them 1 object Product vaof danh sach
                db.SaveChanges(); // luu vao db cac thay doi du lieu
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "Name", product.CategoryID);
            return View(product);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "Name", product.CategoryID);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Image,Price,Description,CategoryID")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified; // trang thai cua db la change (modified)
                db.SaveChanges(); // luu vao db
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "Name", product.CategoryID);
            return View(product);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product); // xoa product khoi danh sach
            db.SaveChanges(); // luu vao db
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
