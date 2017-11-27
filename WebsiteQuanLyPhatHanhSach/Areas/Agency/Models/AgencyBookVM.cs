using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteQuanLyPhatHanhSach.Areas.Agency.Models
{
    public class AgencyBookVM
    {
        public int AgencyID { get; set; }
        public long ISBN { get; set; }
        public string BookName { get; set; }
        public int BookQuantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal BookTotal { get; set; }
    }
}