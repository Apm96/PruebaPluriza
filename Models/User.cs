using System;
using System.Collections.Generic;

namespace ProjectSales.Models
{
    public partial class User
    {
        public User()
        {
            Sales = new HashSet<Sales>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Userr { get; set; }
        public string Password { get; set; }
        public int? IdUserType { get; set; }
        public int? IdEstate { get; set; }
        public string Email { get; set; }

        public virtual State IdEstateNavigation { get; set; }
        public virtual UserType IdUserTypeNavigation { get; set; }
        public virtual ICollection<Sales> Sales { get; set; }
    }
}
