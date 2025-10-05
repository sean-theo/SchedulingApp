using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchedulingApp
{
    public static class Program
    {
        public static BindingList<Customer> Customers = new BindingList<Customer>();
        public static BindingList<Appointment> Appointments = new BindingList<Appointment>();

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Program.Customers = Database.LoadCustomers();
                Program.Appointments = Database.LoadAppointments();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during startup load: " + ex.Message);
            }

            Application.Run(new LoginForm());
        }
    }
}
