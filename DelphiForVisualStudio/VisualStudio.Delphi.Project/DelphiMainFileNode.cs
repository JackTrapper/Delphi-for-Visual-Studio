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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VisualStudio.Delphi.Language;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;


namespace VisualStudio.Delphi.Project
{
  public class DelphiMainFileNode : DelphiFileNode
  {
    #region ctor

    public DelphiMainFileNode(ProjectNode root, ProjectElement e)
      : base(root, e)
    {
    }

    #endregion

    #region private methods

    #endregion

    #region protected methods

    protected override NodeProperties CreatePropertiesObject()
    {
      // TODO: Change this to it's own class
      return new DelphiDependentFileNodeProprties(this);
    }

    /// <summary>
    /// Delphi IDE Project File Node cannot be dragged.
    /// </summary>
    /// <returns>A stringbuilder.</returns>
    protected internal override StringBuilder PrepareSelectedNodesForClipBoard()
    {
      return null;
    }

    /// <summary>
    /// Validates that the file beeing renamed can have its unit name renamed.
    /// For exampe the first line of a project file is 
    /// program project1;
    /// if the file name is renamed the project1 must be renamed.
    /// </summary>
    /// <param name="oldFileName"></param>
    /// <param name="newFileName"></param>
    /// <returns></returns>
    protected override bool CanReanmeUnit(string oldFileName, string newFileName)
    {
      return IsDelphiMainFile(oldFileName) && IsDelphiMainFile(newFileName);
    }

    /// <summary>
    /// Must be overriden by inherited class validates that the 
    /// filename is a main file.
    /// </summary>
    /// <param name="aFileName"></param>
    /// <returns></returns>
    public virtual bool IsDelphiMainFile(string aFileName)
    {
      return false;
    }

    /// <summary>
    /// After a rename of the unit the a the Delphi main file 
    /// must rename the file name if it exists.
    /// </summary>
    /// <param name="aOldUnitName"></param>
    /// <param name="aNewUnitName"></param>
    /// <param name="aScanner"></param>
    protected override void AfterRenameUnitName(string aOldUnitName, string aNewUnitName, TUnitScanner aScanner)
    {
      aScanner.EndToken = DelphiToken.Delimiter;
      if (aScanner.GotoNextToken(DelphiToken.inKeyword))
        if (aScanner.GotoNextToken(DelphiToken.String))
        {
          string lFilePath = TDelphiSource.DelphiStringToText(aScanner.TokenValue);
          if (IsDelphiPasFile(lFilePath))
          {
            lFilePath = Path.Combine(Path.GetDirectoryName(lFilePath), aNewUnitName) + ".pas";
            lFilePath = TDelphiSource.TextToDelphiString(lFilePath);
            ReplaceText(aScanner, lFilePath);
          }
        }
    }

    protected virtual string[] GetUnitFilesAfterKeyword(DelphiToken aKeyword)
    {
      List<string> lResult = new List<string>();
      try
      {
        TUnitScanner lScanner = new TUnitScanner(GetTextLines());
        lScanner.EndToken = DelphiToken.None;
        if (lScanner.GotoNextToken(aKeyword))
        {
          lScanner.EndToken = DelphiToken.EndStatement;
          while (lScanner.GotoNextToken(DelphiToken.String))
            lResult.Add(TDelphiSource.DelphiStringToText(lScanner.TokenValue));
        }
      }
      catch (Exception ex)
      {
        ShowErrorMessageBox(ex.Message, "GetUnitFilesAfterKeyword() Error");
      }
      return lResult.ToArray();
    }

    /// <summary>
    /// Not supported.
    /// </summary>
    protected override int ExcludeFromProject()
    {
      return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
    }

    /// <summary>
    /// Handles the menuitems
    /// </summary>		
    protected override int QueryStatusOnNode(Guid guidCmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result)
    {
      if (guidCmdGroup == Microsoft.VisualStudio.Shell.VsMenus.guidStandardCommandSet97)
      {
        switch ((VsCommands)cmd)
        {
          case VsCommands.Delete:
            result |= QueryStatusResult.NOTSUPPORTED;
            return VSConstants.S_OK;
          //case VsCommands.ViewForm:
          //  if (IsFormSubType)
          //    result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
          //  return VSConstants.S_OK;
        }
      }
      return base.QueryStatusOnNode(guidCmdGroup, cmd, pCmdText, ref result);
    }

