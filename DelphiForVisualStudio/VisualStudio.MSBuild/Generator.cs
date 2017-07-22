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
using System.Reflection;
using System.Xml;

namespace VisualStudio.MSBuild.XSD
{
  public class Generator
  {
    const string MAINFILE = "Microsoft.Build.xsd";
    PropertyDefinitionList FPropertyList = new PropertyDefinitionList();
    ItemDefinitionList FItemList = new ItemDefinitionList();
    TaskDefinitionList FTaskList = new TaskDefinitionList();

    public void GenerateXSD(string aAssemblyFileName, string aMSBuildXSDFileName)
    {
      Assembly lMSBuildDefAsm;
      // the assembly file name must be in the form of ... "Assembly text name, Version, Culture, PublicKeyToken"            
      lMSBuildDefAsm = Assembly.Load(aAssemblyFileName);
      object[] lAttributes = lMSBuildDefAsm.GetCustomAttributes(false);
      Type[] lTypes = lMSBuildDefAsm.GetTypes();
      // Gets all properties and lists
      GetMSBuildProperties(lAttributes, FPropertyList, FItemList);
      // gets all tasks and parameters for each task
      GetMSBuildTasks(lTypes, FTaskList);


      XmlDocument lDocument = new XmlDocument();
      lDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?> " +
                        "<xs:schema targetNamespace=\"http://schemas.microsoft.com/developer/msbuild/2003\" " +
                        "xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" " +
                        "xmlns:msb=\"http://schemas.microsoft.com/developer/msbuild/2003\" " +
                        "elementFormDefault=\"qualified\"> </xs:schema>");
      XmlNode lRootNode = lDocument.FirstChild;
      //lDocument.CreateComment("Auto Generated File");
      foreach (PropertyDefinitionAttribute lProperty in this.FPropertyList)
        AddPropertyNode(lRootNode, lProperty);
      foreach (ItemDefinitionContainer lItemContainer in this.FItemList)
        AddItemNode(lRootNode, lItemContainer);
      foreach (TaskDefinitionContainer lTaskContainer in this.FTaskList)
        AddTaskNode(lRootNode, lTaskContainer);
      AssemblyName lAssemblyName = new AssemblyName(aAssemblyFileName);
      lDocument.Save(aMSBuildXSDFileName);
    }

    public void AddPropertyNode(XmlNode aRootNode, PropertyDefinitionAttribute aProperty)
    {
      XmlNode lNode = aRootNode.OwnerDocument.CreateNode("element", "xs:element", "");
      aRootNode.AppendChild(lNode);
      AddAttribute(lNode, "name", aProperty.PropertyName);
      AddAttribute(lNode, "type", "msb:StringPropertyType");
      AddAttribute(lNode, "substitutionGroup", "msb:Property");
      AddAnnotationNode(lNode, aProperty.Description);
    }

    private void AddAnnotationNode(XmlNode aMSBuildNode, string aDescription)
    {
      if (!String.IsNullOrEmpty(aDescription))
      {
        XmlNode lAnnotationNode = aMSBuildNode.OwnerDocument.CreateNode("element", "xs:annotation", "");
        XmlNode lDocNode = aMSBuildNode.OwnerDocument.CreateNode("element", "xs:documentation", "");
        lDocNode.Value = aDescription;
        lAnnotationNode.AppendChild(lDocNode);
        aMSBuildNode.AppendChild(lAnnotationNode);
      }
    }

    private void AddAttribute(XmlNode aNode, string aName, string aValue)
    {
      XmlAttribute lAttribute = aNode.OwnerDocument.CreateAttribute(aName);
      lAttribute.Value = aValue;
      aNode.Attributes.Append(lAttribute);
    }

    public void AddItemNode(XmlNode aRootNode, ItemDefinitionContainer aItemContainer)
    {
      XmlNode lNode = aRootNode.OwnerDocument.CreateNode("element", "xs:element", "");
      aRootNode.AppendChild(lNode);
      AddAttribute(lNode, "name", aItemContainer.MSBuildItem.PropertyName);
      AddAttribute(lNode, "substitutionGroup", "msb:Item");
      AddAnnotationNode(lNode, aItemContainer.MSBuildItem.Description);
      AddExtensionNodes(lNode, aItemContainer);
    }

