namespace Pomelo_NativeSocket {
    partial class Main {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.btn_send = new System.Windows.Forms.Button();
            this.btn_connect = new System.Windows.Forms.Button();
            this.tb_info = new System.Windows.Forms.TextBox();
            this.tb_name = new System.Windows.Forms.TextBox();
            this.tb_channel = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cb_users = new System.Windows.Forms.ComboBox();
            this.tb_send = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_send
            // 
            this.btn_send.Enabled = false;
            this.btn_send.Location = new System.Drawing.Point(17, 196);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(140, 28);
            this.btn_send.TabIndex = 6;
            this.btn_send.Text = "Send";
            this.btn_send.UseVisualStyleBackColor = true;
            this.btn_send.Click += new System.EventHandler(this.btn_send_Click);
            // 
            // btn_connect
            // 
            this.btn_connect.Location = new System.Drawing.Point(17, 95);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(140, 23);
            this.btn_connect.TabIndex = 3;
            this.btn_connect.Text = "Connect";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // tb_info
            // 
            this.tb_info.BackColor = System.Drawing.SystemColors.InfoText;
            this.tb_info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_info.Font = new System.Drawing.Font("SimSun", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_info.ForeColor = System.Drawing.Color.SpringGreen;
            this.tb_info.Location = new System.Drawing.Point(3, 3);
            this.tb_info.Multiline = true;
            this.tb_info.Name = "tb_info";
            this.tb_info.ReadOnly = true;
            this.tb_info.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_info.Size = new System.Drawing.Size(429, 366);
            this.tb_info.TabIndex = 0;
            this.tb_info.TabStop = false;
            this.tb_info.WordWrap = false;
            // 
            // tb_name
            // 
            this.tb_name.Location = new System.Drawing.Point(62, 23);
            this.tb_name.Name = "tb_name";
            this.tb_name.Size = new System.Drawing.Size(97, 21);
            this.tb_name.TabIndex = 1;
            // 
            // tb_channel
            // 
            this.tb_channel.Location = new System.Drawing.Point(62, 59);
            this.tb_channel.Name = "tb_channel";
            this.tb_channel.Size = new System.Drawing.Size(97, 21);
            this.tb_channel.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 177F));
            this.tableLayoutPanel1.Controls.Add(this.tb_info, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(612, 372);
            this.tableLayoutPanel1.TabIndex = 100001;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(438, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(171, 366);
            this.panel1.TabIndex = 100001;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cb_users);
            this.groupBox2.Controls.Add(this.btn_send);
            this.groupBox2.Controls.Add(this.tb_send);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 136);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(171, 230);
            this.groupBox2.TabIndex = 100004;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Chat";
            // 
            // cb_users
            // 
            this.cb_users.FormattingEnabled = true;
            this.cb_users.Location = new System.Drawing.Point(17, 20);
            this.cb_users.Name = "cb_users";
            this.cb_users.Size = new System.Drawing.Size(145, 20);
            this.cb_users.TabIndex = 100003;
            // 
            // tb_send
            // 
            this.tb_send.ImeMode = System.Windows.Forms.ImeMode.On;
            this.tb_send.Location = new System.Drawing.Point(17, 55);
            this.tb_send.Multiline = true;
            this.tb_send.Name = "tb_send";
            this.tb_send.Size = new System.Drawing.Size(145, 135);
            this.tb_send.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tb_channel);
            this.groupBox1.Controls.Add(this.btn_connect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tb_name);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(171, 124);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "GateServer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Channel:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 372);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Main";
            this.Text = "Pomelo-Chat-Socket";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_send;
        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.TextBox tb_info;
        private System.Windows.Forms.TextBox tb_name;
        private System.Windows.Forms.TextBox tb_channel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tb_send;
        private System.Windows.Forms.ComboBox cb_users;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

