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
using Microsoft.Build.Framework;
using Microsoft.Win32;

namespace VisualStudio.Delphi.Tasks.DCC32.V15_0
{
  public class DCC32 : TDelphiCompilerTool
  {
    private TRecordAlignment FRecordAlignment = null;
    private string FUnitAlias = null;
    private bool FBuildAll = false;
    private bool FConsoleTarget = false;
    private bool FGUITarget;
    private string FDefines;
    private string FOutputPath;
    //    private string FFindErrorOffset;
    private TMapFileType FMapFile = new TMapFileType();
    private bool FOutputHints;
    private ITaskItem[] FIncludePath;
    private bool FOutputOBJFiles;
    private bool FOutputCPPOBJFiles;
    private string FBaseAddress;
    private string FRuntimePackages;
    private bool FBuildModified;
    private string FDCUOutputPath;
    private ITaskItem[] FObjectPath;
    private ITaskItem[] FResourcePath;
    private ITaskItem[] FUnitSearchPath;
    private bool FTurboDebugging;
    private bool FRemoteDebuggingInfo;
    private bool FOldFileNames;
    private bool FDoNotBuildDCPs;
    private TMinEnumSize FMinEnumSize = null;
    private string FStackSizes;

    public abstract class SwitchEnum : DynamicEnum
    {
      private  List<string> FList = new List<string>();
      private  MSBuildCompilerSwitchDictionary FMSBuildCompilerSwitchs = new MSBuildCompilerSwitchDictionary();

      public SwitchEnum(Type aDynamicEnumType)
        : base(aDynamicEnumType)
      { }

      protected class MSBuildCompilerSwitchDictionary : Dictionary<string, string>
      {
      }

      protected abstract void GetMSBuildCompilerSwitches(MSBuildCompilerSwitchDictionary aDictionary);

      protected override IEnumerable<string> GetEnumerable()
      {
        lock (FList)
        {
          if (FList.Count == 0)
          {
            GetMSBuildCompilerSwitches(FMSBuildCompilerSwitchs);
            foreach (KeyValuePair<string, string> lItem in FMSBuildCompilerSwitchs)
              FList.Add(lItem.Key);
          }
        }
        return FList;
      }

      public virtual string GetSwitch()
      {
        return FMSBuildCompilerSwitchs[this.ToString()];
      }
    }

    public class TRecordAlignment : SwitchEnum
    {
      protected override void GetMSBuildCompilerSwitches(MSBuildCompilerSwitchDictionary aDictionary)
      {
        aDictionary.Add("Quad Word", "8");
        aDictionary.Add("Double Word", "4");
        aDictionary.Add("Word", "2");
        aDictionary.Add("None", "1");
      }

      public TRecordAlignment()
        : base(typeof(TRecordAlignment))
      {
      }
    }

    public class TMinEnumSize : SwitchEnum
    {
      protected override void GetMSBuildCompilerSwitches(MSBuildCompilerSwitchDictionary aDictionary)
      {
        aDictionary.Add("Byte", "1"); // default 
        aDictionary.Add("Word", "2");
        aDictionary.Add("Double Word", "4");
      }

