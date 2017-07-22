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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell.Interop;

namespace VisualStudio.Delphi.Project
{
  public class CompilerTypesConverter : DynamicEnumConverter
  {
    public CompilerTypesConverter()
      : base(typeof(CompilerTypes))
    { }
  }

  [PropertyPageTypeConverterAttribute(typeof(CompilerTypesConverter))]
  public class CompilerTypes : DynamicEnum
  {
    private List<string> FCompilers = null;

    public CompilerTypes()
      : base(typeof(CompilerTypes)) // default type
    { }

    protected override IEnumerable<string> GetEnumerable()
    {
      if (FCompilers == null)
      {
        FCompilers = new List<string>();
        FCompilers.Add("None");
        // TODO: I need to go get the compilers list.
        FCompilers.Add("DCC32");
        FCompilers.Add("DCCIL");
      }
      return FCompilers;
    }
  }

  [ComVisible(true), Guid("8C03A5C9-AF51-4a9f-99F6-E7AA1CE549FE")]
  public class DelphiBuildPropertyPage : BuildPropertyPage
  {
  }

  [ComVisible(true), Guid("F2252848-8C39-4115-BE74-30948537BAAD")]
  class DelphiPropertyPage : SettingsPage, EnvDTE80.IInternalExtenderProvider
  {
    private CompilerTypes FCompilerTypes = new CompilerTypes();

    public DelphiPropertyPage()
    {
      this.Name = "Compiler";
    }

    public override string GetClassName()
    {
      return this.GetType().FullName;
    }

    #region exposed properties

    [Category("Compiler")]
    [DisplayName("CompilerVersion")]
    [Description("CompilerVersionDescription")]
    public CompilerTypes CompilerVersion
    {
      get { return this.FCompilerTypes; }
      set { this.FCompilerTypes = value; this.IsDirty = true; }
    }

    #endregion 

    #region IInternalExtenderProvider Members

    bool EnvDTE80.IInternalExtenderProvider.CanExtend(string extenderCATID, string extenderName, object extendeeObject)
    {
      IVsHierarchy lVsHierarchy = HierarchyNode.GetOuterHierarchy(this.ProjectMgr);
      if (lVsHierarchy is EnvDTE80.IInternalExtenderProvider)
        return ((EnvDTE80.IInternalExtenderProvider)lVsHierarchy).CanExtend(extenderCATID, extenderName, extendeeObject);
      return false;
    }

    object EnvDTE80.IInternalExtenderProvider.GetExtender(string extenderCATID, string extenderName, object extendeeObject, EnvDTE.IExtenderSite extenderSite, int cookie)
    {
      IVsHierarchy lVsHierarchy = HierarchyNode.GetOuterHierarchy(this.ProjectMgr);
      if (lVsHierarchy is EnvDTE80.IInternalExtenderProvider)
        return ((EnvDTE80.IInternalExtenderProvider)lVsHierarchy).GetExtender(extenderCATID, extenderName, extendeeObject, extenderSite, cookie);
      return null;
    }

    object EnvDTE80.IInternalExtenderProvider.GetExtenderNames(string extenderCATID, object extendeeObject)
    {
      IVsHierarchy lVsHierarchy = HierarchyNode.GetOuterHierarchy(this.ProjectMgr);
      if (lVsHierarchy is EnvDTE80.IInternalExtenderProvider)
        return ((EnvDTE80.IInternalExtenderProvider)lVsHierarchy).GetExtenderNames(extenderCATID, extendeeObject);
      return null;
    }
    #endregion

    /// <summary>
    /// Gets properties from the current project and saves them to this object
    /// </summary>
    protected override void BindProperties()
    {
      if (this.ProjectMgr == null)
      {
        Debug.Assert(false);
        return;
      }
      this.FCompilerTypes.EnumeratorItem = this.ProjectMgr.GetProjectProperty("DelphiCompiler", true);
    }

    protected override int ApplyChanges()
    {
      if (this.ProjectMgr == null)
      {
        Debug.Assert(false);
        return VSConstants.E_INVALIDARG;
      }
      this.ProjectMgr.SetProjectProperty("DelphiCompiler", this.FCompilerTypes.EnumeratorItem);

      this.IsDirty = false;

      return VSConstants.S_OK;
    }
  }
}
