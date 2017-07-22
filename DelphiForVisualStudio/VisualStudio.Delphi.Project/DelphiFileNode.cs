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
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Package.Automation;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using VisualStudio.Delphi.Language;
using IEditPoint = EnvDTE.EditPoint;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

namespace VisualStudio.Delphi.Project
{
  public delegate void AfterDelphiUnitRename(object sender, string aOldUnitName, string aNewUnitName);
  public delegate void AfterFileRename(DelphiFileNode aDelphiFileNode, string aOldUnitName, string aNewUnitName);

  public class DelphiFileNode : FileNode
  {

    #region fields
    private OAVSProjectItem vsProjectItem;
    private SelectionElementValueChangedListener selectionChangedListener;
    private AfterDelphiUnitRename FOnAfterDelphiUnitRename;
    private AfterFileRename FOnAfterFileRename;

    // private DelphiFileItemAutomation automationObject;
    #endregion

    #region ctors
    internal DelphiFileNode(ProjectNode root, ProjectElement e)
      : base(root, e)
    {
      selectionChangedListener = new SelectionElementValueChangedListener(new ServiceProvider((IOleServiceProvider)root.GetService(typeof(IOleServiceProvider))), root);
      selectionChangedListener.Init();

    }
    #endregion

    #region properties

    /// <summary>
    /// This event occures after a successfull rename of a unit in the editor.
    /// </summary>
    public event AfterDelphiUnitRename OnAfterDelphiUnitRename
    {
      add { FOnAfterDelphiUnitRename += value; }
      remove { FOnAfterDelphiUnitRename -= value; }
    }

    /// <summary>
    /// This event occurs after a successfull rename of the file. 
    /// </summary>
    public event AfterFileRename OnAfterFileRename
    {
      add { FOnAfterFileRename += value; }
      remove { FOnAfterFileRename -= value; }
    }


    /// <summary>
    /// Returns the SubType of an Delphi File that has a "MainSource" ,"Form:Form1", or "DataModule:DataModule1" 
    /// </summary>
    public string SubType
    {
      get
      {
        return this.ItemNode.GetMetadata(ProjectFileConstants.SubType);
      }
      set
      {
        this.ItemNode.SetMetadata(ProjectFileConstants.SubType, value);
      }
    }

    protected internal VSLangProj.VSProjectItem VSProjectItem
    {
      get
      {
        if (null == this.vsProjectItem)
        {
          this.vsProjectItem = new OAVSProjectItem(this);
        }
        return this.vsProjectItem;
      }
    }
    #endregion

    #region overridden properties

    internal override object Object
    {
      get
      {
        return this.VSProjectItem;
      }
    }
    #endregion

    #region overridden methods
    protected override NodeProperties CreatePropertiesObject()
    {
      DelphiFileNodeProprties properties = new DelphiFileNodeProprties(this);
      properties.OnCustomToolChanged += new EventHandler<HierarchyNodeEventArgs>(OnCustomToolChanged);
      properties.OnCustomToolNameSpaceChanged += new EventHandler<HierarchyNodeEventArgs>(OnCustomToolNameSpaceChanged);
      return properties;
    }

    public override int Close()
    {
      if (selectionChangedListener != null)
        selectionChangedListener.Dispose();
      return base.Close();
    }

    /// <summary>
    /// Returs an Delphi FileNode specific object implmenting DTE.ProjectItem
    /// </summary>
    /// <returns></returns>
    public override object GetAutomationObject()
    {
      //if (null == automationObject)
      //{
      //  automationObject = new OADelphiFileItem(this.ProjectMgr.GetAutomationObject() as OAProject, this);
      //}
      //return automationObject;
      return base.GetAutomationObject();
    }

