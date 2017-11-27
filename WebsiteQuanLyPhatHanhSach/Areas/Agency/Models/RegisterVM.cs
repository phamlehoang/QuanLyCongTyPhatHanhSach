using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteQuanLyPhatHanhSach.Areas.Agency.Models
{
    public class RegisterVM
    {
        public string AgencyName { get; set; }
        public string AgencyAdr { get; set; }
        public string AgencyPhone { get; set; }
        public string AgencyUser { get; set; }
        public string AgencyPass { get; set; }
        public string AgencyRePass { get; set; }
    }
}