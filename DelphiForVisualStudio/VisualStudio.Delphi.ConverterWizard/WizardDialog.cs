using System;
using System.IO;
using System.Windows.Forms;

namespace VisualStudio.Delphi.ConverterWizard
{
  public partial class WizardDialog : Form
  {
    private string FDelphiProjectFilters = null;
    private string[] FOutputPaths;
    private bool FUseSameFolder;
    private bool FSolutionModified;

    private void FSolutionTextBox_TextChanged(object sender, EventArgs e)
    {
      FSolutionModified = true;
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      UseSameFolder = checkBox1.Checked;
      EnableControls();
      UpdateLocationPath();
    }

    private void FProjectTextBox_TextChanged(object sender, EventArgs e)
    {
      UpdateSolutionName();
      UpdateLocationPath();
    }

    private void WizardDialog_Shown(object sender, EventArgs e)
    {
      FListBox.Items.Clear();
      if (DelphiProjectFilters != null)
      {
        string[] lList = DelphiProjectFilters.Split('|');
        for (int i = 0; i < lList.Length; i++)
          if (i + 1 % 2 == 1)
            FListBox.Items.Add(lList[i]);
        FFolderComboBox.Items.Clear();
      }
      if (OutputPaths != null)
      {
        foreach (string lItem in OutputPaths)
          FFolderComboBox.Items.Add(lItem);
      }
      // disable solution controls 
      label5.Enabled = SolutionEnabled;
      if (!SolutionEnabled)
        CreateSolutionDir = false;
      checkBox2.Enabled = FSolutionTextBox.Enabled;
      FSolutionModified = SolutionName != "";
      
      EnableControls();
      UpdateLocationPath();
      UpdateSolutionName();
    }

    private void UpdateSolutionName()
    {
      if (FSolutionModified || !this.SolutionEnabled) return;
      char[] lInvalid = Path.GetInvalidFileNameChars();
      try
      {
        string lName = Path.GetFileNameWithoutExtension(DelphiProjectFile);
        if (DelphiProjectFile != "" && lName.IndexOfAny(lInvalid) == -1)
        {
          SolutionName = lName;
          FSolutionModified = false;
        }
      }
      catch { }
    }

    private void WizardDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (this.DialogResult == DialogResult.OK)
      {
        e.Cancel = true;
        if (this.OutputPath == "" && !UseSameFolder)
          this.OutputPath = Directory.GetCurrentDirectory();

        if (!File.Exists(this.DelphiProjectFile))
          MessageBox.Show(Resources.MSG_CodeGearProjectFileNameRequired, Resources.ERROR_DialogCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        else if (this.OutputPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
          MessageBox.Show(Resources.MSG_InvalidLocationPath, Resources.ERROR_DialogCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        else if (this.SolutionEnabled && (this.SolutionName == "" || this.SolutionName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1))
          MessageBox.Show(Resources.MSG_InvalidSolutionName, Resources.ERROR_DialogCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        else
          e.Cancel = false;
      }
    }

    private void FFolderBrowseBtn_Click(object sender, EventArgs e)
    {
      if (Directory.Exists(OutputPath))
        FFolderBrowserDialog.SelectedPath = OutputPath;
      if (FFolderBrowserDialog.ShowDialog() == DialogResult.OK)
        OutputPath = FFolderBrowserDialog.SelectedPath;
    }

    private void FProjectTextBox_Leave(object sender, EventArgs e)
    {
      UpdateLocationPath();
    }

    private void UpdateLocationPath()
    {
      if (!UseSameFolder) return;
      try
      {
        char[] lInvalid = Path.GetInvalidPathChars();
        if (DelphiProjectFile != "" && this.DelphiProjectFile.IndexOfAny(lInvalid) == -1)
        {
          string lPath = Path.GetDirectoryName(DelphiProjectFile);
          if (Directory.Exists(lPath))
            OutputPath = lPath;
        }
      }
      catch { }
    }

    private void FProjectBrowseBtn_Click(object sender, EventArgs e)
    {
      FOpenDialog.FileName = DelphiProjectFile;
      FOpenDialog.Filter = FDelphiProjectFilters;
      if (FOpenDialog.ShowDialog() == DialogResult.OK)
        DelphiProjectFile = FOpenDialog.FileName;

    }

    private void EnableControls()
    {
        checkBox1.Checked = FUseSameFolder;
        label4.Enabled = !FUseSameFolder;
        FFolderComboBox.Enabled = !FUseSameFolder;
        FFolderBrowseBtn.Enabled = FFolderComboBox.Enabled;
        if (FUseSameFolder)
          CreateSolutionDir = false;
        checkBox2.Enabled = FFolderComboBox.Enabled && SolutionEnabled;
        UpdateLocationPath();
      }

    // **** CTOR
    public WizardDialog()
    {
      InitializeComponent();
      UseSameFolder = checkBox1.Checked;
    }

    /// <summary>
    /// Determines if the new Visual Studio project will be created in the same folder as the Delphi project.
    /// </summary>
    public bool UseSameFolder
    {
      get { return FUseSameFolder; }
      set 
      { 
        FUseSameFolder = value;
      }
    }

    /// <summary>
    /// Determines if the solution name is required.
    /// </summary>
    public bool SolutionEnabled
    {
      get { return FSolutionTextBox.Enabled; }
      set { FSolutionTextBox.Enabled = value; }
    }

    /// <summary>
    /// Name of solution to create.
    /// </summary>
    public string SolutionName
    {
      get { return FSolutionTextBox.Text; }
      set { FSolutionTextBox.Text = value; }
    }

    /// <summary>
    /// Should a solution folder should be created
    /// </summary>
    public bool CreateSolutionDir
    {
      get { return checkBox2.Checked; }
      set { checkBox2.Checked = value; }
    }

    /// <summary>
    /// List of paths to use in the drop down combo box of locations.
    /// </summary>
    public string[] OutputPaths
    {
      get { return FOutputPaths; }
      set 
      {
        FOutputPaths = value;
      }
    }

    /// <summary>
    /// Contains the path to create the new Visual Studio project.
    /// </summary>
    public string OutputPath
    {
      get { return FFolderComboBox.Text; }
      set { FFolderComboBox.Text = value; }
    }


    /// <summary>
    /// Name of Delphi project file that will be converted.
    /// </summary>
    public string DelphiProjectFile
    {
      get { return FProjectTextBox.Text; }
      set { FProjectTextBox.Text = value;  }
    }

    /// <summary>
    /// List of filters fromated as "Name 1|(*.xxx)|Name 2|(*.xxx)"
    /// </summary>
    public string DelphiProjectFilters
    {
      get
      {
        return FDelphiProjectFilters;
      }
      set
      {
        FDelphiProjectFilters = value;
      }
    }
  }
}