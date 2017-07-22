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
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using Microsoft.Build.CommandLine;
using System.IO;
using VisualStudio.Delphi.Tasks.DCC32;
using VisualStudio.Delphi.Tasks;

namespace Test
{

  [TestFixture]
  public class DCC32QuickTestClass
  {
    private const string DefaultProject = "BRCC32V5_4Test.proj";

    [Test]
    public void TestNoMainSourceError()
    {
      string lOutput = RunProject(DefaultProject, "/p:RemoveMainSource=true", false);
      Assert.IsTrue(lOutput.Contains("There is no .DPR or .DPK file marked as the MainSource file for SubType property"), "The error message for no MainSource file is misssing.");
    }

    [Test]
    public void TestMultiMainSourceError()
    {
      string lOutput = RunProject(DefaultProject, "/p:MultiMainSource=true", false);
      Assert.IsTrue(lOutput.Contains("There are more than one MainSource files."), "The error message for no MainSource file is misssing.");
    }

    [Test]
    public void TestDelphiAutoCreateResource()
    {
      string lOutput = RunProject(DefaultProject, "/p:DelphiAutoCreateResource=true");
      Assert.IsTrue(lOutput.Contains("Compiled resource files : "), "resouce not compiling ");
    }

    [Test]
    public void TestWithSecondResource()
    {
      string lOutput = RunProject(DefaultProject, "/p:DelphiAutoCreateResource=true;TestWithSecondResource=true");
      Assert.IsTrue(lOutput.Contains("Compiled resource files : TEST.rc;BRCC32V5_4Test.rc"), "resouce not compiling ");
    }


    public DCC32QuickTestClass()
    {
      string lBinDir = System.IO.Directory.GetCurrentDirectory() + "\\";
      System.Environment.SetEnvironmentVariable("VisualStudioForDelphiPath", lBinDir, EnvironmentVariableTarget.Process);
      // copy all *.task and *.targets to the current folder
      string lNetPath = Environment.ExpandEnvironmentVariables(@"%windir%\Microsoft.NET\Framework\v2.0.50727\");
      string[] lTargets = Directory.GetFiles(lNetPath, "*.Targets", SearchOption.TopDirectoryOnly);
      string[] lTasks = Directory.GetFiles(lNetPath, "*.Tasks", SearchOption.TopDirectoryOnly);
      foreach (string lFile in lTargets)
      {
        string lDestFile = Path.Combine(lBinDir, Path.GetFileName(lFile));
        if (!File.Exists(lDestFile))
          File.Copy(lFile, lDestFile);
      }
      foreach (string lFile in lTasks)
      {
        string lDestFile = Path.Combine(lBinDir, Path.GetFileName(lFile));
        if (!File.Exists(lDestFile))
          File.Copy(lFile, lDestFile);
      }
    }


    class MyTextWriter : TextWriter
    {
      StringBuilder FSB = new StringBuilder();
      public MyTextWriter()
        : base()
      { }

      public override Encoding Encoding
      {
        get
        {
          return System.Text.Encoding.Default;
        }
      }

      public override void Write(string value)
      {
        FSB.Append(value);
      }

      public override void WriteLine(string value)
      {
        FSB.AppendLine(value);
      }

      public string Text
      {
        get { return FSB.ToString(); }
      }

    }

    private string RunProject(string aProjectFile, string aParams)
    {
      return RunProject(aProjectFile, aParams, true);
    }

    private string RunProject(string aProjectFile, string aParams,bool aSuccessFullBuild)
    {
      // create a writer to save command line output
      MyTextWriter lMyWriter = new MyTextWriter();
      // set it so the output goes to my writer
      Console.SetOut(lMyWriter);
      // get current dir
      string lBinDir = System.IO.Directory.GetCurrentDirectory();
      // create full path to project
      string lProjectFile = System.IO.Directory.GetCurrentDirectory() + @"\Project\" + aProjectFile;

      string lProjPath = Path.GetDirectoryName(lProjectFile);
      // change dir to that project
      Directory.SetCurrentDirectory(lProjPath);
      string lproj = Directory.GetCurrentDirectory();
      if (lproj == "") throw new Exception();
      MSBuildApp.ExitType lExitType;
      try
      {
        // use MSBuild class found in MSBuild.EXE assembly 
        lExitType = MSBuildApp.Execute("file "+ lProjectFile + " " + aParams);
      }
      finally
      {
        Directory.SetCurrentDirectory(lBinDir);
      }
      // get the text of the MSBuild execution
      string s = lMyWriter.Text;
      // save it to a file in the folder
      using (StreamWriter lWriter = new StreamWriter(lBinDir + "\\MSBuild.log"))
        lWriter.Write(s);
      // check to see if there was any errors.
      if (aSuccessFullBuild)
        if (lExitType != MSBuildApp.ExitType.Success)
          throw new Exception(s);
      return s;
    }

  }
}