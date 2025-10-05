using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingApp
{
    public class Appointment
    {
        public int Id { get; set; }
        public int CustomerId{ get; set; }
        public string CustomerName{ get; set; }
        public string Type { get; set; }
        public DateTime Start {  get; set; }
        public DateTime End { get; set; }

        public Appointment(int id, int customerId, string customerName, string type, DateTime start, DateTime end)
        {
            this.Id = id;
            this.CustomerId = customerId;
            this.CustomerName = customerName;
            this.Type = type;
            this.Start = start;
            this.End = end;
        }

        public Appointment() { }
    }
}
