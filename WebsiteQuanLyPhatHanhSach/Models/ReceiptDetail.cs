//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebsiteQuanLyPhatHanhSach.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReceiptDetail
    {
        public string ReceiptID { get; set; }
        public long ISBN { get; set; }
        public int ReceiptQuatity { get; set; }
        public decimal BookPrice { get; set; }
        public decimal BookTotal { get; set; }
    
        public virtual Book Book { get; set; }
        public virtual Receipt Receipt { get; set; }
    }
}