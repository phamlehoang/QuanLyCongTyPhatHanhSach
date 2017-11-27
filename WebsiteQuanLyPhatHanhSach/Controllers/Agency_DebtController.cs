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
    public class Agency_DebtController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: Agency_Debt
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexAgencyDebt()
        {
            return View();
        }

        // GET: Agency_Debt/Index
        public JsonResult GetAgencyDebt()
        {
            var agency_Debt = db.Agency_Debt.GroupBy(a => a.AgencyID).Select(a => new {
                Agency_ID = a.Key,
                ID = a.Max(x => x.ADebtID)
            }).Join(db.Agency_Debt.Include(a => a.Agency), b => b.ID, c => c.ADebtID, (b, c) => new {
                AgencyID = b.Agency_ID,
                AgencyUser = c.Agency.AgencyUser,
                AgencyName = c.Agency.AgencyName,
                AgencyDebt = c.AgencyDebt
            });
            return Json(agency_Debt.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult StatisticAllDebt()
        {
            //lấy nhiều nợ: mã id, mã đại lý và ngày nợ nhóm theo đại lý và ngày
            var anyagency_anydate = db.Agency_Debt.GroupBy(a => new { a.AgencyID, Date = DbFunctions.TruncateTime(a.DateCreate) }).Select(a => new {
                Id = a.Max(x => x.ADebtID),
                AgencyID = a.Key.AgencyID,
                Date = a.Key.Date
            });
            //join vs bảng nợ để có số nợ
            var joinagencydebt = anyagency_anydate.Join(db.Agency_Debt.Include(a => a.Agency), a => a.Id, b => b.ADebtID, (a, b) => new {
                Id = a.Id,
                AgencyID = a.AgencyID,
                AgencyUser = b.Agency.AgencyUser,
                AgencyName = b.Agency.AgencyName,
                AgencyDebt = b.AgencyDebt,
                Date = a.Date
            }).OrderBy(a => a.AgencyID).ThenBy(a => a.Date);
            return Json(joinagencydebt.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult StatisticDebt(DateTime date)
        {
            DateTime D = date;
            //lấy nhiều nợ: mã id, mã đại lý và ngày nợ nhóm theo đại lý và ngày
            var anyagency_anydate = db.Agency_Debt.GroupBy(a => new { a.AgencyID, Date = DbFunctions.TruncateTime(a.DateCreate) }).Select(a => new {
                Id = a.Max(x => x.ADebtID),
                AgencyID = a.Key.AgencyID,
                Date = a.Key.Date
            });
            //lấy nợ phải trả <= ngày cần tìm nhưng là ngày lớn nhất của đại lý đó
            var debt_max_lessthan = anyagency_anydate.Where(a => DateTime.Compare((DateTime)a.Date, D.Date) <= 0).GroupBy(a => a.AgencyID)
                .Select(a => new {
                    AgencyID = a.Key,
                    Date = a.Max(x => x.Date)
                });
            var result = debt_max_lessthan.Join(anyagency_anydate, a => new { a.AgencyID, a.Date }, b => new { b.AgencyID, b.Date }, (a, b) =>
                 new
                 {
                     Id = b.Id,
                     AgencyID = b.AgencyID,
                     Date = b.Date
                 });
            var debt = result.Join(db.Agency_Debt.Include(a => a.Agency), x => x.Id, y => y.ADebtID, (x, y) => new
            {
                Id = x.Id,
                AgencyUser = y.Agency.AgencyUser,
                AgencyName = y.Agency.AgencyName,
                AgencyDebt = y.AgencyDebt,
                Date = x.Date
            });
            return Json(debt.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Agency_Debt/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var agency_Debt = db.Agency_Debt.Include(a => a.Agency).Where(a => a.AgencyID == id);
            if (agency_Debt == null)
            {
                return HttpNotFound();
            }
            return View(agency_Debt.ToList());
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
