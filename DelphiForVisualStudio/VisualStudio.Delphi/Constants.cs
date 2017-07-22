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
// Guids.cs
// MUST match guids.h
using System;

namespace VisualStudio.Delphi
{
  public static class PathTools
  {
    public static bool HasExtension(string aFileName, string aExtention)
    {
      return String.Compare(System.IO.Path.GetExtension(aFileName), aExtention, StringComparison.OrdinalIgnoreCase) == 0;
    }
  }

  public static class Constants
  {
    public const string DELPHI_Language_PAS_PackageGUID = "c69fe081-2c91-4a65-b464-bd58b8937536";
    public const string DELPHI_Language_DFM_PackageGUID = "54B8AF72-5BC8-4622-B16F-A57941AD0381";
    public const string DELPHI_Language_DPR_PackageGUID = "11435710-6BA7-4130-A3D4-AECC8FDB57D4";
    public const string DELPHI_Language_DPK_PackageGUID = "2C5B1051-AEF1-49d6-A475-AEA843E330E9";
    public const string guidPascalLanguagePackageCmdSetString = "7e30e617-68db-4008-95db-4feac738a68c";
    public const string DELPHI_LanguageServiceGUID = "EE622FA2-E640-49ef-BBA4-7C0C449200DA";

    public static readonly Guid guidPascalLanguagePackagePkg = new Guid(DELPHI_Language_PAS_PackageGUID);
    public static readonly Guid guidPascalLanguagePackageCmdSet = new Guid(guidPascalLanguagePackageCmdSetString);
    public const string DelphiProjectPackageGUID = "31bcd34c-75be-4499-bd20-f9c9904ca8f0";
    public const string guidDelphiProjectCmdSetString = "ac28e306-91f2-4c53-8db4-a5f81d3ccc45";
    public const string DelphiProjectFactoryGUID = "E8E21438-3AA0-4a15-83E9-68C91683929F";
    public const string DelphiProjectNodeGUID = "6925279D-2FF2-4c1c-BEB7-ECC7B72668CF";
    public const string DELPHI_DPRFileNodeGuid = "28700BE3-E5EE-4528-A851-640323285CE9";
    public const string DELPHI_DPKFileNodeGuid = "9370AE3E-3B19-449b-9378-91D7A37C3A7F";
    public const string DelphiProjectPropertiesGUID = "5B18D178-EEF1-42a5-8680-4C376E20CCAA";
    public const string DelphiDependentFileNodeGUID = "8929954B-2205-4b29-8DC7-13C3556E7A19";
    public const string DelphiDependentFileNodeProprtiesGUID = "C04AF722-59EB-446e-9F46-FE813495A817";
    public const string DelphiFileNodeProprtiesGUID = "6BEE10A3-EB1C-439c-90D2-091C4658938B";

    public static readonly Guid guidDelphiProjectPkg = new Guid(DelphiProjectPackageGUID);
    public static readonly Guid guidDelphiProjectCmdSet = new Guid(guidDelphiProjectCmdSetString);

#if (DEBUG)
    public const string DEFAULT_REGISTRY_ROOT = "Software\\Microsoft\\VisualStudio\\8.0Exp";
#else
    public const string DEFAULT_REGISTRY_ROOT = "Software\\Microsoft\\VisualStudio\\8.0Delphi"; 
#endif
    public const string REGISTRY_KEY = DEFAULT_REGISTRY_ROOT + "\\Delphi for Visual Studio";
    public const string DELPHI_TO_VS_VSZ_FILE_PATH = "vsz\\DelphiToVS.vsz";
    public const string DELPHI_TO_VS_BITMAP_FILE_PATH = "images\\DelphiToVS.bmp";
    public const string CONVERT_WIZARD_GUID = "F251D704-27B4-47e8-832E-FDA6CA9E3AED";

    public static class Extentions
    {
      public const string DPR = ".dpr";
      public const string PAS = ".pas";
      public const string DPK = ".dpk";
      public const string DFM = ".dfm";
      public const string RES = ".res";
      public const string RC = ".rc";
      public const string TLB = ".tlb";
      public const string IDL = ".idl";
    }

    public static class MSBuild
    {
      public const string CompileDelphiResource = "CompileDelphiResource";
      public const string OutputFileName = "OutputFileName";
      public const string IncludePath = "IncludePath";
      public const string DefineName = "DefineName";
      public const string ExcludeEnironmentVar = "ExcludeEnironmentVar";
      public const string Unicode = "Unicode";
      public const string DefautCodePage = "DefautCodePage";
      public const string DefaultLanguage = "DefaultLanguage";
    }
  };
}