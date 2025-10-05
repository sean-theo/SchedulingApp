using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchedulingApp
{
    public partial class CustomerForm : Form
    {
        private BindingList<Customer> customers;

        private int nextId = 1;

        private Customer selectedCustomer;

        public CustomerForm()
        {
            InitializeComponent();
            customers = Program.Customers;
            dgvCustomers.DataSource = customers;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var (name, address, phone) = GetCustomerInput();

                if (!ValidateCustomerInput(name, address, phone))
                {
                    return;
                }

                Customer newCustomer = new Customer(0, name, address, phone);
                Database.AddCustomer(newCustomer);
               
                Program.Customers = Database.LoadCustomers();
                customers = Program.Customers;
                dgvCustomers.DataSource = customers;

                ClearInputs();
                MessageBox.Show("Customer added successfully.");
                foreach (var cust in Program.Customers)
                {
                    customers.Add(cust);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding customer " + ex.Message);
            }
        }

        private void dgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow != null)
            {
                selectedCustomer = (Customer)dgvCustomers.CurrentRow.DataBoundItem;

                txtName.Text = selectedCustomer.Name;
                txtAddress.Text = selectedCustomer.Address;
                txtPhone.Text = selectedCustomer.Phone;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedCustomer == null)
                {
                    MessageBox.Show("Please select a customer to update.");
                    return;
                }

                var (name, address, phone) = GetCustomerInput();

                if (!ValidateCustomerInput(name, address, phone))
                {
                    return;
                }

                selectedCustomer.Name = name;
                selectedCustomer.Address = address;
                selectedCustomer.Phone = phone;

                Database.UpdateCustomer(selectedCustomer);
                Program.Customers = Database.LoadCustomers();
                customers = Program.Customers;
                dgvCustomers.DataSource = customers;

                ClearInputs();

                MessageBox.Show("Customer updated succesfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating customer " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedCustomer == null)
                {
                    MessageBox.Show("Please select a customer to delete.");
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this customer?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                {
                    return;
                }

                Database.DeleteCustomer(selectedCustomer.ID);
                Program.Customers = Database.LoadCustomers();
                customers = Program.Customers;
                dgvCustomers.DataSource = customers;
                ClearInputs();

                MessageBox.Show("Customer deleted succesfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting customer " + ex.Message);
            }
        }

        //Validate form fields
        private bool ValidateCustomerInput(string name, string address, string phone)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show(this, "All fields are required.");
                return false;
            }

            foreach (char c in phone)
            {
                if (!char.IsDigit(c) && c != '-')
                {
                    MessageBox.Show("Phone number must only contains digits and dashes");
                    return false;
                }
            }
            return true;
        }

        // Get the information from fields
        private (string name, string address, string phone) GetCustomerInput()
        {
            var name = txtName.Text.Trim();
            var address = txtAddress.Text.Trim();
            var phone = txtPhone.Text.Trim();

            return (name, address, phone);
        }

        private void ClearInputs()
        {
            txtName.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            txtName.Focus();
        }

        // Clicking X button closes
        private void CustomerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void btnAppointments_Click(object sender, EventArgs e)
        {
            AppointmentForm apptForm = new AppointmentForm(Program.Customers);
            apptForm.Show();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            try
            {
                Program.Appointments = Database.LoadAppointments();

                ReportsForm reportsForm = new ReportsForm();
                reportsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening reports " + ex.Message);
            }
        }
    }
}
