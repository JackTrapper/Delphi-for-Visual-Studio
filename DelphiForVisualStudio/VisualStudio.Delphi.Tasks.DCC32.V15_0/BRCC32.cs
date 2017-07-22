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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;

namespace VisualStudio.Delphi.Tasks.DCC32.V15_0
{

  public class BRCC32 : TCompilerTool
  {
    string FResourceSourceFile = null;
    string FOutputFileName = null;
    ITaskItem[] FIncludePath = null;
    string FDefineName = null;
    bool FExcludeEnironmentVar = false;
    bool FUnicode = false;
    string FDefautCodePage = null;
    string FDefaultLanguage = null;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Borland resource source file (*.rc)")]
    [Required]
    public string ResourceSourceFile
    {
      get { return FResourceSourceFile; }
      set { FResourceSourceFile = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Set output filename")]
    [CompilerCommand("-fo", AppendValue = true)]
    public string OutputFileName
    {
      get { return FOutputFileName; }
      set { FOutputFileName = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Set include path")]
    [CompilerCommand("-i", AppendValue = true)]
    public ITaskItem[] IncludePath
    {
      get { return FIncludePath; }
      set { FIncludePath = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Define #define (name[=\"string\"];name[=\"string\"])")]
    [CompilerCommand("-d", AppendValue = true)]
    public string DefineName
    {
      get { return FDefineName; }
      set { FDefineName = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Ignore INCLUDE environment variable (bool)")]
    [BoolCompilerCommand("-x")]
    public bool ExcludeEnironmentVar
    {
      get { return FExcludeEnironmentVar; }
      set { FExcludeEnironmentVar = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Enable multi-byte character support (bool)")]
    [BoolCompilerCommand("-m")]
    public bool Unicode
    {
      get { return FUnicode; }
      set { FUnicode = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("set default code page (int)")]
    [CompilerCommand("-c", AppendValue = true)]
    public string DefautCodePage
    {
      get { return FDefautCodePage; }
      set { FDefautCodePage = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("set default language (int)")]
    [CompilerCommand("-l", AppendValue = true)]
    public string DefaultLanguage
    {
      get { return FDefaultLanguage; }
      set { FDefaultLanguage = value; }
    }


    //OutputFileName        string      -fo  Set output filename
    //IncludePath           ITaskItem[] -i   Set include path
    //DefineName            string      -d   Define #define (name[="string"];name[="string"])
    //ExcludeEnironmentVar  bool        -x   Ignore INCLUDE environment variable (bool)
    //Unicode               bool        -m   Enable multi-byte character support (bool)
    //DefautCodePage        string      -c   set default code page (int)
    //DefaultLanguage       string      -l   set default language (int)

    protected override void AddCommandLines(CommandLineBuilder aCommandLineBuilder)
    {
      base.AddCommandLines(aCommandLineBuilder);
      aCommandLineBuilder.AppendFileNameIfNotNull(this.ResourceSourceFile);
    }

    protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
    {
      if (singleLine.Contains("Warning"))
      {
        base.Log.LogWarning(singleLine, new object[0]);
      }
      if (singleLine.Contains("Error"))
      {
        base.Log.LogError(singleLine, new object[0]);
      }
      if (singleLine.Contains("failed"))
      {
        base.Log.LogError(singleLine, new object[0]);
      }
    }

    protected override string GenerateFullPathToTool()
    {
      // because this DLL is for the spacific version of DCC
      // we can get the path from the registry and not use a env var.
      // HKEY_CURRENT_USER\Software\Borland\Delphi\7.0
      string lRootPath = @"Software\Borland\Delphi\7.0";
      string lPath = GetRegistryValue(lRootPath, "RootDir");
      if (lPath != "")
        lPath = System.IO.Path.Combine(lPath, "Bin");
      return Environment.ExpandEnvironmentVariables(System.IO.Path.Combine(lPath, @"BRCC32.EXE"));
    }

    private string GetRegistryValue(string aKey, string aName)
    {
      string lResult = "";
      RegistryKey lRegKey = Registry.CurrentUser.OpenSubKey(aKey, false);
      if (lRegKey != null)
      {
        try
        {
          lResult = (string)lRegKey.GetValue(aName);
        }
        catch
        {
          lResult = "";
        }
        lRegKey.Close();
      }
      return lResult;
    }


    protected override bool SkipTaskExecution()
    {
      return false;
    }

    protected override string ToolName
    {
      get
      {
        return "BRCC32";
      }
    }


  }
}