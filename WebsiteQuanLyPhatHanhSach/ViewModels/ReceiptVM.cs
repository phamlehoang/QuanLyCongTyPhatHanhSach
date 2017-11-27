using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.ViewModels
{
    public class ReceiptVM
    {
        public int AdminID { get; set; }
        public int PubID { get; set; }
        public string Deliver { get; set; }
        public System.DateTime Date { get; set; }
        public decimal Total { get; set; }
        public List<ReceiptDetail> Details { get; set; }
    }
}