namespace UserInterface
{
    partial class ApplicationWindow
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
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.accelerationX = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.infoButton = new System.Windows.Forms.Button();
            this.connectionSettings = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.angleZ = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.angleY = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.angleX = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.accelerationZ = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.accelerationY = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label8.Location = new System.Drawing.Point(4, 654);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(117, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Obrót względem osi";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label7.Location = new System.Drawing.Point(4, 602);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 15);
            this.label7.TabIndex = 9;
            this.label7.Text = "Przyśpieszenie";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.accelerationX);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(151, 44);
            this.panel2.TabIndex = 0;
            // 
            // accelerationX
            // 
            this.accelerationX.Location = new System.Drawing.Point(51, 11);
            this.accelerationX.Name = "accelerationX";
            this.accelerationX.Size = new System.Drawing.Size(100, 20);
            this.accelerationX.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Oś X";
            // 
            // infoButton
            // 
            this.infoButton.Location = new System.Drawing.Point(599, 647);
            this.infoButton.Name = "infoButton";
            this.infoButton.Size = new System.Drawing.Size(75, 40);
            this.infoButton.TabIndex = 11;
            this.infoButton.Text = "O programie";
            this.infoButton.UseVisualStyleBackColor = true;
            this.infoButton.Click += new System.EventHandler(this.infoButton_Click);
            // 
            // connectionSettings
            // 
            this.connectionSettings.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.connectionSettings.Location = new System.Drawing.Point(599, 595);
            this.connectionSettings.Name = "connectionSettings";
            this.connectionSettings.Size = new System.Drawing.Size(75, 36);
            this.connectionSettings.TabIndex = 8;
            this.connectionSettings.Text = "Ustawienia";
            this.connectionSettings.UseVisualStyleBackColor = true;
            this.connectionSettings.Click += new System.EventHandler(this.connectionSettings_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.panel7, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel6, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(120, 587);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(473, 100);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.angleZ);
            this.panel7.Controls.Add(this.label6);
            this.panel7.Location = new System.Drawing.Point(317, 53);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(151, 44);
            this.panel7.TabIndex = 5;
            // 
            // angleZ
            // 
            this.angleZ.Location = new System.Drawing.Point(51, 11);
            this.angleZ.Name = "angleZ";
            this.angleZ.Size = new System.Drawing.Size(100, 20);
            this.angleZ.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Oś Z";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.angleY);
            this.panel6.Controls.Add(this.label5);
            this.panel6.Location = new System.Drawing.Point(160, 53);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(151, 44);
            this.panel6.TabIndex = 4;
            // 
            // angleY
            // 
            this.angleY.Location = new System.Drawing.Point(51, 11);
            this.angleY.Name = "angleY";
            this.angleY.Size = new System.Drawing.Size(100, 20);
            this.angleY.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Oz Y";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.angleX);
            this.panel5.Controls.Add(this.label4);
            this.panel5.Location = new System.Drawing.Point(3, 53);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(151, 44);
            this.panel5.TabIndex = 3;
            // 
            // angleX
            // 
            this.angleX.Location = new System.Drawing.Point(51, 11);
            this.angleX.Name = "angleX";
            this.angleX.Size = new System.Drawing.Size(100, 20);
            this.angleX.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Oś X";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.accelerationZ);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Location = new System.Drawing.Point(317, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(151, 44);
            this.panel4.TabIndex = 2;
            // 
            // accelerationZ
            // 
            this.accelerationZ.Location = new System.Drawing.Point(51, 11);
            this.accelerationZ.Name = "accelerationZ";
            this.accelerationZ.Size = new System.Drawing.Size(100, 20);
            this.accelerationZ.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Oś Z";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.accelerationY);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Location = new System.Drawing.Point(160, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(151, 44);
            this.panel3.TabIndex = 1;
            // 
            // accelerationY
            // 
            this.accelerationY.Location = new System.Drawing.Point(51, 11);
            this.accelerationY.Name = "accelerationY";
            this.accelerationY.Size = new System.Drawing.Size(100, 20);
            this.accelerationY.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Oś Y";
            // 
            // mainPanel
            // 
            this.mainPanel.ImeMode = System.Windows.Forms.ImeMode.On;
            this.mainPanel.Location = new System.Drawing.Point(12, 12);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(662, 517);
            this.mainPanel.TabIndex = 12;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(199, 541);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(115, 35);
            this.startButton.TabIndex = 13;
            this.startButton.Text = "Rozpocznij pomiar";
            this.startButton.UseCompatibleTextRendering = true;
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(356, 541);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(115, 35);
            this.stopButton.TabIndex = 14;
            this.stopButton.Text = "Zakończ pomiar";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // ApplicationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 710);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.infoButton);
            this.Controls.Add(this.connectionSettings);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ApplicationWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pozycjometr inercyjny";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ApplicationWindow_FormClosed);
            this.Load += new System.EventHandler(this.ApplicationWindow_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox accelerationX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button infoButton;
        private System.Windows.Forms.Button connectionSettings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.TextBox angleZ;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox angleY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox angleX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox accelerationZ;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox accelerationY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;

    }
}