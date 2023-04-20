
namespace MoveHardware
{
    partial class Main
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SecureConnection_S1 = new System.Windows.Forms.CheckBox();
            this.WindowsAuth_S1 = new System.Windows.Forms.RadioButton();
            this.WindowsUser_S1 = new System.Windows.Forms.RadioButton();
            this.toolStripStatusLabel_S1 = new System.Windows.Forms.Label();
            this.Button_Connect_S1 = new System.Windows.Forms.Button();
            this.textBox_Password_S1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_Domain_S1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_User_S1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Address_S1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.treeView_S1 = new System.Windows.Forms.TreeView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SecureConnection_S2 = new System.Windows.Forms.CheckBox();
            this.toolStripStatusLabel_S2 = new System.Windows.Forms.Label();
            this.Button_Connect_S2 = new System.Windows.Forms.Button();
            this.WindowsAuth_S2 = new System.Windows.Forms.RadioButton();
            this.textBox_Password_S2 = new System.Windows.Forms.TextBox();
            this.WindowsUser_S2 = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_Domain_S2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_User_S2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_Address_S2 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.treeView_S2 = new System.Windows.Forms.TreeView();
            this.textBox_Console = new System.Windows.Forms.RichTextBox();
            this.button_SaveLogs = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_MoveGroups = new System.Windows.Forms.Button();
            this.button_MoveBasicUsers = new System.Windows.Forms.Button();
            this.button_MoveRoles = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown_MaxDegreeOfParallelism = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MaxDegreeOfParallelism)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SecureConnection_S1);
            this.groupBox1.Controls.Add(this.WindowsAuth_S1);
            this.groupBox1.Controls.Add(this.WindowsUser_S1);
            this.groupBox1.Controls.Add(this.toolStripStatusLabel_S1);
            this.groupBox1.Controls.Add(this.Button_Connect_S1);
            this.groupBox1.Controls.Add(this.textBox_Password_S1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_Domain_S1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox_User_S1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_Address_S1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(183, 238);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source ";
            // 
            // SecureConnection_S1
            // 
            this.SecureConnection_S1.AutoSize = true;
            this.SecureConnection_S1.Checked = true;
            this.SecureConnection_S1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SecureConnection_S1.Location = new System.Drawing.Point(46, 183);
            this.SecureConnection_S1.Name = "SecureConnection_S1";
            this.SecureConnection_S1.Size = new System.Drawing.Size(84, 17);
            this.SecureConnection_S1.TabIndex = 10;
            this.SecureConnection_S1.Text = "Secure Only";
            this.SecureConnection_S1.UseVisualStyleBackColor = true;
            // 
            // WindowsAuth_S1
            // 
            this.WindowsAuth_S1.AutoSize = true;
            this.WindowsAuth_S1.Checked = true;
            this.WindowsAuth_S1.Location = new System.Drawing.Point(46, 143);
            this.WindowsAuth_S1.Name = "WindowsAuth_S1";
            this.WindowsAuth_S1.Size = new System.Drawing.Size(94, 17);
            this.WindowsAuth_S1.TabIndex = 8;
            this.WindowsAuth_S1.TabStop = true;
            this.WindowsAuth_S1.Tag = "WU";
            this.WindowsAuth_S1.Text = "Windows User";
            this.WindowsAuth_S1.UseVisualStyleBackColor = true;
            // 
            // WindowsUser_S1
            // 
            this.WindowsUser_S1.AutoSize = true;
            this.WindowsUser_S1.Location = new System.Drawing.Point(46, 123);
            this.WindowsUser_S1.Name = "WindowsUser_S1";
            this.WindowsUser_S1.Size = new System.Drawing.Size(84, 17);
            this.WindowsUser_S1.TabIndex = 7;
            this.WindowsUser_S1.Tag = "CU";
            this.WindowsUser_S1.Text = "Current User";
            this.WindowsUser_S1.UseVisualStyleBackColor = true;
            this.WindowsUser_S1.CheckedChanged += new System.EventHandler(this.S1_CheckedChanged);
            // 
            // toolStripStatusLabel_S1
            // 
            this.toolStripStatusLabel_S1.AutoSize = true;
            this.toolStripStatusLabel_S1.Location = new System.Drawing.Point(6, 211);
            this.toolStripStatusLabel_S1.Name = "toolStripStatusLabel_S1";
            this.toolStripStatusLabel_S1.Size = new System.Drawing.Size(35, 13);
            this.toolStripStatusLabel_S1.TabIndex = 2;
            this.toolStripStatusLabel_S1.Text = "label5";
            // 
            // Button_Connect_S1
            // 
            this.Button_Connect_S1.ForeColor = System.Drawing.Color.Black;
            this.Button_Connect_S1.Location = new System.Drawing.Point(99, 206);
            this.Button_Connect_S1.Name = "Button_Connect_S1";
            this.Button_Connect_S1.Size = new System.Drawing.Size(75, 23);
            this.Button_Connect_S1.TabIndex = 6;
            this.Button_Connect_S1.Text = "Connect";
            this.Button_Connect_S1.UseVisualStyleBackColor = true;
            this.Button_Connect_S1.Click += new System.EventHandler(this.Button_Connect_S1_Click);
            // 
            // textBox_Password_S1
            // 
            this.textBox_Password_S1.Location = new System.Drawing.Point(66, 97);
            this.textBox_Password_S1.Name = "textBox_Password_S1";
            this.textBox_Password_S1.PasswordChar = '*';
            this.textBox_Password_S1.Size = new System.Drawing.Size(108, 20);
            this.textBox_Password_S1.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Domain";
            // 
            // textBox_Domain_S1
            // 
            this.textBox_Domain_S1.Location = new System.Drawing.Point(66, 45);
            this.textBox_Domain_S1.Name = "textBox_Domain_S1";
            this.textBox_Domain_S1.Size = new System.Drawing.Size(108, 20);
            this.textBox_Domain_S1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Password";
            // 
            // textBox_User_S1
            // 
            this.textBox_User_S1.Location = new System.Drawing.Point(66, 71);
            this.textBox_User_S1.Name = "textBox_User_S1";
            this.textBox_User_S1.Size = new System.Drawing.Size(108, 20);
            this.textBox_User_S1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "User";
            // 
            // textBox_Address_S1
            // 
            this.textBox_Address_S1.Location = new System.Drawing.Point(66, 19);
            this.textBox_Address_S1.Name = "textBox_Address_S1";
            this.textBox_Address_S1.Size = new System.Drawing.Size(108, 20);
            this.textBox_Address_S1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // treeView_S1
            // 
            this.treeView_S1.BackColor = System.Drawing.SystemColors.Control;
            this.treeView_S1.CheckBoxes = true;
            this.treeView_S1.Location = new System.Drawing.Point(207, 12);
            this.treeView_S1.Name = "treeView_S1";
            this.treeView_S1.Size = new System.Drawing.Size(336, 519);
            this.treeView_S1.TabIndex = 1;
            this.treeView_S1.TabStop = false;
            this.treeView_S1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_S1_AfterCheck);
            this.treeView_S1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_S1_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.SecureConnection_S2);
            this.groupBox2.Controls.Add(this.toolStripStatusLabel_S2);
            this.groupBox2.Controls.Add(this.Button_Connect_S2);
            this.groupBox2.Controls.Add(this.WindowsAuth_S2);
            this.groupBox2.Controls.Add(this.textBox_Password_S2);
            this.groupBox2.Controls.Add(this.WindowsUser_S2);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBox_Domain_S2);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBox_User_S2);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.textBox_Address_S2);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(555, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(183, 238);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Destination";
            // 
            // SecureConnection_S2
            // 
            this.SecureConnection_S2.AutoSize = true;
            this.SecureConnection_S2.Checked = true;
            this.SecureConnection_S2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SecureConnection_S2.Location = new System.Drawing.Point(51, 183);
            this.SecureConnection_S2.Name = "SecureConnection_S2";
            this.SecureConnection_S2.Size = new System.Drawing.Size(84, 17);
            this.SecureConnection_S2.TabIndex = 10;
            this.SecureConnection_S2.Text = "Secure Only";
            this.SecureConnection_S2.UseVisualStyleBackColor = true;
            // 
            // toolStripStatusLabel_S2
            // 
            this.toolStripStatusLabel_S2.AutoSize = true;
            this.toolStripStatusLabel_S2.Location = new System.Drawing.Point(19, 211);
            this.toolStripStatusLabel_S2.Name = "toolStripStatusLabel_S2";
            this.toolStripStatusLabel_S2.Size = new System.Drawing.Size(35, 13);
            this.toolStripStatusLabel_S2.TabIndex = 2;
            this.toolStripStatusLabel_S2.Text = "label5";
            // 
            // Button_Connect_S2
            // 
            this.Button_Connect_S2.ForeColor = System.Drawing.Color.Black;
            this.Button_Connect_S2.Location = new System.Drawing.Point(99, 206);
            this.Button_Connect_S2.Name = "Button_Connect_S2";
            this.Button_Connect_S2.Size = new System.Drawing.Size(75, 23);
            this.Button_Connect_S2.TabIndex = 11;
            this.Button_Connect_S2.Text = "Connect";
            this.Button_Connect_S2.UseVisualStyleBackColor = true;
            this.Button_Connect_S2.Click += new System.EventHandler(this.Button_Connect_S2_Click);
            // 
            // WindowsAuth_S2
            // 
            this.WindowsAuth_S2.AutoSize = true;
            this.WindowsAuth_S2.Location = new System.Drawing.Point(51, 143);
            this.WindowsAuth_S2.Name = "WindowsAuth_S2";
            this.WindowsAuth_S2.Size = new System.Drawing.Size(94, 17);
            this.WindowsAuth_S2.TabIndex = 8;
            this.WindowsAuth_S2.Tag = "WU";
            this.WindowsAuth_S2.Text = "Windows User";
            this.WindowsAuth_S2.UseVisualStyleBackColor = true;
            // 
            // textBox_Password_S2
            // 
            this.textBox_Password_S2.Location = new System.Drawing.Point(66, 97);
            this.textBox_Password_S2.Name = "textBox_Password_S2";
            this.textBox_Password_S2.PasswordChar = '*';
            this.textBox_Password_S2.Size = new System.Drawing.Size(108, 20);
            this.textBox_Password_S2.TabIndex = 10;
            // 
            // WindowsUser_S2
            // 
            this.WindowsUser_S2.AutoSize = true;
            this.WindowsUser_S2.Checked = true;
            this.WindowsUser_S2.Location = new System.Drawing.Point(51, 123);
            this.WindowsUser_S2.Name = "WindowsUser_S2";
            this.WindowsUser_S2.Size = new System.Drawing.Size(84, 17);
            this.WindowsUser_S2.TabIndex = 7;
            this.WindowsUser_S2.TabStop = true;
            this.WindowsUser_S2.Tag = "CU";
            this.WindowsUser_S2.Text = "Current User";
            this.WindowsUser_S2.UseVisualStyleBackColor = true;
            this.WindowsUser_S2.CheckedChanged += new System.EventHandler(this.S2_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Domain";
            // 
            // textBox_Domain_S2
            // 
            this.textBox_Domain_S2.Location = new System.Drawing.Point(66, 45);
            this.textBox_Domain_S2.Name = "textBox_Domain_S2";
            this.textBox_Domain_S2.Size = new System.Drawing.Size(108, 20);
            this.textBox_Domain_S2.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 100);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Password";
            // 
            // textBox_User_S2
            // 
            this.textBox_User_S2.Location = new System.Drawing.Point(66, 71);
            this.textBox_User_S2.Name = "textBox_User_S2";
            this.textBox_User_S2.Size = new System.Drawing.Size(108, 20);
            this.textBox_User_S2.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(31, 74);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "User";
            // 
            // textBox_Address_S2
            // 
            this.textBox_Address_S2.Location = new System.Drawing.Point(66, 19);
            this.textBox_Address_S2.Name = "textBox_Address_S2";
            this.textBox_Address_S2.Size = new System.Drawing.Size(108, 20);
            this.textBox_Address_S2.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(22, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Server";
            // 
            // treeView_S2
            // 
            this.treeView_S2.BackColor = System.Drawing.SystemColors.Control;
            this.treeView_S2.Location = new System.Drawing.Point(750, 12);
            this.treeView_S2.Name = "treeView_S2";
            this.treeView_S2.Size = new System.Drawing.Size(336, 519);
            this.treeView_S2.TabIndex = 1;
            this.treeView_S2.TabStop = false;
            this.treeView_S2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_S2_KeyDown);
            // 
            // textBox_Console
            // 
            this.textBox_Console.BackColor = System.Drawing.SystemColors.InfoText;
            this.textBox_Console.Location = new System.Drawing.Point(12, 537);
            this.textBox_Console.Name = "textBox_Console";
            this.textBox_Console.Size = new System.Drawing.Size(1313, 243);
            this.textBox_Console.TabIndex = 10;
            this.textBox_Console.TabStop = false;
            this.textBox_Console.Text = "";
            // 
            // button_SaveLogs
            // 
            this.button_SaveLogs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_SaveLogs.BackColor = System.Drawing.SystemColors.Desktop;
            this.button_SaveLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SaveLogs.ForeColor = System.Drawing.Color.DimGray;
            this.button_SaveLogs.Location = new System.Drawing.Point(1242, 749);
            this.button_SaveLogs.Margin = new System.Windows.Forms.Padding(0);
            this.button_SaveLogs.Name = "button_SaveLogs";
            this.button_SaveLogs.Size = new System.Drawing.Size(75, 23);
            this.button_SaveLogs.TabIndex = 17;
            this.button_SaveLogs.Text = "SaveLog";
            this.button_SaveLogs.UseVisualStyleBackColor = false;
            this.button_SaveLogs.Click += new System.EventHandler(this.button_SaveLogs_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_MoveGroups);
            this.groupBox3.Controls.Add(this.button_MoveBasicUsers);
            this.groupBox3.Controls.Add(this.button_MoveRoles);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.numericUpDown_MaxDegreeOfParallelism);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(1107, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 519);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Commands";
            // 
            // button_MoveGroups
            // 
            this.button_MoveGroups.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_MoveGroups.Location = new System.Drawing.Point(27, 109);
            this.button_MoveGroups.Name = "button_MoveGroups";
            this.button_MoveGroups.Size = new System.Drawing.Size(147, 75);
            this.button_MoveGroups.TabIndex = 19;
            this.button_MoveGroups.Text = "Move Groups";
            this.button_MoveGroups.UseVisualStyleBackColor = true;
            this.button_MoveGroups.Click += new System.EventHandler(this.button_MoveGroups_Click);
            // 
            // button_MoveBasicUsers
            // 
            this.button_MoveBasicUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_MoveBasicUsers.Location = new System.Drawing.Point(27, 190);
            this.button_MoveBasicUsers.Name = "button_MoveBasicUsers";
            this.button_MoveBasicUsers.Size = new System.Drawing.Size(147, 75);
            this.button_MoveBasicUsers.TabIndex = 20;
            this.button_MoveBasicUsers.Text = "Move Basic Users";
            this.button_MoveBasicUsers.UseVisualStyleBackColor = true;
            this.button_MoveBasicUsers.Click += new System.EventHandler(this.button_MoveBasicUsers_Click);
            // 
            // button_MoveRoles
            // 
            this.button_MoveRoles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_MoveRoles.Location = new System.Drawing.Point(27, 271);
            this.button_MoveRoles.Name = "button_MoveRoles";
            this.button_MoveRoles.Size = new System.Drawing.Size(147, 75);
            this.button_MoveRoles.TabIndex = 21;
            this.button_MoveRoles.Text = "Move Roles";
            this.button_MoveRoles.UseVisualStyleBackColor = true;
            this.button_MoveRoles.Click += new System.EventHandler(this.Button_MoveRoles_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 415);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Degree Of Parallelism";
            // 
            // numericUpDown_MaxDegreeOfParallelism
            // 
            this.numericUpDown_MaxDegreeOfParallelism.Location = new System.Drawing.Point(66, 431);
            this.numericUpDown_MaxDegreeOfParallelism.Name = "numericUpDown_MaxDegreeOfParallelism";
            this.numericUpDown_MaxDegreeOfParallelism.Size = new System.Drawing.Size(81, 20);
            this.numericUpDown_MaxDegreeOfParallelism.TabIndex = 22;
            this.numericUpDown_MaxDegreeOfParallelism.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(27, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(147, 75);
            this.button1.TabIndex = 18;
            this.button1.Text = "Move Hardware";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button_Move_Selection_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1333, 787);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button_SaveLogs);
            this.Controls.Add(this.textBox_Console);
            this.Controls.Add(this.treeView_S2);
            this.Controls.Add(this.treeView_S1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Main";
            this.Text = "MigrationToolV2";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MaxDegreeOfParallelism)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Button_Connect_S1;
        private System.Windows.Forms.TextBox textBox_Password_S1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_Domain_S1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_User_S1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Address_S1;
        private System.Windows.Forms.TreeView treeView_S1;
        private System.Windows.Forms.Label toolStripStatusLabel_S1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label toolStripStatusLabel_S2;
        private System.Windows.Forms.Button Button_Connect_S2;
        private System.Windows.Forms.TextBox textBox_Password_S2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_Domain_S2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_User_S2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_Address_S2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TreeView treeView_S2;
        private System.Windows.Forms.RichTextBox textBox_Console;
        private System.Windows.Forms.Button button_SaveLogs;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_MoveGroups;
        private System.Windows.Forms.Button button_MoveBasicUsers;
        private System.Windows.Forms.Button button_MoveRoles;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown_MaxDegreeOfParallelism;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox SecureConnection_S1;
        private System.Windows.Forms.RadioButton WindowsAuth_S1;
        private System.Windows.Forms.RadioButton WindowsUser_S1;
        private System.Windows.Forms.CheckBox SecureConnection_S2;
        private System.Windows.Forms.RadioButton WindowsAuth_S2;
        private System.Windows.Forms.RadioButton WindowsUser_S2;
    }
}

