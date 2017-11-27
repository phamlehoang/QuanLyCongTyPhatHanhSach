using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.ViewModels
{
    public class BookSoldVM
    {
        public int AgencyID { get; set; }
        public int ReportID { get; set; }
        public long ISBN { get; set; }
        public string BookName { get; set; }
        public int QuatitySold { get; set; }
        public decimal BookPrice { get; set; }
        public decimal BookTotal { get; set; }
        public virtual Book Book { get; set; }
        public virtual ReportSold ReportSold { get; set; }
    }
}