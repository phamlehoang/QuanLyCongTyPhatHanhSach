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
    public class ReportController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: Agency/Report : lấy danh sách những sách đang có.
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAgencyBook()
        {
            db.Configuration.ProxyCreationEnabled = false;
            string id = Session["AgencyID"].ToString();
            var agencybook = db.Agency_Book.Join(db.BookPrices, a => a.ISBN, p => p.ISBN, (a, p) => new {
                AgencyID = a.AgencyID, ISBN = a.ISBN, Quantity = a.BookQuantity, Price = p.SellingPrice, Total = a.BookQuantity*p.SellingPrice
            }).Join(db.Books,x => x.ISBN, y => y.ISBN, (x,y) => new {
                AgencyID = x.AgencyID, ISBN = x.ISBN, BookName = y.BookName, BookQuantity = x.Quantity,
                SellingPrice = x.Price, BookTotal = x.Total
            }).Where(ab => ab.AgencyID.ToString() == id && ab.BookQuantity > 0);
            return Json(agencybook.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateReport(ReportVM R)
        {
            bool status = false;
            if (!ModelState.IsValid)
            {
                ReportSold res = new ReportSold
                {
                    AgencyID = R.AgencyID,
                    ReportDate = DateTime.Now,
                    SoldTotal = R.SoldTotal
                };
                db.ReportSolds.Add(res);
                foreach (var i in R.Details)
                {
                    i.ReportID = res.ReportID;
                    db.ReportSoldDetails.Add(i);
                    //tính lại số lượng sách trong bảng Agency_Book
                    Agency_Book ab = db.Agency_Book.Find(R.AgencyID, i.ISBN);
                    ab.BookQuantity = ab.BookQuantity - i.QuatitySold;
                    db.Entry(ab).State = EntityState.Modified;
                }
                db.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}