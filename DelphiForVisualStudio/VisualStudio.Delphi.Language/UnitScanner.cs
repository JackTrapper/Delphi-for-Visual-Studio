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
  public class TUnitScanner
  {
    private IVsTextLines FTextLines;
    private IScanner FScanner = new DelphiScanner();
    private string FTokenValue;
    private DelphiToken FEndToken = DelphiToken.None;
    private int FStartLine;
    private int FEndLine;
    private int FEndIndex;
    private int FStartIndex;

    public TUnitScanner(IVsTextLines aTextLines)
    {
      System.Diagnostics.Debug.Assert(aTextLines != null, "aTextLines param is null in TUnitScanner");
      FTextLines = aTextLines;
      FStartLine = 0;
      ErrorHandler.ThrowOnFailure(FTextLines.GetLineCount(out FEndLine));
      FEndLine--;
      ErrorHandler.ThrowOnFailure(FTextLines.GetLengthOfLine(FEndLine, out FEndIndex));
    }

    public bool GotoNextToken(DelphiToken aToken)
    {
      int lStartLine, lEndLine, lStartIndex, lEndIndex;
      return GotoNextToken(aToken, out lStartLine, out lStartIndex, out lEndLine, out lEndIndex);
    }

    public bool GotoNextToken(DelphiToken aToken, out int aStartLine, out int aStartIndex, out int aEndLine, out int aEndIndex)
    {
      int lCurrLine, lCurrIndex, lLineLen, lState = 0;
      int lLastState = 0;
      string lLineText = "";
      TokenInfo lTokenInfo = new TokenInfo();
      lCurrLine = StartLine;
      lCurrIndex = StartIndex;
      int lAddChrs = StartIndex;
      aStartIndex = 0;
      aStartLine = 0;
      aEndIndex = 0;
      aEndLine = 0;
      lLineLen = 0;
      if (aToken == DelphiToken.None) return false;
      System.Diagnostics.Debug.Assert(aToken != EndToken, "End Token and aToken parameter match in GotoNextToken");
      bool result = false;
      while (lCurrLine <= EndLine)
      {
        if (lCurrLine == EndLine - 2)
          result = false;
        if (lLineText == "")
        {
          // TODO: Read and check out whats the diff between NativeMethods.ThrowOnFailure and ErrorHandler.ThrowOnFailure?
          ErrorHandler.ThrowOnFailure(FTextLines.GetLengthOfLine(lCurrLine, out lLineLen));
          ErrorHandler.ThrowOnFailure(FTextLines.GetLineText(lCurrLine, 0, lCurrLine, lLineLen, out lLineText));
          FScanner.SetSource(lLineText, lCurrIndex);
        }
        lTokenInfo.Type = TokenType.Unknown;
        lTokenInfo.EndIndex = lLineLen - 1;
        try
        {
          FScanner.ScanTokenAndProvideInfoAboutIt(lTokenInfo, ref lState);
        }
        catch { /* Do we care? */ }

        // lTokenInfo.StartIndex will = 0 for the first token even if  
        // in line "FScanner.SetSource(lLineText, lCurrIndex)" lCurrIndex > 0
        // so we will need to add some values to make the values correct.
        if (lAddChrs > 0)
        {
          lTokenInfo.EndIndex += lAddChrs;
          lTokenInfo.StartIndex += lAddChrs;
        }

        // if we have reached your end token then exit
        if (lTokenInfo.Token == (int)EndToken && lState == DelphiScanner.YYINITIAL)
          break;
        aEndIndex = lTokenInfo.EndIndex; // set end point
        aEndLine = lCurrLine;           // set end line
        lCurrIndex = lTokenInfo.StartIndex; // Set new point

        if ((lState == DelphiScanner.YYINITIAL) && (lTokenInfo.Token == (int)aToken) && (lState != lLastState)) 
        {
          // no worries we found have the StartIndex and StartLine already
          result = true;
        }
        else if ((lState == DelphiScanner.YYINITIAL) || (lState != lLastState)) 
        {
          aStartLine = lCurrLine;
          aStartIndex = lCurrIndex;
          lLastState = lState;
          result = (lTokenInfo.Token == (int)aToken);
        }

        if (result) break;
        lCurrIndex = lTokenInfo.EndIndex + 1;
        if (lCurrIndex >= lLineLen)
        {
          lLineText = "";
          lCurrIndex = 0;
          lAddChrs = 0;
          lCurrLine++;
        }
      }
      if (result)
      {
        StartIndex = lTokenInfo.EndIndex + 1;
        StartLine = aEndLine;
        // aEndIndex is correct but the GetLineText method does not read the text out correctly.
        ErrorHandler.ThrowOnFailure(FTextLines.GetLineText(aStartLine, aStartIndex, aEndLine, aEndIndex + 1, out FTokenValue));
      }
      return result;
    }

    public string TokenValue
    {
      get { return FTokenValue; }
    }

    /// <summary>
    /// Set this to where you want to stop the scanner before 
    /// the EndLine property is reached
    /// </summary>
    public DelphiToken EndToken
    {
      get { return FEndToken; }
      set { FEndToken = value; }
    }

    public IVsTextLines TextLines
    {
      get { return FTextLines; }
    }

    public int StartLine
    {
      get { return FStartLine; }
      set
      {
        if (value < 0)
          FStartLine = 0;
        else
          FStartLine = value;
      }
    }

    public int EndLine
    {
      get { return FEndLine; }
      set { FEndLine = value; }
    }

    public int StartIndex
    {
      get { return FStartIndex; }
      set { FStartIndex = value; }
    }

    public int EndIndex
    {
      get { return FEndIndex; }
      set { FEndIndex = value; }
    }

  }

}
