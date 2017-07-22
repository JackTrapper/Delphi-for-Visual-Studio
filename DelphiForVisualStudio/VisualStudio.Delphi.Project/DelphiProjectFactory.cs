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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Package;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using MSBuild = Microsoft.Build.BuildEngine;

namespace VisualStudio.Delphi.Project
{

  [GuidAttribute(Constants.DelphiProjectFactoryGUID)]
  public class DelphiProjectFactory : ProjectFactory
  {
    #region Constructors
    /// <summary>
    /// Explicit default constructor.
    /// </summary>
    /// <param name="package">Value of the project package for initialize internal package field.</param>
    public DelphiProjectFactory(DelphiProjectPackage package)
      : base(package)
    {
    }
    #endregion Constructors

    #region Methods
    /// <summary>
    /// Creates a new project by cloning an existing template project.
    /// </summary>
    /// <returns></returns>
    protected override Microsoft.VisualStudio.Package.ProjectNode CreateProject()
    {
      DelphiProjectNode project = new DelphiProjectNode();
      project.SetSite((IOleServiceProvider)((IServiceProvider)this.Package).GetService(typeof(IOleServiceProvider)));
      return project;
    }

    protected override bool CanCreateProject(string fileName, uint flags)
    {
      bool lResult = base.CanCreateProject(fileName, flags);
      if (lResult)
      {
        // read a XML property 
        MSBuild.Project lBuildProject = Utilities.ReinitializeMsBuildProject(this.BuildEngine, fileName, null);
        string value = lBuildProject.GetEvaluatedProperty(Resources.ImportDelphiProject);
        // The property ImportDelphiProject should only exist in Import.xdelphiproj file and will be removed later.
        // if it exists the user is importing a delphi project file.
        if (value == "true")
        {
          // get file name from Global Settings Var
          // NOTE: I do not like this method the converter should put the FileName
                // into the project template file 
                
          if (File.Exists(Properties.Settings.Default.WizardConvertProject))
          {
            lBuildProject.SetProperty(Resources.ImportDelphiPojectFile, Properties.Settings.Default.WizardConvertProject);
            lBuildProject.Save(fileName);
          }
          //else
          //{
          //  string[] lFiles = ShowOpenFileDialog("Open Existing Delphi Project", "Delphi Project (*.dpr)|*.dpr|Packages|*.dpk|All Files|*.*");
          //  if (lFiles.Length > 0)
          //  {
          //    // temporarly add the project file full path to the 
          //    lBuildProject.SetProperty(Resources.ImportDelphiPojectFile, lFiles[0]);
          //    lBuildProject.Save(fileName);
          //  }
            else
              lResult = false;
          //}
        }
      }
      return lResult;
    }
    // breaks my testing as I want it in a new folder as this is VS standard behaviour

    //private bool ImportingDpr(string filename)
    //{
    //  MSBuild.Project buildProject = Utilities.ReinitializeMsBuildProject(this.BuildEngine, filename, null);
    //  return buildProject.GetEvaluatedProperty(Resources.ImportDelphiProject).Equals("true", StringComparison.InvariantCultureIgnoreCase);
    //}
   
   protected override void CreateProject(string fileName, string location, string name, uint flags, ref Guid projectGuid, out IntPtr project, out int canceled)
    {
      try
      {
        // Breaks my testing as want it in a new folder as this is VS standard behaviour 
        //if (ImportingDpr(fileName) && (location.IndexOf(name) >= 0))
        //{
        //  location = location.Substring(0, location.IndexOf(name));
        //}
        base.CreateProject(fileName, location, name, flags, ref projectGuid, out project, out canceled);
      }
      finally
      {
        this.BuildProject = null; // corrects a bug on exception
      }
    }

    ///// <summary>
    ///// Visual Studio message box result values.
    ///// </summary>
    //[Flags]
    //internal enum VsOpenFileDialogFlags
    //{
    //  /// <summary>OFN_READONLY</summary>
    //  ReadOnly = 0x00000001,

    //  /// <summary>OFN_OVERWRITEPROMPT</summary>
    //  OverwritePrompt = 0x00000002,

