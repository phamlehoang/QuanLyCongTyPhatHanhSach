using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.Controllers
{
    public class AgenciesController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: Agency
        public ActionResult Index()
        {
            return View(db.Agencies.ToList());
        }

        // GET: DaiLy/Details/5
        public ActionResult Details(int? id)
        {
            //var daily = db.DaiLies.ToList().Find(m => m.MaDL == id);
            //return Json(daily, JsonRequestBehavior.AllowGet);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agency age = db.Agencies.Find(id);
            if (age == null)
            {
                return HttpNotFound();
            }
            return View(age);
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