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
using System.Text;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace VisualStudio.Delphi.Language
{
  public static class TDelphiSource
  {

    private static char DelphiCharNumberToChar(string aNumber, bool aIsHex)
    {
      ushort lCharNum;
      try
      {
        if (aIsHex)
        {
         // aNumber = "0x" + aNumber;
          lCharNum = Convert.ToUInt16(aNumber, 16);
        }
        else
          lCharNum = Convert.ToUInt16(aNumber);
      }
      catch
      {
        throw new Exception("Not a valid delphi literal character number");
      }
      return (char)lCharNum;
    }
    /// <summary>
    /// Converts a Delphi string with character literals like (#13) and converts it to text with the quotes removed.
    /// </summary>
    /// <param name="aDelphiString">A Delphi string with quotes and literals</param>
    /// <returns></returns>
    public static string DelphiStringToText(string aDelphiString)
    {
//      string lResult = aDelphiString;
      StringBuilder lStringResult = new StringBuilder();
      StringBuilder lStringNumber = null;
      bool lIsInStr = false;
      bool lIsHex = false;
      char lChr;
      char lLastChar = '\0';
      for(int i = 0;i<aDelphiString.Length;i++)
      {
        lChr = aDelphiString[i];
        if (!lIsInStr && lStringNumber != null && (lChr == '\'' || lChr == '#'))
        {
          lStringResult.Append(DelphiCharNumberToChar(lStringNumber.ToString(),lIsHex));
          lStringNumber = null;
        }
        if (lChr == '\'')
        {
          lIsInStr = !lIsInStr;
          if (lLastChar == '\'' && lIsInStr)
            lStringResult.Append(lChr);
        }
        else if (!lIsInStr)
        {
          if (lChr == '#')
          {
            lIsHex = false;
            lStringNumber = new StringBuilder();
          }
          else if (lChr == '$')
          {
            lIsHex = true;
          }
          else if (lStringNumber == null)
          {
            throw new Exception("Not a valid Delphi String");
          }
          else
            lStringNumber.Append(lChr);
        }
        else
          lStringResult.Append(lChr);
        lLastChar = lChr;
      }
      if (lStringNumber != null)
      {
        lStringResult.Append(DelphiCharNumberToChar(lStringNumber.ToString(), lIsHex));
      }
      return lStringResult.ToString();
    }

    public static string TextToDelphiString(string aText)
    {
      bool lIsInStr = false;
      StringBuilder lStringResult = new StringBuilder();
      for (int i = 0; i < aText.Length; i++)
      {
        ushort lCharValue = (ushort)aText[i];
        if (lCharValue < 32 || lCharValue > 126)
        {
          if (lIsInStr)
          {
            lStringResult.Append("'");
            lIsInStr = !lIsInStr;
          }
          lStringResult.Append("#");
          lStringResult.Append(lCharValue);
        }
        else
        {
          if (!lIsInStr)
          {
            lStringResult.Append("'");
            lIsInStr = !lIsInStr;
          }
          if (aText[i] == '\'')
            lStringResult.Append("'");
          lStringResult.Append(aText[i]);

        }
      }
      return lStringResult.ToString();
    }

  }

  public class DelphiSource : Source
  {
    public DelphiSource(LanguageService service,
                        IVsTextLines textLines,
                        Colorizer colorizer)
      : base(service, textLines, colorizer)
    { }
  }
}
