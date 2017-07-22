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
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Package;


namespace VisualStudio.Delphi.Project
{

  [ComVisible(true), CLSCompliant(false), System.Runtime.InteropServices.ClassInterface(ClassInterfaceType.AutoDual)]
  [Guid(Constants.DelphiProjectPropertiesGUID)]
  public class DelphiProjectNodeProperties : ProjectNodeProperties
  {
    #region ctors
    public DelphiProjectNodeProperties(ProjectNode node)
      : base(node)
    {
    }
    #endregion

    #region properties

    #endregion
  }

  [ComVisible(true), CLSCompliant(false), System.Runtime.InteropServices.ClassInterface(ClassInterfaceType.AutoDual)]
  [Guid(Constants.DelphiFileNodeProprtiesGUID)]
  public class DelphiFileNodeProprties : SingleFileGeneratorNodeProperties
  {
    #region ctors
    public DelphiFileNodeProprties(HierarchyNode node)
			: base(node)
		{
		}
    #endregion

    #region properties
    #endregion
  }

  [ComVisible(true), CLSCompliant(false), System.Runtime.InteropServices.ClassInterface(ClassInterfaceType.AutoDual)]
  [Guid(Constants.DelphiDependentFileNodeProprtiesGUID)]
  public class DelphiDependentFileNodeProprties : DelphiFileNodeProprties
  {
    #region ctors
    public DelphiDependentFileNodeProprties(HierarchyNode node)
      : base(node)
    {
    }
    #endregion

    #region properties

    [SRCategoryAttribute(SR.Misc)]
    [LocDisplayName(SR.FileName)]
    [SRDescriptionAttribute(SR.FileNameDescription)]
    [ReadOnly(true)]
    public override string FileName
    {
      get
      {
         return base.FileName;
      }
      set
      {
        base.FileName = value;
      }
    }

    //[SRCategoryAttribute(SR.Advanced)]
    //[LocDisplayName(SR.BuildAction)]
    //[SRDescriptionAttribute(SR.BuildActionDescription)]
    //public virtual Microsoft.VisualStudio.Package.BuildAction BuildAction
    //{
    //  get
    //  {
    //    Debug.Assert(this.Node != null, "The associated hierarchy node has not been initialized");
    //    string value = this.Node.ItemNode.ItemName;
    //    if (value == null || value.Length == 0)
    //      return Microsoft.VisualStudio.Package.BuildAction.None;
    //    return (Microsoft.VisualStudio.Package.BuildAction)Enum.Parse(typeof(Microsoft.VisualStudio.Package.BuildAction), value);
    //  }
    //  set
    //  {
    //    Debug.Assert(this.Node != null, "The associated hierarchy node has not been initialized");

    //    this.Node.ItemNode.ItemName = value.ToString();
    //  }
    //}

    //[SRCategoryAttribute(SR.Misc)]
    //[LocDisplayName(SR.FullPath)]
    //[SRDescriptionAttribute(SR.FullPathDescription)]
    //public string FullPath
    //{
    //  get
    //  {
    //    Debug.Assert(this.Node != null, "The associated hierarchy node has not been initialized");

    //    return this.Node.Url;
    //  }
    //}

    //#region non-browsable properties - used for automation only
    //[Browsable(false)]
    //public string Extension
    //{
    //  get
    //  {
    //    Debug.Assert(this.Node != null, "The associated hierarchy node has not been initialized");

    //    return Path.GetExtension(this.Node.Caption);
    //  }
    //}
    //#endregion

    //#endregion

    //#region overridden methods
    //public override string GetClassName()
    //{
    //  return Resources.DependentFileNodeProperties;
    //}
    #endregion


  }
}
