using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteQuanLyPhatHanhSach.Models;
using WebsiteQuanLyPhatHanhSach.ViewModels;

namespace WebsiteQuanLyPhatHanhSach.Controllers
{
    public class PayForPubController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: PayForPub
        public ActionResult Index()
        {
            return View();
        }

        // GET: PayForPub
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pub = db.PayForPubs.Where(a => a.PubID == id && a.AdminID == null && a.PaymentID == null);
            var payforpub = pub.Join(db.Publishings, a => a.PubID, b => b.PubID, (a, b) => new PayForPubVM
            {
                Id = a.Id,
                PubID = a.PubID,
                PubName = b.PubName,
                PubAdr = b.PubAdr,
                PubPhone = b.PubPhone,
                AccountNum = b.AccountNum,
                InvoiceID = a.InvoiceID,
                PayTotal = a.PayTotal,
                DateCreate = a.DateCreate
            });
            if (payforpub == null)
            {
                return HttpNotFound();
            }
            return View(payforpub.ToList());
        }

        private string CreateID()
        {
            string id = "";
            int count = db.PaymentInvoices.ToList().Count();
            if (count == 0) id = "1";
            else id = (count + 1).ToString();
            id = "TTNXB-" + id;
            return id;
        }

        private decimal GetDebtSum(int pubid)
        {
            decimal debt = 0m;
            //lấy công nợ đang nợ của nxb của ngày lớn nhất và Id max.
            DateTime d = db.Pub_Debt.Where(b => b.PubID == pubid).Max(b => b.DateCreate);
            int id = db.Pub_Debt.Where(b => b.DateCreate == d && b.PubID == pubid).Max(b => b.PDebtID);
            Pub_Debt pub = db.Pub_Debt.Find(id);
            decimal iss = pub.PubDebt;
            debt = iss;
            return debt;
        }

        public JsonResult SubmitPay(PaymentVM P)
        {
            bool status = false;
            if (!ModelState.IsValid)
            {
                //Lưu thông tin phiếu thanh toán vào bảng PaymentInvoice
                PaymentInvoice pi = new PaymentInvoice {
                    PaymentID = CreateID(),
                    PubID = P.PubID,
                    AdminID = P.AdminID,
                    PaymentCreate = DateTime.Now,
                    PaymentAmount = P.PaymentAmount
                };
                db.PaymentInvoices.Add(pi);

                //Update lại AdminID và PaymentID trong bảng 
                foreach (var id in P.ListPayForPubID)
                {
                    PayForPub pfp = db.PayForPubs.Find(id);
                    pfp.PaymentID = pi.PaymentID;
                    pfp.AdminID = pi.AdminID;
                    db.Entry(pfp).State = EntityState.Modified;
                }

                //Thêm thông tin vào bảng Pub_Debt
                Pub_Debt p_debt = new Pub_Debt
                {
                    PubID = P.PubID,
                    PaymentID = pi.PaymentID,
                    DateCreate = pi.PaymentCreate,
                    PubInvoicePaid = P.PaymentAmount,
                    PubDebt = Decimal.Subtract(GetDebtSum(P.PubID), P.PaymentAmount),
                };
                db.Pub_Debt.Add(p_debt);

                status = true;
                db.SaveChanges();
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}