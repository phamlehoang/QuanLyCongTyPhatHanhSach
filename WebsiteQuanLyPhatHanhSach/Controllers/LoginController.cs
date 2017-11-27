using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.Controllers
{
    public class LoginController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: Login
        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AdminA adm)
        {
            if (ModelState.IsValid)
            {
                var obj = db.AdminAs.Where(a => a.AdminUser.Equals(adm.AdminUser) && a.AdminPass.Equals(adm.AdminPass)).FirstOrDefault();
                if (obj != null)
                {
                    Session["AdminID"] = obj.AdminID.ToString();
                    Session["AdminName"] = obj.AdminName.ToString();
                    Session["AdminUser"] = obj.AdminUser.ToString();

                    //Session.Add("Admin", obj);
                    return RedirectToAction("Manage", "BookManage");
                }
                else
                    ModelState.AddModelError("", "Đăng nhập không thành công");
            }
            return View("Index");
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session["AdminID"] = null;
            Session["AdminName"] = null;
            Session["AdminUser"] = null;
            return RedirectToAction("Index", "Login");
        }
    }
}