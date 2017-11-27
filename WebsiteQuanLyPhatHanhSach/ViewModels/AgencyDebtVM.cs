using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteQuanLyPhatHanhSach.ViewModels
{
    public class AgencyDebtVM
    {
        public int AgencyID { get; set; }
        public string AgencyUser { get; set; }
        public string AgencyName { get; set; }
        public decimal AgencyDebt { get; set; }
    }
}