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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;


namespace VisualStudio.Delphi.Project
{
  [GuidAttribute(Constants.DelphiDependentFileNodeGUID)]
  class DelphiDependentFileNode : DependentFileNode
  {

    #region ctor
    /// <summary>
    /// Constructor for the DependentFileNode
    /// </summary>
    /// <param name="root">Root of the hierarchy</param>
    /// <param name="e">Associated project element</param>
    public DelphiDependentFileNode(ProjectNode root, ProjectElement e)
      : base(root, e)
    {
      this.HasParentNodeNameRelation = true;
    }

    #endregion

    #region overridden methods
    protected override NodeProperties CreatePropertiesObject()
    {
      DelphiDependentFileNodeProprties properties = new DelphiDependentFileNodeProprties(this);
      return properties;
    }
    /// <summary>
    /// Disable rename
    /// </summary>
    /// <param name="label">new label</param>
    /// <returns>E_NOTIMPLE in order to tell the call that we do not support rename</returns>
    public override string GetEditLabel()
    {
      return null;
    }

    /// <summary>
    /// Gets a handle to the icon that should be set for this node
    /// </summary>
    /// <param name="open">Whether the folder is open, ignored here.</param>
    /// <returns>Handle to icon for the node</returns>
    public override object GetIconHandle(bool open)
    {
      return this.ProjectMgr.ImageHandler.GetIconHandle(this.ImageIndex);
    }

    /// <summary>
    /// Disable certain commands for dependent file nodes 
    /// </summary>
    protected override int QueryStatusOnNode(Guid guidCmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result)
    {
      if (guidCmdGroup == VsMenus.guidStandardCommandSet97)
      {
        switch ((VsCommands)cmd)
        {
          case VsCommands.Copy:
          case VsCommands.Paste:
          case VsCommands.Cut:
          case VsCommands.Delete:
          case VsCommands.Rename:
            result |= QueryStatusResult.NOTSUPPORTED;
            return VSConstants.S_OK;
        
          case VsCommands.ViewCode:
          //case VsCommands.Delete: goto case VsCommands.OpenWith;
          case VsCommands.Open:
          case VsCommands.OpenWith:
            result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
            return VSConstants.S_OK;
        }
      }
      else if (guidCmdGroup == VsMenus.guidStandardCommandSet2K)
      {
        if ((VsCommands2K)cmd == VsCommands2K.EXCLUDEFROMPROJECT)
        {
          result |= QueryStatusResult.NOTSUPPORTED;
          return VSConstants.S_OK;
        }
      }
      else
      {
        return (int)OleConstants.OLECMDERR_E_UNKNOWNGROUP;
      }
      return base.QueryStatusOnNode(guidCmdGroup, cmd, pCmdText, ref result);
    }

    ///// <summary>
    ///// DependentFileNodes node cannot be dragged.
    ///// </summary>
    ///// <returns>null</returns>
    //protected internal override StringBuilder PrepareSelectedNodesForClipBoard()
    //{
    //  return null;
    //}

    #endregion

  }
}
