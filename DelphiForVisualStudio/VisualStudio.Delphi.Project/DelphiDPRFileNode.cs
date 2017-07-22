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
using Microsoft.VisualStudio.TextManager.Interop;
using VisualStudio.Delphi.Language;


namespace VisualStudio.Delphi.Project
{
    /// <summary>
  /// This node manages .DPR files and has methods to add,
  /// delete, and rename items like
  /// unit files - Unit1 in 'Unit1.pas'
  /// contained files - {%File 'filename'}
  /// 
  /// </summary>
  [CLSCompliant(false), ComVisible(true)]
  [GuidAttribute(Constants.DELPHI_DPKFileNodeGuid)]
  public class DelphiPackageFileNode : DelphiMainFileNode
  {
    protected delegate void CannotAddEntryPointErrorMessage();
    #region ctor

    public DelphiPackageFileNode(ProjectNode root, ProjectElement e)
      : base(root, e)
    {
    }

    #endregion

    #region private methods

    #endregion

    #region protected methods

    #endregion

    #region properties

    #endregion

    #region public methods

    /// <summary>
    /// Validates a file to is a DPK main file.
    /// </summary>
    /// <param name="aFileName"></param>
    /// <returns></returns>
    public override bool IsDelphiMainFile(string aFileName)
    {
      return IsDelphiProjectFile(aFileName);
    }

    /// <summary>
    /// Evaluates if a file is an Delphi .DPK file based on is extension
    /// </summary>
    /// <returns>true if is a project file</returns>
    public static bool IsDelphiProjectFile(string aFileName)
    {
      return PathTools.HasExtension(aFileName, Constants.Extentions.DPK);
    }

    /// <summary>
    /// Renames the unit name after package keyword in the DPK 
    /// if the name does not match it will rename the units in the uses
    /// clause
    /// </summary>
    /// <param name="aOldName"></param>
    /// <param name="aNewName"></param>
    public override void RenameDelphiUnit(string aOldUnitName, string aNewUnitName)
    {
      try
      {
        IVsTextLines lLines = GetTextLines();

        if (lLines != null)
          if (RenameUnit(DelphiUnitType.Package, aOldUnitName, aNewUnitName, lLines))

          {
            DoAfterDelphiUnitRename(aOldUnitName, aNewUnitName);
          }
          else
            RenameUsesUnit(aOldUnitName, aNewUnitName, lLines);
      }
      catch (Exception ex)
      {
        ShowErrorMessageBox(ex.Message, "RenameDelphiUnit() Error");
      }
    }

    /// <summary>
    /// Overides the rename process and looks for the keyword "containes" to rename project files (;)
    /// </summary>
    /// <param name="aOldUnitName">The old unit name without file extention</param>
    /// <param name="aNewUnitName">The new unit name without file extention</param>
    /// <param name="aVsTextLines"></param>
    /// <returns></returns>
    protected override bool RenameUsesUnit(string aOldUnitName, string aNewUnitName, IVsTextLines aVsTextLines)
    {
      return RenameUnitAfterKeyword(DelphiToken.containsKeyword, aOldUnitName, aNewUnitName, aVsTextLines);
    }

    public override string[] GetUnitFiles()
    {
      return this.GetUnitFilesAfterKeyword(DelphiToken.containsKeyword);
    }

    /// <summary>
    /// Returns all .NET reference files (this may be moved to a TBDProjDependentFileNode class)
    /// </summary>
    /// <returns></returns>
    public string[] GetReferenceFiles()
    {
      return new string[0];
    }

    #endregion

  }

  /// <summary>
  /// This node manages .DPR files and has methods to add,
  /// delete, and rename items like
  /// unit files - Unit1 in 'Unit1.pas'
  /// contained files - {%File 'filename'}
  /// 
  /// </summary>
  [CLSCompliant(false), ComVisible(true)]
  [GuidAttribute(Constants.DELPHI_DPRFileNodeGuid)]
  public class DelphiProjectFileNode : DelphiMainFileNode
  {
    protected delegate void CannotAddEntryPointErrorMessage();
    #region ctor

    public DelphiProjectFileNode(ProjectNode root, ProjectElement e)
      : base(root, e)
    {
    }

    #endregion

    #region private methods

    #endregion

    #region protected methods

    #endregion

    #region properties

    #endregion

    #region public methods

    /// <summary>
    /// Validates a file to is a DPR main file.
    /// </summary>
    /// <param name="aFileName"></param>
    /// <returns></returns>
    public override bool IsDelphiMainFile(string aFileName)
    {
      return IsDelphiProjectFile(aFileName);
    }

    /// <summary>
    /// Evaluates if a file is an Delphi .DPR file based on is extension
    /// </summary>
    /// <returns>true if is a project file</returns>
    public static bool IsDelphiProjectFile(string aFileName)
    {
      return PathTools.HasExtension(aFileName, Constants.Extentions.DPR);
    }

    /// <summary>
    /// Renames the unit name after Program or Library in the DPR 
    /// if the name does not match it will rename the units in the uses
    /// clause
    /// </summary>
    /// <param name="aOldName"></param>
    /// <param name="aNewName"></param>
    public override void RenameDelphiUnit(string aOldUnitName, string aNewUnitName)
    {
      try
      {
        IVsTextLines lLines = GetTextLines();

        if (lLines != null)
          if (RenameUnit(DelphiUnitType.Program, aOldUnitName, aNewUnitName, lLines) ||
              RenameUnit(DelphiUnitType.Library, aOldUnitName, aNewUnitName, lLines))

          {
            DoAfterDelphiUnitRename(aOldUnitName, aNewUnitName);
          }
          else
            RenameUsesUnit(aOldUnitName, aNewUnitName, lLines);
      }
      catch (Exception ex)
      {
        ShowErrorMessageBox(ex.Message, "RenameDelphiUnit() Error");
      }
    }

    public override string[] GetUnitFiles()
    {
      return this.GetUnitFilesAfterKeyword(DelphiToken.usesKeyword);
    }

    /// <summary>
    /// Returns all .NET reference files (this may be moved to a TBDProjDependentFileNode class)
    /// </summary>
    /// <returns></returns>
    public string[] GetReferenceFiles()
    {
      return new string[0];
    }

    #endregion

    #region  methods





    #endregion

  }
}
