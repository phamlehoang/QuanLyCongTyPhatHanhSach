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
    
    public partial class IssueInvoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IssueInvoice()
        {
            this.Agency_Debt = new HashSet<Agency_Debt>();
            this.PayForPubs = new HashSet<PayForPub>();
            this.ReportSolds = new HashSet<ReportSold>();
            this.Revenues = new HashSet<Revenue>();
        }
    
        public string InvoiceID { get; set; }
        public int AgencyID { get; set; }
        public int AdminID { get; set; }
        public System.DateTime InvoiceCreate { get; set; }
        public decimal InvoiceAmount { get; set; }
    
        public virtual AdminA AdminA { get; set; }
        public virtual Agency Agency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Agency_Debt> Agency_Debt { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayForPub> PayForPubs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportSold> ReportSolds { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Revenue> Revenues { get; set; }
    }
}
