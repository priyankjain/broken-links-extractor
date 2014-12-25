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
            this.OutputBox = new System.Windows.Forms.RichTextBox();
            this.Speed = new System.Windows.Forms.ComboBox();
            this.UploadButton = new System.Windows.Forms.Button();
            this.StartButton = new System.Windows.Forms.Button();
            this.Output = new System.Windows.Forms.GroupBox();
            this.DepthSelector = new System.Windows.Forms.ComboBox();
            this.Actions = new System.Windows.Forms.GroupBox();
            this.Options = new System.Windows.Forms.GroupBox();
            this.Output.SuspendLayout();
            this.Actions.SuspendLayout();
            this.Options.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Title = "Upload Links File";
            // 
            // OutputBox
            // 
            this.OutputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputBox.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.OutputBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.OutputBox.Location = new System.Drawing.Point(6, 16);
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.Size = new System.Drawing.Size(903, 161);
            this.OutputBox.TabIndex = 2;
            this.OutputBox.Text = "";
            this.OutputBox.TextChanged += new System.EventHandler(this.OutputBox_TextChanged);
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
            this.Output.Controls.Add(this.OutputBox);
            this.Output.Location = new System.Drawing.Point(12, 76);
            this.Output.Name = "Output";
            this.Output.Size = new System.Drawing.Size(915, 183);
            this.Output.TabIndex = 5;
            this.Output.TabStop = false;
            this.Output.Text = "Output";
            this.Output.Enter += new System.EventHandler(this.groupBox1_Enter);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 271);
            this.Controls.Add(this.Options);
            this.Controls.Add(this.Output);
            this.Controls.Add(this.Actions);
            this.Name = "Form1";
            this.Text = "Broken Links Extractor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.Output.ResumeLayout(false);
            this.Actions.ResumeLayout(false);
            this.Options.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.RichTextBox OutputBox;
        private System.Windows.Forms.ComboBox Speed;
        private System.Windows.Forms.Button UploadButton;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.GroupBox Output;
        private System.Windows.Forms.ComboBox DepthSelector;
        private System.Windows.Forms.GroupBox Actions;
        private System.Windows.Forms.GroupBox Options;
    }
}

