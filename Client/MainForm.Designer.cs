namespace Rpi
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.connectionLabel = new System.Windows.Forms.Label();
            this.reqButton = new System.Windows.Forms.Button();
            this.autoUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.dataButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.logTextBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.connectionLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.reqButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.dataButton, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(332, 336);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // logTextBox
            // 
            this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTextBox.Location = new System.Drawing.Point(3, 3);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(326, 254);
            this.logTextBox.TabIndex = 0;
            // 
            // connectionLabel
            // 
            this.connectionLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionLabel.ForeColor = System.Drawing.Color.Red;
            this.connectionLabel.Location = new System.Drawing.Point(3, 260);
            this.connectionLabel.Name = "connectionLabel";
            this.connectionLabel.Size = new System.Drawing.Size(326, 18);
            this.connectionLabel.TabIndex = 1;
            this.connectionLabel.Text = "Нет соединения";
            this.connectionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // reqButton
            // 
            this.reqButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.reqButton.Location = new System.Drawing.Point(3, 281);
            this.reqButton.Name = "reqButton";
            this.reqButton.Size = new System.Drawing.Size(326, 23);
            this.reqButton.TabIndex = 2;
            this.reqButton.Text = "Запросить идентификаторы";
            this.reqButton.UseVisualStyleBackColor = true;
            this.reqButton.Click += new System.EventHandler(this.reqButton_Click);
            // 
            // autoUpdateTimer
            // 
            this.autoUpdateTimer.Enabled = true;
            this.autoUpdateTimer.Interval = 1000;
            this.autoUpdateTimer.Tick += new System.EventHandler(this.autoUpdateTimer_Tick);
            // 
            // dataButton
            // 
            this.dataButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataButton.Location = new System.Drawing.Point(3, 310);
            this.dataButton.Name = "dataButton";
            this.dataButton.Size = new System.Drawing.Size(326, 23);
            this.dataButton.TabIndex = 3;
            this.dataButton.Text = "Запросить информацию";
            this.dataButton.UseVisualStyleBackColor = true;
            this.dataButton.Click += new System.EventHandler(this.dataButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 336);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Клиент";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.Label connectionLabel;
        private System.Windows.Forms.Timer autoUpdateTimer;
        private System.Windows.Forms.Button reqButton;
        private System.Windows.Forms.Button dataButton;
    }
}