    private void AddExtensionNodes(XmlNode aMSBuildNode, ItemDefinitionContainer aItemContainer)
    {
      if (aItemContainer.ItemMetadataDefinitionList.Count == 0) return;
      // Create each node and append tem as follows
      //   <xs:complexType>
      //     <xs:complexContent>
      //       <xs:extension base="msb:SimpleItemType">
      //         <xs:sequence minOccurs=0 maxOccurs="unbounded">
      //           <xs:choice>
      XmlNode lComplexType = aMSBuildNode.OwnerDocument.CreateNode("element", "xs:complexType", "");
      XmlNode lComplexContent = aMSBuildNode.OwnerDocument.CreateNode("element", "xs:complexContent", "");
      XmlNode lExtension = aMSBuildNode.OwnerDocument.CreateNode("element", "xs:extension", "");
      XmlNode lSequence = aMSBuildNode.OwnerDocument.CreateNode("element", "xs:sequence", "");
      XmlNode lChoice = aMSBuildNode.OwnerDocument.CreateNode("element", "xs:choice", "");
      aMSBuildNode.AppendChild(lComplexType);
      lComplexType.AppendChild(lComplexContent);
      lComplexContent.AppendChild(lExtension);
      lExtension.AppendChild(lSequence);
      lSequence.AppendChild(lChoice);
      AddAttribute(lExtension, "base", "msb:SimpleItemType");
      AddAttribute(lSequence, "minOccurs", "0");
      AddAttribute(lSequence, "maxOccurs", "unbounded");
      // Build the meta data elements 
      //   <xs:element name="MyMetadata" type="xs:string"/>
      foreach (ItemMetadataDefinitionAttribute lMetadata in aItemContainer.ItemMetadataDefinitionList)
        AddChoiceElement(lChoice, lMetadata);
      // Add the include documentation if need.
      if (!String.IsNullOrEmpty(aItemContainer.MSBuildItem.IncludeDescription))
        AddIncludeAnnotation(lSequence, aItemContainer.MSBuildItem.IncludeDescription);
    }

    private void AddChoiceElement(XmlNode aChoiceNode, ItemMetadataDefinitionAttribute aItemMetadataAttribute)
    {
      XmlNode lElement = aChoiceNode.OwnerDocument.CreateNode("element", "xs:element", "");
      aChoiceNode.AppendChild(lElement);
      AddAttribute(lElement, "name", aItemMetadataAttribute.MetadataName);
      AddAnnotationNode(lElement, aItemMetadataAttribute.Description);
    }

    private void AddIncludeAnnotation(XmlNode aSequenceNode, string aDesciption)
    {
      if (!String.IsNullOrEmpty(aDesciption))
      {
        XmlNode lAttributeNode = aSequenceNode.OwnerDocument.CreateNode("element", "xs:attribute", "");
        aSequenceNode.AppendChild(lAttributeNode);
        AddAttribute(lAttributeNode, "name", "Include");
        AddAttribute(lAttributeNode, "type", "xs:string");
        AddAttribute(lAttributeNode, "use", "required");
        AddAnnotationNode(lAttributeNode, aDesciption);
      }
    }

    public void AddTaskNode(XmlNode aRootNode, TaskDefinitionContainer aTaskContainer)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    //     XmlNode lChildNode = lDocument.CreateNode("element", "xs:element", "");

