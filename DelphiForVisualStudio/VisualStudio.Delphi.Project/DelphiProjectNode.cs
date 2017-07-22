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
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Package.Automation;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace VisualStudio.Delphi.Project     //VisualStudio.Delphi.Project
{
  /// <summary>
  /// The Delphi project node will have all properties the user wishes to change.
  /// The DPR file will be renamed to Entry Point File and have no properties.
  /// Renaming the Visual Studio project will rename the .DPR file and changes
  /// to the data in the DPR file will refreash the VS project file once saved.
  /// The hiearchy will be as follows...
  /// 
  /// (*.DPR) file = TEntryPointFileNode = Entry Point File (you can not rename or delete this node)
  /// (Folder Name) = FolderNode = a folder in the root of the project
  /// (Folder Name) = TLinkedFolderNode = Files that are in delphi search paths that will be links
  /// (*.PAS) file = TPascalFileNode = Code file nodes
  /// (*.DFM) file = TFormFileNode = Are Always sub-item of .PAS files
  /// </summary>
  [GuidAttribute(Constants.DelphiProjectNodeGUID)]
  class DelphiProjectNode : ProjectContainerNode
  {
    #region Constructors
    /// <summary>
    /// Explicitly defined default constructor.
    /// </summary>
    public DelphiProjectNode()
    {
      this.CanFileNodesHaveChilds = true;
      this.SupportsProjectDesigner = true;
      // Add Category IDs mapping in order to support properties for project items
      AddCATIDMapping(typeof(DelphiFileNodeProprties), typeof(DelphiFileNodeProprties).GUID);
      AddCATIDMapping(typeof(DelphiDependentFileNodeProprties), typeof(DelphiDependentFileNodeProprties).GUID);
      AddCATIDMapping(typeof(DelphiProjectNodeProperties), typeof(DelphiProjectNodeProperties).GUID);
      //AddCATIDMapping(typeof(DelphiProjectProperties), typeof(DelphiProjectProperties).GUID);
      //AddCATIDMapping(typeof(FolderNodeProperties), typeof(FolderNodeProperties).GUID);
      // You will need this l8r...
      //this.AddCATIDMapping(typeof(GeneralPropertyPage), typeof(DelphiProjectProperties).GUID);
    }
    #endregion Constructors

    #region Properties
    /// <summary>
    /// Gets project's Guid value.
    /// </summary>
    public override Guid ProjectGuid
    {
      get
      {
        return typeof(DelphiProjectFactory).GUID;
      }
    }
    /// <summary>
    /// Gets project's type as string value.
    /// </summary>
    public override string ProjectType
    {
      get
      {
        return "Delphi";
      }
    }
    #endregion Properties

    #region Methods

    protected override NodeProperties CreatePropertiesObject()
    {
      return new DelphiProjectNodeProperties(this);
    }


    
    //public virtual IList<HierarchyNode> GetSelectedNodesForMoving()
    //{
    //  IList<HierarchyNode> lResult = base.GetSelectedNodes();
    //  // build lists of parents and children
    //  Dictionary<uint, HierarchyNode> lIndependentNodeList = new Dictionary<uint, HierarchyNode>();
    //  Dictionary<uint, HierarchyNode> lDependentNodeList = new Dictionary<uint, HierarchyNode>();
    //  foreach (HierarchyNode lNode in lResult)
    //  {
    //    if (lNode is DelphiFileNode)
    //      lIndependentNodeList.Add(lNode.ID, lNode);
    //    if (lNode is DelphiDependentFileNode)
    //      lDependentNodeList.Add(lNode.ID, lNode);
    //  }

    //  // check to see if all child nodes are added to be moved if not add them
    //  foreach (KeyValuePair<uint,HierarchyNode> lValueKey in lIndependentNodeList)
    //  {
    //    HierarchyNode lNodeChild = lValueKey.Value.FirstChild;
    //    while (lNodeChild != null)
    //    {
    //      HierarchyNode lNodeFound;
    //      if (!lDependentNodeList.TryGetValue(lNodeChild.ID, out lNodeFound))
    //        lResult.Add(lNodeChild);
    //      lNodeChild = lNodeChild.NextSibling;
    //    }
    //  }

    //  // Dependent nodes can not be moved by it's self they need the there parents to move too 
    //  foreach (KeyValuePair<uint, HierarchyNode> lValueKey in lDependentNodeList)
    //  {
    //    HierarchyNode lNodeParent = lValueKey.Value.Parent;
    //    System.Diagnostics.Debug.Assert(lNodeParent != null, "A depenent node as no parent?!?");
    //    if (!lIndependentNodeList.TryGetValue(lNodeParent.ID,out lNodeParent))
    //      lResult.Remove(lValueKey.Value);
    //  }
    //  return lResult;
    //}

    //public List<IVsHierarchy> GetItemsDraggedOrCutOrCopied()
    //{
    //  return this.ItemsDraggedOrCutOrCopied;
    //}

    protected override void OverwriteExistingItemFrom(HierarchyNode existingNode, string aOriginalFile)  
    {
      AddDelphiDirectiveFiles(existingNode as DelphiFileNode, Path.GetDirectoryName(aOriginalFile));
    }

    /// <summary>
    /// Adds a new file node to the hierarchy from an existing one.
    /// NOTE:The file has already been copied.
    /// </summary>
    /// <param name="parentNode">The parent of the new fileNode</param>
    /// <param name="fileName">The file name</param>
    protected override void AddNewFileNodeToHierarchyFrom(HierarchyNode parentNode, string fileName, string aOriginalFile)
    {
      HierarchyNode lChild;

      // In the case of subitem, we want to create dependent file node
      // and set the DependentUpon property
      if (this.CanFileNodesHaveChilds && (parentNode is FileNode || parentNode is DependentFileNode))
      {
        lChild = this.CreateDependentFileNode(fileName);
        lChild.ItemNode.SetMetadata(ProjectFileConstants.DependentUpon, parentNode.ItemNode.GetMetadata(ProjectFileConstants.Include));

        // Make sure to set the HasNameRelation flag on the dependent node if it is related to the parent by name
        if (string.Compare(lChild.GetRelationalName(), parentNode.GetRelationalName(), true, CultureInfo.InvariantCulture) == 0)
        {
          lChild.HasParentNodeNameRelation = true;
        }
      }
      else
      {
        //Create and add new filenode to the project
        lChild = this.CreateFileNode(fileName);
      }

      parentNode.AddChild(lChild);
      // TODO : We need to call DelphiDPRFileNode AddUnitFile once it exits.

      AddDelphiDirectiveFiles(lChild as DelphiFileNode, Path.GetDirectoryName(aOriginalFile));

      this.Tracker.OnItemAdded(fileName, VSADDFILEFLAGS.VSADDFILEFLAGS_NoFlags);
    }

    protected virtual void AddDelphiDirectiveFiles(DelphiFileNode aDelphiFileNode, string aDirectiveSourcePath)
    {
      if (aDelphiFileNode == null) return;
      if (!this.IsCodeFile(aDelphiFileNode.FileName)) 
        return;
      if (!Directory.Exists(aDirectiveSourcePath))
        return;
      string[] lDirectiveFiles = aDelphiFileNode.GetDirectiveFiles();
      // root the path for all resource files so they get copied to the new output folder
      // sorry no referencing these files as the are a part of code file
      for(int i = 0;i<lDirectiveFiles.Length;i++)
      {
        if (!Path.IsPathRooted(lDirectiveFiles[i]))
          lDirectiveFiles[i] = Path.Combine(aDirectiveSourcePath, Path.GetFileName(lDirectiveFiles[i]));
      }

      // remove all existing children
      for (HierarchyNode lChild = aDelphiFileNode.FirstChild; lChild != null; lChild = lChild.NextSibling)
      {
        lChild.Remove(true);
      }
      // add the new children and modify the code file
      CopyFilesToVSProject(lDirectiveFiles, aDelphiFileNode, false);

      // Notify tracker
      for (HierarchyNode lChild = aDelphiFileNode.FirstChild; lChild != null; lChild = lChild.NextSibling)
      {
        this.Tracker.OnItemAdded(lChild.Url, VSADDFILEFLAGS.VSADDFILEFLAGS_NoFlags);
      }
    
    }


    DelphiMainFileNode FDelphiMainFileNode = null;

    /// <summary>
    /// I don't want the reference node so I will not add it I may want to fix this.
    /// </summary>
    /// <param name="node"></param>
    public override void AddChild(HierarchyNode node)
    {
      // do not add more than one TDelphiDPRFileNode!
      if (node is DelphiMainFileNode)
      {
        if (FDelphiMainFileNode != null)
          return;
        FDelphiMainFileNode = node as DelphiMainFileNode;
      }
      if (!String.IsNullOrEmpty(node.VirtualNodeName) && String.Compare(node.VirtualNodeName, ReferenceContainerNode.ReferencesNodeVirtualName, StringComparison.OrdinalIgnoreCase) == 0)
      {
        // ReferenceNode found do not add!
      }
      else
      {
        base.AddChild(node);
      }
    }

    private void AfterDelphiUnitRenameEvent(object sender, string aOldUnitName, string aNewUnitName)
    {
      HierarchyNode lNode = this.FirstChild;
      do
      {
        DelphiFileNode lDelphiNode = lNode as DelphiFileNode;
        if (lDelphiNode != null && sender != lDelphiNode)
          lDelphiNode.RenameDelphiUnit(aOldUnitName, aNewUnitName);
        lNode = lNode.NextSibling;
      } while (lNode != null);
    }

    private void AfterFileRenameEvent(DelphiFileNode aDelphiFileNode, string aOldUnitName, string aNewUnitName)
    {
      DelphiMainFileNode lNode = GetDelphiMainFileNode();
      if (lNode != null)
      {
        if (!lNode.IsDelphiFile(aOldUnitName))
          lNode.RenameContainedFile(aOldUnitName, aNewUnitName);
      }
    }

    /// <summary>
    /// Create my file nodes that know about the file
    /// </summary>
    /// <param name="item">msbuild item</param>
    /// <returns>FileNode added</returns>
    public override FileNode CreateFileNode(ProjectElement item)
    {
      if (item == null)
      {
        throw new ArgumentNullException("item");
      }
      DelphiFileNode lNewNode;
      string include = item.GetMetadata(ProjectFileConstants.Include);
      if (DelphiProjectFileNode.IsDelphiProjectFile(include))
      {
        lNewNode = new DelphiProjectFileNode(this, item);
      }
      else if (DelphiPackageFileNode.IsDelphiProjectFile(include))
      {
        lNewNode = new DelphiPackageFileNode(this, item);
      }
      else
      {
        lNewNode = new DelphiFileNode(this, item);
      }
      lNewNode.OleServiceProvider.AddService(typeof(EnvDTE.Project), new OleServiceProvider.ServiceCreatorCallback(this.CreateServices), false);
      lNewNode.OleServiceProvider.AddService(typeof(VSLangProj.VSProject), new OleServiceProvider.ServiceCreatorCallback(this.CreateServices), false);

      lNewNode.OnAfterDelphiUnitRename += new AfterDelphiUnitRename(AfterDelphiUnitRenameEvent);
      lNewNode.OnAfterFileRename += new AfterFileRename(AfterFileRenameEvent);

      return lNewNode;
    }

    public override DependentFileNode CreateDependentFileNode(ProjectElement item)
    {
      DependentFileNode lNode = new DelphiDependentFileNode(this, item);
      //if (null != node)
      //{
      //  string include = item.GetMetadata(ProjectFileConstants.Include);
      //  if (IsCodeFile(include))
      //  {
      //    node.OleServiceProvider.AddService(
      //        typeof(SVSMDCodeDomProvider), new OleServiceProvider.ServiceCreatorCallback(this.CreateServices), false);
      //  }
      //}

      return lNode;

    }

    protected internal override FolderNode CreateFolderNode(string path, ProjectElement item)
    {
      return new DelpheFolderNode(this,path,item);
    }


    public override bool IsEmbeddedResource(string strFileName)
    {
      return PathTools.HasExtension(strFileName, Constants.Extentions.RES) ||
             PathTools.HasExtension(strFileName, Constants.Extentions.TLB);
    }

    /// <summary>
		/// Renames the delphi project files 
		/// </summary>
		/// <param name="newFile">The full path of the new project file.</param>
    protected override void RenameProjectFile(string newFile)
    {
      base.RenameProjectFile(newFile);

      DelphiMainFileNode lDelphiPrj = GetDelphiMainFileNode();
      if (lDelphiPrj != null)
      {
        string lDPRFileName = Path.GetFileNameWithoutExtension(this.FileName) + Path.GetExtension(lDelphiPrj.FileName);
        lDelphiPrj.SetEditLabel(lDPRFileName);
      }
    }

    public virtual DelphiMainFileNode GetDelphiMainFileNode()
    {
        DelphiMainFileNode result = null;
        for (HierarchyNode child = this.FirstChild; child != null; child = child.NextSibling)
        {
          result = child as DelphiMainFileNode;
          if (result != null)
            return result;
        }
        return result;
    }

    /// <summary>
    /// Creates Automation object services for scripting 
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    private object NodeCreateServices(Type serviceType)
    {
      object service = null;
      if (typeof(EnvDTE.ProjectItem) == serviceType)
      {
        service = GetAutomationObject();
      }
      return service;
    }

    /// <summary>
    /// Adds a file to the msbuild project.
    /// </summary>
    /// <param name="file">The file to be added.</param>
    /// <returns>A Projectelement describing the newly added file.</returns>
    protected override ProjectElement AddFileToMsBuild(string file)
    {
      ProjectElement newItem;

      string itemPath = PackageUtilities.MakeRelativeIfRooted(file, this.BaseURI);
     // Debug.Assert(!Path.IsPathRooted(itemPath), "Cannot add item with full path.");

      if (PathTools.HasExtension(file, ".rc"))
      {
        newItem = this.CreateMsBuildFileItem(itemPath, Constants.MSBuild.CompileDelphiResource);
        newItem.SetMetadata(Constants.MSBuild.OutputFileName,Path.GetFileNameWithoutExtension(itemPath)+".res");
        newItem.SetMetadata(Constants.MSBuild.IncludePath,"");
        newItem.SetMetadata(Constants.MSBuild.DefineName,"");
        newItem.SetMetadata(Constants.MSBuild.ExcludeEnironmentVar,"false");
        newItem.SetMetadata(Constants.MSBuild.Unicode,"false");
        newItem.SetMetadata(Constants.MSBuild.DefautCodePage,"1252");
        newItem.SetMetadata(Constants.MSBuild.DefaultLanguage,"4105");
      }
      else
      {
        newItem = base.AddFileToMsBuild(file);
      }

      return newItem;
    }

    protected override bool IsItemTypeFileType(string type)
    {
      bool lResult = base.IsItemTypeFileType(type);
      if (String.Compare(type, Constants.MSBuild.CompileDelphiResource, StringComparison.OrdinalIgnoreCase) == 0) 
         return true;
       else
         return lResult;
    }
    
    //public virtual bool IsDPRFile(string strFileName)
    //{
    //  // We do not want to assert here, just return silently.
    //  if (String.IsNullOrEmpty(strFileName))
    //  {
    //    return false;
    //  }
    //  return PathTools.HasExtension(strFileName, Constants.Extentions.DPR);
    //}

    /// <summary>
    /// Evaluates if a file is an Delphi code file based on is extension
    /// </summary>
    /// <param name="strFileName">The filename to be evaluated</param>
    /// <returns>true if is a code file</returns>
    public override bool IsCodeFile(string strFileName)
    {
      // We do not want to assert here, just return silently.
      if (String.IsNullOrEmpty(strFileName))
      {
        return false;
      }
      return PathTools.HasExtension(strFileName, Constants.Extentions.PAS) ||
             PathTools.HasExtension(strFileName, Constants.Extentions.DPK) ||
             PathTools.HasExtension(strFileName, Constants.Extentions.DPR);

    }

    /// <summary>
    /// Generate new Guid value and update it with GeneralPropertyPage GUID.
    /// </summary>
    /// <returns>Returns the property pages that are independent of configuration.</returns>
    protected override Guid[] GetConfigurationIndependentPropertyPages()
    {
      Guid[] result = new Guid[1];
      result[0] = typeof(DelphiPropertyPage).GUID;
      return result;
    }

 
    /// <summary>
    /// Specify here a property page. 
    /// By returning no property page the configuration dependent properties will be neglected.
    /// </summary>
    /// <returns>Returns the configuration dependent property pages.</returns>
    protected override Guid[] GetConfigurationDependentPropertyPages()
    {
      Guid[] result = new Guid[1];
      result[0] = typeof(DelphiBuildPropertyPage).GUID;
      return result;
    }

    // the .vstemplate does the work for me!!! Yehay!
    ///// <summary>
    ///// Overriding to provide customization of files on add files.
    ///// This will replace tokens in the file with actual value (namespace, class name,...)
    ///// </summary>
    ///// <param name="source">Full path to template file.</param>
    ///// <param name="target">Full path to destination file.</param>
    ///// <exception cref="FileNotFoundException">Template file is not founded.</exception>
    ////public override void AddFileFromTemplate(string source, string target)
    //{
    //  if (!File.Exists(source))
    //  {
    //    throw new FileNotFoundException(string.Format("Template file not found: {0}", source));
    //  }
    //  if (Path.GetExtension(source).ToUpper() != ".RES")
    //  {
    //    // The class name is based on the new file name
    //    string fileName = Path.GetFileNameWithoutExtension(target);
    //    string nameSpace = this.FileTemplateProcessor.GetFileNamespace(target, this);

    //    this.FileTemplateProcessor.AddReplace("%className%", fileName);
    //    this.FileTemplateProcessor.AddReplace("%namespace%", nameSpace);

    //    try
    //    {
    //      this.FileTemplateProcessor.UntokenFile(source, target);

    //    }
    //    catch (Exception exceptionObj)
    //    {
    //      throw new FileLoadException(Resources.ResourceManager.GetString("MsgFailedToLoadTemplateFile"), target, exceptionObj);
    //    }
    //  }
    //}

    /// <summary>
    /// Copies files to the visual studio project from the absolute file path provided or 
    /// the current working directory for releative paths.
    /// </summary>
    /// <param name="aFileList">List of files to copy</param>
    /// <param name="aParentNode">What parent node to place under null = project</param>
    /// <param name="aMayHaveDirectiveFiles">The files may have dependent files that need to be copied</param>
    private void CopyFilesToVSProject(string[] aFileList, HierarchyNode aParentNode, bool aMayHaveDirectiveFiles)
    {
      string lNewFile;
      FileNode lFileNode;
      if (aParentNode == null)
        aParentNode = this;
      string lDestPath = Path.GetDirectoryName( aParentNode.Url );
      foreach (string lFile in aFileList)
      {                                 
        /*
         * This code Preserves the folder structure 
         * and will do no copying if the file exists in the 
         * parent node folder
         */
        // Check to see if the file has an absolute path
        if (!Path.IsPathRooted(lFile))
        {
          // build a path to the new project folder based on the relative path found in file
          string lPath = Path.GetDirectoryName(lFile);
          // the path can be empty either because this is a dependent file or the file will be placed in the dest path.
          // ie "" or "MyCode\" or "..\..\MyCode" or ".\"
          lPath = Path.Combine(lDestPath, lPath);
          // removed any .\ or ..\
          lPath = Path.GetFullPath(lPath);
          // check to see if in the new location the folder does not exist.
          if (!Directory.Exists(lPath))
            Directory.CreateDirectory(lPath);// create it.

          lNewFile = Path.Combine(lPath, Path.GetFileName(lFile));
        }
        else
        {
          // If the file is rooted thats a No No and it will be copied to the project aDestPath or 
          lNewFile = System.IO.Path.Combine(lDestPath, System.IO.Path.GetFileName(lFile));
        }
        
        if (!File.Exists(lNewFile))
          System.IO.File.Copy(lFile, lNewFile);

        lNewFile = Utilities.GetFileNameInProperCase(lNewFile);
        // by creating the node it addes it to the MSBuild project file 
        if (aParentNode is DelphiFileNode)
        { // if there is a parent node where this project belongs lets add it as dependent
          lFileNode =  this.CreateDependentFileNode(lNewFile);
          aParentNode.AddChild(lFileNode);
          lFileNode.ItemNode.SetMetadata(ProjectFileConstants.DependentUpon, aParentNode.ItemNode.GetMetadata(ProjectFileConstants.Include));
          // TODO: A method is needed on the depenent node to rename it with the same name so that 
          // the parent will update its filename reference to the correct relative path.
        }
        else 
         lFileNode = this.CreateFileNode(lNewFile);
        // the delphi file may have directive files files {$R filename} or {$I filename} lets attach them as dependents.
       if (aMayHaveDirectiveFiles && lFileNode is DelphiFileNode)
       {
         //
         // TODO: if the lFile is an absolute path we need to copy all files over to the same folder
         // the reason fo this is the directive file may be on {$I 'E:\myinc.inc'} but the main file 
         // is on C:\Files and VS does not like it.
         //

         // Most cases the directive files are in the same path
         // but for the corner case where it is not we need to 
         // change the working dir to the path of the current file
         string lSourcePath = Path.GetDirectoryName(Path.GetFullPath(lFile));
         // save it first
         string lOldPath = Environment.CurrentDirectory;
         try
         {
           Environment.CurrentDirectory = lSourcePath;
           CopyFilesToVSProject(((DelphiFileNode)lFileNode).GetDirectiveFiles(), lFileNode, false);
         }
         finally
         {
           // put it back even after error.
           // trying to avoid a bad state even though it may occur anyway
           Environment.CurrentDirectory = lOldPath;
         }
       }
      }
    }
    /// <summary>
    /// Imports a delphi Main file (DPR or DPK) into the project that has no Main file
    /// the path of the current visual studio project must exist.
    /// </summary>
    /// <param name="aDelphiMainFile">Full path of existing main file</param>
    public virtual void ImportDelphiMainFile(string aDelphiMainFile)
    {
      if (!File.Exists(aDelphiMainFile)) return;
      // change to drectory where the main file exists.
      Environment.CurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(aDelphiMainFile)); 
      // copy the file
      string lNewFile = Path.Combine(Path.GetDirectoryName(this.Url), Path.GetFileName(aDelphiMainFile));
      if (!NativeMethods.IsSamePath(aDelphiMainFile, lNewFile))
        System.IO.File.Copy(aDelphiMainFile, lNewFile, true);
      // TODO: this should be changed to Main file node 
      // create DelphiDPRFileNode that is not attached to project
      DelphiMainFileNode lFileNode = (DelphiMainFileNode)this.CreateFileNode(lNewFile);
      // copy contained files - {%File 'filename'}
      CopyFilesToVSProject(lFileNode.GetContainedFiles(), this, false);
      // copy unit files - Unit1 in 'unit1.pas'
      CopyFilesToVSProject(lFileNode.GetUnitFiles(), this, true);
      // copy directive files - {$R 'filename'} or {$I 'filename'}
      CopyFilesToVSProject(lFileNode.GetDirectiveFiles(), lFileNode, false);
      /* Removed from project (WorkItem 8808) I may want to read them for settings though?
      string[] lSettingFiles = new string[2];
      lSettingFiles[0] = Path.GetFileNameWithoutExtension(aDelphiMainFile) + ".cfg";
      lSettingFiles[1] = Path.GetFileNameWithoutExtension(aDelphiMainFile) + ".dof";
      CopyFilesToVSProject(lSettingFiles, lFileNode, false);
       */
      lFileNode.Close();
      lFileNode = null;
    }

    public override void Load(string filename, string location, string name, uint flags, ref Guid iidProject, out int canceled)
    {
      // assuming we are creating new, and is going through the importer

      base.Load(filename, location, name, flags, ref iidProject, out canceled);
        
      string lValue = this.BuildProject.GetEvaluatedProperty(Resources.ImportDelphiProject);
      // based on the passed in flags, this either reloads/loads a project, or tries to create a new one
      // now we create a new project... we do that by loading the template and then saving under a new name
      // we also need to copy all the associated files with it.					
      if (lValue == "true")
      {
        lValue = this.BuildProject.GetEvaluatedProperty(Resources.ImportDelphiPojectFile);
        // remove import settings.
        this.BuildProject.SetProperty(Resources.ImportDelphiProject, "false");
        // scan through project file and extract all filenames and add them to the MSBuild
        this.ImportDelphiMainFile(lValue);

        this.BuildProject.SetProperty(Resources.ImportDelphiPojectFile, "");
        // save this project out with all the files
        this.BuildProject.Save(Path.Combine(location,Path.GetFileName(filename)));
        // load in the project files 
        this.Reload();
      }
    }

    /// <summary>
    /// Creates the format list for the open file dialog.
    /// </summary>
    /// <param name="ppszFormatList">The format list to return.</param>
    /// <returns>S_OK if method is succeeded.</returns>
    public override int GetFormatList(out string ppszFormatList)
    {
      ppszFormatList = String.Format(CultureInfo.CurrentCulture, "Delphi Project File (*.delphiproj){0}*.delphiproj{1}", "\0", "\0");
      return VSConstants.S_OK;
    }

    /// <summary>
    /// Adds support for project properties.
    /// </summary>
    /// <returns>Return the automation object associated to this project.</returns>
    public override object GetAutomationObject()
    {
      return new OAProject(this);
    }

    Microsoft.VisualStudio.Package.Automation.OAVSProject FvsProject;

    /// <summary>
    /// Get the VSProject corresponding to this project
    /// </summary>
    protected internal VSLangProj.VSProject VSProject
    {
      get
      {
        if (FvsProject == null)
          FvsProject = new Microsoft.VisualStudio.Package.Automation.OAVSProject(this);
        return FvsProject;
      }
    }

    private object CreateServices(Type serviceType)
    {
      object lService = null;
      if (typeof(System.CodeDom.Compiler.CodeDomProvider) == serviceType)
      {
        lService = null;
      }
      else if (typeof(VSLangProj.VSProject) == serviceType)
      {
        lService = this.VSProject;
      }
      else if (typeof(EnvDTE.Project) == serviceType)
      {
        lService = this.GetAutomationObject();
      }
      return lService;
    }

    #endregion Methods
  }
}
