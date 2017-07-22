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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace VisualStudio.Delphi.Language
{
  internal class DelphiAuthoringScope : AuthoringScope
  {
    public override string GetDataTipText(int line, int col, out TextSpan span)
    {
      span = new TextSpan();
      return null;
    }

    public override Declarations GetDeclarations(IVsTextView view,
                                                 int line,
                                                 int col,
                                                 TokenInfo info,
                                                 ParseReason reason)
    {
      return null;
    }

    public override Methods GetMethods(int line, int col, string name)
    {
      return null;
    }

    public override string Goto(VSConstants.VSStd97CmdID cmd,
                                 IVsTextView textView,
                                 int line,
                                 int col,
                                 out TextSpan span)
    {
      span = new TextSpan();
      return null;
    }
  }
}
