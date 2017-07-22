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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace VisualStudio.Delphi.Tasks
{
  /// <summary>
  /// MSBuild is not robust enough to accomplish all testing need to be don
  /// on the project file to ensure that all data is correct.  Simple features
  /// like count of the ITaskItem is just not availabe in the MSBuild world
  /// this task will do all nessary tasks need to insure that all data in the
  /// project file is correct and prep the data for compiling.
  /// </summary>
  public class DelphiProjectProcessor : Task
  {
    private ITaskItem[] FCompile;
    private ITaskItem[] FCompileDelphiResource;
    private ITaskItem[] FEmbeddedResource;
    private bool FDelphiAutoCreateResource = false;
    private string FProjectName = "";
    private int FMainSourceCount;
    private string FProjectFileName = "";

    [Required]
    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Provide Compile Item Group.")]
    public ITaskItem[] Compile
    {
      get { return FCompile; }
      set { FCompile = value; }
    }


    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Provide CompileDelphiResource Item Group.")]
    public ITaskItem[] CompileDelphiResource
    {
      get { return FCompileDelphiResource; }
      set { FCompileDelphiResource = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Provide EmbeddedResource Item Group.")]
    public ITaskItem[] EmbeddedResource
    {
      get { return FEmbeddedResource; }
      set { FEmbeddedResource = value; }
    }

    [Output]
    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Outputs if a resource file should be created.")]
    public bool AutoCreateResource
    {
      get { return FDelphiAutoCreateResource; }
      set { FDelphiAutoCreateResource = value; }
    }

    [Output]
    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Delphi project name with out the extention.", DefineProperty = true, PropertyName = "DelphiProjectName")]
    public string ProjectName
    {
      get { return FProjectName; }
    }

    [Output]
    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Total number of main source files found in Compile item group.")]
    public string MainSourceCount
    {
      get { return FMainSourceCount.ToString(); }
    }

    [Output]
    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Name of the Delphi main source file with out the extention.")]
    public string ProjectFileName
    {
      get { return FProjectFileName; }
    }

    //[Output]
    //[VisualStudio.MSBuild.XSD.TaskParameterDefinition("Returns a list of resource source files that need to be compiled (*.rc).")]
    //public ITaskItem[] ResourceSourceFiles
    //{
    //  get { return FResourceSourceFiles; }
    //}
 


    public override bool Execute()
    {
      FindMainSourceFile();
      ResolveResourceFiles();
      return true;
    }

    private void ResolveResourceFiles()
    {
      if (FProjectName == "") return;
      if (this.CompileDelphiResource != null)
        foreach (ITaskItem lItem in CompileDelphiResource)
          if (Path.GetExtension(lItem.ItemSpec).ToLower() == ".rc")
          {
            if (String.Compare(ProjectName + ".rc", Path.GetFileName(lItem.ItemSpec), true) == 0)
              FDelphiAutoCreateResource = false;
          }
      if (this.EmbeddedResource != null)
        foreach (ITaskItem lItem in this.EmbeddedResource)
          if (Path.GetExtension(lItem.ItemSpec).ToLower() == ".res")
          {
            if (String.Compare(ProjectName + ".res", Path.GetFileName(lItem.ItemSpec), true) == 0)
              FDelphiAutoCreateResource = false;
          }
    }

    private void FindMainSourceFile()
    {
      FMainSourceCount = 0;

      foreach (ITaskItem lItem in Compile)
        if (lItem.GetMetadata("SubType") == "MainSource")
        {
          FMainSourceCount++;
          FProjectFileName = lItem.ItemSpec;
          FProjectName = Path.GetFileNameWithoutExtension(FProjectFileName);
        }
    }
  }
}