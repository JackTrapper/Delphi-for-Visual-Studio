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

namespace VisualStudio.Delphi.Tasks.DCC32.V15_0.Test
{

  [TestFixture]
  public class DCC32TestClass
  {
    private const string DefaultProject = "DCC32.V15_0.proj";
    public DCC32TestClass()
    {
      string lBinDir = System.IO.Directory.GetCurrentDirectory() + "\\";
      System.Environment.SetEnvironmentVariable("VisualStudioForDelphiPath", lBinDir,EnvironmentVariableTarget.Process);
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
          return System.Text.Encoding.Default; }
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
        lExitType = MSBuildApp.Execute("dummyParam " + lProjectFile + " " + aParams);
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
      if (lExitType != MSBuildApp.ExitType.Success)
        throw new Exception(s);
      return s;
    }

    [Test]
    public void Test1MinimalProject()
    {
      RunProject(DefaultProject,"");
    }

    [Test]
    public void TestDelphiUnitAliasList()
    {
      string lOutput = RunProject(DefaultProject, "/p:DelphiUnitAliasList=dataEE1f=data1");
      Assert.IsTrue(lOutput.Contains("-AdataEE1f=data1"), "DelphiUnitAliasList failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestDelphiBuildAll()
    {
      const string PROPERTY_Name = "DelphiBuildAll";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -B"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestDelphiConsoleAssembly()
    {
      const string PROPERTY_Name = "DelphiConsoleAssembly";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -CC"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestDefineConstants()
    {
      const string PROPERTY_Name = "DefineConstants";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=DELPHIX7");
      Assert.IsTrue(lOutput.Contains("-DDELPHIX7"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestOutputPath()
    {
      const string PROPERTY_Name = "OutputPath";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=..\\");
      Assert.IsTrue(lOutput.Contains("-E..\\"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestDelphiMapFile()
    {
      const string PROPERTY_Name = "DelphiMapFile";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=Segments");
      Assert.IsTrue(lOutput.Contains(" -GS"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
       lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=Publics");
      Assert.IsTrue(lOutput.Contains(" -GP"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
       lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=Detailed");
      Assert.IsTrue(lOutput.Contains(" -GD"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestDelphiOutputHints()
    {
      const string PROPERTY_Name = "DelphiOutputHints";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -H"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    //[Test]
    public void TestDelphiIncludePath()
    {
      const string PROPERTY_Name = "DelphiIncludePath";
      string lOutput = RunProject(DefaultProject, "/p:TestDelphiIncludePath=true");
      Assert.IsTrue(lOutput.Contains("IncludePathTest.dpr -CC -I\"Test Path1\\;Test Path2\\\""), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestOutputOBJFiles()
    {
      const string PROPERTY_Name = "DelphiOutputOBJFiles";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -J"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestOutputCPPOBJFiles()
    {
      const string PROPERTY_Name = "DelphiOutputCPPOBJFiles";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -JP"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestBaseAddress()
    {
      const string PROPERTY_Name = "DelphiBaseAddress";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=$00400000");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -K$00400000"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestRuntimePackages()
    {
      const string PROPERTY_Name = "TestRuntimePackages";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -LUvcl;rtl;vclx;indy;inet;xmlrtl;vclie;inetdbbde;inetdbxpress;dbrtl;dsnap;vcldb;dsnapcon;soaprtl;VclSmp;dbexpress;dbxcds;inetdb;bdertl;vcldbx;webdsnap;websnap;adortl;ibxpress;"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestBuildModified()
    {
      const string PROPERTY_Name = "DelphiBuildModified";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -M"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestDCUOutputPath()
    {
      const string PROPERTY_Name = "DelphiDCUOutputPath";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=.\\DCU\\;DelphiBuildAll=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -B -N.\\DCU\\"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestObjectPath()
    {
      const string PROPERTY_Name = "TestObjectPath";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -O\"Test Path1\\;Test Path2\""), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestResourcePath()
    {
      const string PROPERTY_Name = "TestResourcePath";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -R\"Test Path1\\;Test Path2\""), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestUnitSearchPath()
    {
      const string PROPERTY_Name = "TestUnitSearchPath";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -U\"Test Path1\\;Test Path2\""), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestTurboDebuggingInfo()
    {
      const string PROPERTY_Name = "DelphiTurboDebuggingInfo";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -V"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestRemoteDebuggingInfo()
    {
      const string PROPERTY_Name = "DelphiRemoteDebuggingInfo";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -VR"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestOutputNeverBuildDcps()
    {
      const string PROPERTY_Name = "DelphiOutputNeverBuildDcps";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -Z"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestOldFileNames()
    {
      const string PROPERTY_Name = "DelphiOldFileNames";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -P"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }
    [Test]
    public void TestSYMBOL_DEPRECATEDEnabled()
    {
      const string PROPERTY_Name = "DelphiSymbol_DeprecatedEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-SYMBOL_DEPRECATED"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestSYMBOL_LIBRARYEnabled()
    {
      const string PROPERTY_Name = "DelphiSymbol_LibraryEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-SYMBOL_LIBRARY"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestSYMBOL_PLATFORMEnabled()
    {
      const string PROPERTY_Name = "DelphiSymbol_PlatformEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-SYMBOL_PLATFORM"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestUNIT_LIBRARYEnabled()
    {
      const string PROPERTY_Name = "DelphiUnit_LibraryEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-UNIT_LIBRARY"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestUNIT_PLATFORMEnabled()
    {
      const string PROPERTY_Name = "DelphiUnit_PlatformEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-UNIT_PLATFORM"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestUNIT_DEPRECATEDEnabled()
    {
      const string PROPERTY_Name = "DelphiUnit_DeprecatedEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-UNIT_DEPRECATED"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestHRESULT_COMPATEnabled()
    {
      const string PROPERTY_Name = "DelphiHresult_CompatEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-HRESULT_COMPAT"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestHIDING_MEMBEREnabled()
    {
      const string PROPERTY_Name = "DelphiHiding_MemberEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-HIDING_MEMBER"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestHIDDEN_VIRTUALEnabled()
    {
      const string PROPERTY_Name = "DelphiHidden_VirtualEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-HIDDEN_VIRTUAL"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestGARBAGEEnabled()
    {
      const string PROPERTY_Name = "DelphiGarbageEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-GARBAGE"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestBOUNDS_ERROREnabled()
    {
      const string PROPERTY_Name = "DelphiBounds_ErrorEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-BOUNDS_ERROR"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestZERO_NIL_COMPATEnabled()
    {
      const string PROPERTY_Name = "DelphiZero_Nil_CompatEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-ZERO_NIL_COMPAT"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestSTRING_CONST_TRUNCEDEnabled()
    {
      const string PROPERTY_Name = "DelphiString_Const_TruncedEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-STRING_CONST_TRUNCED"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestFOR_LOOP_VAR_VARPAREnabled()
    {
      const string PROPERTY_Name = "DelphiFor_Loop_Var_VarparEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-FOR_LOOP_VAR_VARPAR"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestTYPED_CONST_VARPAREnabled()
    {
      const string PROPERTY_Name = "DelphiTyped_Const_VarparEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-TYPED_CONST_VARPAR"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestASG_TO_TYPED_CONSTEnabled()
    {
      const string PROPERTY_Name = "DelphiAsg_To_Typed_ConstEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-ASG_TO_TYPED_CONST"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestCASE_LABEL_RANGEEnabled()
    {
      const string PROPERTY_Name = "DelphiCase_Label_RangeEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-CASE_LABEL_RANGE"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestFOR_VARIABLEEnabled()
    {
      const string PROPERTY_Name = "DelphiFor_VariableEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-FOR_VARIABLE"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestCONSTRUCTING_ABSTRACTEnabled()
    {
      const string PROPERTY_Name = "DelphiConstructing_AbstractEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-CONSTRUCTING_ABSTRACT"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestCOMPARISON_FALSEEnabled()
    {
      const string PROPERTY_Name = "DelphiComparison_FalseEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-COMPARISON_FALSE"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestCOMPARISON_TRUEEnabled()
    {
      const string PROPERTY_Name = "DelphiComparison_TrueEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-COMPARISON_TRUE"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestCOMPARING_SIGNED_UNSIGNEDEnabled()
    {
      const string PROPERTY_Name = "DelphiComparing_Signed_UnsignedEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-COMPARING_SIGNED_UNSIGNED"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestCOMBINING_SIGNED_UNSIGNEDEnabled()
    {
      const string PROPERTY_Name = "DelphiCombining_Signed_UnsignedEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-COMBINING_SIGNED_UNSIGNED"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestUNSUPPORTED_CONSTRUCTEnabled()
    {
      const string PROPERTY_Name = "DelphiUnsupported_ConstructEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-UNSUPPORTED_CONSTRUCT"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestFILE_OPENEnabled()
    {
      const string PROPERTY_Name = "DelphiFile_OpenEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-FILE_OPEN"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestFILE_OPEN_UNITSRCEnabled()
    {
      const string PROPERTY_Name = "DelphiFile_Open_UnitsrcEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-FILE_OPEN_UNITSRC"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestBAD_GLOBAL_SYMBOLEnabled()
    {
      const string PROPERTY_Name = "DelphiBad_Global_SymbolEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-BAD_GLOBAL_SYMBOL"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestDUPLICATE_CTOR_DTOREnabled()
    {
      const string PROPERTY_Name = "DelphiDuplicate_Ctor_DtorEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-DUPLICATE_CTOR_DTOR"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestINVALID_DIRECTIVEEnabled()
    {
      const string PROPERTY_Name = "DelphiInvalid_DirectiveEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-INVALID_DIRECTIVE"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestPACKAGE_NO_LINKEnabled()
    {
      const string PROPERTY_Name = "DelphiPackage_No_LinkEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-PACKAGE_NO_LINK"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestPACKAGED_THREADVAREnabled()
    {
      const string PROPERTY_Name = "DelphiPackaged_ThreadvarEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-PACKAGED_THREADVAR"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestIMPLICIT_IMPORTEnabled()
    {
      const string PROPERTY_Name = "DelphiImplicit_ImportEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-IMPLICIT_IMPORT"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestHPPEMIT_IGNOREDEnabled()
    {
      const string PROPERTY_Name = "DelphiHppemit_IgnoredEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-HPPEMIT_IGNORED"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestNO_RETVALEnabled()
    {
      const string PROPERTY_Name = "DelphiNo_RetvalEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-NO_RETVAL"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestUSE_BEFORE_DEFEnabled()
    {
      const string PROPERTY_Name = "DelphiUse_Before_DefEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-USE_BEFORE_DEF"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestFOR_LOOP_VAR_UNDEFEnabled()
    {
      const string PROPERTY_Name = "DelphiFor_Loop_Var_UndefEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-FOR_LOOP_VAR_UNDEF"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestUNIT_NAME_MISMATCHEnabled()
    {
      const string PROPERTY_Name = "DelphiUnit_Name_MismatchEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-UNIT_NAME_MISMATCH"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestNO_CFG_FILE_FOUNDEnabled()
    {
      const string PROPERTY_Name = "DelphiNo_Cfg_File_FoundEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-NO_CFG_FILE_FOUND"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestIMPLICIT_VARIANTSEnabled()
    {
      const string PROPERTY_Name = "DelphiImplicit_VariantsEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-IMPLICIT_VARIANTS"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestUNICODE_TO_LOCALEEnabled()
    {
      const string PROPERTY_Name = "DelphiUnicode_To_LocaleEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-UNICODE_TO_LOCALE"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestLOCALE_TO_UNICODEEnabled()
    {
      const string PROPERTY_Name = "DelphiLocale_To_UnicodeEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-LOCALE_TO_UNICODE"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestIMAGEBASE_MULTIPLEEnabled()
    {
      const string PROPERTY_Name = "DelphiImagebase_MultipleEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-IMAGEBASE_MULTIPLE"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestSUSPICIOUS_TYPECASTEnabled()
    {
      const string PROPERTY_Name = "DelphiSuspicious_TypecastEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-SUSPICIOUS_TYPECAST"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestPRIVATE_PROPACCESSOREnabled()
    {
      const string PROPERTY_Name = "DelphiPrivate_PropaccessorEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-PRIVATE_PROPACCESSOR"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestUNSAFE_TYPEEnabled()
    {
      const string PROPERTY_Name = "DelphiUnsafe_TypeEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-UNSAFE_TYPE"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestUNSAFE_CODEEnabled()
    {
      const string PROPERTY_Name = "DelphiUnsafe_CodeEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-UNSAFE_CODE"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestUNSAFE_CASTEnabled()
    {
      const string PROPERTY_Name = "DelphiUnsafe_CastEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-UNSAFE_CAST"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestMESSAGE_DIRECTIVEEnabled()
    {
      const string PROPERTY_Name = "DelphiMessage_DirectiveEnabled";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -W-MESSAGE_DIRECTIVE"), PROPERTY_Name + " property failed.");
    }

    [Test]
    public void TestFullBoolEval()
    {
      const string PROPERTY_Name = "DelphiFullBoolEval";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$B+"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestRuntimeAssertions()
    {
      const string PROPERTY_Name = "DelphiRuntimeAssertions";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$C-"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestDebugInfo()
    {
      const string PROPERTY_Name = "DelphiDebugInfo";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$D-"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestUseImportedRefs()
    {
      const string PROPERTY_Name = "DelphiUseImportedRefs";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$G-"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestUseLongString()
    {
      const string PROPERTY_Name = "DelphiUseLongString";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$H-"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestIOChecking()
    {
      const string PROPERTY_Name = "DelphiIOChecking";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$I-"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestWritableConsts()
    {
      const string PROPERTY_Name = "DelphiWritableConsts";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$J+"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestLocalDebugSym()
    {
      const string PROPERTY_Name = "DelphiLocalDebugSym";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$L-"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestRuntimeTypeInfo()
    {
      const string PROPERTY_Name = "DelphiRuntimeTypeInfo";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$M+"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestOptimization()
    {
      const string PROPERTY_Name = "DelphiOptimization";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$O-"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestOpenString()
    {
      const string PROPERTY_Name = "DelphiOpenString";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$P-"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestIntOverflowChecking()
    {
      const string PROPERTY_Name = "DelphiIntOverflowChecking";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$Q+"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestRangeChecking()
    {
      const string PROPERTY_Name = "DelphiRangeChecking";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$R+"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestStrongTypedPointer()
    {
      const string PROPERTY_Name = "DelphiStrongTypedPointer";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$T+"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestPentiumSafeDivide()
    {
      const string PROPERTY_Name = "DelphiPentiumSafeDivide";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$U+"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestStrongVarStrings()
    {
      const string PROPERTY_Name = "DelphiStrongVarStrings";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$V-"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestStackFrames()
    {
      const string PROPERTY_Name = "DelphiStackFrames";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$W+"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestExtendedSyntax()
    {
      const string PROPERTY_Name = "DelphiExtendedSyntax";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$X-"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestSymbolRef()
    {
      const string PROPERTY_Name = "DelphiSymbolRef";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=false");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr -$Y-"), PROPERTY_Name + " property switch failed.");
    }

    [Test]
    public void TestRecordAlignment()
    {
      const string PROPERTY_Name = "DelphiRecordAlignment";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=\"Quad Word\"");
      Assert.IsTrue(lOutput.Contains(" -$A8"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
      lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=\"Double Word\"");
      Assert.IsTrue(lOutput.Contains(" -$A4"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
      lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=Word");
      Assert.IsTrue(lOutput.Contains(" -$A2"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
      lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=None");
      Assert.IsTrue(lOutput.Contains(" -$A1"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestMinEnumSized()
    {
      const string PROPERTY_Name = "DelphiMinEnumSize";
      string lOutput;
      lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=\"Double Word\"");
      Assert.IsTrue(lOutput.Contains(" -$Z4"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
      lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=Word");
      Assert.IsTrue(lOutput.Contains(" -$Z2"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
      lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=Byte");
      Assert.IsTrue(lOutput.Contains(" -$Z1"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestDelphiStackSizes()
    {
      const string PROPERTY_Name = "DelphiStackSizes";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=\"16384,1048576\"");
      Assert.IsTrue(lOutput.Contains("-$M16384,1048576"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }

    [Test]
    public void TestLargeCommandLine()
    {
      const string PROPERTY_Name = "TestRuntimePackages";
      string lOutput = RunProject(DefaultProject, "/p:" + PROPERTY_Name + "=true;DelphiBaseAddress=$00400000;DelphiDCUOutputPath=.\\DCU\\;DelphiBuildAll=true");
      Assert.IsTrue(lOutput.Contains("DCC32V15_0Test.dpr"), PROPERTY_Name + " failed as the parameter is missing in the command line for the compiler");
    }
  }
}
