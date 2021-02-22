using System;
using System.Collections.Generic;

namespace ProjectSales.Models
{
    public partial class SaleDetail
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Price { get; set; }
        public int? IdSales { get; set; }

        public virtual Sales IdSalesNavigation { get; set; }
    }
}
