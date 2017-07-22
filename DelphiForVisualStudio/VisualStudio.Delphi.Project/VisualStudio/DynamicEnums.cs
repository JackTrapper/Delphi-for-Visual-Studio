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
using System.Collections.Generic;
using System.Collections;
using System.Globalization;

namespace VisualStudio
{
  public class DynamicEnumConverter : TypeConverter
  {
    private DynamicEnum FDynamicEnum;
    private Type FDynamicEnumType;
    private List<DynamicEnum> FEnumList = null;
    // Summary:
    //     Initializes a new instance of the System.ComponentModel.EnumConverter class
    //     for the given type.
    //
    // Parameters:
    //   type:
    //     A System.Type that represents the type of enumeration to associate with this
    //     enumeration converter.
    public DynamicEnumConverter(Type aType)
    {
      FDynamicEnumType = aType;
      FDynamicEnum = CreateNewDynamicEnum();
    }

    private ICollection GetCollection()
    {
      if (FEnumList == null)
      {
        FEnumList = new List<DynamicEnum>();
        foreach (DynamicEnum lDynamicEnum in FDynamicEnum)
          FEnumList.Add(lDynamicEnum);
      }
      return FEnumList;
    }

    private DynamicEnum CreateNewDynamicEnum()
    {
      return (DynamicEnum)Activator.CreateInstance(this.FDynamicEnumType);
    }

    // Summary:
    //     Gets an System.Collections.IComparer that can be used to sort the values
    //     of the enumeration.
    //
    // Returns:
    //     An System.Collections.IComparer for sorting the enumeration values.
    protected virtual System.Collections.IComparer Comparer
    {
      get { return null; }
    }

    // Summary:
    //     Gets or sets a System.ComponentModel.TypeConverter.StandardValuesCollection
    //     that specifies the possible values for the enumeration.
    //
    // Returns:
    //     A System.ComponentModel.TypeConverter.StandardValuesCollection that specifies
    //     the possible values for the enumeration.
    protected TypeConverter.StandardValuesCollection Values
    {
      get { return new TypeConverter.StandardValuesCollection(this.GetCollection()); }
      set { /*the list is read only way*/ }
    }

    // Summary:
    //     Gets a value indicating whether this converter can convert an object in the
    //     given source type to an enumeration object using the specified context.
    //
    // Parameters:
    //   context:
    //     An System.ComponentModel.ITypeDescriptorContext that provides a format context.
    //
    //   sourceType:
    //     A System.Type that represents the type you wish to convert from.
    //
    // Returns:
    //     true if this converter can perform the conversion; otherwise, false.
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(string)) return true;

