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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;


namespace VisualStudio.Delphi.Project
{

  public class DelpheFolderNode : FolderNode
  {

    #region ctors
    public DelpheFolderNode(ProjectNode root, string strDirectoryPath, ProjectElement e)
      : base(root,strDirectoryPath, e)
    {

    }
    #endregion

    protected override int QueryStatusOnNode(Guid guidCmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result)
    {
      if (guidCmdGroup == Microsoft.VisualStudio.Shell.VsMenus.guidStandardCommandSet97)
        switch ((VsCommands)cmd)
        {
          case VsCommands.Delete:
          case VsCommands.DeleteQuery:
            result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
            return VSConstants.S_OK;
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
      if (guidCmdGroup == Microsoft.VisualStudio.Shell.VsMenus.guidStandardCommandSet2K)
      {
        switch ((VsCommands2K)cmd)
        {
          case VsCommands2K.DELETE:
          case VsCommands2K.DELETEKEY:
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
      return base.ExecCommandOnNode(guidCmdGroup, cmd, nCmdexecopt, pvaIn, pvaOut);
    }

  }
}