namespace VisualStudio.Delphi.ConverterWizard
{
  partial class WizardDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardDialog));
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.checkBox2 = new System.Windows.Forms.CheckBox();
      this.FSolutionTextBox = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.FFolderBrowseBtn = new System.Windows.Forms.Button();
      this.FProjectBrowseBtn = new System.Windows.Forms.Button();
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.FFolderComboBox = new System.Windows.Forms.ComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.FProjectTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.FCancelBtn = new System.Windows.Forms.Button();
      this.FFinishBtn = new System.Windows.Forms.Button();
      this.FOpenDialog = new System.Windows.Forms.OpenFileDialog();
      this.FFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
      this.panel2 = new System.Windows.Forms.Panel();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.FListBox = new System.Windows.Forms.ListBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.panel3 = new System.Windows.Forms.Panel();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // pictureBox1
      // 
      this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.InitialImage = null;
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(567, 373);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // panel1
      // 
      this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel1.Controls.Add(this.checkBox2);
      this.panel1.Controls.Add(this.FSolutionTextBox);
      this.panel1.Controls.Add(this.label5);
      this.panel1.Controls.Add(this.FFolderBrowseBtn);
      this.panel1.Controls.Add(this.FProjectBrowseBtn);
      this.panel1.Controls.Add(this.checkBox1);
      this.panel1.Controls.Add(this.FFolderComboBox);
      this.panel1.Controls.Add(this.label4);
      this.panel1.Controls.Add(this.FProjectTextBox);
      this.panel1.Controls.Add(this.label3);
      this.panel1.Location = new System.Drawing.Point(0, 234);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(566, 145);
      this.panel1.TabIndex = 1;
      // 
      // checkBox2
      // 
      this.checkBox2.AutoSize = true;
      this.checkBox2.Location = new System.Drawing.Point(296, 111);
      this.checkBox2.Name = "checkBox2";
      this.checkBox2.Size = new System.Drawing.Size(154, 17);
      this.checkBox2.TabIndex = 13;
      this.checkBox2.Text = "Create &directory for solution";
      this.checkBox2.UseVisualStyleBackColor = true;
      // 
      // FSolutionTextBox
      // 
      this.FSolutionTextBox.Location = new System.Drawing.Point(11, 108);
      this.FSolutionTextBox.Name = "FSolutionTextBox";
      this.FSolutionTextBox.Size = new System.Drawing.Size(259, 20);
      this.FSolutionTextBox.TabIndex = 12;
      this.FSolutionTextBox.TextChanged += new System.EventHandler(this.FSolutionTextBox_TextChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(8, 92);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(76, 13);
      this.label5.TabIndex = 11;
      this.label5.Text = "Solution Name";
      // 
      // FFolderBrowseBtn
      // 
      this.FFolderBrowseBtn.Location = new System.Drawing.Point(483, 68);
      this.FFolderBrowseBtn.Name = "FFolderBrowseBtn";
      this.FFolderBrowseBtn.Size = new System.Drawing.Size(75, 23);
      this.FFolderBrowseBtn.TabIndex = 10;
      this.FFolderBrowseBtn.Text = "Browse...";
      this.FFolderBrowseBtn.UseVisualStyleBackColor = true;
      this.FFolderBrowseBtn.Click += new System.EventHandler(this.FFolderBrowseBtn_Click);
      // 
      // FProjectBrowseBtn
      // 
      this.FProjectBrowseBtn.Location = new System.Drawing.Point(483, 25);
      this.FProjectBrowseBtn.Name = "FProjectBrowseBtn";
      this.FProjectBrowseBtn.Size = new System.Drawing.Size(75, 23);
      this.FProjectBrowseBtn.TabIndex = 6;
      this.FProjectBrowseBtn.Text = "&Browse...";
      this.FProjectBrowseBtn.UseVisualStyleBackColor = true;
      this.FProjectBrowseBtn.Click += new System.EventHandler(this.FProjectBrowseBtn_Click);
      // 
      // checkBox1
      // 
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new System.Drawing.Point(370, 48);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new System.Drawing.Size(107, 17);
      this.checkBox1.TabIndex = 7;
      this.checkBox1.Text = "&Use Same Folder";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
      // 
      // FFolderComboBox
      // 
      this.FFolderComboBox.FormattingEnabled = true;
      this.FFolderComboBox.Location = new System.Drawing.Point(11, 68);
      this.FFolderComboBox.Name = "FFolderComboBox";
      this.FFolderComboBox.Size = new System.Drawing.Size(466, 21);
      this.FFolderComboBox.TabIndex = 9;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(8, 52);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(176, 13);
      this.label4.TabIndex = 8;
      this.label4.Text = "New Visual Studio Project  &Location";
      // 
      // FProjectTextBox
      // 
      this.FProjectTextBox.Location = new System.Drawing.Point(11, 25);
      this.FProjectTextBox.Name = "FProjectTextBox";
      this.FProjectTextBox.Size = new System.Drawing.Size(466, 20);
      this.FProjectTextBox.TabIndex = 5;
      this.FProjectTextBox.WordWrap = false;
      this.FProjectTextBox.Leave += new System.EventHandler(this.FProjectTextBox_Leave);
      this.FProjectTextBox.TextChanged += new System.EventHandler(this.FProjectTextBox_TextChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(8, 9);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(199, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "CodeGear Delphi &Project File To Convert";
      // 
      // FCancelBtn
      // 
      this.FCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.FCancelBtn.Location = new System.Drawing.Point(397, 385);
      this.FCancelBtn.Name = "FCancelBtn";
      this.FCancelBtn.Size = new System.Drawing.Size(75, 23);
      this.FCancelBtn.TabIndex = 2;
      this.FCancelBtn.Text = "Cancel";
      this.FCancelBtn.UseVisualStyleBackColor = true;
      // 
      // FFinishBtn
      // 
      this.FFinishBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.FFinishBtn.Location = new System.Drawing.Point(478, 385);
      this.FFinishBtn.Name = "FFinishBtn";
      this.FFinishBtn.Size = new System.Drawing.Size(75, 23);
      this.FFinishBtn.TabIndex = 3;
      this.FFinishBtn.Text = "Finish";
      this.FFinishBtn.UseVisualStyleBackColor = true;
      // 
      // FOpenDialog
      // 
      this.FOpenDialog.FileName = "openFileDialog1";
      this.FOpenDialog.Filter = "Delphi Project (*.dpr)|*.dpr|Packages|*.dpk|All Files|*.*";
      this.FOpenDialog.Title = "Open Delphi Project To Convert";
      // 
      // FFolderBrowserDialog
      // 
      this.FFolderBrowserDialog.Description = "New Project Location";
      // 
      // panel2
      // 
      this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel2.Controls.Add(this.textBox2);
      this.panel2.Controls.Add(this.FListBox);
      this.panel2.Controls.Add(this.label2);
      this.panel2.Controls.Add(this.label1);
      this.panel2.Location = new System.Drawing.Point(151, 80);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(415, 155);
      this.panel2.TabIndex = 4;
      // 
      // textBox2
      // 
      this.textBox2.BackColor = System.Drawing.SystemColors.Control;
      this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.textBox2.CausesValidation = false;
      this.textBox2.Cursor = System.Windows.Forms.Cursors.Default;
      this.textBox2.Location = new System.Drawing.Point(278, 62);
      this.textBox2.Multiline = true;
      this.textBox2.Name = "textBox2";
      this.textBox2.ReadOnly = true;
      this.textBox2.ShortcutsEnabled = false;
      this.textBox2.Size = new System.Drawing.Size(116, 73);
      this.textBox2.TabIndex = 11;
      this.textBox2.TabStop = false;
      this.textBox2.Text = "CodeGear Delphi has many project types and Delphi For Visual Studio does not supp" +
          "ort all of them at this time.";
      // 
      // FListBox
      // 
      this.FListBox.FormattingEnabled = true;
      this.FListBox.Location = new System.Drawing.Point(24, 62);
      this.FListBox.Name = "FListBox";
      this.FListBox.Size = new System.Drawing.Size(248, 69);
      this.FListBox.TabIndex = 10;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(21, 46);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(124, 13);
      this.label2.TabIndex = 9;
      this.label2.Text = "Supported Project Types";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(6, 11);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(401, 20);
      this.label1.TabIndex = 8;
      this.label1.Text = "Welcome to CodeGear Delphi Project Conversion";
      // 
      // panel3
      // 
      this.panel3.Location = new System.Drawing.Point(152, 222);
      this.panel3.Name = "panel3";
      this.panel3.Size = new System.Drawing.Size(413, 19);
      this.panel3.TabIndex = 12;
      // 
      // WizardDialog
      // 
      this.AcceptButton = this.FFinishBtn;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.FCancelBtn;
      this.ClientSize = new System.Drawing.Size(566, 414);
      this.Controls.Add(this.panel3);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.FFinishBtn);
      this.Controls.Add(this.FCancelBtn);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.pictureBox1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.KeyPreview = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "WizardDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "CodeGear Delphi Project Conversion";
      this.TopMost = true;
      this.Shown += new System.EventHandler(this.WizardDialog_Shown);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WizardDialog_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.TextBox FProjectTextBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button FFolderBrowseBtn;
    private System.Windows.Forms.Button FProjectBrowseBtn;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.ComboBox FFolderComboBox;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox FSolutionTextBox;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Button FCancelBtn;
    private System.Windows.Forms.Button FFinishBtn;
    private System.Windows.Forms.OpenFileDialog FOpenDialog;
    private System.Windows.Forms.FolderBrowserDialog FFolderBrowserDialog;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.ListBox FListBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.CheckBox checkBox2;
  }
}