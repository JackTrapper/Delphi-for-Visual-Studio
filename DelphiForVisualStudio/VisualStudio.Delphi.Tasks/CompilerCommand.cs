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

namespace VisualStudio.Delphi.Tasks
{
  [AttributeUsage(AttributeTargets.Property)]
  public class CompilerCommandAttribute : Attribute
  {
    private string FSwitch = null;
    private string FDefaultSwitch = null;
    private bool FAppendValue = false;
    private bool FQuotedValue = true;
    private bool FUseSingleItem = false;
    private string FDelimiter = ";";

    public CompilerCommandAttribute(string aSwitch)
    {
      FSwitch = aSwitch;
    }

    /// <summary>
    /// Compiler command line option that will be used if the value exists.
    /// </summary>
    public string Switch
    {
      get { return FSwitch; }
    }

    /// <summary>
    /// Value of the property is appended to the end of the compiler option. (Example: -i"C:\Test" or -i"true" )
    /// </summary>
    public bool AppendValue
    {
      get { return FAppendValue; }
      set { FAppendValue = value; }
    }

    /// <summary>
    /// If the property is a ITaskItem[] only the first item is used.
    /// </summary>
    public bool UseSingleItem
    {
      get { return FUseSingleItem; }
      set { FUseSingleItem = value; }
    }

    /// <summary>
    /// For each item in an ITaskItem[] is separated by a ";" if the property is not set.
    /// </summary>
    public string Delimiter
    {
      get { return FDelimiter; }
      set 
      {
        if (value == null)
          FDelimiter = ";";
        else
          FDelimiter = value; 
      }
    }

    /// <summary>
    /// If the value of the property is appeneded it is appened with quotes 
    /// </summary>
    public bool QuotedValue
    {
      get { return FQuotedValue; }
      set { FQuotedValue = value; }
    }
  }

  public enum BoolSwitchDefault { Disabled, Enabled }

  public class BoolCompilerCommandAttribute : CompilerCommandAttribute
  {
    private string FDisabledSwitch;
    private BoolSwitchDefault FSwitchDefault;

    public BoolCompilerCommandAttribute(string aEnabledSwitch, string aDisabledSwitch, BoolSwitchDefault aSwitchDefault)
      : base(aEnabledSwitch)
    {
      FDisabledSwitch = aDisabledSwitch;
      FSwitchDefault = aSwitchDefault;
    }

    public BoolCompilerCommandAttribute(string aEnabledSwitch)
      : base(aEnabledSwitch)
    {
      FDisabledSwitch = null;
      FSwitchDefault = BoolSwitchDefault.Disabled;
    }

    public BoolSwitchDefault SwitchDefault
    {
      get { return FSwitchDefault; }
    }

    public string EnabledSwitch
    {
      get { return Switch; }
    }

    public string DisabledSwitch
    {
      get { return FDisabledSwitch; }
    }
  }

}  

