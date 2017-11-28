using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteQuanLyPhatHanhSach.Models;
using WebsiteQuanLyPhatHanhSach.ViewModels;

namespace WebsiteQuanLyPhatHanhSach.Controllers
{
    public class IssuesController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: Issues
        public ActionResult Index()
        {
            var orderAs = db.OrderAs.Include(o => o.AdminA).Include(o => o.Agency);
            return View(orderAs.ToList());
        }

        // GET: Issues/Details/5
        public ActionResult OrderDetails(int? id, int? agencyid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int orderid = Convert.ToInt32(id); int ageid = Convert.ToInt32(agencyid);
            string query = "select Ag.AgencyID, Ag.AgencyName,  CASE WHEN AD.AgencyDebt IS NULL THEN 0 ELSE AD.AgencyDebt END as AgencyDebt "+
                        "from Agency Ag left join (select A.AgencyID, Ag.AgencyName, A.AgencyDebt "+
                        "from Agency_Debt A, Agency Ag, (select M.AgencyID, Max(A.ADebtID) as MaxID " +
                        "from Agency_Debt A Inner Join(select A.AgencyID, MAX(A.DateCreate) as MaxDate " +
                        "from Agency_Debt A group by A.AgencyID) M " +
                        "on A.DateCreate = M.MaxDate where A.AgencyID = M.AgencyID group by M.AgencyID) M " +
                        "where A.ADebtID = M.MaxID and Ag.AgencyID = A.AgencyID) AD "+
                        "on Ag.AgencyID = AD.AgencyID where Ag.AgencyID = " + ageid;
            IEnumerable<AgencyDebtVM> agency = db.Database.SqlQuery<AgencyDebtVM>(query);

            query = "select OD.OrderID, OD.ISBN, AI.BookName, OD.OrderQuantity, OD.BookPrice, OD.BookTotal, AI.InventoryQua, O.OrderTotal " +
                           "from OrderA O, OrderDetail OD left join " +
                     "(select B.ISBN, B.BookName, CASE WHEN D.InventoryQua IS NULL THEN 0 ELSE D.InventoryQua END as InventoryQua " +
                      "from Book B left join (select Iv.ISBN, Iv.InventoryQua from Inventory Iv, " +
                                "(select I.ISBN, Max(Id) as MaxId " +
                                 "from Inventory I Inner Join(select ISBN, Max(Date) as MaxDate " +
                                                             "from Inventory " +
                                                             "group by ISBN) M " +
                                 "On I.ISBN = M.ISBN and I.Date = M.MaxDate " +
                                 "group by I.ISBN) MM " +
                                             "where Iv.Id = MM.MaxId) D " +
                      "on B.ISBN = D.ISBN) AI " +
                            "on OD.ISBN = AI.ISBN " + "where O.OrderID = OD.OrderID and OD.OrderID = " + orderid;
            IEnumerable<OrderDetailVM> orders = db.Database.SqlQuery<OrderDetailVM>(query);

            dynamic mymodel = new ExpandoObject();
            mymodel.AgencyDebt = agency.ToList();
            mymodel.OrderDetail = orders.ToList();
            if (agency == null || orders == null)
            {
                return HttpNotFound();
            }
            //return View(orders.ToList());
            return View(mymodel);
        }

        private string CreateID()
        {
            string id = "";
            int count = db.Issues.ToList().Count();
            if (count == 0) id = "1";
            else id = (count + 1).ToString();
            id = "PX-" + id;
            return id;
        }

        private int GetInvent(long ISBN, DateTime date)
        {
            int i = 0;
            long isbn = Convert.ToInt64(ISBN);
            DateTime datenow = DateTime.Now;
            Console.WriteLine(datenow);
            if (db.Inventories.Where(b => b.ISBN == isbn).Count() == 0) return 0;
            else {
                //lấy tồn kho của sách isbn của ngày lớn nhất và Id max.
                DateTime d = db.Inventories.Where(b => b.ISBN == isbn).Max(b => b.Date);
                int id = db.Inventories.Where(b => b.Date == d && b.ISBN == isbn).Max(b => b.Id);
                Inventory inv = db.Inventories.Find(id);
                int invent = inv.InventoryQua;
                i = invent;
                return i;
            }
        }

