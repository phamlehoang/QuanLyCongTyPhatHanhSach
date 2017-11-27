using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.Controllers
{
    public class BookPricesController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: BookPrices
        public ActionResult Index()
        {
            var bookPrices = db.BookPrices.Include(b => b.Book);
            return View(bookPrices.ToList());
        }

        // GET: BookPrices/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookPrice bookPrice = db.BookPrices.Find(id);
            if (bookPrice == null)
            {
                return HttpNotFound();
            }
            return View(bookPrice);
        }

        // GET: BookPrices/Create
        public ActionResult Create()
        {
            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "BookName");
            return View();
        }

        // POST: BookPrices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ISBN,DateCreate,PurchasePrice,SellingPrice")] BookPrice bookPrice)
        {
            if (ModelState.IsValid)
            {
                db.BookPrices.Add(bookPrice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "BookName", bookPrice.ISBN);
            return View(bookPrice);
        }

        // GET: BookPrices/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookPrice bookPrice = db.BookPrices.Find(id);
            if (bookPrice == null)
            {
                return HttpNotFound();
            }
            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "BookName", bookPrice.ISBN);
            return View(bookPrice);
        }

        // POST: BookPrices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ISBN,DateCreate,PurchasePrice,SellingPrice")] BookPrice bookPrice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookPrice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "BookName", bookPrice.ISBN);
            return View(bookPrice);
        }

        // GET: BookPrices/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookPrice bookPrice = db.BookPrices.Find(id);
            if (bookPrice == null)
            {
                return HttpNotFound();
            }
            return View(bookPrice);
        }

        // POST: BookPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            BookPrice bookPrice = db.BookPrices.Find(id);
            db.BookPrices.Remove(bookPrice);
            db.SaveChanges();
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
