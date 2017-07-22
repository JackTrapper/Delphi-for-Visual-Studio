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

// Example of defining MSbuild properties for you .Targets file and .Proj file
//[assembly: MSBuildProperty("DelphiMainSource")]
//[assembly: MSBuildItem("DelphiCompile")]
//[assembly: MSBuildItemMetadata("IsForm","DelphiCompile")]

namespace VisualStudio.MSBuild.XSD
{
  public enum ValueType { Unknown, Boolean, Integer, @String, Path, File, PathList, FileList };

  /// <summary>
  /// Assign this attribute to the class that is a task and provide a short description.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class TaskDefinitionAttribute : Attribute
  {
    private string FDescription = "";


    public TaskDefinitionAttribute(string aDescription)
    {
      FDescription = aDescription;
    }

    /// <summary>
    /// Short description of the task and what it does.
    /// </summary>
    public string Description
    {
      get { return FDescription; }
    }

  }

  /// <summary>
  /// Add this attribute to a input or output property to describe it's usage.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class TaskParameterDefinitionAttribute : Attribute
  {
    private string FDescription = "";


    public TaskParameterDefinitionAttribute(string aDescription)
    {
      FDescription = aDescription;
    }

    /// <summary>
    /// A short description of the value the task parameter expects.
    /// </summary>
    public string Description
    {
      get { return FDescription; }
    }

    private bool FDefineProperty;

    public bool DefineProperty
    {
      get { return FDefineProperty; }
      set { FDefineProperty = value; }
    }

    private string FPropertyName;

    public string PropertyName
    {
      get { return FPropertyName; }
      set { FPropertyName = value; }
    }

  }





  /// <summary>
  /// Base class of a MSBuild p
  /// </summary>
  public abstract class BaseDefinitionAttribute : Attribute
  {
    private string FDescription = "";
    private ValueType FDataType = ValueType.Unknown;
    private string FName = null;

    protected void ValidateName(string aName)
    {
      if (String.IsNullOrEmpty(aName))
        throw new ArgumentNullException("Property name can not be null or empty");
      System.Text.RegularExpressions.Match lMatch = System.Text.RegularExpressions.Regex.Match(aName, "[a-zA-Z_]([a-zA-Z0-9_])*");
      if (lMatch == null || !lMatch.Success || lMatch.ToString() != aName)
        throw new ArgumentException(String.Format("'{0}' is an invalid property name.", aName));
    }

    public BaseDefinitionAttribute(string aPropertyName)
    {
      ValidateName(aPropertyName);
      FName = aPropertyName;
    }

    public BaseDefinitionAttribute(string aPropertyName, string aDescription, ValueType aDataType)
    {
      FName = aPropertyName;
      FDescription = aDescription;
      FDataType = aDataType;
    }

    /// <summary>
    /// Name of the property used in the MSBuild project file or target file.
    /// </summary>
    public string PropertyName
    {
      get { return FName; }
    }

    /// <summary>
    /// A short description of the value the task parameter expects.
    /// </summary>
    public string Description
    {
      get { return FDescription; }
      set { FDescription = value; }
    }

    /// <summary>
    /// The type of data the may be found in the property.  This is not entirly used by the XSD generator but is used by the IDE.
    /// </summary>
    public virtual ValueType DataType
    {
      get { return FDataType; }
      set { FDataType = value; }
    }

    private bool FIsIDESetting = false;


    public bool IsIDESetting
    {
      get { return FIsIDESetting; }
      set { FIsIDESetting = value; }
    }
  }


  /// <summary>
  /// Use this assembly attribute to define a MSBuid property (PropetyGroup) for generating an XSD file.
  /// </summary>
  /// <remarks>
  /// MSBuild properties unlike ItemGroup (ITaksItem[]) only holds one value and not a list.  If the
  /// DataType is a ValueType.FileList or a ValueType.PathList it is assumed that the value of the 
  /// property is one large string with each path or file separated by a ';'.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  public sealed class PropertyDefinitionAttribute : BaseDefinitionAttribute
  {
    public PropertyDefinitionAttribute(string aPropertyName)
      : base(aPropertyName)
    {
    }

    public PropertyDefinitionAttribute(string aPropertyName, string aDescription, ValueType aDataType)
      : base(aPropertyName, aDescription, aDataType)
    {
    }
  }

  /// <summary>
  /// Use this assembly attribute to define a MSBuid item property (ItemGroup) for generating an XSD file.
  /// </summary>
  /// <remarks>
  /// The array of ITaskItems may require only one item if this is the case the DataType sould be ValueType.File or
  /// ValueType.Path to document this fact.  Also if the DataType is Unknown the 'Include' is not a path or a file.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Assembly)]
  public sealed class ItemDefinitionAttribute : BaseDefinitionAttribute
  {
    private string FIncludeDescription = null;

    public ItemDefinitionAttribute(string aPropertyName)
      : base(aPropertyName)
    { }

    public ItemDefinitionAttribute(string aPropertyName, string aDescription, ValueType aDataType, string aIncludeDescription)
      : base(aPropertyName, aDescription, aDataType)
    {
      IncludeDescription = aIncludeDescription;
    }

    /// <summary>
    /// The type of data the may be found in the property.
    /// </summary>
    public override ValueType DataType
    {
      get { return base.DataType; }
      set
      {
        if (value != ValueType.Unknown && value != ValueType.File && value != ValueType.FileList && value != ValueType.Path && value != ValueType.PathList)
          throw new ArgumentException("ValueType is invalid for MSBuldItem attribute.");
        base.DataType = value;
      }
    }

    /// <summary>
    /// The include desciption describes the path type if not provided the DataType property is used.
    /// </summary>
    public string IncludeDescription
    {
      get { return FIncludeDescription; }
      set { FIncludeDescription = value; }
    }
  }

  /// <summary>
  /// Enables ITaskItem's custom meta data to be described in a generated XSD.
  /// </summary>
  /// <remarks>
  /// See MSBuildItemAttribute remarks regarding DataType.
  /// </remarks>
  public sealed class ItemMetadataDefinitionAttribute : BaseDefinitionAttribute
  {
    private string FMetadataName = null;

    public ItemMetadataDefinitionAttribute(string aMetadataName, string aPropertyName)
      : base(aPropertyName)
    {
      ValidateName(aMetadataName);
      FMetadataName = aMetadataName;
    }
    public ItemMetadataDefinitionAttribute(string aMetadataName, string aPropertyName, string aDescription, ValueType aDataType)
      : base(aPropertyName, aDescription, aDataType)
    {
      ValidateName(aMetadataName);
      FMetadataName = aMetadataName;
    }

    /// <summary>
    /// Name of metadata that is connected with the ITaskItem
    /// </summary>
    public string MetadataName
    {
      get { return FMetadataName; }
    }
  }


}
