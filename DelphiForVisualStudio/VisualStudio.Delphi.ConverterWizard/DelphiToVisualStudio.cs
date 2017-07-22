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
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.Win32;


namespace VisualStudio.Delphi.ConverterWizard
{
  [ComVisible(true), Guid(Constants.CONVERT_WIZARD_GUID)]
  [ClassInterface(ClassInterfaceType.None)]
  [ComDefaultInterface(typeof(IDTWizard))]
  public class DelphiToVisualStudio : IDTWizard
  {
    #region IDTWizard Members

    private class OwnerHandler : IWin32Window
    {
      IntPtr FHandle;
      public OwnerHandler(int ahwndOwner)
      {
        FHandle = new IntPtr(ahwndOwner);
      }

      #region IWin32Window Members

      public IntPtr Handle
      {
        get { return FHandle; }
      }

      #endregion
    }

    public void Execute(object Application, int hwndOwner, ref object[] ContextParams, ref object[] CustomParams, ref wizardResult retval)
    {
      if ((string)ContextParams[0] != "{A5B66A92-9F46-4A9D-85F6-ADC9DF03A410}")
      {
        System.Windows.Forms.MessageBox.Show("This wizard is used for conversion only.");
        retval = wizardResult.wizardResultFailure;
        return;
      }
      DTE2 lDTE = Application as DTE2;
      //lDTE.Solution.Create("C:\\", "test1");
      //Delphi.Properties.Settings.Default.ConvertProject = @"C:\Code\Custom\DelphiForVisualStudio\New Delphi Projects Sample\VCLFormApp\Project1.dpr";
      //lDTE.Solution.AddFromTemplate(@"C:\Code\Custom\DelphiForVisualStudio\Bin\Templates\Imports\Import Existing Delphi Project (dpr).vstemplate", @"C:\temp", "Project100", false);

      WizardDialog lWizardDialog = new WizardDialog();
      lWizardDialog.SolutionEnabled = !Convert.ToBoolean(ContextParams[1]);
      lWizardDialog.DelphiProjectFilters = Resources.DelphiProjectFilters;
      lWizardDialog.OutputPaths = Delphi.Properties.Settings.Default.WizardLocations.Split(';');
      // must use active solution path
      if (!lWizardDialog.SolutionEnabled)
        lWizardDialog.OutputPath = Path.GetDirectoryName(lDTE.Solution.FileName);
      else
        lWizardDialog.OutputPath = Delphi.Properties.Settings.Default.WizardOutputPath;
      lWizardDialog.UseSameFolder = Delphi.Properties.Settings.Default.WizardUseSameFolder;
      lWizardDialog.CreateSolutionDir = Delphi.Properties.Settings.Default.WizardCreateSolutionDir;
      if (lWizardDialog.ShowDialog() == DialogResult.OK)
      {
        // Save settings 
        if (!lWizardDialog.UseSameFolder)
        {
          string[] lList = lWizardDialog.OutputPaths;
          Dictionary<string, int> lLookup = new Dictionary<string, int>();
          lLookup.Add(lWizardDialog.OutputPath,0);
          StringBuilder lLocations = new StringBuilder(lWizardDialog.OutputPath);
          foreach (string s in lList)
          {
            if (!lLookup.ContainsKey(s))
            {
              lLocations.Append(";");
              lLocations.Append(s);
              lLookup.Add(s, 0);
            }
         }
          Delphi.Properties.Settings.Default.WizardLocations = lLocations.ToString();
        }
        Delphi.Properties.Settings.Default.WizardOutputPath = lWizardDialog.OutputPath;
        Delphi.Properties.Settings.Default.WizardUseSameFolder = lWizardDialog.UseSameFolder;
        Delphi.Properties.Settings.Default.WizardCreateSolutionDir = lWizardDialog.CreateSolutionDir;
        Delphi.Properties.Settings.Default.Save();
        // get project name
        string lProjectName = Path.GetFileNameWithoutExtension(lWizardDialog.DelphiProjectFile);
        // if the locaiton is not the same folder then location is in the Project Name folder 
        string lLocation = lWizardDialog.OutputPath;
        string lSolutionLocation = lLocation;
        // Make new solution location if nessarry
        if (lWizardDialog.SolutionEnabled && lWizardDialog.CreateSolutionDir)
          lSolutionLocation = Path.Combine(lLocation, lWizardDialog.SolutionName);
        // Make new project location if nessary
        if (!lWizardDialog.UseSameFolder)
          lLocation = Path.Combine(lSolutionLocation, lProjectName);
        // Create solution if nessary
        if (lWizardDialog.SolutionEnabled)
          lDTE.Solution.Create(lSolutionLocation, lWizardDialog.SolutionName);
        // if a new solution needs to be created make one in the root location
        // Save the Global Var value 
        // NOTE: I do not like this i should copy the template files and add it to the 
        // copied template.
        Delphi.Properties.Settings.Default.WizardConvertProject = lWizardDialog.DelphiProjectFile;
        System.Reflection.Assembly lMyAsm = System.Reflection.Assembly.GetExecutingAssembly();

        lDTE.Solution.AddFromTemplate(Path.Combine(Path.GetDirectoryName(lMyAsm.Location), Resources.ConvertTemplateFile), lLocation, lProjectName, false);

      }
    }


    #endregion

   
    [ComRegisterFunction()]
    private static void RegisterClass(Type regObject)
    {
      RegisterWithVisualStudio();
    }

    [ComUnregisterFunction()]
    private static void UnregisterClass(Type regObject)
    {
      UnregisterWithVisualStudio();
    }

