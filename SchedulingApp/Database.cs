using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.ComponentModel;
using System.Security.Cryptography;

namespace SchedulingApp
{
    public static class Database
    {
        private static MySqlConnection conn;

        private static string server = "localhost";
        private static string database = "testdb";
        private static string usernmae = "test";
        private static string password = "test";

        private static string connectionString =
            $"Server={server};Database={database};Uid={usernmae};Pwd={password};SslMode=none;AllowPublicKeyRetrieval=true";

        public static void OpenConnection()
        {
            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Error opening connection" + ex.Message);
            }
        }

        public static void CloseConnection()
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }

        public static bool TestConnection()
        {
            try
            {
                OpenConnection();
                CloseConnection();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static MySqlConnection GetConnection()
        {
            if (conn == null || conn.State != System.Data.ConnectionState.Open)
            {
                OpenConnection();
            }
            return conn;
        }

        //Load data from SQL database
        public static BindingList<Customer> LoadCustomers()
        {
           BindingList<Customer> customers = new BindingList<Customer>();

            string query = @"
                SELECT
                    c.customerId,
                    c.customerName,
                    a.address,
                    a.phone
                FROM customer AS c
                INNER JOIN address AS a ON c.addressId = a.addressId;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32("customerId");
                        string name = reader.GetString("customerName");
                        string address = reader.GetString("address");
                        string phone = reader.GetString("phone");

                        customers.Add(new Customer(id, name, address, phone));
                    }

                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error loading customers: " + ex.Message);
            }
            return customers;
        }

        //Add customer
        public static void AddCustomer(Customer c)
        {
            string insertCustomer = @"
                INSERT INTO customer( customerName, addressId, active, createDate, createdBy, lastUpdate, lastUpdateBy)
                VALUES (@name, @addressId, 1, NOW(), 'test', NOW(), 'test')
                ";

            string insertAddress = @"
                INSERT INTO address( address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdate, lastUpdateBy)
                VALUES (@address, @address2, 1, '00000', @phone, NOW(), 'test', NOW(), 'test')
                ";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                int addressId;

                using (MySqlCommand cmdAddr = new MySqlCommand(insertAddress, conn))
                {
                    cmdAddr.Parameters.AddWithValue("@address", c.Address);
                    cmdAddr.Parameters.AddWithValue("@address2", "");
                    cmdAddr.Parameters.AddWithValue("@phone", c.Phone);
                    cmdAddr.ExecuteNonQuery();
                    addressId = (int)cmdAddr.LastInsertedId;
                }

                using (MySqlCommand cmdCust = new MySqlCommand(insertCustomer, conn))
                {
                    cmdCust.Parameters.AddWithValue("@name", c.Name);
                    cmdCust.Parameters.AddWithValue("@addressId", addressId);
                    cmdCust.ExecuteNonQuery();
                }
            }
        }

        //Update Customer
        public static void UpdateCustomer(Customer c)
        {
            string query = @"
                UPDATE customer c
                JOIN address a ON c.addressID = a.addressId
                SET c.customerName = @name,
                    a.address = @address,
                    a.phone = @phone,
                    c.lastUpdateBy = 'test',
                    c.lastUpdateBy = 'test'
                WHERE c.customerId = @id;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", c.Name);
                    cmd.Parameters.AddWithValue("@address", c.Address);
                    cmd.Parameters.AddWithValue("@phone", c.Phone);
                    cmd.Parameters.AddWithValue("@id", c.ID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Delete Customer
        public static void DeleteCustomer(int customerId)
        {
            string query = @"
                DELETE FROM customer WHERE customerId = @id;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", customerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Load Appointments
        public static BindingList<Appointment> LoadAppointments()
        {
            BindingList<Appointment> appointments = new BindingList<Appointment>();

            string query = @"
                SELECT
                    a.appointmentId,
                    a.customerId,
                    c.customerName,
                    a.type,
                    a.start,
                    a.end
                FROM appointment AS a
                INNER JOIN customer AS c ON a.customerId = c.customerId;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32("appointmentId");
                        int customerId = reader.GetInt32("customerId");
                        string customerName = reader.GetString("customerName");
                        string type = reader.GetString("type");

                        DateTime startUtc = reader.GetDateTime("start");
                        DateTime endUtc = reader.GetDateTime("end");

                        startUtc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
                        endUtc = DateTime.SpecifyKind(endUtc, DateTimeKind.Utc);

                        DateTime localStart = TimeHelper.ConvertFromUTCtoLocal(startUtc);
                        DateTime localEnd = TimeHelper.ConvertFromUTCtoLocal(endUtc);

                        appointments.Add(new Appointment(id, customerId, customerName, type, localStart, localEnd));
                    }

                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error loading appointments: " + ex.Message);
            }

            return appointments;
        }

        //Add Appointment
        public static void AddAppointment(Appointment a)
        {
            string query = @"
                INSERT INTO appointment
                    (customerId, userId, title, description, location, contact, type, url, start, end, 
                    createDate, createdBy, lastUpdate, lastUpdateBy)
                VALUES
                    (@customerId, 1, @title, @description, @location, @contact, @type, @url, @startUtc, @endUtc,
                    NOW(), 'test', NOW(), 'test');";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmdAddr = new MySqlCommand(query, conn))
                {
                    cmdAddr.Parameters.AddWithValue("@customerId", a.CustomerId);
                    cmdAddr.Parameters.AddWithValue("@title", "Appointment");
                    cmdAddr.Parameters.AddWithValue("@description", "N/A");
                    cmdAddr.Parameters.AddWithValue("@location", "Office");
                    cmdAddr.Parameters.AddWithValue("@contact", "N/A");
                    cmdAddr.Parameters.AddWithValue("@url", "N/A");
                    cmdAddr.Parameters.AddWithValue("@type", a.Type);

                    DateTime startUtc = TimeHelper.ConvertFromLocalToUTC(a.Start);
                    DateTime endUtc = TimeHelper.ConvertFromLocalToUTC(a.End);

                    cmdAddr.Parameters.AddWithValue("@startUtc", startUtc);
                    cmdAddr.Parameters.AddWithValue("@endUtc", endUtc);

                    cmdAddr.ExecuteNonQuery();
                }
            }
        }

        //Update Appointment
        public static void UpdateAppointment(Appointment a)
        {
            string query = @"
                UPDATE appointment
                SET customerId = @customerId,
                    type = @type,
                    start = @startUtc,
                    end = @endUtc
                WHERE appointmentId = @id;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", a.Id);
                    cmd.Parameters.AddWithValue("@customerId", a.CustomerId);
                    cmd.Parameters.AddWithValue("@type", a.Type);

                    DateTime startUtc = TimeHelper.ConvertFromLocalToUTC(a.Start);
                    DateTime endUtc = TimeHelper.ConvertFromLocalToUTC(a.End);

                    cmd.Parameters.AddWithValue("@startUtc", startUtc);
                    cmd.Parameters.AddWithValue("@endUtc", endUtc);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Delete Appointment
        public static void DeleteAppointment(int appointmentId)
        {
            string query = "DELETE FROM appointment WHERE appointmentId = @id;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", appointmentId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
