using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteQuanLyPhatHanhSach.Models;
using WebsiteQuanLyPhatHanhSach.ViewModels;

namespace WebsiteQuanLyPhatHanhSach.Controllers
{
    public class ReceiptsController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: Receipts
        public ActionResult Index()
        {
            var receipts = db.Receipts.Include(r => r.AdminA).Include(r => r.Publishing);
            return View(receipts.ToList());
        }
        public JsonResult GetBook(int pubid)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return Json( db.Books.Where(a => a.PubID == pubid).ToList(), JsonRequestBehavior.AllowGet );
        }

        public JsonResult GetPrice(long isbn)
        {
            db.Configuration.ProxyCreationEnabled = false;
            long id = Convert.ToInt64(isbn);//sẽ có lỗi nếu ko convert qua đúng kiểu dữ liệu
            return Json( db.BookPrices.Where(a => a.ISBN == id).ToList(), JsonRequestBehavior.AllowGet );
        }

        // GET: Receipts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Receipt receipt = db.Receipts.Find(id);
            if (receipt == null)
            {
                return HttpNotFound();
            }
            return View(receipt);
        }

        // GET: Receipts/Create
        public ActionResult Create()
        {
            ViewBag.AdminID = new SelectList(db.AdminAs, "AdminID", "AdminName");
            ViewBag.PubID = new SelectList(db.Publishings, "PubID", "PubName");
            return View();
        }

        private string CreateID()
        {
            string id = "";
            int count = db.Receipts.ToList().Count();
            if (count == 0) id = "1";
            else id = (count + 1).ToString();
            id = "PN-" + id;
            return id;
        }

        private int GetInvent(long ISBN, DateTime date)
        {
            int i = 0;
            long isbn = Convert.ToInt64(ISBN);
            DateTime datenow = DateTime.Now;
            Console.WriteLine(datenow);
            if (db.Inventories.Where(b => b.ISBN == isbn).Count() == 0) return 0;
            if (DateTime.Compare(date.Date, datenow.Date) >= 0)//Nếu ngày nhập là hiện tại.
            {
                //lấy tồn kho của sách isbn của ngày lớn nhất và Id max.
                DateTime d = db.Inventories.Where(b => b.ISBN == isbn).Max(b => b.Date);
                int id = db.Inventories.Where(b => b.Date == d && b.ISBN == isbn).Max(b => b.Id);
                Inventory inv = db.Inventories.Find(id);
                int invent = inv.InventoryQua;
                i = invent;
            }
            else //Nếu ngày nhập là quá khứ.
            {
                //lấy tồn kho của sách isbn của ngày lớn nhất <= date và Id max.
                //Lỗi 1: nhập ngày 18/11 (OK), test nhập ngày 16/11(Erorr) vì database không có ngày nhập nào nhỏ hơn 16/11
                DateTime dMin = db.Inventories.Where(b => b.ISBN == isbn).Min(b => b.Date);
                if (DateTime.Compare(date.Date, dMin.Date) < 0) {
                    i = 0;
                }
                else {
                    DateTime d = db.Inventories.Where(b => b.ISBN == isbn && b.Date <= date).Max(b => b.Date);
                    int id = db.Inventories.Where(b => b.Date == d && b.ISBN == isbn).Max(b => b.Id);
                    Inventory inv = db.Inventories.Find(id);
                    int invent = inv.InventoryQua;
                    i = invent;
                }
            }

            return i;
        }

        private decimal GetDebtSum(int pubid, DateTime date)
        {
            decimal debt = 0m;
            DateTime datenow = DateTime.Now;
            if (db.Pub_Debt.Where(b => b.PubID == pubid).Count() == 0) return 0m;
            if (DateTime.Compare(date.Date, datenow.Date) >= 0)//Nếu ngày nhập là hiện tại.
            {
                //lấy công nợ đang nợ của nxb của ngày lớn nhất và Id max.
                DateTime d = db.Pub_Debt.Where(b => b.PubID == pubid).Max(b => b.DateCreate);
                int id = db.Pub_Debt.Where(b => b.DateCreate == d && b.PubID == pubid).Max(b => b.PDebtID);
                Pub_Debt pub = db.Pub_Debt.Find(id);
                decimal iss = pub.PubDebt;
                debt = iss;
            }
            else
            {
                //lấy công nợ đang nợ của nxb của ngày lớn nhất <= date và Id max.
                //Lỗi 1: nhập ngày 18/11 (OK), test nhập ngày 16/11(Erorr) vì database không có ngày nhập nào nhỏ hơn 16/11
                DateTime dMin = db.Pub_Debt.Where(b => b.PubID == pubid).Min(b => b.DateCreate);
                if (DateTime.Compare(date.Date, dMin.Date) < 0)
                {
                    debt = 0m;
                }
                else
                {
                    DateTime d = db.Pub_Debt.Where(b => b.PubID == pubid && b.DateCreate <= date).Max(b => b.DateCreate);
                    int id = db.Pub_Debt.Where(b => b.DateCreate == d && b.PubID == pubid).Max(b => b.PDebtID);
                    Pub_Debt pub = db.Pub_Debt.Find(id);
                    decimal iss = pub.PubDebt;
                    debt = iss;
                }
            }
            return debt;
        }

        [HttpPost]
        public JsonResult CreateReceipt(ReceiptVM R)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                //Tạo phiếu nhập sách và chi tiết phiếu nhập.
                Receipt rep = new Receipt { ReceiptID = CreateID(), AdminID = R.AdminID, PubID = R.PubID,
                    Deliver = R.Deliver, ReceiptCreate = R.Date, ReceiptTotal = R.Total };
                foreach (var i in R.Details)
                {
                    i.ReceiptID = rep.ReceiptID;
                    rep.ReceiptDetails.Add(i);
                    //Tính tồn kho.
                    Inventory invent = new Inventory();
                    invent.ISBN = i.ISBN; invent.ReceiptID = rep.ReceiptID; invent.ReceiptQua = i.ReceiptQuatity;
                    /*invent.IssueQua = 0;*/ invent.Date = rep.ReceiptCreate;
                    invent.InventoryQua = i.ReceiptQuatity + GetInvent(i.ISBN, rep.ReceiptCreate);
                    rep.Inventories.Add(invent);
                    
                    //Tính tồn kho cho các ngày sau nếu ngày nhập là quá khứ
                    if (DateTime.Compare(rep.ReceiptCreate.Date, DateTime.Now.Date) < 0)
                    {
                        var listinv = db.Inventories.Where(b => b.ISBN == i.ISBN && b.Date > rep.ReceiptCreate).ToList();
                        foreach(var inv in listinv)
                        {
                            inv.InventoryQua += i.ReceiptQuatity;
                            if (ModelState.IsValid)
                            {
                                db.Entry(inv).State = EntityState.Modified;
                                //db.SaveChanges();
                            }
                        }
                    }
                }
                db.Receipts.Add(rep);

                //Tạo phiếu nợ nhà xuất bản.
                Pub_Debt p_debt = new Pub_Debt { PubID = rep.PubID, ReceiptID = rep.ReceiptID, DateCreate = rep.ReceiptCreate,
                    PubIssueDebt = rep.ReceiptTotal, PubDebt = Decimal.Add(rep.ReceiptTotal,GetDebtSum(rep.PubID, rep.ReceiptCreate))
                };
                rep.Pub_Debt.Add(p_debt);
                //Tính lại nợ cho các ngày sau nếu ngày nhập là quá khứ
                if (DateTime.Compare(rep.ReceiptCreate.Date, DateTime.Now.Date) < 0)
                {
                    var listdebt = db.Pub_Debt.Where(b => b.PubID == rep.PubID && b.DateCreate > rep.ReceiptCreate).ToList();
                    foreach (var debt in listdebt)
                    {
                        debt.PubDebt += rep.ReceiptTotal;
                        if (ModelState.IsValid)
                        {
                            db.Entry(debt).State = EntityState.Modified;
                            //db.SaveChanges();
                        }
                    }
                }

                db.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        // POST: Receipts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AdminID,PubID,Deliver,ReceiptCreate,ReceiptTotal")] Receipt receipt)
        {
            if (ModelState.IsValid)
            {
                db.Receipts.Add(receipt);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdminID = new SelectList(db.AdminAs, "AdminID", "AdminName", receipt.AdminID);
            ViewBag.PubID = new SelectList(db.Publishings, "PubID", "PubName", receipt.PubID);
            return View(receipt);
        }

        // GET: Receipts/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Receipt receipt = db.Receipts.Find(id);
            if (receipt == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdminID = new SelectList(db.AdminAs, "AdminID", "AdminName", receipt.AdminID);
            ViewBag.PubID = new SelectList(db.Publishings, "PubID", "PubName", receipt.PubID);
            return View(receipt);
        }

        // POST: Receipts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AdminID,PubID,Deliver,ReceiptCreate,ReceiptTotal")] Receipt receipt)
        {
            if (ModelState.IsValid)
            {
                db.Entry(receipt).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdminID = new SelectList(db.AdminAs, "AdminID", "AdminName", receipt.AdminID);
            ViewBag.PubID = new SelectList(db.Publishings, "PubID", "PubName", receipt.PubID);
            return View(receipt);
        }

        // GET: Receipts/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Receipt receipt = db.Receipts.Find(id);
            if (receipt == null)
            {
                return HttpNotFound();
            }
            return View(receipt);
        }

        // POST: Receipts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Receipt receipt = db.Receipts.Find(id);
            db.Receipts.Remove(receipt);
            db.SaveChanges();
            return RedirectToAction("Index");
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
