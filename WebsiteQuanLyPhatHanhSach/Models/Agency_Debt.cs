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
    
    public partial class Agency_Debt
    {
        public int ADebtID { get; set; }
        public int AgencyID { get; set; }
        public string IssueID { get; set; }
        public string InvoiceID { get; set; }
        public Nullable<decimal> AgencyInvoicePaid { get; set; }
        public Nullable<decimal> AgencyIssueDebt { get; set; }
        public decimal AgencyDebt { get; set; }
        public System.DateTime DateCreate { get; set; }
    
        public virtual Agency Agency { get; set; }
        public virtual IssueInvoice IssueInvoice { get; set; }
        public virtual Issue Issue { get; set; }
    }
}
