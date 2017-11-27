using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteQuanLyPhatHanhSach.ViewModels
{
    public class PaymentVM
    {
        public List<Int32> ListPayForPubID { get; set; }
        public int PubID { get; set; }
        public int AdminID { get; set; }
        public System.DateTime PaymentCreate { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}