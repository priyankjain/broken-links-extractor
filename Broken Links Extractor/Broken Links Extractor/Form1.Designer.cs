namespace Broken_Links_Extractor
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Speed = new System.Windows.Forms.ComboBox();
            this.UploadButton = new System.Windows.Forms.Button();
            this.StartButton = new System.Windows.Forms.Button();
            this.Output = new System.Windows.Forms.GroupBox();
            this.OutputTable = new System.Windows.Forms.DataGridView();
            this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResponseCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DepthSelector = new System.Windows.Forms.ComboBox();
            this.Actions = new System.Windows.Forms.GroupBox();
            this.Options = new System.Windows.Forms.GroupBox();
            this.PercentageBox = new System.Windows.Forms.TextBox();
            this.Output.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OutputTable)).BeginInit();
            this.Actions.SuspendLayout();
            this.Options.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Title = "Upload Links File";
            // 
            // Speed
            // 
            this.Speed.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Speed.Dock = System.Windows.Forms.DockStyle.Right;
            this.Speed.FormattingEnabled = true;
            this.Speed.Location = new System.Drawing.Point(137, 16);
            this.Speed.Name = "Speed";
            this.Speed.Size = new System.Drawing.Size(87, 21);
            this.Speed.TabIndex = 4;
            this.Speed.Text = "Speed";
            this.Speed.SelectedIndexChanged += new System.EventHandler(this.Speed_SelectedIndexChanged);
            // 
            // UploadButton
            // 
            this.UploadButton.BackColor = System.Drawing.Color.Gainsboro;
            this.UploadButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UploadButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.UploadButton.Location = new System.Drawing.Point(3, 16);
            this.UploadButton.Name = "UploadButton";
            this.UploadButton.Size = new System.Drawing.Size(77, 29);
            this.UploadButton.TabIndex = 3;
            this.UploadButton.Text = "Upload";
            this.UploadButton.UseVisualStyleBackColor = false;
            this.UploadButton.Click += new System.EventHandler(this.UploadButton_Click);
            // 
            // StartButton
            // 
            this.StartButton.BackColor = System.Drawing.Color.Gainsboro;
            this.StartButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.StartButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.StartButton.Location = new System.Drawing.Point(145, 16);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(87, 29);
            this.StartButton.TabIndex = 1;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = false;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // Output
            // 
            this.Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Output.Controls.Add(this.OutputTable);
            this.Output.Location = new System.Drawing.Point(12, 83);
            this.Output.Name = "Output";
            this.Output.Size = new System.Drawing.Size(915, 176);
            this.Output.TabIndex = 5;
            this.Output.TabStop = false;
            this.Output.Text = "Output";
            this.Output.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // OutputTable
            // 
            this.OutputTable.AllowUserToAddRows = false;
            this.OutputTable.AllowUserToDeleteRows = false;
            this.OutputTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.OutputTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.OutputTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Address,
            this.ResponseCode,
            this.Description});
            this.OutputTable.Location = new System.Drawing.Point(6, 19);
            this.OutputTable.Name = "OutputTable";
            this.OutputTable.ReadOnly = true;
            this.OutputTable.RowHeadersVisible = false;
            this.OutputTable.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.OutputTable.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.OutputTable.Size = new System.Drawing.Size(903, 151);
            this.OutputTable.TabIndex = 4;
            this.OutputTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OutputTable_CellContentClick);
            // 
            // Address
            // 
            this.Address.HeaderText = "Address";
            this.Address.Name = "Address";
            this.Address.ReadOnly = true;
            // 
            // ResponseCode
            // 
            this.ResponseCode.HeaderText = "Response Code";
            this.ResponseCode.Name = "ResponseCode";
            this.ResponseCode.ReadOnly = true;
            // 
            // Description
            // 
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            // 
            // DepthSelector
            // 
            this.DepthSelector.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DepthSelector.Dock = System.Windows.Forms.DockStyle.Left;
            this.DepthSelector.FormattingEnabled = true;
            this.DepthSelector.Location = new System.Drawing.Point(3, 16);
            this.DepthSelector.Name = "DepthSelector";
            this.DepthSelector.Size = new System.Drawing.Size(77, 21);
            this.DepthSelector.TabIndex = 6;
            this.DepthSelector.Text = "Depth";
            this.DepthSelector.SelectedIndexChanged += new System.EventHandler(this.Depth_SelectedIndexChanged);
            // 
            // Actions
            // 
            this.Actions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Actions.Controls.Add(this.UploadButton);
            this.Actions.Controls.Add(this.StartButton);
            this.Actions.Location = new System.Drawing.Point(692, 3);
            this.Actions.Name = "Actions";
            this.Actions.Size = new System.Drawing.Size(235, 48);
            this.Actions.TabIndex = 7;
            this.Actions.TabStop = false;
            this.Actions.Text = "Actions";
            this.Actions.Enter += new System.EventHandler(this.groupBox1_Enter_1);
            // 
            // Options
            // 
            this.Options.Controls.Add(this.Speed);
            this.Options.Controls.Add(this.DepthSelector);
            this.Options.Location = new System.Drawing.Point(12, 3);
            this.Options.Name = "Options";
            this.Options.Size = new System.Drawing.Size(227, 48);
            this.Options.TabIndex = 8;
            this.Options.TabStop = false;
            this.Options.Text = "Options";
            // 
            // PercentageBox
            // 
            this.PercentageBox.Location = new System.Drawing.Point(12, 57);
            this.PercentageBox.Name = "PercentageBox";
            this.PercentageBox.ReadOnly = true;
            this.PercentageBox.Size = new System.Drawing.Size(100, 20);
            this.PercentageBox.TabIndex = 9;
            this.PercentageBox.Text = "Completed: 0%";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 271);
            this.Controls.Add(this.PercentageBox);
            this.Controls.Add(this.Options);
            this.Controls.Add(this.Output);
            this.Controls.Add(this.Actions);
            this.Name = "Form1";
            this.Text = "Broken Links Extractor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form1_Closed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Output.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OutputTable)).EndInit();
            this.Actions.ResumeLayout(false);
            this.Options.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ComboBox Speed;
        private System.Windows.Forms.Button UploadButton;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.GroupBox Output;
        private System.Windows.Forms.ComboBox DepthSelector;
        private System.Windows.Forms.GroupBox Actions;
        private System.Windows.Forms.GroupBox Options;
        private System.Windows.Forms.DataGridView OutputTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResponseCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.TextBox PercentageBox;
    }
}

