using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchedulingApp
{
    public partial class CalendarForm : Form
    {
        private BindingList<Appointment> _appointments;

        public CalendarForm(BindingList<Appointment> appointments)
        {
            InitializeComponent();
            _appointments = appointments;
            dgvAppointments.DataSource = new BindingList<Appointment>();
        }
        private void CalendarForm_Load(object sender, EventArgs e)
        {
            lblSelectedDate.Text = "Select a date to view appointments for that date.";
        }

        private void calMonth_DateChanged(object sender, DateRangeEventArgs e)
        {
            try
            {
                var selectedDate = calMonth.SelectionRange.Start.Date;

                lblSelectedDate.Text = "Appointments for " + selectedDate.ToShortDateString();

                var dailyAppointments = new BindingList<Appointment>();

                foreach (Appointment appointment in Program.Appointments)
                {
                    DateTime localStart = appointment.Start;
                    DateTime localEnd = appointment.End;

                    if (localStart.Date == selectedDate.Date)
                    {
                        dailyAppointments.Add(new Appointment
                        {
                            Id = appointment.Id,
                            CustomerId = appointment.CustomerId,
                            CustomerName = appointment.CustomerName,
                            Type = appointment.Type,
                            Start = localStart,
                            End = localEnd,
                        });
                    }
                }

                dgvAppointments.DataSource = dailyAppointments;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading calender " + ex.Message);
            }
        }
    }
}