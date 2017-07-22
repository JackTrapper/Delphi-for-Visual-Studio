/* This file is part of "Delphi For Visual Studio" project
 * http://www.codeplex.com/Delphi4VisualStudio
 * Copyright (c) 2006 Davinci Jeremie. All rights reserved.
 * Created 2007/04/20 by Davinci Jeremie (Web: http://www.Jeremie.ca )
 * 
 * LICENSE: LGPL
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License (LGPL) as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * For more details on the GNU Lesser General Public License,
 * see http://www.gnu.org/copyleft/lesser.html
 *************************************************************************/
using System;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;



namespace VisualStudio.Delphi.Tasks
{

  public abstract class TDelphiCompilerTool : TCompilerTool
  {
    private string FMainSourceFile;
    private StringBuilder FConfigFileLines;
    private string FReplaceConfigFile= "";
    private const int MAX_PATH = 260;
    private bool FRewriteConfigFile = false;
    private bool FDeleteConfigFile = false;


    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Delphi main source file .DPR .DPK",DefineProperty=true,PropertyName="DelphiMainSourceFile")]
    [Required]
    public string MainSourceFile
    {
      get { return FMainSourceFile; }
      set { FMainSourceFile = value;  }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("If true the config file is rewriten and replaced if diffrent. (bool)", DefineProperty = true, PropertyName = "DelphiRewriteConfigFile")]
    public bool RewriteConfigFile
    {
      get { return FRewriteConfigFile; }
      set { FRewriteConfigFile = value; }
    }


    //protected TDelphiCompilerTool()
    //{
    //}


    public override bool Execute()
    {
      FReplaceConfigFile = "";
      FDeleteConfigFile = false;
      try
      {
        return base.Execute();
      }
      finally
      {
        RestoreConfigFile();
      }
    }

    private void RestoreConfigFile()
    {
      string lConfigFileName = Path.GetFileNameWithoutExtension(MainSourceFile) + ".cfg";
      if (FDeleteConfigFile && File.Exists(lConfigFileName))
        File.Delete(lConfigFileName);
      if (!String.IsNullOrEmpty(FReplaceConfigFile) && File.Exists(FReplaceConfigFile))
      {
        if (File.Exists(lConfigFileName))
          File.Delete(lConfigFileName);
        File.Move(FReplaceConfigFile, lConfigFileName);
      }
    }

    /// <summary>
    /// When the task is used in the IDE you may want to compile it diffrently but not right now.
    /// </summary>
    /// <returns></returns>
    protected override HostObjectInitializationStatus InitializeHostObject()
    {
      return HostObjectInitializationStatus.UseAlternateToolToExecute;
    }

    protected override void AddCommandLines(CommandLineBuilder aCommandLineBuilder)
    {
      //*** Add main source file first 
      aCommandLineBuilder.AppendSwitch(MainSourceFile);
      // create Empty string builder for config file.
      FConfigFileLines = new StringBuilder();
      // MSBuild does nod diplay the config file data correctly so addding a line displays it nicely on the screen
      FConfigFileLines.AppendLine();

      base.AddCommandLines(aCommandLineBuilder);
    }

    protected override void AddCommand(CommandLineBuilder aCommandLineBuilder, CompilerCommandAttribute aCommand, object aValue)
    {
      // keep a copy of the current command line
      string lBefore = aCommandLineBuilder.ToString();
     
      base.AddCommand(aCommandLineBuilder, aCommand, aValue);
      // after adding the command we need t get the appended value out
      // and place it on a line if one exists.
      string lAfter = aCommandLineBuilder.ToString();
      if (lBefore != lAfter)
      {
        lAfter = lAfter.Substring(lBefore.Length, lAfter.Length - lBefore.Length).TrimStart();
        if (lAfter != "")
          FConfigFileLines.AppendLine(lAfter);
      }
    }
    /// <summary>
    /// sometimes the command line can not be used because of lenght or the end user requested a responce file
    /// </summary>
    /// <returns></returns>
    protected override string GenerateResponseFileCommands()
    {
      if (this.RewriteConfigFile || CommandLine.Length + this.GenerateFullPathToTool().Length > MAX_PATH)
      {
        return FConfigFileLines.ToString();
      }
      return String.Empty;
    }

    protected override string GetResponseFileSwitch(string responseFilePath)
    {
      // writing config file (.cfg) for delphi compilers to use 
      bool lSameConfig = false;
      string lConfigFileName = Path.GetFileNameWithoutExtension(MainSourceFile) + ".cfg";
      if (File.Exists(lConfigFileName))
      {
        // check to see if the config files are the exact same
        lSameConfig = FileCompare(responseFilePath, lConfigFileName);
        if (!lSameConfig && !RewriteConfigFile)
        {
          // back up config file if they are not the same and we can not over write it.
          FReplaceConfigFile = lConfigFileName + ".DFVSBackup";
          if (File.Exists(FReplaceConfigFile))
            File.Delete(FReplaceConfigFile);
          File.Move(lConfigFileName, FReplaceConfigFile);
        }
      }
      else
        FDeleteConfigFile = true;
      // Copy the temp config to the new location.
      if (!lSameConfig)
        File.Copy(responseFilePath, lConfigFileName, true);
      this.Log.LogMessage(MainSourceFile,null);
      // all delphi compilers use a master file that has all files and code to compile
      return MainSourceFile;
    }

    // This method accepts two strings the represent two files to 
    // compare. A return value of 0 indicates that the contents of the files
    // are the same. A return value of any other value indicates that the 
    // files are not the same.
    private bool FileCompare(string file1, string file2)
    {
      int file1byte;
      int file2byte;
      FileStream fs1;
      FileStream fs2;

      // Determine if the same file was referenced two times.
      if (file1 == file2)
      {
        // Return true to indicate that the files are the same.
        return true;
      }

      // Open the two files.
      fs1 = new FileStream(file1, FileMode.Open);
      fs2 = new FileStream(file2, FileMode.Open);

      // Check the file sizes. If they are not the same, the files 
      // are not the same.
      if (fs1.Length != fs2.Length)
      {
        // Close the file
        fs1.Close();
        fs2.Close();

        // Return false to indicate files are different
        return false;
      }

      // Read and compare a byte from each file until either a
      // non-matching set of bytes is found or until the end of
      // file1 is reached.
      do
      {
        // Read one byte from each file.
        file1byte = fs1.ReadByte();
        file2byte = fs2.ReadByte();
      }
      while ((file1byte == file2byte) && (file1byte != -1));

      // Close the files.
      fs1.Close();
      fs2.Close();

      // Return the success of the comparison. "file1byte" is 
      // equal to "file2byte" at this point only if the files are 
      // the same.
      return ((file1byte - file2byte) == 0);
    }

    protected override Encoding ResponseFileEncoding
    {
      get
      {
        return Encoding.ASCII;
      }
    }

    [Output]
    public virtual string DelphiCompilerTargetsPath
    {
      get
      {
        return "";
      }
    }
  }
}