      return base.CanConvertFrom(context, sourceType);
    }

    // Summary:
    //     Gets a value indicating whether this converter can convert an object to the
    //     given destination type using the context.
    //
    // Parameters:
    //   context:
    //     An System.ComponentModel.ITypeDescriptorContext that provides a format context.
    //
    //   destinationType:
    //     A System.Type that represents the type you wish to convert to.
    //
    // Returns:
    //     true if this converter can perform the conversion; otherwise, false.
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == FDynamicEnumType) return true;
      return base.CanConvertTo(context, destinationType);
    }

    // Summary:
    //     Converts the specified value object to an enumeration object.
    //
    // Parameters:
    //   culture:
    //     An optional System.Globalization.CultureInfo. If not supplied, the current
    //     culture is assumed.
    //
    //   context:
    //     An System.ComponentModel.ITypeDescriptorContext that provides a format context.
    //
    //   value:
    //     The System.Object to convert.
    //
    // Returns:
    //     An System.Object that represents the converted value.
    //
    // Exceptions:
    //   System.NotSupportedException:
    //     The conversion cannot be performed.
    //
    //   System.FormatException:
    //     value is not a valid value for the target type.
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      string str = value as string;
      if (str != null)
      {
        DynamicEnum lResult = CreateNewDynamicEnum();

        lResult.EnumeratorItem = str;
        return lResult;
      }

      return base.ConvertFrom(context, culture, value);
    }

    // Summary:
    //     Converts the given value object to the specified destination type.
    //
    // Parameters:
    //   culture:
    //     An optional System.Globalization.CultureInfo. If not supplied, the current
    //     culture is assumed.
    //
    //   context:
    //     An System.ComponentModel.ITypeDescriptorContext that provides a format context.
    //
    //   destinationType:
    //     The System.Type to convert the value to.
    //
    //   value:
    //     The System.Object to convert.
    //
    // Returns:
    //     An System.Object that represents the converted value.
    //
    // Exceptions:
    //   System.ArgumentException:
    //     value is not a valid value for the enumeration.
    //
    //   System.ArgumentNullException:
    //     destinationType is null.
    //
    //   System.NotSupportedException:
    //     The conversion cannot be performed.
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        if (value != null)
        {
          return value.ToString();
        }
        else
        {
          return CreateNewDynamicEnum().ToString();
        }
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    // Summary:
    //     Gets a collection of standard values for the data type this validator is
    //     designed for.
    //
    // Parameters:
    //   context:
    //     An System.ComponentModel.ITypeDescriptorContext that provides a format context.
    //
    // Returns:
    //     A System.ComponentModel.TypeConverter.StandardValuesCollection that holds
    //     a standard set of valid values, or null if the data type does not support
    //     a standard set of values.
    public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
      return new TypeConverter.StandardValuesCollection(this.GetCollection());
    }

    // Summary:
    //     Gets a value indicating whether the list of standard values returned from
    //     System.ComponentModel.TypeConverter.GetStandardValues() is an exclusive list
    //     using the specified context.
    //
    // Parameters:
    //   context:
    //     An System.ComponentModel.ITypeDescriptorContext that provides a format context.
    //
    // Returns:
    //     true if the System.ComponentModel.TypeConverter.StandardValuesCollection
    //     returned from System.ComponentModel.TypeConverter.GetStandardValues() is
    //     an exhaustive list of possible values; false if other values are possible.
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
    {
      return false;
    }

    // Summary:
    //     Gets a value indicating whether this object supports a standard set of values
    //     that can be picked from a list using the specified context.
    //
    // Parameters:
    //   context:
    //     An System.ComponentModel.ITypeDescriptorContext that provides a format context.
    //
    // Returns:
    //     true because System.ComponentModel.TypeConverter.GetStandardValues() should
    //     be called to find a common set of values the object supports. This method
    //     never returns false.
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    // Summary:
    //     Gets a value indicating whether the given object value is valid for this
    //     type.
    //
    // Parameters:
    //   context:
    //     An System.ComponentModel.ITypeDescriptorContext that provides a format context.
    //
    //   value:
    //     The System.Object to test.
    //
    // Returns:
    //     true if the specified value is valid for this object; otherwise, false.
    public override bool IsValid(ITypeDescriptorContext context, object value)
    {
      return this.FDynamicEnum.IsEnumeratorItem((string)value);
    }
  }

  /// <summary>
  /// Abstract dynamic enumeration object to provide list of items that may change depending on context.
  /// </summary>
  public abstract class DynamicEnum : IEnumerable<DynamicEnum>
  {
    private string FEnumeratorItem = null;
    private Type FDynamicEnumType;
    private const string NULL_Enum = "Enumerator items can not be empty string or null."; 

    public DynamicEnum(Type aDynamicEnumType)
    {
      IEnumerator<string> lEnum = GetEnumerable().GetEnumerator();
      if (lEnum.Current == null)
        lEnum.MoveNext();
      this.FEnumeratorItem = lEnum.Current;
      if (String.IsNullOrEmpty(FEnumeratorItem))
        throw new ArgumentNullException(DynamicEnum.NULL_Enum);
      FDynamicEnumType = aDynamicEnumType;
    }

    protected virtual DynamicEnum CreateNewDynamicEnum(string aEnumeratorItem)
    {
      DynamicEnum lResult = (DynamicEnum)Activator.CreateInstance(this.FDynamicEnumType);
      lResult.EnumeratorItem = aEnumeratorItem;
      return lResult;
    }

    protected abstract IEnumerable<string> GetEnumerable();

    public override string ToString()
    {
      return EnumeratorItem;
    }

    public bool IsEnumeratorItem(string aItem)
    {
      if (String.IsNullOrEmpty(aItem))
        throw new ArgumentNullException(DynamicEnum.NULL_Enum);
      IEnumerable<string> lList = GetEnumerable();
      if (lList == null)
        throw new ArgumentNullException("Call to GetEnumerable() returned null");
      bool lFound = false;
      foreach (string lItem in lList)
      {
        lFound = (lItem == aItem);
        if (lFound) break;
      }
      return lFound;
    }

    public virtual string EnumeratorItem
    {
      get
      {
        if (string.IsNullOrEmpty(FEnumeratorItem))
          throw new Exception("DynamicEnum is not assigned a value.");
        return FEnumeratorItem;
      }
      set
      {
        if (!IsEnumeratorItem(value))
          throw new ArgumentException(String.Format("Item '{0}' does not exist in Enumeration.",value));
        FEnumeratorItem = value;
      }
    }

    private class SelfEnumerator : IEnumerator<DynamicEnum>
    {
      DynamicEnum FDynamicEnum;
      IEnumerator<string> FStringEnumerator;

      public SelfEnumerator(DynamicEnum aSelf)
      {
        FDynamicEnum = aSelf;
        FStringEnumerator = FDynamicEnum.GetEnumerable().GetEnumerator();
      }

      #region IEnumerator<DynamicEnum> Members

      public DynamicEnum Current
      {
        get 
        {
          return FDynamicEnum.CreateNewDynamicEnum(FStringEnumerator.Current);
        }
      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
        FStringEnumerator.Dispose();
        FStringEnumerator = null;
        FDynamicEnum = null;
      }

      #endregion

      #region IEnumerator Members

      object IEnumerator.Current
      {
        get { return this.Current; }
      }

      public bool MoveNext()
      {
        return FStringEnumerator.MoveNext();
      }

      public void Reset()
      {
        FStringEnumerator.Reset();
      }

      #endregion
    }

    #region IEnumerable<DynamicEnum> Members

    public IEnumerator<DynamicEnum> GetEnumerator()
    {
      return new SelfEnumerator(this);
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion
  }
}