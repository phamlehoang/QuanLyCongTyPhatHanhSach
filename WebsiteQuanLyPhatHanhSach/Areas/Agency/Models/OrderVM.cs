using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.Areas.Agency.Models
{
    public class OrderVM
    {
        public int AgencyID { get; set; }
        public int AdminID { get; set; }
        public System.DateTime Date { get; set; }
        public int Status { get; set; }
        public decimal Total { get; set; }
        public List<OrderDetail> Details { get; set; }
    }
}