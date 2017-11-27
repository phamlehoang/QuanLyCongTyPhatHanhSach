using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteQuanLyPhatHanhSach.Areas.Agency.Models;
using WebsiteQuanLyPhatHanhSach.Models;
using WebsiteQuanLyPhatHanhSach.ViewModels;

namespace WebsiteQuanLyPhatHanhSach.Areas.Agency.Controllers
{
    public class RegisterController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: Agency/Register
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM reg)
        {
            if (ModelState.IsValid)
            {
                if (reg.AgencyPass.Equals(reg.AgencyRePass))
                {
                    WebsiteQuanLyPhatHanhSach.Models.Agency age = new WebsiteQuanLyPhatHanhSach.Models.Agency
                    {
                        AgencyName = reg.AgencyName,
                        AgencyAdr = reg.AgencyAdr,
                        AgencyPhone = reg.AgencyPhone,
                        AgencyUser = reg.AgencyUser,
                        AgencyPass = reg.AgencyPass
                    };
                    db.Agencies.Add(age);
                    db.SaveChanges();
                    ViewData["Alert"] = "Đăng ký tài khoản thành công";
                    return RedirectToAction("Index", "Login");//Agency/Agency/Login/Index
                }
                else
                    ModelState.AddModelError("", "Mật khẩu không khớp");
            }
            return View("Index");
        }
    }
}