        private decimal GetDebtSum(int agencyid)
        {
            decimal debt = 0m;
            if (db.Agency_Debt.Where(b => b.AgencyID == agencyid).Count() == 0) return 0m;
            else{
                //lấy công nợ đang nợ của đại lý của ngày lớn nhất và Id max.
                DateTime d = db.Agency_Debt.Where(b => b.AgencyID == agencyid).Max(b => b.DateCreate);
                int id = db.Agency_Debt.Where(b => b.DateCreate == d && b.AgencyID == agencyid).Max(b => b.ADebtID);
                Agency_Debt age = db.Agency_Debt.Find(id);
                decimal iss = age.AgencyDebt;
                debt = iss;
                return debt;
            }
        }

        public JsonResult CreateIssue(IssueVM I)
        {
            bool status = false;
            if (!ModelState.IsValid)
            {
                //Tạo phiếu nhập.
                Issue issue = new Issue { IssueID = CreateID(), OrderID = I.OrderID, IssueCreate = DateTime.Now,
                    AdminIssue = I.AdminIssue, Recepient = I.Recepient
                };
                db.Issues.Add(issue);
                //Đổi trạng thái cột OrderStatus và thêm dữ liệu vào cột AdminConfirm.
                OrderA order = db.OrderAs.Find(I.OrderID);
                order.OrderStatus = 1;
                order.AdminConfirm = issue.AdminIssue;
                db.Entry(order).State = EntityState.Modified;
                //Cập nhật công nợ cho đại lý.
                Agency_Debt a_debt = new Agency_Debt { AgencyID = I.AgencyID, IssueID = issue.IssueID, DateCreate = issue.IssueCreate,
                    AgencyIssueDebt = I.AgencyIssueDebt, AgencyDebt = Decimal.Add(I.AgencyIssueDebt, GetDebtSum(I.AgencyID)),
                };
                db.Agency_Debt.Add(a_debt);
                //nợ trong bảng nợ và đã trả
                Agency_Paid_Debt a_paid_debt = db.Agency_Paid_Debt.Find(I.AgencyID);
                if(a_paid_debt == null)
                {
                    Agency_Paid_Debt apd = new Agency_Paid_Debt();
                    apd.AgencyID = I.AgencyID;
                    apd.Paid = 0;
                    apd.Debt = a_debt.AgencyDebt;
                    db.Agency_Paid_Debt.Add(apd);
                }
                else
                {
                    a_paid_debt.Debt = Decimal.Add(a_paid_debt.Debt, I.AgencyIssueDebt);
                    db.Entry(a_paid_debt).State = EntityState.Modified;
                }
                
                //Tính tồn kho.
                foreach (var i in I.Details)
                {
                    Inventory invent = new Inventory();
                    invent.ISBN = i.ISBN; invent.IssueID = issue.IssueID; invent.IssueQua = i.OrderQuantity;
                    invent.Date = issue.IssueCreate;
                    invent.InventoryQua = GetInvent(i.ISBN, issue.IssueCreate) - i.OrderQuantity;
                    db.Inventories.Add(invent);
                }
                //Cộng số sách đại lý đặt vào bảng Agency_Book
                foreach (var i in I.Details)
                {
                    Agency_Book ab = db.Agency_Book.Find(I.AgencyID,i.ISBN);
                    if(ab == null)
                    {
                        Agency_Book a = new Agency_Book();
                        a.AgencyID = I.AgencyID;
                        a.ISBN = i.ISBN;
                        a.BookQuantity = i.OrderQuantity;
                        db.Agency_Book.Add(a);
                    }
                    else
                    {
                        ab.BookQuantity = ab.BookQuantity + i.OrderQuantity;
                        db.Entry(ab).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        public JsonResult CancleIssue(int orderid, int? admincancle)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                //Đổi trạng thái cột OrderStatus và thêm dữ liệu vào cột AdminConfirm.
                OrderA order = db.OrderAs.Find(orderid);
                order.OrderStatus = 2;
                order.AdminConfirm = admincancle;
                db.Entry(order).State = EntityState.Modified;
                
                db.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        // GET: Issues/IssueDetails/5
        public ActionResult IssueDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = db.OrderAs.Include(o => o.AdminA).Include(o => o.Agency).Where(o => o.OrderID == id);
            var issue = db.Issues.Include(i => i.AdminA).Include(i => i.OrderA).Where(i => i.OrderID == id);
            var orderdetail = db.OrderDetails.Include(od => od.Book).Where(od => od.OrderID == id);
            dynamic mymodel = new ExpandoObject();
            mymodel.Order = order.ToList();
            mymodel.Issue = issue.ToList();
            mymodel.OrderDetail = orderdetail.ToList();

            if (order == null || issue == null || orderdetail == null)
            {
                return HttpNotFound();
            }
            return View(mymodel);
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