    //public override int ImageIndex
    //{
    //  get
    //  {
    //    if (IsFormSubType)
    //    {
    //      return (int)ProjectNode.ImageName.WindowsForm;
    //    }
    //    if (this.FileName.ToLower().EndsWith(".py"))
    //    {
    //      return PythonProjectNode.ImageOffset + (int)PythonProjectNode.PythonImageName.PyFile;
    //    }
    //    return base.ImageIndex;
    //  }
    //}

    ///// <summary>
    ///// Open a file depending on the SubType property associated with the file item in the project file
    ///// </summary>
    //protected override void DoDefaultAction()
    //{
    //  FileDocumentManager manager = this.GetDocumentManager() as FileDocumentManager;
    //  Debug.Assert(manager != null, "Could not get the FileDocumentManager");

    //  Guid viewGuid = (IsFormSubType ? NativeMethods.LOGVIEWID_Designer : NativeMethods.LOGVIEWID_Code);
    //  IVsWindowFrame frame;
    //  manager.Open(false, false, viewGuid, out frame, WindowFrameShowAction.Show);
    //}

    //protected internal override StringBuilder PrepareSelectedNodesForClipBoard()
    //{
    //  DelphiProjectNode lProjectMgr = this.ProjectMgr as DelphiProjectNode;
    //  Debug.Assert(lProjectMgr != null, " No project mananager available for this node " + ToString());
    //  Debug.Assert(lProjectMgr.GetItemsDraggedOrCutOrCopied() != null, " The itemsdragged list should have been initialized prior calling this method");
    //  List<IVsHierarchy> lItemsDragged = lProjectMgr.GetItemsDraggedOrCutOrCopied();
    //  StringBuilder sb = new StringBuilder();
    //  string projref = String.Empty;
    //  IVsSolution solution = this.GetService(typeof(IVsSolution)) as IVsSolution;
    //  bool lFailed = (this.ID == VSConstants.VSITEMID_ROOT || solution == null);
    //  if (!lFailed)
    //  {
    //    // Get all nodes for drag and drop or cut copy
    //    HierarchyNode lNode = this;
    //    do
    //    {
    //      lItemsDragged.Add(lNode);
    //      // file must exist or no dragging can occur
    //      if (!File.Exists(lNode.Url))
    //        lFailed = true;
    //      ErrorHandler.ThrowOnFailure(solution.GetProjrefOfItem(this.ProjectMgr, lNode.ID, out projref));
    //      if (!String.IsNullOrEmpty(projref))
    //      {
    //        sb.Append(projref);
    //        sb.Append('\0');
    //      }
    //      else
    //        lFailed = true;
    //      if (lNode is DelphiFileNode)
    //        lNode = lNode.FirstChild;
    //      else
    //        lNode = lNode.NextSibling;
    //    } while (lNode != null);
    //  }
    //  // it did not work
    //  if (lFailed) 
    //  {
    //    lItemsDragged.Clear();// abort
    //    sb = new StringBuilder();
    //  }
    //  return sb;
    //}