    //  /// <summary>OFN_HIDEREADONLY</summary>
    //  HideReadOnly = 0x00000004,

    //  /// <summary>OFN_NOCHANGEDIR</summary>
    //  NoChangeDir = 0x00000008,

    //  /// <summary>OFN_SHOWHELP</summary>
    //  ShowHelp = 0x00000010,

    //  /// <summary>OFN_ENABLEHOOK</summary>
    //  EnableHook = 0x00000020,

    //  /// <summary>OFN_ENABLETEMPLATE</summary>
    //  EnableTemplate = 0x00000040,

    //  /// <summary>OFN_ENABLETEMPLATEHANDLE</summary>
    //  EnableTemplateHandle = 0x00000080,

    //  /// <summary>OFN_NOVALIDATE</summary>
    //  NoValidate = 0x00000100,

    //  /// <summary>OFN_ALLOWMULTISELECT</summary>
    //  AllowMultiSelect = 0x00000200,

    //  /// <summary>OFN_EXTENSIONDIFFERENT</summary>
    //  ExtensionDifferent = 0x00000400,

    //  /// <summary>OFN_PATHMUSTEXIST</summary>
    //  PathMustExist = 0x00000800,

    //  /// <summary>OFN_FILEMUSTEXIST</summary>
    //  FileMustExist = 0x00001000,

    //  /// <summary>OFN_CREATEPROMPT</summary>
    //  CreatePrompt = 0x00002000,

    //  /// <summary>OFN_SHAREAWARE</summary>
    //  ShareAware = 0x00004000,

    //  /// <summary>OFN_NOREADONLYRETURN</summary>
    //  NoReadOnly = 0x00008000,

    //  /// <summary>OFN_NOTESTFILECREATE</summary>
    //  NoTestFileCreate = 0x00010000,

    //  /// <summary>OFN_NONETWORKBUTTON</summary>
    //  NoNetworkButton = 0x00020000,

    //  /// <summary>OFN_NOLONGNAMES</summary>
    //  NoLongNames = 0x00040000,

    //  /// <summary></summary>
    //  Explorer = 0x00080000,

    //  /// <summary></summary>
    //  NoDereferenceLinks = 0x00100000,

    //  /// <summary></summary>
    //  LongNames = 0x00200000,

    //  /// <summary></summary>
    //  EnableIncludeNotify = 0x00400000,

    //  /// <summary></summary>
    //  EnableSizing = 0x00800000,

    //  /// <summary></summary>
    //  DontAddToRecent = 0x02000000,

    //  /// <summary></summary>
    //  ForceShowHidden = 0x10000000,
    //}
    ///// <summary>
    ///// Shows the Visual Studio open file dialog.
    ///// </summary>
    ///// <param name="dialogTitle">The title for the dialog box.</param>
    ///// <param name="filter">The filter for the dialog.</param>
    ///// <returns>The paths to the chosen files or null if the user canceled the dialog.</returns>
    //public string[] ShowOpenFileDialog(string dialogTitle, string filter)
    //{
    //  return this.ShowOpenFileDialog(dialogTitle, filter, null);
    //}

    ///// <summary>
    ///// Shows the Visual Studio open file dialog.
    ///// </summary>
    ///// <param name="dialogTitle">The title for the dialog box.</param>
    ///// <param name="filter">The filter for the dialog.</param>
    ///// <param name="initialDirectory">The initial starting directory. Can be null to use the current directory.</param>
    ///// <returns>An array of paths to the chosen files or an empty array if the user canceled the dialog.</returns>
    //public string[] ShowOpenFileDialog(string dialogTitle, string filter, string initialDirectory)
    //{
    //  ArrayList fileNames = new ArrayList();
    //  int bufferSize = NativeMethods.MAX_PATH;

    //  // Get the HWND to use for the modal file dialog.
    //  IVsUIShell lVSShell = Site.GetService(typeof(SVsUIShell)) as IVsUIShell;
    //  IntPtr hwnd;
    //  NativeMethods.ThrowOnFailure(lVSShell.GetDialogOwnerHwnd(out hwnd));

