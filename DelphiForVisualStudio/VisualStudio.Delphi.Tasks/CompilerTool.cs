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
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace VisualStudio.Delphi.Tasks
{

  public abstract class TCompilerTool : ToolTask
  {
    private string FCommandLine = "";
    private bool FUseDefaultSwitches = false;

    protected string CommandLine
    {
      get { return FCommandLine; }
    }

    [VisualStudio.MSBuild.XSD.TaskParameterDefinition("Default switches can be added to the config or command line call. (bool)", DefineProperty = true, PropertyName = "DelphiUseDefautSwitches")]
    public bool UseDefautSwitches
    {
      get { return FUseDefaultSwitches; }
      set { FUseDefaultSwitches = value; }
    }

    /// <summary>
    /// Gets the command line commands this method shound not be overridden by child classes override AddCommandLines instead
    /// </summary>
    /// <returns></returns>
    protected override string GenerateCommandLineCommands()
    {
      this.FCommandLine = GetCommandLine();
      if (GenerateResponseFileCommands() != string.Empty)
        return String.Empty;
      return this.FCommandLine;
    }

    /// <summary>
    /// This method will assume you have used the CompilerCommandAttribute and build the command line 
    /// </summary>
    /// <param name="aCommandLineBuilder"></param>
    protected virtual void AddCommandLines(CommandLineBuilder aCommandLineBuilder)
    {
      // get the type for this object
      Type lMyType = this.GetType();
      // get all properties of this object
      PropertyInfo[] lPropertyInfos = lMyType.GetProperties();
      // check each property for the CompilerCommandAttribute
      foreach (PropertyInfo lPropertyInfo in lPropertyInfos)
      {
        if (!lPropertyInfo.CanRead) continue;

        object[] lObjects = lPropertyInfo.GetCustomAttributes(typeof(CompilerCommandAttribute), true);
        if (lObjects != null && lObjects.Length > 0)
        {
          // if there is a compiler command we will add a switch if need
          CompilerCommandAttribute lCommand = (CompilerCommandAttribute)lObjects[0];
          // we need to get the property value of the switch
          object lValue = lPropertyInfo.GetGetMethod().Invoke(this, null);
          AddCommand(aCommandLineBuilder, lCommand, lValue);
        }
      }
    }

    protected virtual void AddCommand(CommandLineBuilder aCommandLineBuilder, CompilerCommandAttribute aCommand, object aValue)
    {
      // if we are not appending values to the switch then we
      // will simply add the switch 
      if (!aCommand.AppendValue)
      {
        string lSwitch = aCommand.Switch;

        // get boolean switch value
        BoolCompilerCommandAttribute lBoolCommand = aCommand as BoolCompilerCommandAttribute;
        if (lBoolCommand != null)
        {
          bool lOption = (bool)aValue;
          if (!lOption)
            lSwitch = lBoolCommand.DisabledSwitch;
          // switch is not used in for some compilers 
          if (!UseDefautSwitches && (lOption == (lBoolCommand.SwitchDefault == BoolSwitchDefault.Enabled)))
            lSwitch = null;
        }

        // Add the switch if one exists and is not default when defaults should not be dislpayed
        if (!String.IsNullOrEmpty(lSwitch))
          aCommandLineBuilder.AppendSwitch(lSwitch);
      }
      else
      {
        // This will append the value of the property to the switch
        if (aValue is ITaskItem[])
        {
          ITaskItem[] lTaskItems = aValue as ITaskItem[];
          AppendTaskItemsToSwitch(aCommandLineBuilder, lTaskItems, aCommand);
        }
        else
          AppendValueToSwitch(aCommandLineBuilder, aValue, aCommand);
      }
    }

    private void AppendValueToSwitch(CommandLineBuilder aCommandLineBuilder, object aValue, CompilerCommandAttribute aCommand)
    {
      string lValueString = null;
      if (aValue != null)
        lValueString = aValue.ToString();
      if (aCommand.Switch != null)
      {
        if (aCommand.QuotedValue)
          aCommandLineBuilder.AppendSwitchIfNotNull(aCommand.Switch, lValueString);
        else
          aCommandLineBuilder.AppendSwitchUnquotedIfNotNull(aCommand.Switch, lValueString);
      }
    }

    private void AppendTaskItemsToSwitch(CommandLineBuilder aCommandLineBuilder, ITaskItem[] aTaskItems, CompilerCommandAttribute aCommand)
    {
      if (aCommand.QuotedValue)
      {
        if (!aCommand.UseSingleItem)
        {
          if (String.IsNullOrEmpty(aCommand.Switch))
          {
            aCommandLineBuilder.AppendFileNamesIfNotNull(aTaskItems, aCommand.Delimiter);
          }
          else // delphi 7 does not support -Switch"Path Name";"Path Name" so use GetItems method instead
            aCommandLineBuilder.AppendSwitchIfNotNull(aCommand.Switch, GetItems(aTaskItems, aCommand.Delimiter));
        }
        else
        {
          if (String.IsNullOrEmpty(aCommand.Switch))
            aCommandLineBuilder.AppendFileNameIfNotNull(aTaskItems[0]);
          else
            aCommandLineBuilder.AppendSwitchIfNotNull(aCommand.Switch, aTaskItems[0]);
        }
      }
      else
      {
        if (!aCommand.UseSingleItem)
        {
          if (String.IsNullOrEmpty(aCommand.Switch))
            aCommandLineBuilder.AppendSwitchUnquotedIfNotNull("", aTaskItems, aCommand.Delimiter);
          else
            aCommandLineBuilder.AppendSwitchUnquotedIfNotNull(aCommand.Switch, aTaskItems, aCommand.Delimiter);
        }
        else
        {
          if (String.IsNullOrEmpty(aCommand.Switch))
            aCommandLineBuilder.AppendSwitchUnquotedIfNotNull("", aTaskItems[0]);
          else
            aCommandLineBuilder.AppendSwitchUnquotedIfNotNull(aCommand.Switch, aTaskItems[0]);
        }
      }
    }

    private string GetItems(ITaskItem[] aTaskItems, string aDelimiter)
    {
      string lResult = "";
      if (aTaskItems != null)
        foreach (ITaskItem lItem in aTaskItems)
          lResult += lItem.ItemSpec + aDelimiter;
      if (lResult != "")
        lResult = lResult.Substring(0, lResult.Length - aDelimiter.Length);
      else
        lResult = null;
      return lResult;
    }

    protected string GetCommandLine()
    {
      CommandLineBuilder lCommandLineBuilder = new CommandLineBuilder();
      this.AddCommandLines(lCommandLineBuilder);
      return lCommandLineBuilder.ToString();
    }
  }

}