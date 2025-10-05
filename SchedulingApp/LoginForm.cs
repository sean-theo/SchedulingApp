using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Drawing.Text;
using System.Globalization;

namespace SchedulingApp
{
    public partial class LoginForm : Form
    {
        private bool isSpanish = false;

        public LoginForm()
        {
            InitializeComponent();
            ShowLocation();
           
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var user = txtUser.Text.Trim();
            var password = txtPass.Text.Trim();

            if (user == "test" && password == "test")
            {
                lblStatus.Text = Translate("LoginSuccess");
                LogLogin(user, true);

                CheckForUpcomingAppointments();

                CustomerForm newForm = new CustomerForm();
                newForm.Show();
                this.Hide();
            }
            else
            {
                lblStatus.Text = Translate("InvalidCredentials");
                LogLogin(user, false);

                txtUser.Clear();
                txtPass.Clear();
                txtUser.Focus();
            }
        }

        public void LogLogin(string username, bool value)
        {
            var path = Application.StartupPath + "\\Login_History.txt";
            var time = DateTime.UtcNow.ToString("O");
            var log = time + "\t" + username + "\t" + (value ? "SUCCESS" : "FAIL");
            File.AppendAllText(path, log + Environment.NewLine);
        }

        private void ShowLocation()
        {
            var region = new RegionInfo(CultureInfo.CurrentCulture.LCID);
            var timeZone = TimeZoneInfo.Local;

            isSpanish = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "es";

            var message = $"Detected {region.DisplayName}, {timeZone.DisplayName}";
            lblLocation.Text = message;
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void CheckForUpcomingAppointments()
        {
            var appointments = Program.Appointments;

            if (appointments == null || appointments.Count == 0)
            {
                return;
            }

            DateTime nowUtc = DateTime.UtcNow;
            DateTime cutoff = nowUtc.AddMinutes(15);
            bool found = false;

            foreach (var appointment in appointments)
            {
                if (appointment.Start >= nowUtc && appointment.Start < cutoff)
                {
                    DateTime localStart = TimeHelper.ConvertFromUTCtoLocal(appointment.Start);

                    string msg = Translate("UpcomingAppt") + "\n\n" +
                        $"{localStart:g} - {appointment.Type} for {appointment.CustomerName}";  

                    MessageBox.Show(msg, Translate("UpcomingApptitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    found = true;
                }
            }

            if (!found)
            {
                MessageBox.Show(Translate("NoUpcoming"),
                    Translate("UpcomingApptTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string Translate(string key)
        {
            if (!isSpanish)
            {
                switch (key)
                {
                    case "LoginSuccess": return "Login successful";
                    case "InvalidCredentials": return "The username and password do not match.";
                    case "UpcomingAppt": return "You have an upcoming appointment within 15 minutes:";
                    case "NoUpcoming": return "No appointments within the next 15 minutes.";
                    case "UpcomingApptTitle": return "Appointment Reminder";
                    default: return key;
                }
            }
            else
            {
                // Spanish translations
                switch (key)
                {
                    case "LoginSuccess": return "Inicio de sesión exitoso.";
                    case "InvalidCredentials": return "El nombre de usuario y la contraseña no coinciden.";
                    case "UpcomingAppt": return "Tiene una cita programada dentro de los próximos 15 minutos:";
                    case "NoUpcoming": return "No tiene citas en los próximos 15 minutos.";
                    case "UpcomingApptTitle": return "Recordatorio de Cita";
                    default: return key;
                }
            }
        }
    }
}
