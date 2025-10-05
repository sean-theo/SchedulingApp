using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchedulingApp
{
    public partial class ReportsForm : Form
    {
        public ReportsForm()
        {
            InitializeComponent();
        }

        private class TypeByMonthRow
        {
            public int Year { get; set; }
            public string Month { get; set; }
            public string Type { get; set; }
            public int Count { get; set; }

            public int MonthNumber { get; set; }
        }

        private class ScheduleRow
        {
            public string User { get; set; }
            public string Customer { get; set; }
            public string Type { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        private class CustomerCountRow
        {
            public string Customer { get; set; }
            public int AppointmentCount { get; set; }
        }

        private void btnAppointmentsType_Click(object sender, EventArgs e)
        {
            var appts = Program.Appointments;
            dgvReports.DataSource = null;

            if (appts == null || appts.Count == 0)
            {
                dgvReports.DataSource = null;
                MessageBox.Show("No appointments to report.");
                return;
            }

            var counts = new Dictionary<Tuple<int,int,string>, int>();

            foreach (var a in appts)
            {
                DateTime localStart = a.Start;
                DateTime localEnd = a.End;
                int year = localStart.Year;
                int monthNum = localStart.Month;
                string Type = string.IsNullOrWhiteSpace(a.Type) ? "(Unspecified)" : a.Type.Trim();

                var key = Tuple.Create(year, monthNum, Type);
                if (!counts.ContainsKey(key)) counts[key] = 0;
                counts[key]++;
            }

            var rows = new List<TypeByMonthRow>();
            foreach (var kvp in counts)
            {
                int year = kvp.Key.Item1;
                int monthNum = kvp.Key.Item2;
                string type = kvp.Key.Item3;
                int count = kvp.Value;

                rows.Add(new TypeByMonthRow
                {
                    Year = year,
                    MonthNumber = monthNum,
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNum),
                    Type = type,
                    Count = count
                });
            }

            //lambda
            Comparison<TypeByMonthRow> sortLogic = (x, y) =>
            {
                int c = x.Year.CompareTo(y.Year);
                if (c != 0) return c;
                c = x.MonthNumber.CompareTo(y.MonthNumber);
                if (c != 0) return c;
                return string.Compare(x.Type, y.Type, StringComparison.OrdinalIgnoreCase);
            };

            rows.Sort(sortLogic);

            dgvReports.DataSource = rows;

            if (dgvReports.Columns["MonthNumber"] != null)
            {
                dgvReports.Columns["MonthNumber"].Visible = false;
            }
        }


        private void btnUser_Click(object sender, EventArgs e)
        {
            var appts = Program.Appointments;
            dgvReports.DataSource = null;

            if (appts == null || appts.Count == 0)
            {
                dgvReports.DataSource = null;
                MessageBox.Show("No appointments to report.");
                return;
            }

            List<ScheduleRow> rows = new List<ScheduleRow>();

            foreach (var a in appts)
            {
                DateTime localStart = a.Start;
                DateTime localEnd = a.End;

                rows.Add(new ScheduleRow
                {
                    User = "test",
                    Customer = a.CustomerName,
                    Type = a.Type,
                    Start = localStart,
                    End = localEnd,
                });
            }

            //lambda
            rows.Sort((x, y) => x.Start.CompareTo(y.Start));

            dgvReports.DataSource = null;
            dgvReports.AutoGenerateColumns = true;
            dgvReports.DataSource = rows;
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            var appts = Program.Appointments;
            dgvReports.DataSource = null;

            if (appts == null || appts.Count == 0)
            {
                dgvReports.DataSource = null;
                MessageBox.Show("No appointments to report.");
                return;
            }

            Dictionary<string, int> counts = new Dictionary<string, int>();

            foreach (var a in appts)
            {
                string name = string.IsNullOrWhiteSpace(a.CustomerName) ? "(Unknown)" : a.CustomerName.Trim();

                if (!counts.ContainsKey(name))
                    counts[name] = 0;

                counts[name]++;
            }

            List<CustomerCountRow> rows = new List<CustomerCountRow> ();
            
            foreach (var pair in counts)
            {
                rows.Add(new CustomerCountRow
                {
                    Customer = pair.Key,
                    AppointmentCount = pair.Value
                });
            }

            //lambda
            rows.Sort((x, y) => string.Compare(x.Customer, y.Customer, StringComparison.OrdinalIgnoreCase));

            dgvReports.DataSource = null;
            dgvReports.AutoGenerateColumns = true;
            dgvReports.DataSource = rows;
        }
    }
}
