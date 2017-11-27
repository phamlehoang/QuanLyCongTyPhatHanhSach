using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.ViewModels
{
    public class BookVM
    {
        public long ISBN { get; set; }
        public string BookName { get; set; }
        public string BookCategory { get; set; }
        public int PubID { get; set; }
        public string BookAuthor { get; set; }
        public string BookPages { get; set; }
        public string BookDescribe { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
    }
}