    private static bool FindTextInFile(string aText, string aFileName)
    {
      try
      {
        using (StreamReader lReader = new StreamReader(aFileName))
        {
          string s = lReader.ReadToEnd();
          lReader.Close();
          return s.Contains(aText);
        }
      }
      catch
      {
        return false;
      }
    }

    private static void AddRegistryValue(string aKey,string aName, string aValue)
    {
      RegistryKey lRegKey = Registry.LocalMachine.OpenSubKey(aKey, true);
      if (lRegKey != null)
      {
        lRegKey.SetValue(aName, aValue, RegistryValueKind.String);
        lRegKey.Close();
      }
    }

    private static void DeleteRegistryValue(string aKey, string aName)
    {
      RegistryKey lRegKey = Registry.LocalMachine.OpenSubKey(aKey, true);
      if (lRegKey != null)
      {
        lRegKey.DeleteValue(aName);
        lRegKey.Close();
      }
    }

    private static string GetRegistryValue(string aKey, string aName)
    {
      string lResult = "";
      RegistryKey lRegKey = Registry.LocalMachine.OpenSubKey(aKey, false);
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

    private static void WriteVSZFile(string aFileName, string aGUID)
    {
      using (StreamWriter lWriter = new StreamWriter(new FileStream(aFileName, FileMode.Create)))
      {
        lWriter.WriteLine("VSWizard 7.0");
        lWriter.WriteLine("Wizard="+aGUID);
        lWriter.Flush();
        lWriter.Close();
      }
    }

    private static void AddTextToFile(string lMyLine, string aFileName)
    {
      using (StreamWriter lWriter = new StreamWriter(aFileName,true))
      {
        lWriter.WriteLine(lMyLine);
        lWriter.Flush();
        lWriter.Close();
      }
    }

    private static void DeleteTextFromFile(string aMyline, string aFileName)
    {
      try
      {
        List<string> lList = new List<string>();
        using (StreamReader lReader = new StreamReader(aFileName))
        {
          while (!lReader.EndOfStream)
          {
            string s = lReader.ReadLine().Trim();
            if (s != "")
              lList.Add(s);
          }
          lReader.Close();
        }
        using (StreamWriter lWriter = new StreamWriter(new FileStream(aFileName, FileMode.Create)))
        {
          foreach (string lLine in lList)
          {
            if (!lLine.Contains(aMyline))
              lWriter.WriteLine(lLine);
          }
          lWriter.Flush();
          lWriter.Close();
        }
      }
      catch
      {
      }
    }

    private static void DeleteFile(string aFileName)
    {
      if (File.Exists(aFileName))
        File.Delete(aFileName);
    }

    public static void RegisterWithVisualStudio()
    {
      // Get GUID attribute for calling my COM object
      string lGUID;
      Type lType = typeof(DelphiToVisualStudio);
      object[] lAttrs = typeof(DelphiToVisualStudio).GetCustomAttributes(typeof(GuidAttribute), false);
      lGUID = "{" + (lAttrs[0] as GuidAttribute).Value + "}";
      string lRootPath = Constants.DEFAULT_REGISTRY_ROOT;
      AddRegistryValue(lRootPath + "\\Converters", lGUID, typeof(DelphiToVisualStudio).Name);
      string lPath = GetRegistryValue(lRootPath,"InstallDir");
      if (lPath != "")
      {
        string lFileName = Path.Combine(lPath, "convert.dir");
        string lMyline = Constants.DELPHI_TO_VS_VSZ_FILE_PATH + "|"; 
        lMyline += Constants.DELPHI_TO_VS_BITMAP_FILE_PATH + "|";
        lMyline += Resources.CONVERT_WizardName + "|";
        lMyline += Resources.CONVERT_WizardDescription + "|1";
        if (!FindTextInFile(lMyline, lFileName))
        {
          AddTextToFile(lMyline, lFileName);
          WriteVSZFile(Path.Combine(lPath, Constants.DELPHI_TO_VS_VSZ_FILE_PATH), lGUID);
          Resources.DelphiToVS.Save(Path.Combine(lPath, Constants.DELPHI_TO_VS_BITMAP_FILE_PATH));
        }
      }
    }
      
    

    public static void UnregisterWithVisualStudio()
    {
      string lGUID;
      Type lType = typeof(DelphiToVisualStudio);
      object[] lAttrs = typeof(DelphiToVisualStudio).GetCustomAttributes(typeof(GuidAttribute), false);
      lGUID = "{" + (lAttrs[0] as GuidAttribute).Value + "}";
      string lRootPath = Constants.DEFAULT_REGISTRY_ROOT;
      DeleteRegistryValue(lRootPath + "\\Converters", lGUID);
      string lPath = GetRegistryValue(lRootPath, "InstallDir");
      if (lPath != "")
      {
        string lFileName = Path.Combine(lPath, "convert.dir");
        string lMyline = Constants.DELPHI_TO_VS_VSZ_FILE_PATH + "|"; 
        lMyline += Constants.DELPHI_TO_VS_BITMAP_FILE_PATH + "|";
        lMyline += Resources.CONVERT_WizardName + "|";
        lMyline += Resources.CONVERT_WizardDescription + "|1";
        if (FindTextInFile(lMyline, lFileName))
        {
          DeleteTextFromFile(lMyline, lFileName);
          DeleteFile(Path.Combine(lPath, Constants.DELPHI_TO_VS_VSZ_FILE_PATH));

          DeleteFile(Path.Combine(lPath, Constants.DELPHI_TO_VS_BITMAP_FILE_PATH));
        }
      }
    }

  }
}
