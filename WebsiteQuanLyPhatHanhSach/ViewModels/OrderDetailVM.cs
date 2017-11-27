using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.ViewModels
{
    public class OrderDetailVM
    {
        //select OD.OrderID, OD.ISBN, OD.OrderQuantity, OD.BookPrice, OD.BookTotal, AI.InventoryQua
        public int OrderID { get; set; }
        public long ISBN { get; set; }
        public string BookName { get; set; }
        public int OrderQuantity { get; set; }
        public decimal BookPrice { get; set; }
        public decimal BookTotal { get; set; }
        public int InventoryQua { get; set; }
        public decimal OrderTotal { get; set; }
        public virtual Book Book { get; set; }
        public virtual OrderA OrderA { get; set; }
    }
}