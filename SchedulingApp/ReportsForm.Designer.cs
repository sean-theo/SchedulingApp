namespace SchedulingApp
{
    partial class ReportsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvReports = new System.Windows.Forms.DataGridView();
            this.btnAppointmentsType = new System.Windows.Forms.Button();
            this.btnUser = new System.Windows.Forms.Button();
            this.btnCustomer = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvReports
            // 
            this.dgvReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReports.Location = new System.Drawing.Point(12, 196);
            this.dgvReports.Name = "dgvReports";
            this.dgvReports.Size = new System.Drawing.Size(776, 242);
            this.dgvReports.TabIndex = 0;
            // 
            // btnAppointmentsType
            // 
            this.btnAppointmentsType.Location = new System.Drawing.Point(40, 88);
            this.btnAppointmentsType.Name = "btnAppointmentsType";
            this.btnAppointmentsType.Size = new System.Drawing.Size(223, 23);
            this.btnAppointmentsType.TabIndex = 1;
            this.btnAppointmentsType.Text = "Appointments by Type/Month";
            this.btnAppointmentsType.UseVisualStyleBackColor = true;
            this.btnAppointmentsType.Click += new System.EventHandler(this.btnAppointmentsType_Click);
            // 
            // btnUser
            // 
            this.btnUser.Location = new System.Drawing.Point(314, 88);
            this.btnUser.Name = "btnUser";
            this.btnUser.Size = new System.Drawing.Size(184, 23);
            this.btnUser.TabIndex = 2;
            this.btnUser.Text = "Schedule by User";
            this.btnUser.UseVisualStyleBackColor = true;
            this.btnUser.Click += new System.EventHandler(this.btnUser_Click);
            // 
            // btnCustomer
            // 
            this.btnCustomer.Location = new System.Drawing.Point(574, 88);
            this.btnCustomer.Name = "btnCustomer";
            this.btnCustomer.Size = new System.Drawing.Size(165, 23);
            this.btnCustomer.TabIndex = 3;
            this.btnCustomer.Text = "Appointments by Customer";
            this.btnCustomer.UseVisualStyleBackColor = true;
            this.btnCustomer.Click += new System.EventHandler(this.btnCustomer_Click);
            // 
            // ReportsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnCustomer);
            this.Controls.Add(this.btnUser);
            this.Controls.Add(this.btnAppointmentsType);
            this.Controls.Add(this.dgvReports);
            this.Name = "ReportsForm";
            this.Text = "Reports";
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvReports;
        private System.Windows.Forms.Button btnAppointmentsType;
        private System.Windows.Forms.Button btnUser;
        private System.Windows.Forms.Button btnCustomer;
    }
}