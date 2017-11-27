using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.Areas.Agency.Models
{
    public class ReportVM
    {
        public int AgencyID { get; set; }
        public Nullable<int> AdminConfirm { get; set; }
        public System.DateTime ReportDate { get; set; }
        public decimal SoldTotal { get; set; }
        public List<ReportSoldDetail> Details { get; set; }
    }
}