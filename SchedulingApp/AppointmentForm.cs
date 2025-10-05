using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchedulingApp
{
    public partial class AppointmentForm : Form   
    {
        private BindingList<Customer> _customers;
        private int nextAppointmentId = 1;
        private Appointment selectedAppointment;

        public AppointmentForm(BindingList<Customer> customersList)
        {
            InitializeComponent();

            cmbType.Items.Add("Consultation");
            cmbType.Items.Add("Follow-Up");
            cmbType.Items.Add("Strategy Meetng");
            cmbType.Items.Add("Planning Session");
            cmbType.Items.Add("Implementation");
            cmbType.Items.Add("Support");

            _customers = customersList;

            var appointments = Program.Appointments;
            dgvAppointments.DataSource = appointments;

            cmbCustomers.DataSource = _customers;
            cmbCustomers.DisplayMember = "Name";
            cmbCustomers.ValueMember = "ID";
        }

        //Add Click
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedAppointment != null)
                {
                    MessageBox.Show("You are currently editing an appointment. Please click Update before adding a new appointment");
                    return;
                }

                var (customerId, customerName, type, date, start, end) = GetAppointmentInfo();

                if (cmbType.SelectedIndex == -1)
                {
                    MessageBox.Show("Type field cannot be blank");
                }

                if (StartEndTimeCheck(start, end))
                {
                    MessageBox.Show("Start time must be before end time");
                    return;
                }

                if (IsNotWithinBusinessHours(date, start, end))
                {
                    MessageBox.Show("Appointments must be Monday - Friday between 9:00 AM and 5:00 PM EST");
                    return;
                }

                if (AppointmentOverlap(customerId, start, end))
                {
                    MessageBox.Show("Overlapping appointments are not allowed");
                    return;
                }

                Appointment newAppointment = new Appointment(0, customerId, customerName, type, start, end);
                Database.AddAppointment(newAppointment);

                RefreshAppointmentsFromDb();

                ClearAppointmentInfo();

                MessageBox.Show("Appointment added successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding appointment." + ex.Message);
            }
        }

        private void dgvAppointments_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvAppointments.Rows)
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;
            }

            if (dgvAppointments.CurrentRow != null && dgvAppointments.CurrentRow.DataBoundItem != null)
            {
                selectedAppointment = (Appointment)dgvAppointments.CurrentRow.DataBoundItem;

                cmbCustomers.SelectedValue = selectedAppointment.CustomerId;
                cmbType.Text = selectedAppointment.Type;
                dtpDate.Value = selectedAppointment.Start.Date;
                dtpStart.Value = selectedAppointment.Start;
                dtpEnd.Value = selectedAppointment.End;

                dgvAppointments.CurrentRow.DefaultCellStyle.BackColor = Color.LightBlue;
                dgvAppointments.CurrentRow.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            }
            else
            {
                selectedAppointment = null;
            }
        }

        //Update Appointment
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {

                if (selectedAppointment == null)
                {
                    MessageBox.Show("Please select an appointment to update");
                    return;
                }

                var (customerId, customerName, type, date, start, end) = GetAppointmentInfo();

                if (cmbType.SelectedIndex == -1)
                {
                    MessageBox.Show("Type field cannot be blank");
                }

                if (StartEndTimeCheck(start, end))
                {
                    MessageBox.Show("Start time must be before end time");
                    return;
                }

                if (IsNotWithinBusinessHours(date, start, end))
                {
                    MessageBox.Show("Appointments must be Monday - Friday between 9:00 AM and 5:00 PM EST");
                    return;
                }

                if (AppointmentOverlap(customerId, start, end))
                {
                    MessageBox.Show("Overlapping appointments are not allowed");
                    return;
                }

                selectedAppointment.CustomerId = customerId;
                selectedAppointment.CustomerName = customerName;
                selectedAppointment.Type = type;
                selectedAppointment.Start = start;
                selectedAppointment.End = end;

                Database.UpdateAppointment(selectedAppointment);

                RefreshAppointmentsFromDb();
                ClearAppointmentInfo();

                MessageBox.Show("Appointment updated succesfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating appointment." + ex.Message);
            }
        }

        //Delete appointment
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedAppointment == null)
                {
                    MessageBox.Show("Please select an appointment to delete");
                    return;
                }

                var confirm = MessageBox.Show("Are you sure you want to delete this appointment?", "Confirm Delete", MessageBoxButtons.YesNo);

                if (confirm == DialogResult.No) 
                { 
                    return; 
                }

                Database.DeleteAppointment(selectedAppointment.Id);
                RefreshAppointmentsFromDb();

                MessageBox.Show("Appointment succesfully deleted");
                ClearAppointmentInfo();
                selectedAppointment = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting appointment" + ex.Message);
            }
        }

        private (int customerId, string customerName, string type, DateTime date, DateTime start, DateTime end) GetAppointmentInfo()
        {
            var customerId = (int)cmbCustomers.SelectedValue;
            var customerName = cmbCustomers.Text;
            var type = cmbType.Text.Trim();
            var date = dtpDate.Value.Date;
            var start = dtpDate.Value.Date + dtpStart.Value.TimeOfDay;
            var end = dtpDate.Value.Date + dtpEnd.Value.TimeOfDay;

            return (customerId, customerName, type, date, start, end);
        }

        private bool IsNotWithinBusinessHours(DateTime date, DateTime start, DateTime end)
        {
            if ((start.Hour < 9 || end.Hour >= 17) || (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
            {
                return true;
            }
            return false;
        }

        private bool AppointmentOverlap(int customerId, DateTime start, DateTime end)
        {
            start = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0);
            end = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, 0);
            foreach (Appointment existingappointment in Program.Appointments)
            {   
                if (selectedAppointment != null && existingappointment.Id == selectedAppointment.Id)
                    continue;
                
                if (existingappointment.CustomerId == customerId)
                {
                    if ((start < existingappointment.End) && (end > existingappointment.Start))
                    {
                        if ((start == existingappointment.End) && (end == existingappointment.Start))
                            return true;

                        if ((start <= existingappointment.End) && (end >= existingappointment.Start))
                            return true;
                    }
                }
            }
            return false;
        }

        private void ClearAppointmentInfo()
        {
            cmbCustomers.SelectedIndex = -1;
            cmbType.SelectedIndex = -1;
            dtpDate.Value = DateTime.Today;
            dtpStart.Value = DateTime.Today.AddHours(9);
            dtpEnd.Value = DateTime.Today.AddHours(10);
        }

        private bool StartEndTimeCheck(DateTime start, DateTime end)
        {
            if (start > end)
            {
                return true;
            }
            return false;
        }

        private void btnCalendar_Click(object sender, EventArgs e)
        {
            new CalendarForm(Program.Appointments).ShowDialog();
        }

        private void RefreshAppointmentsFromDb()
        {
            var fresh = Database.LoadAppointments();
            Program.Appointments.Clear();
            foreach (var app in fresh)
            {
                Program.Appointments.Add(app);
            }

            dgvAppointments.Refresh();
        }
    }
}
