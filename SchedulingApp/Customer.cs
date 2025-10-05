using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingApp
{
    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public Customer(int ID, string Name, string Address, string Phone)
        {
            this.ID = ID;
            this.Name = Name;
            this.Address = Address;
            this.Phone = Phone;
        }
    }
}
