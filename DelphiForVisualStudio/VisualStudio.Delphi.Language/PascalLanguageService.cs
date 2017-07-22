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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

// docs
// http://msdn2.microsoft.com/en-us/library/bb165744(VS.80).aspx
namespace VisualStudio.Delphi.Language
{
  [Guid(Constants.DELPHI_LanguageServiceGUID)]
  class PascalLanguageService : LanguageService
  {
    private LanguagePreferences FLangaugePreferences;
    private DelphiScanner FDelphiScanner = null;
    public override LanguagePreferences GetLanguagePreferences()
    {
      if (FLangaugePreferences == null)
      {
        FLangaugePreferences = new LanguagePreferences(this.Site, typeof(PascalLanguageService).GUID, this.Name);
        FLangaugePreferences.Init();
      }
      return FLangaugePreferences;
    }

    public override IScanner GetScanner(IVsTextLines buffer)
    {
      if (FDelphiScanner == null)
        FDelphiScanner = new DelphiScanner();
      return FDelphiScanner;
    }

    public override string Name
    {
      get { return "Delphi"; }
    }

    public override AuthoringScope ParseSource(ParseRequest req)
    {
      return new DelphiAuthoringScope();
    }

    public override Source CreateSource(IVsTextLines buffer)
    {
      return new DelphiSource(this, buffer, new Colorizer(this, buffer, GetScanner(buffer)));
    }

  }
}
