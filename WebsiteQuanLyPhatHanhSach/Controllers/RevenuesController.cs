using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.Controllers
{
    public class RevenuesController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: Revenues
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllRevenue()
        {
            var rev = db.Revenues.Include(a => a.IssueInvoice).Join(db.Agencies, a => a.IssueInvoice.AgencyID, b => b.AgencyID,
                (a, b) => new {
                    Id = a.Id, InvoiceID = a.InvoiceID, AgencyID = a.IssueInvoice.AgencyID, AgencyName = b.AgencyName,
                    InvoiceRevenue = a.InvoiceRevenue, RevenueTotal = a.RevenueTotal, RevenueDate = a.RevenueDate
            });

            return Json(rev.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRevenues(DateTime start, DateTime end)
        {
            var rev = db.Revenues.Include(a => a.IssueInvoice).Join(db.Agencies, a => a.IssueInvoice.AgencyID, b => b.AgencyID,
                (a, b) => new {
                    Id = a.Id,
                    InvoiceID = a.InvoiceID,
                    AgencyID = a.IssueInvoice.AgencyID,
                    AgencyName = b.AgencyName,
                    InvoiceRevenue = a.InvoiceRevenue,
                    RevenueTotal = a.RevenueTotal,
                    RevenueDate = a.RevenueDate
                }).Where(a => 
                DateTime.Compare((DateTime)DbFunctions.TruncateTime(a.RevenueDate), start.Date) >= 0 &&
                DateTime.Compare((DateTime)DbFunctions.TruncateTime(a.RevenueDate), end.Date) <= 0
                );

            return Json(rev.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}