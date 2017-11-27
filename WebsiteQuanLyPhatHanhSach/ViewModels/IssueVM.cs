using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.ViewModels
{
    public class IssueVM
    {
        public string IssueID { get; set; }
        public int OrderID { get; set; }
        public System.DateTime IssueCreate { get; set; }
        public int AdminIssue { get; set; }
        public string Recepient { get; set; }
        public int AgencyID { get; set; }
        public decimal AgencyIssueDebt { get; set; }
        public List<OrderDetail> Details { get; set; }
    }
}