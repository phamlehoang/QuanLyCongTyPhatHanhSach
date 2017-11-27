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
    public class BookSoldController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: BookSold
        public ActionResult Index()
        {
            return View();
        }

        // GET: BookSold/Details/2013
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var report = db.ReportSolds.Where(a => a.AgencyID == id && a.AdminConfirm == null);
            var booksold = db.ReportSoldDetails.Join(report, a => a.ReportID, b => b.ReportID, (a, b) => new
            {
                AgencyID = b.AgencyID,
                ReportID = a.ReportID,
                ISBN = a.ISBN,
                QuatitySold = a.QuatitySold,
                BookPrice = a.BookPrice,
                BookTotal = a.BookTotal,
            }).Join(db.Books, x => x.ISBN, y => y.ISBN, (x, y) => new BookSoldVM
            {
                AgencyID = x.AgencyID,
                ReportID = x.ReportID,
                ISBN = x.ISBN,
                BookName = y.BookName,
                QuatitySold = x.QuatitySold,
                BookPrice = x.BookPrice,
                BookTotal = x.BookTotal,
            });
            if (booksold == null)
            {
                return HttpNotFound();
            }
            return View(booksold.ToList());
        }

        private string CreateID()
        {
            string id = "";
            int count = db.IssueInvoices.ToList().Count();
            if (count == 0) id = "1";
            else id = (count + 1).ToString();
            id = "DLTT-" + id;
            return id;
        }

        private decimal GetDebtSum(int agencyid)
        {
            decimal debt = 0m;
            if (db.Agency_Debt.Where(b => b.AgencyID == agencyid).Count() == 0) return 0m;
            else
            {
                //lấy công nợ đang nợ của đại lý của ngày lớn nhất và Id max.
                DateTime d = db.Agency_Debt.Where(b => b.AgencyID == agencyid).Max(b => b.DateCreate);
                int id = db.Agency_Debt.Where(b => b.DateCreate == d && b.AgencyID == agencyid).Max(b => b.ADebtID);
                Agency_Debt age = db.Agency_Debt.Find(id);
                decimal iss = age.AgencyDebt;
                debt = iss;
                return debt;
            }
        }

        private decimal GetRevenue()
        {
            decimal debt = 0m;
            if (db.Revenues.Count() == 0) return 0m;
            else
            {
                //lấy doanh thu hiện tại.
                int id = db.Revenues.Max(b => b.Id);
                Revenue rev = db.Revenues.Find(id);
                decimal iss = rev.RevenueTotal;
                debt = iss;
                return debt;
            }
        }

        public JsonResult SubmitPay(InvoiceVM I)
        {
            bool status = false;
            if (!ModelState.IsValid)
            {
                //Lưu thông tin phiếu thanh toán vào bảng IssueInvoice
                IssueInvoice ii = new IssueInvoice {
                    InvoiceID = CreateID(), AgencyID = I.AgencyID, AdminID = I.AdminID,
                    InvoiceCreate = DateTime.Now, InvoiceAmount = I.InvoiceAmount
                };
                db.IssueInvoices.Add(ii);

                //Chỉnh sửa bảng ReportSold đã đc admin xác nhận, và update cột InvoiceID
                foreach(var id in I.ListReportID)
                {
                    ReportSold rep = db.ReportSolds.Find(id);
                    rep.InvoiceID = ii.InvoiceID;
                    rep.AdminConfirm = ii.AdminID;
                    db.Entry(rep).State = EntityState.Modified;
                }
                //Thêm thông tin vào bảng Agency_Debt
                Agency_Debt a_debt = new Agency_Debt
                {
                    AgencyID = I.AgencyID,
                    InvoiceID = ii.InvoiceID,
                    DateCreate = ii.InvoiceCreate,
                    AgencyInvoicePaid = I.InvoiceAmount,
                    AgencyDebt = Decimal.Subtract(GetDebtSum(I.AgencyID), I.InvoiceAmount),
                };
                db.Agency_Debt.Add(a_debt);
                //Cập nhật lại nợ vào bảng Agency_Paid_Debt
                Agency_Paid_Debt a_paid_debt = db.Agency_Paid_Debt.Find(I.AgencyID);
                a_paid_debt.Paid = Decimal.Add(a_paid_debt.Paid, I.InvoiceAmount);
                a_paid_debt.Debt = Decimal.Subtract(a_paid_debt.Debt, I.InvoiceAmount);
                db.Entry(a_paid_debt).State = EntityState.Modified;
                //Tính doanh thu và //Tính tiền phải trả cho Nhà xuất bản
                decimal revenue = 0m, pay = 0m;
                var booksold = db.ReportSoldDetails.Join(db.BookPrices, a => a.ISBN, b => b.ISBN, (a, b) => new
                {
                    ReportID = a.ReportID,
                    ISBN = a.ISBN,
                    QuatitySold = a.QuatitySold,
                    BookTotal = a.BookTotal,
                    Price = b.PurchasePrice * a.QuatitySold
                }).Where(x => I.ListReportID.Contains(x.ReportID)).ToList();
                foreach(var item in booksold)
                {
                    pay = Decimal.Add(pay, item.Price);
                }
                //doanh thu
                revenue = Decimal.Subtract(I.InvoiceAmount, pay);
                Revenue rev = new Revenue {
                    InvoiceID = ii.InvoiceID, InvoiceRevenue = revenue,
                    RevenueTotal = Decimal.Add(revenue, GetRevenue()), RevenueDate = ii.InvoiceCreate};
                db.Revenues.Add(rev);
                //doanh thu
                //phải trả cho nxb
                var pubpay = db.ReportSoldDetails.Join(db.BookPrices, a => a.ISBN, b => b.ISBN, (a, b) => new
                {
                    ReportID = a.ReportID,
                    ISBN = a.ISBN,
                    QuatitySold = a.QuatitySold,
                    BookTotal = a.BookTotal,
                    Price = b.PurchasePrice * a.QuatitySold
                }).Join(db.Books, c => c.ISBN, d => d.ISBN, (c, d) => new {
                    ReportID = c.ReportID,
                    ISBN = c.ISBN,
                    PubID = d.PubID,
                    QuatitySold = c.QuatitySold,
                    BookTotal = c.BookTotal,
                    Price = c.Price
                }).Where(x => I.ListReportID.Contains(x.ReportID)).GroupBy(x => x.PubID).Select(x => new {
                    PubID = x.Key,
                    Pay = x.Sum(y => y.Price)
                }).ToList();
                foreach (var item in pubpay)
                {
                    PayForPub pfp = new PayForPub
                    {
                        PubID = item.PubID,
                        InvoiceID = ii.InvoiceID,
                        PayTotal = item.Pay,
                        DateCreate = ii.InvoiceCreate
                    };//chưa cập nhật AdminID và PaymentID vì chưa thực hiện thanh toán cho NXB
                    db.PayForPubs.Add(pfp);
                }
                //phải trả cho nxb

                status = true;
                db.SaveChanges();
                
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}