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
    
    public partial class Inventory
    {
        public int Id { get; set; }
        public long ISBN { get; set; }
        public string ReceiptID { get; set; }
        public string IssueID { get; set; }
        public Nullable<int> ReceiptQua { get; set; }
        public Nullable<int> IssueQua { get; set; }
        public int InventoryQua { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual Book Book { get; set; }
        public virtual Issue Issue { get; set; }
        public virtual Receipt Receipt { get; set; }
    }
}