    //     XmlAttribute lAttribute = lDocument.CreateAttribute("name");
    //     lAttribute.Value = aName;
    //     lAttribute = lDocument.CreateAttribute("type");
    //     lAttribute.Value 
    //     lChildNode.Attributes.Append();
    ////    <!--<xs:element name="MySimpleProperty" type="msb:StringPropertyType" substitutionGroup="msb:Property"/>-->
    //   // <!--<xs:element name="MyItem" substitutionGroup="msb:Item">
    //       // <xs:annotation>
    //       //    <xs:documentation><!-- _locID_text="Reference" _locComment="" -->Reference to an assembly</xs:documentation>
    //       //</xs:annotation>
    //  //    <xs:complexType>
    //   //        <xs:complexContent>
    //   //            <xs:extension base="msb:SimpleItemType">
    //   //                <xs:sequence maxOccurs="1">
    //   //                    <xs:choice>
    //   //                        <xs:element name="MyMetadata" type="xs:string"/>
    //   //                    </xs:choice>
    //   //                </xs:sequence>
    //   //            </xs:extension>
    //   //        </xs:complexContent>
    //   //    </xs:complexType>
    //   //</xs:element>-->
    //<xs:element name="AL" substitutionGroup="msb:Task">
    //   <xs:complexType>
    //       <xs:complexContent>
    //           <xs:extension base="msb:TaskType">
    //               <xs:attribute name="AlgorithmId" />
    //               <xs:attribute name="BaseAddress" />
    //               <xs:attribute name="CompanyName" />
    //                 <xs:attribute name="GenerateFullPaths" type="msb:boolean" />
    //               <xs:attribute name="KeyContainer" />
    //               <xs:attribute name="KeyFile" />
    //               <xs:attribute name="LinkResources" />
    //               <xs:attribute name="MainEntryPoint" />
    //               <xs:attribute name="OutputAssembly" use="required" />



    //   }

    public void GetMSBuildTasks(Type[] aTypeArray, TaskDefinitionList aTaskList)
    {
      foreach (Type lType in aTypeArray)
        if (lType.IsClass)
        {
          object[] lObjects = lType.GetCustomAttributes(typeof(TaskDefinitionAttribute), true);
          if (lObjects != null || lObjects.Length > 0)
          {
            TaskDefinitionContainer lContainer = new TaskDefinitionContainer((TaskDefinitionAttribute)lObjects[0]);
            GetTaskParameters(lType, lContainer);
            aTaskList.Add(lContainer);
          }
        }
    }

    private void GetTaskParameters(Type aType, TaskDefinitionContainer aContainer)
    {
      PropertyInfo[] lProperties = aType.GetProperties();
      foreach (PropertyInfo lProperty in lProperties)
      {
        object[] lObjects = lProperty.GetCustomAttributes(typeof(TaskParameterDefinitionAttribute), true);
        if (lObjects != null || lObjects.Length > 0)
        {
          aContainer.TaskParameterDefinitionList.Add((TaskParameterDefinitionAttribute)lObjects[0]);
        }
      }
    }

    public void GetMSBuildProperties(object[] aAssemblyAttributes, PropertyDefinitionList aPropertyList, ItemDefinitionList aItemList)
    {
      if (aPropertyList == null)
        throw new ArgumentNullException("PropertyList parameter is null");
      if (aItemList == null)
        throw new ArgumentNullException("ItemList parameter is null");
      if (aAssemblyAttributes == null)
        return;
      Dictionary<string, ItemDefinitionContainer> lItemContainerDictionary = new Dictionary<string, ItemDefinitionContainer>();
      List<ItemMetadataDefinitionAttribute> lItemMetadataList = new List<ItemMetadataDefinitionAttribute>();
      foreach (object lObj in aAssemblyAttributes)
      {
        PropertyDefinitionAttribute lProperty = lObj as PropertyDefinitionAttribute;
        ItemDefinitionAttribute lItem = lObj as ItemDefinitionAttribute;
        ItemMetadataDefinitionAttribute lItemMetadata = lObj as ItemMetadataDefinitionAttribute;
        if (lProperty != null)
          aPropertyList.Add(lProperty);
        if (lItem != null)
        {
          ItemDefinitionContainer lContainer = new ItemDefinitionContainer(lItem);
          aItemList.Add(lContainer);
          lItemContainerDictionary.Add(lItem.PropertyName, lContainer);
        }
        if (lItemMetadata != null)
          lItemMetadataList.Add(lItemMetadata);
      }
      // now add all the meta data with the ItemGroup 
      foreach (ItemMetadataDefinitionAttribute lItemMetadata in lItemMetadataList)
        if (lItemContainerDictionary.ContainsKey(lItemMetadata.PropertyName))
          lItemContainerDictionary[lItemMetadata.PropertyName].ItemMetadataDefinitionList.Add(lItemMetadata);
    }

    public string GetMSBuildSchemaPath()
    {
      throw new Exception("The method or operation is not implemented.");
      //string lRootPath = Constants.DEFAULT_REGISTRY_ROOT;
      //string lPath = GetRegistryValue(lRootPath, "InstallDir");
    }
  }

}
