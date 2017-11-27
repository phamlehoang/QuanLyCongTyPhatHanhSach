using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteQuanLyPhatHanhSach.Areas.Agency.Models;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.Areas.Agency.Controllers
{
    public class OrderController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: Agency/Order
        public ActionResult Index()
        {
            var books =db.Books.Include(b => b.Publishing)/*.Include(b => b.BookPrices)*/;
            return View(books.ToList());
        }

        public JsonResult GetBook()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var books = db.Books.Join(db.BookPrices, a => a.ISBN, p => p.ISBN, (a, p) => new {
                ISBN = a.ISBN,
                PubID = a.PubID,
                Name = a.BookName,
                Cate = a.BookCategory,
                Author = a.BookAuthor,
                Price = p.SellingPrice
            }).Join(db.Publishings, x => x.PubID, y => y.PubID, (x, y) => new {
                ISBN = x.ISBN,
                BookName = x.Name,
                BookCategory = x.Cate,
                BookAuthor = x.Author,
                SellingPrice = x.Price,
                PubName = y.PubName
            });
            return Json(books.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateOrder(OrderVM O)
        {
            bool status = false;
            if (!ModelState.IsValid)
            {
                OrderA ord = new OrderA
                {
                    AgencyID = O.AgencyID,
                    OrderCreate = DateTime.Now,
                    OrderStatus = O.Status,
                    OrderTotal = O.Total
                };
                db.OrderAs.Add(ord);
                foreach (var i in O.Details)
                {
                    i.OrderID = ord.OrderID;
                    db.OrderDetails.Add(i);
                }
                db.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}