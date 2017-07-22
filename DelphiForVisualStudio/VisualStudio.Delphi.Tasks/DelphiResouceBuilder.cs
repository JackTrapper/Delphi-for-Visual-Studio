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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace VisualStudio.Delphi.Tasks
{
  public class DelphiResourceBuilder : Task
  {
    private string FIconFile = null;
    private int FMajorVersion = 0;
    private int FMinorVersion = 0;
    private int FReleaseVersion = 0;
    private int FBuildVersion = 0;
    private bool FPrivateBuild = false;
    private bool FDebugBuild = false;
    private bool FSpecialBuild = false;
    private bool FDLLBuild = false;
    private bool FPreReleaseBuild = false;
    private string FCompanyName = null;
    private string FFileDescription = null;
    private string FFileVersion = null;
    private string FInternalName = null;
    private string FLegalCopyright = null;
    private string FOriginalFilename = null;
    private string FLegalTrademarks = null;
    private string FProductName = null;
    private string FProductVersoin = null;
    private string FComments = null;
    private string FCustomVersionInfo = null;
    private bool FAutoUpdateFileVersion = false;
    private bool FAutoUpdateProductVersion = false;
    private bool FIncludeVersionInfo = false;
    private string FLocalID = "";
    private string FCodePage = "";
    private int FIntLocalID = 4105;
    private int FIntCodePage = 1252;
    private ITaskItem[] FCompileDelphiResource = null;

    private string FResourceSourceFile;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition(".", DefineProperty = true, PropertyName = "DelphiResourceSourceFile")]
    [Required]
    public string ResourceSourceFile
    {
      get { return FResourceSourceFile; }
      set { FResourceSourceFile = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition(".", DefineProperty = true, PropertyName = "DelphiIncludeVersionInfo")]
    public bool IncludeVersionInfo
    {
      get { return FIncludeVersionInfo; }
      set { FIncludeVersionInfo = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("File name of a icon file (.ico) that will becompiled into resource file.", DefineProperty = true, PropertyName = "DelphiIconFile")]
    public string IconFile
    {
      get { return FIconFile; }
      set { FIconFile = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiMajorVersion")]
    public int MajorVersion
    {
      get { return FMajorVersion; }
      set { FMajorVersion = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiMinorVersion")]
    public int MinorVersion
    {
      get { return FMinorVersion; }
      set { FMinorVersion = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiReleaseVersion")]
    public int ReleaseVersion
    {
      get { return FReleaseVersion; }
      set { FReleaseVersion = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiBuildVersion")]
    public int BuildVersion
    {
      get { return FBuildVersion; }
      set { FBuildVersion = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiDebugBuild")]
    public bool DebugBuild
    {
      get { return FDebugBuild; }
      set { FDebugBuild = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiSpecialBuild")]
    public bool SpecialBuild
    {
      get { return FSpecialBuild; }
      set { FSpecialBuild = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiDLLBuild")]
    public bool DLLBuild
    {
      get { return FDLLBuild; }
      set { FDLLBuild = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiPreReleaseBuild")]
    public bool PreReleaseBuild
    {
      get { return FPreReleaseBuild; }
      set { FPreReleaseBuild = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiPrivateBuild")]
    public bool PrivateBuild
    {
      get { return FPrivateBuild; }
      set { FPrivateBuild = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiCompanyName")]
    public string CompanyName
    {
      get { return FCompanyName; }
      set { FCompanyName = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiFileDescription")]
    public string FileDescription
    {
      get { return FFileDescription; }
      set { FFileDescription = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiFileVersion")]
    public string FileVersion
    {
      get { return FFileVersion; }
      set { FFileVersion = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiInternalName")]
    public string InternalName
    {
      get { return FInternalName; }
      set { FInternalName = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiLegalCopyright")]
    public string LegalCopyright
    {
      get { return FLegalCopyright; }
      set { FLegalCopyright = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiLegalTrademarks")]
    public string LegalTrademarks
    {
      get { return FLegalTrademarks; }
      set { FLegalTrademarks = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiOriginalFilename")]
    public string OriginalFilename
    {
      get { return FOriginalFilename; }
      set { FOriginalFilename = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiProductName")]
    public string ProductName
    {
      get { return FProductName; }
      set { FProductName = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiProductVersion")]
    public string ProductVersion
    {
      get { return FProductVersoin; }
      set { FProductVersoin = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiComments")]
    public string Comments
    {
      get { return FComments; }
      set { FComments = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("(NAME=\"VALUE\";NAME2=\"VALUE\"", DefineProperty = true, PropertyName = "DelphiCustomVersionInfo")]
    public string CustomVersionInfo
    {
      get { return FCustomVersionInfo; }
      set { FCustomVersionInfo = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiAutoUpdateFileVersion")]
    public bool AutoUpdateFileVersion
    {
      get { return FAutoUpdateFileVersion; }
      set { FAutoUpdateFileVersion = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiAutoUpdateProductVerion")]
    public bool AutoUpdateProductVerion
    {
      get { return FAutoUpdateProductVersion; }
      set { FAutoUpdateProductVersion = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiAssemblyInfoLocalID")]
    public string LocalID
    {
      get { return FLocalID; }
      set 
      { 
        FLocalID = value;
        try
        {
          FIntLocalID = Convert.ToInt32(FLocalID);
        }
        catch 
        {
          FLocalID = "";
        }
      }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("", DefineProperty = true, PropertyName = "DelphiAssemblyInfoCodePage")]
    public string CodePage
    {
      get { return FCodePage; }
      set 
      { 
        FCodePage = value;
        try
        {
          FIntCodePage = Convert.ToInt32(FCodePage);
        }
        catch 
        {
          FCodePage = "";
        }
      }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Provide CompileDelphiResource Item Group.")]
    [Output]
    public ITaskItem[] CompileDelphiResource
    {
      get { return FCompileDelphiResource; }
      set { FCompileDelphiResource = value; }
    }



    const int VS_FF_DEBUG = 0x00000001;
    const int VS_FF_PRERELEASE = 0x00000002;
    const int VS_FF_PATCHED = 0x00000004;
    const int VS_FF_PRIVATEBUILD = 0x00000008;
    const int VS_FF_INFOINFERRED = 0x00000010;
    const int VS_FF_SPECIALBUILD = 0x00000020;

    public override bool Execute()
    {
      StringBuilder lRCText = new StringBuilder();
      lRCText.AppendLine("/*********************************************");
      lRCText.AppendLine("This File should NOT be checked into source ");
      lRCText.AppendLine("control systems as it changes to often.");
      lRCText.AppendLine("*********************************************/");
      if (!String.IsNullOrEmpty(this.IconFile))
      {
        lRCText.AppendFormat("MAINICON ICON \"{0}\"\r\n",this.IconFile);
      }
      if (IncludeVersionInfo)
      {
        lRCText.AppendLine("1 VERSIONINFO");
        lRCText.AppendFormat("FILEVERSION {0},{1},{2},{3}", new object[] { MajorVersion, MinorVersion, ReleaseVersion, BuildVersion });
        lRCText.AppendFormat("PRODUCTVERSION {0},{1},{2},{3}", new object[] { MajorVersion, MinorVersion, ReleaseVersion, BuildVersion });
        lRCText.AppendLine();
        lRCText.AppendLine("FILEFLAGSMASK 0x3FL");
        int lFileFlag = 0;
        if (this.DebugBuild)
          lFileFlag |= VS_FF_DEBUG;
        if (this.SpecialBuild)
          lFileFlag |= VS_FF_SPECIALBUILD;
        if (this.PreReleaseBuild)
          lFileFlag |= VS_FF_PRERELEASE;
        if (this.PrivateBuild)
          lFileFlag |= VS_FF_PRIVATEBUILD;
        lFileFlag |= VS_FF_SPECIALBUILD;
        lRCText.AppendLine(String.Format("FILEFLAGS {0:X}L", lFileFlag));
        lRCText.AppendLine("FILEOS 0x40004L");
        if (this.DLLBuild)
          lRCText.AppendLine("FILETYPE 2L");
        else
          lRCText.AppendLine("FILETYPE 1L");
        const string lSpacing = "    ";
        lRCText.AppendLine("FILESUBTYPE 0x0L");
        lRCText.AppendLine("BEGIN");
        lRCText.AppendLine(lSpacing + "BLOCK \"StringFileInfo\"");
        lRCText.AppendLine(lSpacing + "BEGIN");
        lRCText.AppendLine(String.Format(lSpacing + lSpacing + "BLOCK \"{0}{1}\"", FIntLocalID.ToString("X").PadLeft(4, '0'), FIntCodePage.ToString("X").PadLeft(4, '0')));
        lRCText.AppendLine(lSpacing + lSpacing + "BEGIN");
        string lSpace = lSpacing + lSpacing + lSpacing;
        lRCText.Append(lSpace);
        WriteVersionInfo(lRCText, "CompanyName", CompanyName);
        lRCText.Append(lSpace);
        WriteVersionInfo(lRCText, "FileDescription", FileDescription);
        lRCText.Append(lSpace);
        WriteVersionInfo(lRCText, "FileVersion", FileVersion);
        lRCText.Append(lSpace);
        WriteVersionInfo(lRCText, "InternalName", InternalName);
        lRCText.Append(lSpace);
        WriteVersionInfo(lRCText, "LegalCopyright", LegalCopyright);
        lRCText.Append(lSpace);
        WriteVersionInfo(lRCText, "LegalTrademarks", LegalTrademarks);
        lRCText.Append(lSpace);
        WriteVersionInfo(lRCText, "OriginalFilename", OriginalFilename);
        lRCText.Append(lSpace);
        WriteVersionInfo(lRCText, "ProductName", ProductName);
        lRCText.Append(lSpace);
        WriteVersionInfo(lRCText, "ProductVersion", ProductVersion);
        lRCText.Append(lSpace);
        WriteVersionInfo(lRCText, "Comments", Comments);
        if (!String.IsNullOrEmpty(this.CustomVersionInfo))
        {
          string[] lValues = CustomVersionInfo.Split(new char[] { ';' });
          foreach (string lInfo in lValues)
            if (lInfo.Contains("="))
            {
              string lName = lInfo.Split('=')[0];
              string lValue = RemoveQuotedString(lInfo.Split('=')[1]);
              lRCText.Append(lSpace);
              WriteVersionInfo(lRCText, lName, lValue);
            }
        }
        lRCText.AppendLine(lSpacing + lSpacing + "END");
        lRCText.AppendLine(lSpacing + "END");
        lRCText.AppendLine(lSpacing + "BLOCK \"VarFileInfo\"");
        lRCText.AppendLine(lSpacing + "BEGIN");
        lRCText.AppendFormat(lSpacing + lSpacing + "VALUE \"Translation\", 0x{0}, {1}\r\n", FIntLocalID.ToString("X").PadLeft(4, '0'), FIntCodePage);
        lRCText.AppendLine(lSpacing + "END");
        lRCText.AppendLine("END");
      }
      if (File.Exists(ResourceSourceFile))
        File.Delete(ResourceSourceFile);
      if (!String.IsNullOrEmpty(lRCText.ToString()))
      {
        using (StreamWriter lStreamWriter = new StreamWriter(this.ResourceSourceFile))
          lStreamWriter.Write(lRCText.ToString());
      }
      Dictionary<string, string> lMetadata = new Dictionary<string, string>();

      lMetadata.Add("OutputFileName", "");
      lMetadata.Add("IncludePath", "");
      lMetadata.Add("DefineName", "");
      lMetadata.Add("ExcludeEnironmentVar", "false");
      lMetadata.Add("Unicode", "false");
      lMetadata.Add("DefautCodePage", CodePage);
      lMetadata.Add("DefaultLanguage", LocalID);
      FCompileDelphiResource = new ITaskItem[] { new TaskItem(ResourceSourceFile, lMetadata) };

      return true;
    }

    private void WriteVersionInfo(StringBuilder aSB, string aName, string aValue)
    {
      if (String.IsNullOrEmpty(aValue)) return;
      aSB.AppendFormat("VALUE \"{0}\", \"{1}\\0\"\r\n", aName, aValue);
    }

    //FILESUBTYPE 0x0L
    //BEGIN
    //    BLOCK "StringFileInfo"
    //    BEGIN
    //         BLOCK "100904E4"
    //         BEGIN
    //              VALUE "CompanyName", "comp teest\0"
    //              VALUE "FileDescription", "fd test\0"
    //              VALUE "FileVersion", "1.0.5.0\0"
    //              VALUE "InternalName", "IN test\0"
    //              VALUE "LegalCopyright", "lc test\0"
    //              VALUE "LegalTrademarks", "lt test\0"
    //              VALUE "OriginalFilename", "of test\0"
    //              VALUE "ProductName", "pn test\0"
    //              VALUE "ProductVersion", "1.0.5.0\0"
    //              VALUE "Comments", "comment test\0"
    //              VALUE "customTest", "ct test\0"
    //              VALUE "CompileDate", "November 6, 2007 12:48 PM\0"
    //         END
    //    END
    //    BLOCK "VarFileInfo"
    //    BEGIN
    //         VALUE "Translation", 0x1009, 1252
    //    END
    //END
    private string RemoveQuotedString(string aString)
    {
      char lQuoteChar = '\"';
      if (aString == null || aString == "")
        return "";
      if (aString[0] != lQuoteChar || aString[aString.Length - 1] != lQuoteChar)
        return aString;

      return aString.Substring(1, aString.Length - 2).Replace(String.Format("{0}{0}", lQuoteChar), String.Format("{0}", lQuoteChar));
    }

  }
}