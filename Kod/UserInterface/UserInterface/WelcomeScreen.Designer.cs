namespace UserInterface
{
    partial class WelcomeScreen
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
            this.appTitle = new System.Windows.Forms.Label();
            this.engineeringThesis = new System.Windows.Forms.Label();
            this.authors = new System.Windows.Forms.Label();
            this.department = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // appTitle
            // 
            this.appTitle.AutoSize = true;
            this.appTitle.Font = new System.Drawing.Font("Segoe UI Light", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.appTitle.ForeColor = System.Drawing.Color.White;
            this.appTitle.Location = new System.Drawing.Point(110, 88);
            this.appTitle.Name = "appTitle";
            this.appTitle.Size = new System.Drawing.Size(343, 47);
            this.appTitle.TabIndex = 0;
            this.appTitle.Text = "Pozycjometr inercyjny";
            // 
            // engineeringThesis
            // 
            this.engineeringThesis.AutoSize = true;
            this.engineeringThesis.Font = new System.Drawing.Font("Segoe UI Light", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.engineeringThesis.ForeColor = System.Drawing.Color.White;
            this.engineeringThesis.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.engineeringThesis.Location = new System.Drawing.Point(192, 151);
            this.engineeringThesis.Name = "engineeringThesis";
            this.engineeringThesis.Size = new System.Drawing.Size(172, 30);
            this.engineeringThesis.TabIndex = 1;
            this.engineeringThesis.Text = "Praca inżynierska";
            this.engineeringThesis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // authors
            // 
            this.authors.AutoSize = true;
            this.authors.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.authors.ForeColor = System.Drawing.Color.White;
            this.authors.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.authors.Location = new System.Drawing.Point(12, 253);
            this.authors.Name = "authors";
            this.authors.Size = new System.Drawing.Size(271, 21);
            this.authors.TabIndex = 2;
            this.authors.Text = "Wykonali: Łukasz Cisowski, Paweł Mazik";
            // 
            // department
            // 
            this.department.AutoSize = true;
            this.department.Font = new System.Drawing.Font("Segoe UI Light", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.department.ForeColor = System.Drawing.Color.White;
            this.department.Location = new System.Drawing.Point(113, 192);
            this.department.Name = "department";
            this.department.Size = new System.Drawing.Size(321, 25);
            this.department.TabIndex = 3;
            this.department.Text = "Wydział Fizyki i Informatyki Stosowanej";
            // 
            // WelcomeScreen
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GrayText;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(553, 283);
            this.Controls.Add(this.department);
            this.Controls.Add(this.authors);
            this.Controls.Add(this.engineeringThesis);
            this.Controls.Add(this.appTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "WelcomeScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Shown += new System.EventHandler(this.WelcomeScreen_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label appTitle;
        private System.Windows.Forms.Label engineeringThesis;
        private System.Windows.Forms.Label authors;
        private System.Windows.Forms.Label department;
    }
}