      public TMinEnumSize()
        : base(typeof(TRecordAlignment))
      {
      }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Minimum Enum Size (Byte, Word, Double Word, Quad Word)", DefineProperty = true, PropertyName = "DelphiMinEnumSize")]
    [CompilerCommand("-$Z", AppendValue = true, QuotedValue = false)]
    public string MinEnumSize
    {
      get
      {
        if (FMinEnumSize == null)
        {
            return null;
        }
        return FMinEnumSize.GetSwitch();
      }

      set 
      {
        if (!String.IsNullOrEmpty(value))
        {
          if (FMinEnumSize == null)
            FMinEnumSize = new TMinEnumSize();
          FMinEnumSize.EnumeratorItem = value;
        }
        else
          FMinEnumSize = null;
      }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Aligned record fields posible Values (Quad Word, Double Word, Word, None)", DefineProperty = true, PropertyName = "DelphiRecordAlignment")]
    [CompilerCommand("-$A", AppendValue = true, QuotedValue = false)]
    public string RecordAlignment
    {
      get
      {
        if (FRecordAlignment == null)
          return null;
        else
          return FRecordAlignment.GetSwitch();
      }
      set
      {
        if (!String.IsNullOrEmpty(value))
        {
          if (FRecordAlignment == null)
            FRecordAlignment = new TRecordAlignment();
          FRecordAlignment.EnumeratorItem = value;
        }
        else
          FRecordAlignment = null;
      }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Unit alias values (unit=alias;unit=alias)", DefineProperty = true, PropertyName = "DelphiUnitAliasList")]
    [CompilerCommand("-A", AppendValue = true, QuotedValue = false)]
    public string UnitAlias
    {
      get { return FUnitAlias; }
      set { FUnitAlias = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Build All Units (bool)", DefineProperty = true, PropertyName = "DelphiBuildAll")]
    [BoolCompilerCommand("-B", null, BoolSwitchDefault.Disabled)]
    public bool BuildAll
    {
      get { return FBuildAll; }
      set { FBuildAll = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Target assembly is a console (bool)", DefineProperty = true, PropertyName = "DelphiConsoleAssembly")]
    [BoolCompilerCommand("-CC", "-CG", BoolSwitchDefault.Disabled)]
    public bool ConsoleTarget
    {
      get { return FConsoleTarget; }
      set { FConsoleTarget = value; }
    }

    // reuse Msbuild property <DefineConstants>
    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("compiler definitions for conditions (syms;syms)")]
    [CompilerCommand("-D", AppendValue = true, QuotedValue = false)]
    public string DefineConstants
    {
      get { return FDefines; }
      set { FDefines = value; }
    }


    // reuse Msbuild property <OutputPath>
    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Assembly output directory")]
    [CompilerCommand("-E", AppendValue = true)]
    public string OutputPath
    {
      get { return FOutputPath; }
      set { FOutputPath = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Assembly min and max stack sizes - default '16384,1048576' (minStackSize[,maxStackSize])", DefineProperty = true, PropertyName = "DelphiStackSizes")]
    [CompilerCommand("-$M", AppendValue = true, QuotedValue=false)]
    public string StackSizes
    {
      get { return FStackSizes; }
      set { FStackSizes = value; }
    }

    // does not work on Delphi 7
    //[VisualStudio.MSBuild.XSD.TaskParameterDefinition("Find Error at offset (int)", DefineProperty = true, PropertyName = "DelphiFindErrorOffset")]
    //[CompilerCommand("-F", AppendValue = true)]
    //public string FindErrorOffset
    //{
    //  get { return FFindErrorOffset; }
    //  set { FFindErrorOffset = value; }
    //}

    public class TMapFileType : SwitchEnum
    {
      protected override void GetMSBuildCompilerSwitches(MSBuildCompilerSwitchDictionary aDictionary)
      {
        aDictionary.Add("Off", null);
        aDictionary.Add("Segments", "S");
        aDictionary.Add("Publics", "P");
        aDictionary.Add("Detailed", "D");
      }

      public TMapFileType()
        : base(typeof(TRecordAlignment))
      {
      }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Generate map file of type (Off, Segments, Publics, Detailed)", DefineProperty = true, PropertyName = "DelphiMapFile")]
    [CompilerCommand("-G", AppendValue = true)]
    public string MapFileType
    {
      get
      {
        return FMapFile.GetSwitch();
      }
      set
      {
        if (!string.IsNullOrEmpty(value))
          FMapFile.EnumeratorItem = value;
      }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Output hint messages (bool)", DefineProperty = true, PropertyName = "DelphiOutputHints")]
    [BoolCompilerCommand("-H")]
    public bool OutputHints
    {
      get { return FOutputHints; }
      set { FOutputHints = value; }
    }

    // TODO: Include directories (DelphiIncludePath) does not work on the DCC32 compiler 
    // we will need to copy the files to the current dir and delete them after the compile.
    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Include directories", DefineProperty = true, PropertyName = "DelphiIncludePath")]
    [CompilerCommand("-I", AppendValue = true)]
    public ITaskItem[] IncludePath
    {
      get { return FIncludePath; }
      set { FIncludePath = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Generate .obj files (bool)", DefineProperty = true, PropertyName = "DelphiOutputOBJFiles")]
    [BoolCompilerCommand("-J")]
    public bool OutputOBJFiles
    {
      get { return FOutputOBJFiles; }
      set { FOutputOBJFiles = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Generate C++ .obj files (bool)", DefineProperty = true, PropertyName = "DelphiOutputCPPOBJFiles")]
    [BoolCompilerCommand("-JP")]
    public bool OutputCPPOBJFiles
    {
      get { return FOutputCPPOBJFiles; }
      set { FOutputCPPOBJFiles = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Set image base address (hex)", DefineProperty = true, PropertyName = "DelphiBaseAddress")]
    [CompilerCommand("-K", AppendValue = true, QuotedValue = false)]
    public string BaseAddress
    {
      get { return FBaseAddress; }
      set { FBaseAddress = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Build with Runtime packages (package;package)", DefineProperty = true, PropertyName = "DelphiRuntimePackages")]
    [CompilerCommand("-LU", AppendValue = true, QuotedValue = false)]
    public string RuntimePackages
    {
      get { return FRuntimePackages; }
      set { FRuntimePackages = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Instructs the compiler to check all units upon which the file being compiled depends. (bool)", DefineProperty = true, PropertyName = "DelphiBuildModified")]
    [BoolCompilerCommand("-M")]
    public bool BuildModified
    {
      get { return FBuildModified; }
      set { FBuildModified = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("The directory to output .DCU files.", DefineProperty = true, PropertyName = "DelphiDCUOutputPath")]
    [CompilerCommand("-N", AppendValue = true)]
    public string DCUOutputPath
    {
      get { return FDCUOutputPath; }
      set { FDCUOutputPath = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Specify a list of directories in which to search for .OBJ files", DefineProperty = true, PropertyName = "DelphiObjectPath")]
    [CompilerCommand("-O", AppendValue = true)]
    public ITaskItem[] ObjectPath
    {
      get { return FObjectPath; }
      set { FObjectPath = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Additional directories to look for resource files.", DefineProperty = true, PropertyName = "DelphiResourcePath")]
    [CompilerCommand("-R", AppendValue = true)]
    public ITaskItem[] ResourcePath
    {
      get { return FResourcePath; }
      set { FResourcePath = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Specify additional directories in which to search for units.", DefineProperty = true, PropertyName = "DelphiUnitSearchPath")]
    [CompilerCommand("-U", AppendValue = true)]
    public ITaskItem[] UnitSearchPath
    {
      get { return FUnitSearchPath; }
      set { FUnitSearchPath = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Instructs the compiler appends Turbo Debugger 5.0-compatible debug information. (bool)", DefineProperty = true, PropertyName = "DelphiTurboDebuggingInfo")]
    [BoolCompilerCommand("-V")]
    public bool TurboDebuggingInfo
    {
      get { return FTurboDebugging; }
      set { FTurboDebugging = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Instructs the compiler generates debugging symbol information in an .rsm file. (bool)", DefineProperty = true, PropertyName = "DelphiRemoteDebuggingInfo")]
    [BoolCompilerCommand("-VR")]
    public bool RemoteDebuggingInfo
    {
      get { return FRemoteDebuggingInfo; }
      set { FRemoteDebuggingInfo = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Output 'never build' DCPs. (bool)", DefineProperty = true, PropertyName = "DelphiOutputNeverBuildDcps")]
    [BoolCompilerCommand("-Z")]
    public bool OutputNeverBuildDcps
    {
      get { return FDoNotBuildDCPs; }
      set { FDoNotBuildDCPs = value; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Look for 8.3 file names also. (bool)", DefineProperty = true, PropertyName = "DelphiOldFileNames")]
    [BoolCompilerCommand("-P")]
    public bool OldFileNames
    {
      get { return FOldFileNames; }
      set { FOldFileNames = value; }
    }

    private bool FSymbol_DeprecatedWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Deprecated Symbol", DefineProperty = true, PropertyName = "DelphiSymbol_DeprecatedEnabled")]
    [BoolCompilerCommand("-W+SYMBOL_DEPRECATED", "-W-SYMBOL_DEPRECATED", BoolSwitchDefault.Enabled)]
    public bool Symbol_DeprecatedWarnning
    {
      get { return FSymbol_DeprecatedWarnning; }
      set { FSymbol_DeprecatedWarnning = value; }
    }

    private bool FSymbol_LibraryWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Library Symbol", DefineProperty = true, PropertyName = "DelphiSymbol_LibraryEnabled")]
    [BoolCompilerCommand("-W+SYMBOL_LIBRARY", "-W-SYMBOL_LIBRARY", BoolSwitchDefault.Enabled)]
    public bool Symbol_LibraryWarnning
    {
      get { return FSymbol_LibraryWarnning; }
      set { FSymbol_LibraryWarnning = value; }
    }

    private bool FSymbol_PlatformWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Platform Symbol", DefineProperty = true, PropertyName = "DelphiSymbol_PlatformEnabled")]
    [BoolCompilerCommand("-W+SYMBOL_PLATFORM", "-W-SYMBOL_PLATFORM", BoolSwitchDefault.Enabled)]
    public bool Symbol_PlatformWarnning
    {
      get { return FSymbol_PlatformWarnning; }
      set { FSymbol_PlatformWarnning = value; }
    }

    //private bool FSymbol_ExperimentalWarnning = true;

    //[VisualStudio.MSBuild.XSD.TaskParameterDefinition("Experimental Symbol", DefineProperty = true, PropertyName = "DelphiSymbol_ExperimentalEnabled")]
    //[BoolCompilerCommand("-W+SYMBOL_EXPERIMENTAL", "-W-SYMBOL_EXPERIMENTAL", BoolSwitchDefault.Enabled)]
    //public bool Symbol_ExperimentalWarnning
    //{
    //  get { return FSymbol_ExperimentalWarnning; }
    //  set { FSymbol_ExperimentalWarnning = value; }
    //}

    private bool FUnit_LibraryWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Library Unit", DefineProperty = true, PropertyName = "DelphiUnit_LibraryEnabled")]
    [BoolCompilerCommand("-W+UNIT_LIBRARY", "-W-UNIT_LIBRARY", BoolSwitchDefault.Enabled)]
    public bool Unit_LibraryWarnning
    {
      get { return FUnit_LibraryWarnning; }
      set { FUnit_LibraryWarnning = value; }
    }

    private bool FUnit_PlatformWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Platform Unit", DefineProperty = true, PropertyName = "DelphiUnit_PlatformEnabled")]
    [BoolCompilerCommand("-W+UNIT_PLATFORM", "-W-UNIT_PLATFORM", BoolSwitchDefault.Enabled)]
    public bool Unit_PlatformWarnning
    {
      get { return FUnit_PlatformWarnning; }
      set { FUnit_PlatformWarnning = value; }
    }

    private bool FUnit_DeprecatedWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Deprecated Unit", DefineProperty = true, PropertyName = "DelphiUnit_DeprecatedEnabled")]
    [BoolCompilerCommand("-W+UNIT_DEPRECATED", "-W-UNIT_DEPRECATED", BoolSwitchDefault.Enabled)]
    public bool Unit_DeprecatedWarnning
    {
      get { return FUnit_DeprecatedWarnning; }
      set { FUnit_DeprecatedWarnning = value; }
    }

    private bool FHresult_CompatWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Integer and HRESULT interchanged", DefineProperty = true, PropertyName = "DelphiHresult_CompatEnabled")]
    [BoolCompilerCommand("-W+HRESULT_COMPAT", "-W-HRESULT_COMPAT", BoolSwitchDefault.Enabled)]
    public bool Hresult_CompatWarnning
    {
      get { return FHresult_CompatWarnning; }
      set { FHresult_CompatWarnning = value; }
    }

    private bool FHiding_MemberWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Redeclaration of symbol hides a member in the base class", DefineProperty = true, PropertyName = "DelphiHiding_MemberEnabled")]
    [BoolCompilerCommand("-W+HIDING_MEMBER", "-W-HIDING_MEMBER", BoolSwitchDefault.Enabled)]
    public bool Hiding_MemberWarnning
    {
      get { return FHiding_MemberWarnning; }
      set { FHiding_MemberWarnning = value; }
    }

    private bool FHidden_VirtualWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Method hides virtual method of base type", DefineProperty = true, PropertyName = "DelphiHidden_VirtualEnabled")]
    [BoolCompilerCommand("-W+HIDDEN_VIRTUAL", "-W-HIDDEN_VIRTUAL", BoolSwitchDefault.Enabled)]
    public bool Hidden_VirtualWarnning
    {
      get { return FHidden_VirtualWarnning; }
      set { FHidden_VirtualWarnning = value; }
    }

    private bool FGarbageWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Text after final ''END.'' - ignored by compiler", DefineProperty = true, PropertyName = "DelphiGarbageEnabled")]
    [BoolCompilerCommand("-W+GARBAGE", "-W-GARBAGE", BoolSwitchDefault.Enabled)]
    public bool GarbageWarnning
    {
      get { return FGarbageWarnning; }
      set { FGarbageWarnning = value; }
    }

    private bool FBounds_ErrorWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Constant expression violates subrange bounds", DefineProperty = true, PropertyName = "DelphiBounds_ErrorEnabled")]
    [BoolCompilerCommand("-W+BOUNDS_ERROR", "-W-BOUNDS_ERROR", BoolSwitchDefault.Enabled)]
    public bool Bounds_ErrorWarnning
    {
      get { return FBounds_ErrorWarnning; }
      set { FBounds_ErrorWarnning = value; }
    }

    private bool FZero_Nil_CompatWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Constant 0 converted to NIL", DefineProperty = true, PropertyName = "DelphiZero_Nil_CompatEnabled")]
    [BoolCompilerCommand("-W+ZERO_NIL_COMPAT", "-W-ZERO_NIL_COMPAT", BoolSwitchDefault.Enabled)]
    public bool Zero_Nil_CompatWarnning
    {
      get { return FZero_Nil_CompatWarnning; }
      set { FZero_Nil_CompatWarnning = value; }
    }

    private bool FString_Const_TruncedWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("String constant truncated", DefineProperty = true, PropertyName = "DelphiString_Const_TruncedEnabled")]
    [BoolCompilerCommand("-W+STRING_CONST_TRUNCED", "-W-STRING_CONST_TRUNCED", BoolSwitchDefault.Enabled)]
    public bool String_Const_TruncedWarnning
    {
      get { return FString_Const_TruncedWarnning; }
      set { FString_Const_TruncedWarnning = value; }
    }

    private bool FFor_Loop_Var_VarparWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("FOR-Loop variable cannot be passed as var parameter", DefineProperty = true, PropertyName = "DelphiFor_Loop_Var_VarparEnabled")]
    [BoolCompilerCommand("-W+FOR_LOOP_VAR_VARPAR", "-W-FOR_LOOP_VAR_VARPAR", BoolSwitchDefault.Enabled)]
    public bool For_Loop_Var_VarparWarnning
    {
      get { return FFor_Loop_Var_VarparWarnning; }
      set { FFor_Loop_Var_VarparWarnning = value; }
    }

    private bool FTyped_Const_VarparWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Typed constant cannot be passed as var parameter", DefineProperty = true, PropertyName = "DelphiTyped_Const_VarparEnabled")]
    [BoolCompilerCommand("-W+TYPED_CONST_VARPAR", "-W-TYPED_CONST_VARPAR", BoolSwitchDefault.Enabled)]
    public bool Typed_Const_VarparWarnning
    {
      get { return FTyped_Const_VarparWarnning; }
      set { FTyped_Const_VarparWarnning = value; }
    }

    private bool FAsg_To_Typed_ConstWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Assignment to typed constant", DefineProperty = true, PropertyName = "DelphiAsg_To_Typed_ConstEnabled")]
    [BoolCompilerCommand("-W+ASG_TO_TYPED_CONST", "-W-ASG_TO_TYPED_CONST", BoolSwitchDefault.Enabled)]
    public bool Asg_To_Typed_ConstWarnning
    {
      get { return FAsg_To_Typed_ConstWarnning; }
      set { FAsg_To_Typed_ConstWarnning = value; }
    }

    private bool FCase_Label_RangeWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Case label outside of range of case expression", DefineProperty = true, PropertyName = "DelphiCase_Label_RangeEnabled")]
    [BoolCompilerCommand("-W+CASE_LABEL_RANGE", "-W-CASE_LABEL_RANGE", BoolSwitchDefault.Enabled)]
    public bool Case_Label_RangeWarnning
    {
      get { return FCase_Label_RangeWarnning; }
      set { FCase_Label_RangeWarnning = value; }
    }

    private bool FFor_VariableWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("For loop control variable must be simple local variable", DefineProperty = true, PropertyName = "DelphiFor_VariableEnabled")]
    [BoolCompilerCommand("-W+FOR_VARIABLE", "-W-FOR_VARIABLE", BoolSwitchDefault.Enabled)]
    public bool For_VariableWarnning
    {
      get { return FFor_VariableWarnning; }
      set { FFor_VariableWarnning = value; }
    }

    private bool FConstructing_AbstractWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Constructing instance containing abstract method", DefineProperty = true, PropertyName = "DelphiConstructing_AbstractEnabled")]
    [BoolCompilerCommand("-W+CONSTRUCTING_ABSTRACT", "-W-CONSTRUCTING_ABSTRACT", BoolSwitchDefault.Enabled)]
    public bool Constructing_AbstractWarnning
    {
      get { return FConstructing_AbstractWarnning; }
      set { FConstructing_AbstractWarnning = value; }
    }

    private bool FComparison_FalseWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Comparison always evaluates to False", DefineProperty = true, PropertyName = "DelphiComparison_FalseEnabled")]
    [BoolCompilerCommand("-W+COMPARISON_FALSE", "-W-COMPARISON_FALSE", BoolSwitchDefault.Enabled)]
    public bool Comparison_FalseWarnning
    {
      get { return FComparison_FalseWarnning; }
      set { FComparison_FalseWarnning = value; }
    }

    private bool FComparison_TrueWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Comparison always evaluates to True", DefineProperty = true, PropertyName = "DelphiComparison_TrueEnabled")]
    [BoolCompilerCommand("-W+COMPARISON_TRUE", "-W-COMPARISON_TRUE", BoolSwitchDefault.Enabled)]
    public bool Comparison_TrueWarnning
    {
      get { return FComparison_TrueWarnning; }
      set { FComparison_TrueWarnning = value; }
    }

    private bool FComparing_Signed_UnsignedWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Comparing signed and unsigned types - widened both operands", DefineProperty = true, PropertyName = "DelphiComparing_Signed_UnsignedEnabled")]
    [BoolCompilerCommand("-W+COMPARING_SIGNED_UNSIGNED", "-W-COMPARING_SIGNED_UNSIGNED", BoolSwitchDefault.Enabled)]
    public bool Comparing_Signed_UnsignedWarnning
    {
      get { return FComparing_Signed_UnsignedWarnning; }
      set { FComparing_Signed_UnsignedWarnning = value; }
    }

    private bool FCombining_Signed_UnsignedWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Combining signed and unsigned types - widened both operands", DefineProperty = true, PropertyName = "DelphiCombining_Signed_UnsignedEnabled")]
    [BoolCompilerCommand("-W+COMBINING_SIGNED_UNSIGNED", "-W-COMBINING_SIGNED_UNSIGNED", BoolSwitchDefault.Enabled)]
    public bool Combining_Signed_UnsignedWarnning
    {
      get { return FCombining_Signed_UnsignedWarnning; }
      set { FCombining_Signed_UnsignedWarnning = value; }
    }

    private bool FUnsupported_ConstructWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Unsupported language feature", DefineProperty = true, PropertyName = "DelphiUnsupported_ConstructEnabled")]
    [BoolCompilerCommand("-W+UNSUPPORTED_CONSTRUCT", "-W-UNSUPPORTED_CONSTRUCT", BoolSwitchDefault.Enabled)]
    public bool Unsupported_ConstructWarnning
    {
      get { return FUnsupported_ConstructWarnning; }
      set { FUnsupported_ConstructWarnning = value; }
    }

    private bool FFile_OpenWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("File not found", DefineProperty = true, PropertyName = "DelphiFile_OpenEnabled")]
    [BoolCompilerCommand("-W+FILE_OPEN", "-W-FILE_OPEN", BoolSwitchDefault.Enabled)]
    public bool File_OpenWarnning
    {
      get { return FFile_OpenWarnning; }
      set { FFile_OpenWarnning = value; }
    }

    private bool FFile_Open_UnitsrcWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Unit or binary equivalent (DCU) not found", DefineProperty = true, PropertyName = "DelphiFile_Open_UnitsrcEnabled")]
    [BoolCompilerCommand("-W+FILE_OPEN_UNITSRC", "-W-FILE_OPEN_UNITSRC", BoolSwitchDefault.Enabled)]
    public bool File_Open_UnitsrcWarnning
    {
      get { return FFile_Open_UnitsrcWarnning; }
      set { FFile_Open_UnitsrcWarnning = value; }
    }

    private bool FBad_Global_SymbolWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Bad global symbol definition in object file", DefineProperty = true, PropertyName = "DelphiBad_Global_SymbolEnabled")]
    [BoolCompilerCommand("-W+BAD_GLOBAL_SYMBOL", "-W-BAD_GLOBAL_SYMBOL", BoolSwitchDefault.Enabled)]
    public bool Bad_Global_SymbolWarnning
    {
      get { return FBad_Global_SymbolWarnning; }
      set { FBad_Global_SymbolWarnning = value; }
    }

    private bool FDuplicate_Ctor_DtorWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Duplicate constructor/destructor with identical parameters will be inacessible from C++", DefineProperty = true, PropertyName = "DelphiDuplicate_Ctor_DtorEnabled")]
    [BoolCompilerCommand("-W+DUPLICATE_CTOR_DTOR", "-W-DUPLICATE_CTOR_DTOR", BoolSwitchDefault.Enabled)]
    public bool Duplicate_Ctor_DtorWarnning
    {
      get { return FDuplicate_Ctor_DtorWarnning; }
      set { FDuplicate_Ctor_DtorWarnning = value; }
    }

    private bool FInvalid_DirectiveWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Invalid compiler directive", DefineProperty = true, PropertyName = "DelphiInvalid_DirectiveEnabled")]
    [BoolCompilerCommand("-W+INVALID_DIRECTIVE", "-W-INVALID_DIRECTIVE", BoolSwitchDefault.Enabled)]
    public bool Invalid_DirectiveWarnning
    {
      get { return FInvalid_DirectiveWarnning; }
      set { FInvalid_DirectiveWarnning = value; }
    }

    private bool FPackage_No_LinkWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Package will not be written to disk because -J option is enabled", DefineProperty = true, PropertyName = "DelphiPackage_No_LinkEnabled")]
    [BoolCompilerCommand("-W+PACKAGE_NO_LINK", "-W-PACKAGE_NO_LINK", BoolSwitchDefault.Enabled)]
    public bool Package_No_LinkWarnning
    {
      get { return FPackage_No_LinkWarnning; }
      set { FPackage_No_LinkWarnning = value; }
    }

    private bool FPackaged_ThreadvarWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Exported package threadvar cannot be used outside of this package", DefineProperty = true, PropertyName = "DelphiPackaged_ThreadvarEnabled")]
    [BoolCompilerCommand("-W+PACKAGED_THREADVAR", "-W-PACKAGED_THREADVAR", BoolSwitchDefault.Enabled)]
    public bool Packaged_ThreadvarWarnning
    {
      get { return FPackaged_ThreadvarWarnning; }
      set { FPackaged_ThreadvarWarnning = value; }
    }

    private bool FImplicit_ImportWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Unit implicitly imported into package", DefineProperty = true, PropertyName = "DelphiImplicit_ImportEnabled")]
    [BoolCompilerCommand("-W+IMPLICIT_IMPORT", "-W-IMPLICIT_IMPORT", BoolSwitchDefault.Enabled)]
    public bool Implicit_ImportWarnning
    {
      get { return FImplicit_ImportWarnning; }
      set { FImplicit_ImportWarnning = value; }
    }

    private bool FHppemit_IgnoredWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("$HPPEMIT ignored", DefineProperty = true, PropertyName = "DelphiHppemit_IgnoredEnabled")]
    [BoolCompilerCommand("-W+HPPEMIT_IGNORED", "-W-HPPEMIT_IGNORED", BoolSwitchDefault.Enabled)]
    public bool Hppemit_IgnoredWarnning
    {
      get { return FHppemit_IgnoredWarnning; }
      set { FHppemit_IgnoredWarnning = value; }
    }

    private bool FNo_RetvalWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Return value of function might be undefined", DefineProperty = true, PropertyName = "DelphiNo_RetvalEnabled")]
    [BoolCompilerCommand("-W+NO_RETVAL", "-W-NO_RETVAL", BoolSwitchDefault.Enabled)]
    public bool No_RetvalWarnning
    {
      get { return FNo_RetvalWarnning; }
      set { FNo_RetvalWarnning = value; }
    }

    private bool FUse_Before_DefWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Variable might not have been initialized", DefineProperty = true, PropertyName = "DelphiUse_Before_DefEnabled")]
    [BoolCompilerCommand("-W+USE_BEFORE_DEF", "-W-USE_BEFORE_DEF", BoolSwitchDefault.Enabled)]
    public bool Use_Before_DefWarnning
    {
      get { return FUse_Before_DefWarnning; }
      set { FUse_Before_DefWarnning = value; }
    }

    private bool FFor_Loop_Var_UndefWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("FOR-Loop variable may be undefined after loop", DefineProperty = true, PropertyName = "DelphiFor_Loop_Var_UndefEnabled")]
    [BoolCompilerCommand("-W+FOR_LOOP_VAR_UNDEF", "-W-FOR_LOOP_VAR_UNDEF", BoolSwitchDefault.Enabled)]
    public bool For_Loop_Var_UndefWarnning
    {
      get { return FFor_Loop_Var_UndefWarnning; }
      set { FFor_Loop_Var_UndefWarnning = value; }
    }

    private bool FUnit_Name_MismatchWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Unit identifier does not match file name", DefineProperty = true, PropertyName = "DelphiUnit_Name_MismatchEnabled")]
    [BoolCompilerCommand("-W+UNIT_NAME_MISMATCH", "-W-UNIT_NAME_MISMATCH", BoolSwitchDefault.Enabled)]
    public bool Unit_Name_MismatchWarnning
    {
      get { return FUnit_Name_MismatchWarnning; }
      set { FUnit_Name_MismatchWarnning = value; }
    }

    private bool FNo_Cfg_File_FoundWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("No configuration files found", DefineProperty = true, PropertyName = "DelphiNo_Cfg_File_FoundEnabled")]
    [BoolCompilerCommand("-W+NO_CFG_FILE_FOUND", "-W-NO_CFG_FILE_FOUND", BoolSwitchDefault.Enabled)]
    public bool No_Cfg_File_FoundWarnning
    {
      get { return FNo_Cfg_File_FoundWarnning; }
      set { FNo_Cfg_File_FoundWarnning = value; }
    }

    private bool FImplicit_VariantsWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Implicit use of Variants unit", DefineProperty = true, PropertyName = "DelphiImplicit_VariantsEnabled")]
    [BoolCompilerCommand("-W+IMPLICIT_VARIANTS", "-W-IMPLICIT_VARIANTS", BoolSwitchDefault.Enabled)]
    public bool Implicit_VariantsWarnning
    {
      get { return FImplicit_VariantsWarnning; }
      set { FImplicit_VariantsWarnning = value; }
    }

    private bool FUnicode_To_LocaleWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Error converting Unicode char to locale charset", DefineProperty = true, PropertyName = "DelphiUnicode_To_LocaleEnabled")]
    [BoolCompilerCommand("-W+UNICODE_TO_LOCALE", "-W-UNICODE_TO_LOCALE", BoolSwitchDefault.Enabled)]
    public bool Unicode_To_LocaleWarnning
    {
      get { return FUnicode_To_LocaleWarnning; }
      set { FUnicode_To_LocaleWarnning = value; }
    }

    private bool FLocale_To_UnicodeWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Error converting locale string to Unicode", DefineProperty = true, PropertyName = "DelphiLocale_To_UnicodeEnabled")]
    [BoolCompilerCommand("-W+LOCALE_TO_UNICODE", "-W-LOCALE_TO_UNICODE", BoolSwitchDefault.Enabled)]
    public bool Locale_To_UnicodeWarnning
    {
      get { return FLocale_To_UnicodeWarnning; }
      set { FLocale_To_UnicodeWarnning = value; }
    }

    private bool FImagebase_MultipleWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Imagebase is not a multiple of 64k", DefineProperty = true, PropertyName = "DelphiImagebase_MultipleEnabled")]
    [BoolCompilerCommand("-W+IMAGEBASE_MULTIPLE", "-W-IMAGEBASE_MULTIPLE", BoolSwitchDefault.Enabled)]
    public bool Imagebase_MultipleWarnning
    {
      get { return FImagebase_MultipleWarnning; }
      set { FImagebase_MultipleWarnning = value; }
    }

    private bool FSuspicious_TypecastWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Suspicious typecast", DefineProperty = true, PropertyName = "DelphiSuspicious_TypecastEnabled")]
    [BoolCompilerCommand("-W+SUSPICIOUS_TYPECAST", "-W-SUSPICIOUS_TYPECAST", BoolSwitchDefault.Enabled)]
    public bool Suspicious_TypecastWarnning
    {
      get { return FSuspicious_TypecastWarnning; }
      set { FSuspicious_TypecastWarnning = value; }
    }

    private bool FPrivate_PropaccessorWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Property declaration references private ancestor member", DefineProperty = true, PropertyName = "DelphiPrivate_PropaccessorEnabled")]
    [BoolCompilerCommand("-W+PRIVATE_PROPACCESSOR", "-W-PRIVATE_PROPACCESSOR", BoolSwitchDefault.Enabled)]
    public bool Private_PropaccessorWarnning
    {
      get { return FPrivate_PropaccessorWarnning; }
      set { FPrivate_PropaccessorWarnning = value; }
    }

    private bool FUnsafe_TypeWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Unsafe type", DefineProperty = true, PropertyName = "DelphiUnsafe_TypeEnabled")]
    [BoolCompilerCommand("-W+UNSAFE_TYPE", "-W-UNSAFE_TYPE", BoolSwitchDefault.Enabled)]
    public bool Unsafe_TypeWarnning
    {
      get { return FUnsafe_TypeWarnning; }
      set { FUnsafe_TypeWarnning = value; }
    }

    private bool FUnsafe_CodeWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Unsafe code", DefineProperty = true, PropertyName = "DelphiUnsafe_CodeEnabled")]
    [BoolCompilerCommand("-W+UNSAFE_CODE", "-W-UNSAFE_CODE", BoolSwitchDefault.Enabled)]
    public bool Unsafe_CodeWarnning
    {
      get { return FUnsafe_CodeWarnning; }
      set { FUnsafe_CodeWarnning = value; }
    }

    private bool FUnsafe_CastWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Unsafe typecast", DefineProperty = true, PropertyName = "DelphiUnsafe_CastEnabled")]
    [BoolCompilerCommand("-W+UNSAFE_CAST", "-W-UNSAFE_CAST", BoolSwitchDefault.Enabled)]
    public bool Unsafe_CastWarnning
    {
      get { return FUnsafe_CastWarnning; }
      set { FUnsafe_CastWarnning = value; }
    }

    private bool FMessage_DirectiveWarnning = true;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("User message", DefineProperty = true, PropertyName = "DelphiMessage_DirectiveEnabled")]
    [BoolCompilerCommand("-W+MESSAGE_DIRECTIVE", "-W-MESSAGE_DIRECTIVE", BoolSwitchDefault.Enabled)]
    public bool Message_DirectiveWarnning
    {
      get { return FMessage_DirectiveWarnning; }
      set { FMessage_DirectiveWarnning = value; }
    }








    private bool FFullBoolEval = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Full boolean Evaluation", PropertyName = "DelphiFullBoolEval")]
    [BoolCompilerCommand("-$B+", "-$B-", BoolSwitchDefault.Disabled)]
    public bool FullBoolEval
    {
      get { return FFullBoolEval; }
      set { FFullBoolEval = value; }
    }

    private bool FRuntimeAssertions = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Evaluate assertions at runtime", PropertyName = "DelphiRuntimeAssertions")]
    [BoolCompilerCommand("-$C+", "-$C-", BoolSwitchDefault.Enabled)]
    public bool RuntimeAssertions
    {
      get { return FRuntimeAssertions; }
      set { FRuntimeAssertions = value; }
    }

    private bool FDebugInfo = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Debug information", PropertyName = "DelphiDebugInfo")]
    [BoolCompilerCommand("-$D+", "-$D-", BoolSwitchDefault.Enabled)]
    public bool DebugInfo
    {
      get { return FDebugInfo; }
      set { FDebugInfo = value; }
    }

    private bool FUseImportedRefs = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Use imported data references", PropertyName = "DelphiUseImportedRefs")]
    [BoolCompilerCommand("-$G+", "-$G-", BoolSwitchDefault.Enabled)]
    public bool UseImportedRefs
    {
      get { return FUseImportedRefs; }
      set { FUseImportedRefs = value; }
    }

    private bool FUseLongString = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Use long strings by default", PropertyName = "DelphiUseLongString")]
    [BoolCompilerCommand("-$H+", "-$H-", BoolSwitchDefault.Enabled)]
    public bool UseLongString
    {
      get { return FUseLongString; }
      set { FUseLongString = value; }
    }

    private bool FIOChecking = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("I/O checking", PropertyName = "DelphiIOChecking")]
    [BoolCompilerCommand("-$I+", "-$I-", BoolSwitchDefault.Enabled)]
    public bool IOChecking
    {
      get { return FIOChecking; }
      set { FIOChecking = value; }
    }

    private bool FWritableConsts = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Writeable structured consts", PropertyName = "DelphiWritableConsts")]
    [BoolCompilerCommand("-$J+", "-$J-", BoolSwitchDefault.Disabled)]
    public bool WritableConsts
    {
      get { return FWritableConsts; }
      set { FWritableConsts = value; }
    }

    private bool FLocalDebugSym = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Local debug symbols", PropertyName = "DelphiLocalDebugSym")]
    [BoolCompilerCommand("-$L+", "-$L-", BoolSwitchDefault.Enabled)]
    public bool LocalDebugSym
    {
      get { return FLocalDebugSym; }
      set { FLocalDebugSym = value; }
    }

    private bool FRuntimeTypeInfo = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("The compiler generates runtime type information for fields, methods, and properties that are declared in a published section.", PropertyName = "DelphiRuntimeTypeInfo")]
    [BoolCompilerCommand("-$M+", "-$M-", BoolSwitchDefault.Disabled)]
    public bool RuntimeTypeInfo
    {
      get { return FRuntimeTypeInfo; }
      set { FRuntimeTypeInfo = value; }
    }

    private bool FOptimization = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("The compiler performs a number of code optimizations.", PropertyName = "DelphiOptimization")]
    [BoolCompilerCommand("-$O+", "-$O-", BoolSwitchDefault.Enabled)]
    public bool Optimization
    {
      get { return FOptimization; }
      set { FOptimization = value; }
    }

    private bool FOpenString = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Open string params", PropertyName = "DelphiOpenString")]
    [BoolCompilerCommand("-$P+", "-$P-", BoolSwitchDefault.Enabled)]
    public bool OpenString
    {
      get { return FOpenString; }
      set { FOpenString = value; }
    }

    private bool FIntOverflowChecking = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Integer overflow checking", PropertyName = "DelphiIntOverflowChecking")]
    [BoolCompilerCommand("-$Q+", "-$Q-", BoolSwitchDefault.Disabled)]
    public bool IntOverflowChecking
    {
      get { return FIntOverflowChecking; }
      set { FIntOverflowChecking = value; }
    }

    private bool FRangeChecking = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Application raises a Range checking error if one occurs.", PropertyName = "DelphiRangeChecking")]
    [BoolCompilerCommand("-$R+", "-$R-", BoolSwitchDefault.Disabled)]
    public bool RangeChecking
    {
      get { return FRangeChecking; }
      set { FRangeChecking = value; }
    }

    private bool FStrongTypedPointer = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Typed @ operator", PropertyName = "DelphiStrongTypedPointer")]
    [BoolCompilerCommand("-$T+", "-$T-", BoolSwitchDefault.Disabled)]
    public bool StrongTypedPointer
    {
      get { return FStrongTypedPointer; }
      set { FStrongTypedPointer = value; }
    }

    private bool FPentiumSafeDivide = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Pentium(tm)-safe divide", PropertyName = "DelphiPentiumSafeDivide")]
    [BoolCompilerCommand("-$U+", "-$U-", BoolSwitchDefault.Disabled)]
    public bool PentiumSafeDivide
    {
      get { return FPentiumSafeDivide; }
      set { FPentiumSafeDivide = value; }
    }

    private bool FStrongVarStrings = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Strict var-strings", PropertyName = "DelphiStrongVarStrings")]
    [BoolCompilerCommand("-$V+", "-$V-", BoolSwitchDefault.Enabled)]
    public bool StrongVarStrings
    {
      get { return FStrongVarStrings; }
      set { FStrongVarStrings = value; }
    }

    private bool FStackFrames = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Generate stack frames", PropertyName = "DelphiStackFrames")]
    [BoolCompilerCommand("-$W+", "-$W-", BoolSwitchDefault.Disabled)]
    public bool StackFrames
    {
      get { return FStackFrames; }
      set { FStackFrames = value; }
    }

    private bool FExtendedSyntax = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Extended syntax", PropertyName = "DelphiExtendedSyntax")]
    [BoolCompilerCommand("-$X+", "-$X-", BoolSwitchDefault.Enabled)]
    public bool ExtendedSyntax
    {
      get { return FExtendedSyntax; }
      set { FExtendedSyntax = value; }
    }

    private bool FSymbolRef = false;

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Symbol reference info", PropertyName = "DelphiSymbolRef")]
    [BoolCompilerCommand("-$Y+", "-$Y-", BoolSwitchDefault.Enabled)]
    public bool SymbolRef
    {
      get { return FSymbolRef; }
      set { FSymbolRef = value; }
    }

    protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
    {
      if (singleLine.Contains("Hint:"))
      {
        base.Log.LogWarning(singleLine, new object[0]);
      }
      if (singleLine.Contains("Warning:"))
      {
        base.Log.LogWarning(singleLine, new object[0]);
      }
      if (singleLine.Contains("Error:"))
      {
        base.Log.LogError(singleLine, new object[0]);
      }
      if (singleLine.Contains("Fatal:"))
      {
        base.Log.LogError(singleLine, new object[0]);
      }
      if (singleLine.IndexOf("Syntax:") == 0)
      {
        base.Log.LogError(singleLine, new object[0]);
      }
    }

    protected override string GenerateFullPathToTool()
    {
      // because this DLL is for the spacific version of DCC
      // we can get the path from the registry and not use a env var.
      // HKEY_CURRENT_USER\Software\Borland\Delphi\7.0
      string lRootPath = @"Software\Borland\Delphi\7.0";
      string lPath = GetRegistryValue(lRootPath, "RootDir");
      if (lPath != "")
        lPath = System.IO.Path.Combine(lPath, "Bin");
      return Environment.ExpandEnvironmentVariables(System.IO.Path.Combine(lPath, @"DCC32.EXE"));
    }

    private string GetRegistryValue(string aKey, string aName)
    {
      string lResult = "";
      RegistryKey lRegKey = Registry.CurrentUser.OpenSubKey(aKey, false);
      if (lRegKey != null)
      {
        try
        {
          lResult = (string)lRegKey.GetValue(aName);
        }
        catch
        {
          lResult = "";
        }
        lRegKey.Close();
      }
      return lResult;
    }


    protected override bool SkipTaskExecution()
    {
      return false;
    }

    protected override string ToolName
    {
      get
      {
        return "DCC32";
      }
    }
  }
}