    //  // Create a native string buffer for the file name.
    //  IntPtr pwzFileName = Marshal.StringToHGlobalUni(new string('\0', bufferSize));

    //  try
    //  {
    //    // Fill in open file options structure.
    //    int hr;
    //    bool canceled;
    //    VSOPENFILENAMEW[] openFileOptions = new VSOPENFILENAMEW[1];
    //    //do
    //    //{
    //      openFileOptions[0].lStructSize = (uint)Marshal.SizeOf(typeof(VSOPENFILENAMEW));
    //      openFileOptions[0].hwndOwner = hwnd;
    //      openFileOptions[0].pwzDlgTitle = dialogTitle;
    //      openFileOptions[0].pwzFileName = pwzFileName;
    //      openFileOptions[0].nMaxFileName = (uint)bufferSize;
    //      openFileOptions[0].pwzFilter = filter.Replace('|', '\x0000')+ "\x0000";
    //      openFileOptions[0].pwzInitialDir = initialDirectory;
    //      openFileOptions[0].nFilterIndex = 0;
    //      // openFileOptions[0].dwFlags = (uint)(VsOpenFileDialogFlags.FileMustExist);

    //      // Open the Visual Studio open dialog.
    //      hr = lVSShell.GetOpenFileNameViaDlg(openFileOptions);
    //      canceled = (hr == NativeMethods.OLE_E_PROMPTSAVECANCELLED);
    //    //}
    //    //while (NativeMethods.Failed(hr));

    //    if (NativeMethods.Failed(hr) && !canceled)
    //    {
    //      NativeMethods.ThrowOnFailure(hr);
    //    }

    //    // Get the file name(s).
    //    if (openFileOptions[0].pwzFileName != IntPtr.Zero && !canceled)
    //    {
    //      // We want to get the entire buffered string because if multiple files were selected then it has
    //      // the following format: directory\0file1\0file2\0...fileN\0\0. Note that it ends with two null
    //      // terminators.
    //      string rawDialogPath = Marshal.PtrToStringUni(openFileOptions[0].pwzFileName, bufferSize);

    //      // These will hold our currently parsed values.
    //      StringBuilder directory = new StringBuilder();
    //      StringBuilder fileName = new StringBuilder();
    //      bool parsingDirectory = true;

    //      // Walk over the raw string to pull out the directory and the file names.
    //      for (int i = 0; i < rawDialogPath.Length; i++)
    //      {
    //        char c = rawDialogPath[i];
    //        char nextC = (i + 1 < rawDialogPath.Length ? rawDialogPath[i + 1] : '\0');

    //        // If we've hit a null termination, then we have to stop parsing for a second and add an
    //        // item to our array.
    //        if (c != '\0')
    //        {
    //          if (parsingDirectory)
    //          {
    //            directory.Append(c);
    //          }
    //          else
    //          {
    //            fileName.Append(c);
    //          }
    //        }
    //        else
    //        {
    //          if (parsingDirectory)
    //          {
    //            parsingDirectory = false;
    //          }
    //          else
    //          {
    //            // We've seen another file, so let's add the absolute path to our array.
    //            string absolutePath = System.IO.Path.Combine(directory.ToString(), fileName.ToString());
    //            absolutePath = (new Uri(absolutePath)).LocalPath;
    //            fileNames.Add(absolutePath);

    //            // Clear the file name StringBuilder for the next round.
    //            fileName.Length = 0;
    //          }

    //          // If we are at the double null termination then we can quit parsing.
    //          if (nextC == '\0')
    //          {
    //            // If the user only selected one file, then our parsed directory should be the full file name.
    //            if (fileNames.Count == 0)
    //            {
    //              fileNames.Add(directory.ToString());
    //            }
    //            break;
    //          }
    //        }
    //      }
    //    }
    //  }
    //  finally
    //  {
    //    // Release the string buffer.
    //    if (pwzFileName != IntPtr.Zero)
    //    {
    //      Marshal.FreeHGlobal(pwzFileName);
    //    }
    //  }

    //  return (string[])fileNames.ToArray(typeof(string));
    //}

    #endregion Methods
  }
}
