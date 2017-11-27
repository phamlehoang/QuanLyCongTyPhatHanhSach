using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteQuanLyPhatHanhSach.Areas.Agency.Models
{
    public class BooksVM
    {
        public long ISBN { get; set; }
        public string BookName { get; set; }
        public string BookCategory { get; set; }
        public string PubName { get; set; }
        public string BookAuthor { get; set; }
        public string BookPages { get; set; }
        public string BookDescribe { get; set; }
        public Nullable<decimal> SellingPrice { get; set; }
    }

}