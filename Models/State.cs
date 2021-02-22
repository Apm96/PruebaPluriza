using System;
using System.Collections.Generic;

namespace ProjectSales.Models
{
    public partial class State
    {
        public State()
        {
            Sales = new HashSet<Sales>();
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Sales> Sales { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
