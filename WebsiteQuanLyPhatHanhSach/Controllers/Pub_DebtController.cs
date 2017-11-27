using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
    public class Pub_DebtController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: Pub_Debt
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPubDebt()
        {
            return View();
        }

        // GET: Pub_Debt/Index
        public JsonResult GetPubDebt()
        {
            //lấy nhiều nợ: mã id, mã nxb và ngày nợ nhóm theo nxb và ngày
            var anypub_anydate = db.Pub_Debt.GroupBy(a => new { a.PubID, Date = DbFunctions.TruncateTime(a.DateCreate) }).Select(a => new {
                Id = a.Max(x => x.PDebtID),
                PubID = a.Key.PubID,
                Date = a.Key.Date
            });
            //join vs bảng nợ để có số nợ
            var joinpubdebt = anypub_anydate.Join(db.Pub_Debt.Include(a => a.Publishing), a => a.Id, b => b.PDebtID, (a, b) => new {
                Id = a.Id,
                PubID = a.PubID,
                PubName = b.Publishing.PubName,
                PubDebt = b.PubDebt,
                Date = a.Date
            }).OrderBy(a => a.PubID).ThenBy(a => a.Date);

            //lấy một nợ hiện tại duy nhất : mã nxb và ngày lớn nhất
            var pubdebt_present = anypub_anydate.GroupBy(a => a.PubID).Select(a => new
            {
                PubID = a.Key, Date = a.Max(x => x.Date),
            });

            var result = joinpubdebt.Join(pubdebt_present, a => new { a.PubID, a.Date }, b => new { b.PubID, b.Date }, (a, b) => new {
                Id = a.Id, PubID = a.PubID, PubName = a.PubName, PubDebt = a.PubDebt, Date = a.Date
            });
            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Pub_Debt/Details/3001
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pub_Debt = db.Pub_Debt.Include(a => a.Publishing).Where(a => a.PubID == id).OrderBy(a => a.DateCreate).ThenBy(a => a.PDebtID);
            if (pub_Debt == null)
            {
                return HttpNotFound();
            }
            return View(pub_Debt);
        }

        public JsonResult StatisticAllDebt()
        {
            //lấy nhiều nợ: mã id, mã nxb và ngày nợ nhóm theo nxb và ngày
            var anypub_anydate = db.Pub_Debt.GroupBy(a => new { a.PubID, Date = DbFunctions.TruncateTime(a.DateCreate) }).Select(a => new {
                Id = a.Max(x => x.PDebtID),
                PubID = a.Key.PubID,
                Date = a.Key.Date
            });
            //join vs bảng nợ để có số nợ
            var joinpubdebt = anypub_anydate.Join(db.Pub_Debt.Include(a => a.Publishing), a => a.Id, b => b.PDebtID, (a, b) => new {
                Id = a.Id,
                PubID = a.PubID,
                PubName = b.Publishing.PubName,
                PubDebt = b.PubDebt,
                Date = a.Date
            }).OrderBy(a => a.PubID).ThenBy(a => a.Date);
            return Json(joinpubdebt.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult StatisticDebt(DateTime date)
        {
            DateTime D = date;
            //lấy nhiều nợ: mã id, mã nxb và ngày nợ nhóm theo nxb và ngày
            var anypub_anydate = db.Pub_Debt.GroupBy(a => new { a.PubID, Date = DbFunctions.TruncateTime(a.DateCreate) }).Select(a => new {
                Id = a.Max(x => x.PDebtID),
                PubID = a.Key.PubID,
                Date = a.Key.Date
            });
            //lấy nợ phải trả <= ngày cần tìm nhưng là ngày lớn nhất của nxb đó
            var debt_max_lessthan = anypub_anydate.Where(a => DateTime.Compare((DateTime)a.Date, D.Date) <= 0).GroupBy(a => a.PubID)
                .Select(a => new {
                    PubID = a.Key,
                    Date = a.Max(x => x.Date)
                });
            var result = debt_max_lessthan.Join(anypub_anydate, a => new { a.PubID, a.Date }, b => new { b.PubID, b.Date }, (a, b) =>
                 new
                 {
                     Id = b.Id,
                     PubID = b.PubID,
                     Date = b.Date
                 });
            var debt = result.Join(db.Pub_Debt.Include(a => a.Publishing), x => x.Id, y => y.PDebtID, (x, y) => new
            {
                Id = x.Id,
                PubID = x.PubID,
                PubName = y.Publishing.PubName,
                PubDebt = y.PubDebt,
                Date = x.Date
            });
            return Json(debt.ToList(), JsonRequestBehavior.AllowGet);
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