    protected override bool CanDeleteItem(__VSDELETEITEMOPERATION deleteOperation)
    {
      return false;
    }

    #endregion

    #region properties

    #endregion

    #region public methods

    /// <summary>
    /// Disable inline editing of Caption
    /// 
    /// </summary>
    /// <returns>null</returns>
    public override string GetEditLabel()
    {
      return null; // TODO: we may want to rethink this.
    }

    /// <summary>
    /// After a successful rename of a project file this method is called to rename it in the DPR file.
    /// </summary>
    /// <param name="aOldFileName">full path of the old file</param>
    /// <param name="aNewFileName">full path of the new file</param>
    public virtual void RenameContainedFile(string aOldFileName, string aNewFileName)
    {
      const string FILE_Key = "{%File";
      try
      {
        TUnitScanner lScanner = new TUnitScanner(GetTextLines());
        lScanner.GotoNextToken(DelphiToken.EndStatement); // look for semicolen
        lScanner.EndToken = DelphiToken.EndStatement; // now before an semicolen look for comments
        while (lScanner.GotoNextToken(DelphiToken.ItemComment))
        {
          if (lScanner.TokenValue.IndexOf(FILE_Key, StringComparison.OrdinalIgnoreCase) == 0)
          {
            // save scanner values
            int lSaveStartIndex = lScanner.StartIndex;
            int lSaveStartLine = lScanner.StartLine;
            // move scanner to point inside comment
            lScanner.StartIndex -= (lScanner.TokenValue.Length - FILE_Key.Length);
            // scan for delphi string
            if (lScanner.GotoNextToken(DelphiToken.String))
            {
              string lFileName = TDelphiSource.DelphiStringToText(lScanner.TokenValue);
              // test to see if the filename is the same ones
              if (String.Compare(Path.GetFileName(lFileName), Path.GetFileName(aOldFileName), true) == 0)
              {
                string lNewFile = PackageUtilities.MakeRelativeIfRooted(aNewFileName, this.ProjectMgr.BaseURI);
                // move the index based on size of new file name
                lSaveStartIndex += lNewFile.Length - lFileName.Length;
                // replace just the string not the whole comment 
                ReplaceText(lScanner, TDelphiSource.TextToDelphiString(lNewFile));
              }
            }
            // move scanner back to where it should be
            lScanner.StartIndex = lSaveStartIndex;
            lScanner.StartLine = lSaveStartLine;
          }
        }
      }
      catch (Exception ex)
      {
        ShowErrorMessageBox(ex.Message, "RenameContainedFile() error");
      }
    }

    /// <summary>
    /// Contained files are files located in {%File filename} comments 
    /// </summary>
    /// <returns>returns a list of contained file names</returns>
    public string[] GetContainedFiles()
    {
      List<string> lResult = new List<string>();
      const string FILE_Key = "{%File";
      try
      {
        TUnitScanner lScanner = new TUnitScanner(GetTextLines());
        lScanner.GotoNextToken(DelphiToken.EndStatement); // scan past program, library or package 
        lScanner.EndToken = DelphiToken.EndStatement;
        while (lScanner.GotoNextToken(DelphiToken.ItemComment))
        {
          if (lScanner.TokenValue.IndexOf(FILE_Key, StringComparison.OrdinalIgnoreCase) == 0)
          {
            // save scanner values
            int lSaveStartIndex = lScanner.StartIndex;
            int lSaveStartLine = lScanner.StartLine;
            // move scanner to the end of the comment
            lScanner.StartIndex -= (lScanner.TokenValue.Length - FILE_Key.Length);
            // scan for delphi string
            if (lScanner.GotoNextToken(DelphiToken.String))
            {
              // Get the file as printed in the DPR
              lResult.Add(TDelphiSource.DelphiStringToText(lScanner.TokenValue));
            }

            // move scanner back to where it should be
            lScanner.StartIndex = lSaveStartIndex;
            lScanner.StartLine = lSaveStartLine;
          }
        }
      }
      catch (Exception ex)
      {
        ShowErrorMessageBox(ex.Message, "GetContainedFiles() Error");
      }
      return lResult.ToArray();
    }



    /// <summary>
    /// Gets a list of code files that belong to the main file
    /// this must be overriden by inherited call and call be made to
    /// GetUnitFilesAfterKeyword
    /// </summary>
    /// <returns></returns>
    public virtual string[] GetUnitFiles()
    {
      return new List<string>().ToArray();
    }


    #endregion

  }
}
