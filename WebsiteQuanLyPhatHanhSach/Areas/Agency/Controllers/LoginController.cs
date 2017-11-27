using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.Areas.Agency.Controllers
{
    public class LoginController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();
        // GET: Agency/Login
        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(WebsiteQuanLyPhatHanhSach.Models.Agency age)
        {
            if (ModelState.IsValid)
            {
                var obj = db.Agencies.Where(a => a.AgencyUser.Equals(age.AgencyUser) && a.AgencyPass.Equals(age.AgencyPass)).FirstOrDefault();
                if (obj != null)
                {
                    Session["AgencyID"] = obj.AgencyID.ToString();
                    Session["AgencyName"] = obj.AgencyName.ToString();
                    Session["AgencyUser"] = obj.AgencyUser.ToString();

                    //Session.Add("Admin", obj);
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Đăng nhập không thành công");
            }
            return View("Index");
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session["AgencyID"] = null;
            Session["AgencyName"] = null;
            Session["AgencyUser"] = null;
            return RedirectToAction("Index", "Login");
        }
    }
}