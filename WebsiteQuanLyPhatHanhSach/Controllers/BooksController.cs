using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteQuanLyPhatHanhSach.Models;
using WebsiteQuanLyPhatHanhSach.ViewModels;

namespace WebsiteQuanLyPhatHanhSach.Controllers
{
    public class BooksController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: Books
        public ActionResult Index()
        {
            var books = db.Books.Include(b => b.Publishing)/*.Include(b => b.BookPrices)*/;
            return View(books.ToList());
        }

        // GET: Books/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            ViewBag.PubID = new SelectList(db.Publishings, "PubID", "PubName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookVM B)
        {
            //bool status = false;
            if (ModelState.IsValid)
            {
                Book book = new Book
                {
                    ISBN = B.ISBN, BookName = B.BookName, BookCategory = B.BookCategory, PubID = B.PubID,
                    BookAuthor = B.BookAuthor, BookPages = B.BookPages, BookDescribe = B.BookDescribe
                };
                BookPrice bookprice = new BookPrice
                {
                    ISBN = B.ISBN, DateCreate = DateTime.Now,
                    PurchasePrice = B.PurchasePrice, SellingPrice = B.SellingPrice
                };
                book.BookPrices.Add(bookprice);
                db.Books.Add(book);
                //db.BookPrices.Add(bookprice);
                db.SaveChanges();
                //status = true;
                return RedirectToAction("Index");
            }
            //return new JsonResult { Data = new { status = status } };
            ViewBag.PubID = new SelectList(db.Publishings, "PubID", "PubName", B.PubID);
            return View(B);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.PubID = new SelectList(db.Publishings, "PubID", "PubName", book.PubID);
            return PartialView(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ISBN,BookName,BookCategory,PubID,BookAuthor,BookPages,BookDescribe")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PubID = new SelectList(db.Publishings, "PubID", "PubName", book.PubID);
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return PartialView(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
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
