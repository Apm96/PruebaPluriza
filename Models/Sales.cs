using System;
using System.Collections.Generic;

namespace ProjectSales.Models
{
    public partial class Sales
    {
        public Sales()
        {
            SaleDetail = new HashSet<SaleDetail>();
        }

        public int Id { get; set; }
        public string Settled { get; set; }
        public DateTime? Date { get; set; }
        public string Observation { get; set; }
        public int? IdUser { get; set; }
        public int? IdEstate { get; set; }
        public DateTime? AuditDate { get; set; }

        public virtual State IdEstateNavigation { get; set; }
        public virtual User IdUserNavigation { get; set; }
        public virtual ICollection<SaleDetail> SaleDetail { get; set; } = new List<SaleDetail>();
    }
}
