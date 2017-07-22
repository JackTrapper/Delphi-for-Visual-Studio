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
// Implementation of DelphiLanguagePackage
//

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace VisualStudio.Delphi.Language
{
  [Guid(Constants.DELPHI_Language_PAS_PackageGUID)]
  [ProvideLanguageExtension(typeof(PascalLanguageService), ".pas")]
  public sealed class PASLanguagePackage : DelphiLanguagePackage
  {
    public PASLanguagePackage() : base() { }
  }

  [Guid(Constants.DELPHI_Language_DPR_PackageGUID)]
  [ProvideLanguageExtension(typeof(PascalLanguageService), ".dpr")]
  public sealed class DPRLanguagePackage : DelphiLanguagePackage
  {
    public DPRLanguagePackage() : base() { }
  }

  [Guid(Constants.DELPHI_Language_DFM_PackageGUID)]
  [ProvideLanguageExtension(typeof(PascalLanguageService), ".dfm")]
  public sealed class DFMLanguagePackage : DelphiLanguagePackage
  {
    public DFMLanguagePackage() : base() { }
  }

  [Guid(Constants.DELPHI_Language_DPK_PackageGUID)]
  [ProvideLanguageExtension(typeof(PascalLanguageService), ".dpk")]
  public sealed class DPKLanguagePackage : DelphiLanguagePackage
  {
    public DPKLanguagePackage() : base() { }
  }

  /// <summary>
  /// This is the class that implements the package exposed by this assembly.
  ///
  /// The minimum requirement for a class to be considered a valid package for Visual Studio
  /// is to implement the IVsPackage interface and register itself with the shell.
  /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
  /// to do it: it derives from the Package class that provides the implementation of the 
  /// IVsPackage interface and uses the registration attributes defined in the framework to 
  /// register itself and its components with the shell.
  /// </summary>
  // This attribute tells the registration utility (regpkg.exe) that this class needs
  // to be registered as package.
  [PackageRegistration(UseManagedResourcesOnly = true)]
  // A Visual Studio component can be registered under different regitry roots; for instance
  // when you debug your package you want to register it in the experimental hive. This
  // attribute specifies the registry root to use if no one is provided to regpkg.exe with
  // the /root switch.
#if (DEBUG)
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\8.0Exp")]
#else
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\8.0Delphi")]
#endif
  // This attribute is used to register the informations needed to show the this package
  // in the Help/About dialog of Visual Studio.
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
  // package needs to have a valid load key (it can be requested at 
  // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
  // package has a load key embedded in its resources.
  [ProvideLoadKey("Standard", "1.0", "Delphi For Visual Studio", "Jeremie.ca Inc", 1)]
  [ProvideService(typeof(PascalLanguageService), ServiceName = "Delphi")]
  [ProvideLanguageService(typeof(PascalLanguageService), "Delphi", 100,
    CodeSense = false,
    DefaultToInsertSpaces = true,
    RequestStockColors = true,
    EnableCommenting = true,
    MatchBraces = false,
    ShowCompletion = false,
    ShowMatchingBrace = false)]
  public class DelphiLanguagePackage : Package, IOleComponent
  {

    /// <summary>
    /// Default constructor of the package.
    /// Inside this method you can place any initialization code that does not require 
    /// any Visual Studio service because at this point the package object is created but 
    /// not sited yet inside Visual Studio environment. The place to do all the other 
    /// initialization is the Initialize method.
    /// </summary>
    public DelphiLanguagePackage()
    {
      Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
    }


    private uint m_componentID;

    /////////////////////////////////////////////////////////////////////////////
    // Overriden Package Implementation
    #region Package Members

    /// <summary>
    /// Initialization of the package; this method is called right after the package is sited, so this is the place
    /// where you can put all the initilaization code that rely on services provided by VisualStudio.
    /// </summary>
    protected override void Initialize()
    {
      Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
      base.Initialize();  // required

      // Proffer the service.
      IServiceContainer serviceContainer = this as IServiceContainer;
      PascalLanguageService langService = new PascalLanguageService();
      langService.SetSite(this);
      serviceContainer.AddService(typeof(PascalLanguageService),
                                  langService,
                                  true);

      // Register a timer to call our language service during
      // idle periods.
      IOleComponentManager mgr = GetService(typeof(SOleComponentManager))
                                 as IOleComponentManager;
      if (m_componentID == 0 && mgr != null)
      {
        OLECRINFO[] crinfo = new OLECRINFO[1];
        crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
        crinfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime |
                                      (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
        crinfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal |
                                      (uint)_OLECADVF.olecadvfRedrawOff |
                                      (uint)_OLECADVF.olecadvfWarningsOff;
        crinfo[0].uIdleTimeInterval = 1000;
        int hr = mgr.FRegisterComponent(this, crinfo, out m_componentID);
      }

    }
    #endregion
    protected override void Dispose(bool disposing)
    {
      if (m_componentID != 0)
      {
        IOleComponentManager mgr = GetService(typeof(SOleComponentManager))
                                   as IOleComponentManager;
        if (mgr != null)
        {
          int hr = mgr.FRevokeComponent(m_componentID);
        }
        m_componentID = 0;
      }

      base.Dispose(disposing);
    }


    #region IOleComponent Members

    public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
    {
      return 1;
    }

    public int FDoIdle(uint grfidlef)
    {
      PascalLanguageService pl = GetService(typeof(PascalLanguageService)) as PascalLanguageService;
      if (pl != null)
      {
        pl.OnIdle((grfidlef & (uint)_OLEIDLEF.oleidlefPeriodic) != 0);
      }
      //if (null != libraryManager)
      //{
      //  libraryManager.OnIdle();
      //}
      return 0;
    }

    public int FPreTranslateMessage(MSG[] pMsg)
    {
      return 0;
    }

    public int FQueryTerminate(int fPromptUser)
    {
      return 1;
    }

    public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
    {
      return 1;
    }

    public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
    {
      return IntPtr.Zero;
    }

    public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
    {
    }

    public void OnAppActivate(int fActive, uint dwOtherThreadID)
    {
    }

    public void OnEnterState(uint uStateID, int fEnter)
    {
    }

    public void OnLoseActivation()
    {
    }

    public void Terminate()
    {
    }

    #endregion
  }
}