    protected override int ExecCommandOnNode(Guid guidCmdGroup, uint cmd, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      if (guidCmdGroup == Microsoft.VisualStudio.Shell.VsMenus.guidStandardCommandSet97)
      {
        switch ((VsCommands)cmd)
        {
          case VsCommands.Delete:
            HierarchyNode lNode = this.FirstChild;
            while (lNode != null)
            {
              lNode.Remove(true);
              lNode = this.FirstChild;
            }
            this.Remove(true);
            return VSConstants.S_OK;
        }
      }
      //Debug.Assert(this.ProjectMgr != null, "The PythonFileNode has no project manager");

      //if (this.ProjectMgr == null)
      //{
      //  throw new InvalidOperationException();
      //}

      //if (guidCmdGroup == PythonMenus.guidIronPythonProjectCmdSet)
      //{
      //  if (cmd == (uint)PythonMenus.SetAsMain.ID)
      //  {
      //    // Set the MainFile project property to the Filename of this Node
      //    ((PythonProjectNode)this.ProjectMgr).SetProjectProperty(PythonProjectFileConstants.MainFile, this.GetRelativePath());
      //    return VSConstants.S_OK;
      //  }
      //}
      return base.ExecCommandOnNode(guidCmdGroup, cmd, nCmdexecopt, pvaIn, pvaOut);
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
          case VsCommands.AddNewItem:
          case VsCommands.AddExistingItem:
          case VsCommands.Delete:
          case VsCommands.ViewCode:
            result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
            return VSConstants.S_OK;
          case VsCommands.Cut:
          case VsCommands.Paste:
          case VsCommands.Copy:
            result |= QueryStatusResult.NOTSUPPORTED;
            return VSConstants.S_OK;
          //case VsCommands.ViewForm:
          //  if (IsFormSubType)
          //    result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
          //  return VSConstants.S_OK;
        }
      }
      if (guidCmdGroup == Microsoft.VisualStudio.Shell.VsMenus.guidStandardCommandSet2K)
      {
        switch ((VsCommands2K)cmd)
        {
          case VsCommands2K.DELETE:
          case VsCommands2K.DELETEKEY:
            result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
            return VSConstants.S_OK;
        }
      }
      return base.QueryStatusOnNode(guidCmdGroup, cmd, pCmdText, ref result);
    }

    protected override FileNode RenameFileNode(string oldFileName, string newFileName, uint newParentId)
    {
      FileNode result = base.RenameFileNode(oldFileName, newFileName, newParentId);
      if (CanReanmeUnit(oldFileName,newFileName))
        this.RenameDelphiUnit(Path.GetFileNameWithoutExtension(oldFileName), Path.GetFileNameWithoutExtension(newFileName));
      DoAfterFileRename(oldFileName, newFileName);
      return result;
    }

    protected virtual void DoAfterFileRename(string aOldFileName, string aNewFileName)
    {
      if (FOnAfterFileRename != null)
        FOnAfterFileRename(this, aOldFileName, aNewFileName);
    }

    #endregion

    #region methods

    protected virtual bool CanReanmeUnit(string oldFileName, string newFileName)
    {
      return IsDelphiPasFile(oldFileName) && IsDelphiPasFile(newFileName);
    }

    /// <summary>
    /// Any file that belongs to delphi (pas,dfm,dpr,dpk,nfm)
    /// </summary>
    /// <param name="aFileName"></param>
    /// <returns></returns>
    public virtual bool IsDelphiFile(string aFileName)
    {
      string lExt = Path.GetExtension(aFileName).ToLower();
      return lExt == ".pas" || lExt == ".dfm" || lExt == ".dpr" || lExt == ".dpk" || lExt == ".nfm";
    }

    /// <summary>
    /// Is a file that has the extenstion .pas 
    /// </summary>
    /// <param name="aFileName"></param>
    /// <returns></returns>
    public bool IsDelphiPasFile(string aFileName)
    {
      return Path.GetExtension(aFileName).ToLower() == ".pas";
    }

    /// <summary>
    /// A code file is a file that has code that will be compiled
    /// </summary>
    /// <param name="aFileName"></param>
    /// <returns></returns>
    public virtual bool IsDelphiCodeFile(string aFileName)
    {
      string lExt = Path.GetExtension(aFileName).ToLower();
      return lExt == ".pas" || lExt == ".dpr" || lExt == ".dpk";
    }


    protected virtual void ShowErrorMessageBox(string aMessage, string aTitle)
    {
      OLEMSGICON lIcon = OLEMSGICON.OLEMSGICON_CRITICAL;
      OLEMSGBUTTON lButtons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
      OLEMSGDEFBUTTON lDefaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
      VsShellUtilities.ShowMessageBox(this.ProjectMgr.Site, aTitle, aMessage, lIcon, lButtons, lDefaultButton);
    }

    protected virtual IVsTextLines GetTextLines()
    {
      CCITracing.TraceCall();
      FileDocumentManager manager = this.GetDocumentManager() as FileDocumentManager;
      Debug.Assert(manager != null, "Could not get the FileDocumentManager");
      Guid logicalView = Guid.Empty;
      IVsWindowFrame windowFrame = null;
      object docData;
      VsTextBuffer lBuffer;
      IVsTextLines lLines;
      manager.Open(false, false, logicalView, out windowFrame, WindowFrameShowAction.DontShow);
      Debug.Assert(windowFrame != null, "Could not get the IVsWindowFrame");

      windowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocData, out docData);

      // Get the VsTextBuffer
      lBuffer = docData as VsTextBuffer;
      if (lBuffer == null)
      {
        IVsTextBufferProvider lBufferProvider = docData as IVsTextBufferProvider;
        if (lBufferProvider != null)
          NativeMethods.ThrowOnFailure(lBufferProvider.GetTextBuffer(out lLines));
        else
          throw new Exception("Could get TextLines object.");
      }
      else
        lLines = lBuffer as IVsTextLines;
      return lLines;
    }

    protected virtual void DoAfterDelphiUnitRename(string aOldUnitName, string aNewUnitName)
    {
      if (FOnAfterDelphiUnitRename != null)
        FOnAfterDelphiUnitRename(this, aOldUnitName, aNewUnitName);
    }

    /// <summary>
    /// Renames all unit names found in Uses and Unit clauses.
    /// </summary>
    /// <param name="aOldName"></param>
    /// <param name="aNewName"></param>
    public virtual void RenameDelphiUnit(string aOldUnitName, string aNewUnitName)
    {
      try
      {
        IVsTextLines lLines = GetTextLines();

        if (lLines != null)
          if (RenameUnit(DelphiUnitType.Unit, aOldUnitName, aNewUnitName, lLines))
          {
            DoAfterDelphiUnitRename(aOldUnitName, aNewUnitName);
          }
          else
            RenameUsesUnit(aOldUnitName, aNewUnitName, lLines);
      }
      catch (Exception ex)
      {
        ShowErrorMessageBox(ex.Message, "Unit Rename Error");
      }
    }
   
    /// <summary>
    /// Reads the {$R filename} or {$I filename} compiler definintions and returns a list of filenames with there wild card converted.
    /// </summary>
    /// <returns>list of resource files located in the file.</returns>
    public virtual string[] GetDirectiveFiles()
    {
      // This is a bad hack but oh well.
      List<string> lResult = new List<string>();
      string lResKey = "{$RESOURCE";
      string lIncKey = "{$INCLUDE";
      if (IsDelphiCodeFile(this.FileName))
      {
        try
        {
          TUnitScanner lScanner = new TUnitScanner(GetTextLines());
          lScanner.EndToken = DelphiToken.None;
          while (lScanner.GotoNextToken(DelphiToken.CompilerDirective))
          {
            if (lScanner.TokenValue.Substring(0, 3) == lResKey.Substring(0, 3) ||
                lScanner.TokenValue.Substring(0, 3) == lIncKey.Substring(0, 3))
            {
              int i = lScanner.TokenValue.Length - 1;
              char lChar;
              do
              {
                lChar = lScanner.TokenValue[i];
                i--;
              } while (lChar == '}' || lChar == ' ');
              StringBuilder sb = new StringBuilder();
              // if {$R-} or {$R+} directive found go to next directive
              if (lChar == '-' || lChar == '+') continue;

              // get filename out
              do
              {
                sb.Insert(0, lChar);
                lChar = lScanner.TokenValue[i];
                i--;
              } while (lChar != ' ');
              string lFileName = sb.ToString();
              lFileName = lFileName.Replace("*", Path.GetFileNameWithoutExtension(this.FileName));
              if (lFileName[0] == '\'')
                lFileName = TDelphiSource.DelphiStringToText(lFileName);
              //if (aWithAbsolutePathFlag)
              //{
              //  lFileName = Path.Combine(Path.GetDirectoryName(this.Url), lFileName);
              //  // get rid of .\ or ..\
              //  lFileName = Path.GetFullPath(lFileName);
              //}
              lResult.Add(lFileName);
            }
          }
        }
        catch (Exception ex)
        {
          ShowErrorMessageBox(ex.Message, "GetUnitFiles() Error");
        }
      }
      return lResult.ToArray();
    }


    public enum DelphiUnitType
    {
      Program = (int)DelphiToken.programKeyword,
      Package = (int)DelphiToken.packageKeyword,
      Library = (int)DelphiToken.libraryKeyword,
      Unit = (int)DelphiToken.unitKeyword
    };
    /// <summary>
    /// Looks for "Unit UnitName;" and renames the unit name.
    /// </summary>
    /// <param name="aOldUnitName"></param>
    /// Name of the old unit name this name must exist in the file or the method returns false
    /// <param name="aNewUnitName"></param>
    /// New name of the unit
    /// <param name="aVsTextLines"></param>
    /// Editor lines to modify
    /// <returns></returns>
    protected virtual bool RenameUnit(DelphiUnitType aUnitType, string aOldUnitName, string aNewUnitName, IVsTextLines aVsTextLines)
    {
      TUnitScanner lScanner = new TUnitScanner(aVsTextLines);
      bool lResult = false;
      // any data found before Program,Package,Library,Unit is not valid
      lScanner.EndToken = DelphiToken.EndStatement;
      if (lScanner.GotoNextToken((DelphiToken)aUnitType))
      {
        if (lScanner.GotoNextToken(DelphiToken.Identifier))
        {
          if (String.Compare(lScanner.TokenValue, aOldUnitName, true) == 0)
          {
            ReplaceText(lScanner, aNewUnitName);
            lResult = true;
          }
        }
      }
      return lResult;
    }

    /// <summary>
    /// Renames unit names found after the a keyword and before semicolen (;)
    /// </summary>
    /// <param name="aKeywordToken"></param>
    /// <param name="aOldUnitName"></param>
    /// <param name="aNewUnitName"></param>
    /// <param name="aVsTextLines"></param>
    /// <returns></returns>
    protected virtual bool RenameUnitAfterKeyword(DelphiToken aKeywordToken, string aOldUnitName, string aNewUnitName, IVsTextLines aVsTextLines)
    {
      TUnitScanner lScanner = new TUnitScanner(aVsTextLines);
      bool lResult = false;
      // any data found before Program,Package,Library,Unit is not valid
      lScanner.EndToken = DelphiToken.None;
      while (lScanner.GotoNextToken(aKeywordToken))
      {
        lScanner.EndToken = DelphiToken.EndStatement;
        while (lScanner.GotoNextToken(DelphiToken.Identifier))
        {
          if (String.Compare(lScanner.TokenValue, aOldUnitName, true) == 0)
          {
            ReplaceText(lScanner, aNewUnitName);
            AfterRenameUnitName(lScanner.TokenValue, aNewUnitName, lScanner); // for project files
            lResult = true;
            //
            // NOTE: We keep looking just in case there are compiler directives.
            // 
            lScanner.EndToken = DelphiToken.EndStatement;
          }
        }
        lScanner.EndToken = DelphiToken.None;
        //
        // NOTE: Look for all uses clases just in case the are in compiler directives.
        // 
      }
      return lResult;
    }

    /// <summary>
    /// Renames a unit found after the keyword "uses" and before the first (;)
    /// </summary>
    /// <param name="aOldUnitName">The old unit name without file extention</param>
    /// <param name="aNewUnitName">The new unit name without file extention</param>
    /// <param name="aVsTextLines"></param>
    /// <returns></returns>
    protected virtual bool RenameUsesUnit(string aOldUnitName, string aNewUnitName, IVsTextLines aVsTextLines)
    {
      return RenameUnitAfterKeyword(DelphiToken.usesKeyword, aOldUnitName, aNewUnitName, aVsTextLines);
    }

    /// <summary>
    /// Replaces the text at the scanned location.  After using a TUnitScanner to find some text you may want to replace
    /// that text with new data.
    /// </summary>
    /// <param name="aScanner">A scanner at the Token that will be replaced</param>
    /// <param name="aNewText">The text that will be replaced.</param>
    protected void ReplaceText(TUnitScanner aScanner, string aNewText)
    {
      aScanner.TextLines.LockBuffer();
      try
      {
        object lObj;
        NativeMethods.ThrowOnFailure(aScanner.TextLines.CreateEditPoint(aScanner.StartLine, aScanner.StartIndex - aScanner.TokenValue.Length, out lObj));
        IEditPoint lEditPoint = lObj as IEditPoint;
        if (lEditPoint != null)
        {
          lEditPoint.Delete(aScanner.TokenValue.Length);
          lEditPoint.Insert(aNewText);
        }
      }
      finally
      {
        aScanner.TextLines.UnlockBuffer();
      }
    }

    /// <summary>
    /// This method is so that project files can rename the file name as well.
    /// For example in the following line the unit1.pas file name must be renamed.
    /// <code>
    /// Unit1 in 'Unit1.pas', {Form} 
    /// </code>
    /// Since .PAS files do not follow this format it is not coded here.
    /// </summary>
    /// <param name="aScanner">A scanner at the point where the unit was found.</param>
    protected virtual void AfterRenameUnitName(string aOldUnitName, string aNewUnitName,TUnitScanner aScanner)
    {
    }

    protected virtual bool FindKeyword(string aKeyword, int aStartLine, IVsTextLines aVsTextLines, TUnitScanner aUnitScanner, out int aFindLine, out int aFindIndex)
    {
      aFindIndex = -1;
      aFindLine = -1;
      return false;
    }

    public string GetRelativePath()
    {
      string relativePath = Path.GetFileName(this.ItemNode.GetMetadata(ProjectFileConstants.Include));
      HierarchyNode parent = this.Parent;
      while (parent != null && !(parent is ProjectNode))
      {
        relativePath = Path.Combine(parent.Caption, relativePath);
        parent = parent.Parent;
      }
      return relativePath;
    }

    internal OleServiceProvider.ServiceCreatorCallback ServiceCreator
    {
      get { return new OleServiceProvider.ServiceCreatorCallback(this.CreateServices); }
    }

    private object CreateServices(Type serviceType)
    {
      object service = null;
      if (typeof(EnvDTE.ProjectItem) == serviceType)
      {
        service = GetAutomationObject();
      }
      return service;
    }
    #endregion
  }

  public class SelectionElementValueChangedListener : SelectionListener
  {
    #region fileds
    private ProjectNode projMgr;
    #endregion
    #region ctors
    public SelectionElementValueChangedListener(ServiceProvider serviceProvider, ProjectNode proj)
      : base(serviceProvider)
    {
      projMgr = proj;
    }
    #endregion

    #region overridden methods
    public override int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
    {
      int hr = VSConstants.S_OK;
      if (elementid == VSConstants.DocumentFrame)
      {

        IVsWindowFrame pWindowFrame = varValueOld as IVsWindowFrame;
        if (pWindowFrame != null)
        {
          object document;
          // Get the name of the document associated with the old window frame
          hr = pWindowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_pszMkDocument, out document);
          if (ErrorHandler.Succeeded(hr))
          {
            uint itemid;
            IVsHierarchy hier = projMgr as IVsHierarchy;
            hr = hier.ParseCanonicalName((string)document, out itemid);
            DelphiFileNode node = projMgr.NodeFromItemId(itemid) as DelphiFileNode;
            if (null != node && node.NodeProperties is SingleFileGeneratorNodeProperties)
            {
              node.RunGenerator();
            }
          }
        }
      }

      return hr;
    }
    #endregion

  }

}
