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

namespace VisualStudio.MSBuild.XSD
{
  public class PropertyDefinitionList : List<PropertyDefinitionAttribute>
  { }


  public class ItemDefinitionList : List<ItemDefinitionContainer>
  { }

  public class TaskDefinitionList : List<TaskDefinitionContainer>
  { }

  public class ItemDefinitionContainer
  {
    private ItemDefinitionAttribute FItemDefinition;
    private List<ItemMetadataDefinitionAttribute> FItemMetadataDefinitionList;

    public ItemDefinitionContainer(ItemDefinitionAttribute aItem)
    {
      FItemDefinition = aItem;
      FItemMetadataDefinitionList = new List<ItemMetadataDefinitionAttribute>();
    }

    public ItemDefinitionAttribute MSBuildItem
    {
      get { return FItemDefinition; }
      set { FItemDefinition = value; }
    }

    public List<ItemMetadataDefinitionAttribute> ItemMetadataDefinitionList
    {
      get { return FItemMetadataDefinitionList; }
      set { FItemMetadataDefinitionList = value; }
    }
  }

  public class TaskDefinitionContainer
  {
    private List<TaskParameterDefinitionAttribute> FTaskParameterDefinitionList;
    private TaskDefinitionAttribute FTaskDefinition;

    public TaskDefinitionContainer(TaskDefinitionAttribute aTaskDefinitionAttribute)
    {
      FTaskDefinition = aTaskDefinitionAttribute;
      FTaskParameterDefinitionList = new List<TaskParameterDefinitionAttribute>();
    }

    public TaskDefinitionAttribute TaskDefinition
    {
      get { return FTaskDefinition; }
      set { FTaskDefinition = value; }
    }

    public List<TaskParameterDefinitionAttribute> TaskParameterDefinitionList
    {
      get { return FTaskParameterDefinitionList; }
      set { FTaskParameterDefinitionList = value; }
    }

  }



}
