using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.Controllers
{
    public class InventoriesController : Controller
    {
        private QLPhatHanhSachEntities db = new QLPhatHanhSachEntities();

        // GET: Inventories/Index
        public ActionResult Index()
        {
            var inventories = db.Inventories.Include(i => i.Book).Include(i => i.Issue).Include(i => i.Receipt)
                .OrderBy(i => i.ISBN).ThenBy(i => i.Id).ThenBy(i => i.Date);
            return View(inventories.ToList());
        }

        public ActionResult IndexInventory()
        {
            return View();
        }

        // GET: Inventories/IndexInventory
        public JsonResult GetInventory(DateTime date)
        {
            DateTime D = date;
            //Lấy tồn kho theo từng sách và từng ngày-list lớn
            var inv_By_isbn_By_date = db.Inventories.GroupBy(a => new { a.ISBN, Date = DbFunctions.TruncateTime(a.Date) }).Select(a => new {
                Id = a.Max(x=>x.Id), ISBN = a.Key.ISBN, Date = a.Key.Date
            });
            //lấy tồn kho <= ngày cần tìm nhưng là ngày lớn nhất của mã ISBN sách đó
            var inv_max_lessthan = inv_By_isbn_By_date.Where(a => DateTime.Compare((DateTime)a.Date, D.Date) <= 0).GroupBy(a => a.ISBN)
                .Select(a => new {
                    ISBN = a.Key, Date = a.Max(x => x.Date)
                });
            var result = inv_max_lessthan.Join(inv_By_isbn_By_date, a => new { a.ISBN, a.Date }, b => new { b.ISBN, b.Date }, (a, b) =>
                 new
                 {
                     Id = b.Id, ISBN = b.ISBN, Date = b.Date
                 });
            var inv = result.Join(db.Inventories.Include(a => a.Book), x => x.Id, y => y.Id, (x, y) => new 
            {
                Id = x.Id, ISBN = x.ISBN, BookName = y.Book.BookName, InventoryQua = y.InventoryQua, Date = x.Date
            });
            return Json(inv.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Inventories/IndexInventory
        public JsonResult GetAllInventory()
        {
            //Lấy tồn kho theo từng sách và từng ngày-list lớn
            var inv_By_isbn_By_date = db.Inventories.GroupBy(a => new { a.ISBN, Date = DbFunctions.TruncateTime(a.Date) }).Select(a => new {
                Id = a.Max(x => x.Id),
                ISBN = a.Key.ISBN,
                Date = a.Key.Date
            }).Join(db.Inventories.Include(a => a.Book), a => a.Id, b => b.Id, (a, b) => new {
                Id = a.Id,
                ISBN = a.ISBN,
                BookName = b.Book.BookName,
                InventoryQua = b.InventoryQua,
                Date = a.Date
            }).OrderBy(a => a.ISBN).ThenBy(a => a.Date);
            
            return Json(inv_By_isbn_By_date.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Inventories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
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
