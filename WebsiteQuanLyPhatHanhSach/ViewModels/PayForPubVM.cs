using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteQuanLyPhatHanhSach.Models;

namespace WebsiteQuanLyPhatHanhSach.ViewModels
{
    public class PayForPubVM
    {
        public int Id { get; set; }
        public int PubID { get; set; }
        public string PubName { get; set; }
        public string PubAdr { get; set; }
        public string PubPhone { get; set; }
        public string AccountNum { get; set; }
        public Nullable<int> AdminID { get; set; }
        public string InvoiceID { get; set; }
        public string PaymentID { get; set; }
        public decimal PayTotal { get; set; }
        public System.DateTime DateCreate { get; set; }
    }
}