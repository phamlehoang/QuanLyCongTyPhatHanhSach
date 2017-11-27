using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteQuanLyPhatHanhSach.ViewModels
{
    public class InvoiceVM
    {
        public List<Int32> ListReportID { get; set; }
        public int AgencyID { get; set; }
        public int AdminID { get; set; }
        public System.DateTime InvoiceCreate { get; set; }
        public decimal InvoiceAmount { get; set; }
    }
}