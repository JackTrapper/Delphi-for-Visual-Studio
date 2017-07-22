/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * C# Flex 1.4                                                             *
 * Copyright (C) 2004-2005  Jonathan Gilbert <logic@deltaq.org>            *
 * Derived from:                                                           *
 *                                                                         *
 *   JFlex 1.4                                                             *
 *   Copyright (C) 1998-2004  Gerwin Klein <lsf@jflex.de>                  *
 *   All rights reserved.                                                  *
 *                                                                         *
 * This program is free software; you can redistribute it and/or modify    *
 * it under the terms of the GNU General Public License. See the file      *
 * COPYRIGHT for more information.                                         *
 *                                                                         *
 * This program is distributed in the hope that it will be useful,         *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of          *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the           *
 * GNU General Public License for more details.                            *
 *                                                                         *
 * You should have received a copy of the GNU General Public License along *
 * with this program; if not, write to the Free Software Foundation, Inc., *
 * 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA                 *
 *                                                                         *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

namespace CSFlex
{

/**
 * Stores an interval of characters together with the character class
 *
 * A character belongs to an interval, if its Unicode value is greater than or equal
 * to the Unicode value of <CODE>start</code> and smaller than or euqal to the Unicode
 * value of <CODE>end</code>.
 *
 * All characters of the interval must belong to the same character class.
 *
 * @author Gerwin Klein
 * @version JFlex 1.4, $Revision: 2.2 $, $Date: 2004/04/12 10:07:48 $
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
public class CharClassInterval {

  /**
   * The first character of the interval
   */
  internal int start;

  /**
   * The last character of the interval
   */
  internal int end;

  /**
   * The code of the class all characters of this interval belong to.
   */
  internal int charClass;
  

  /**
   * Creates a new CharClassInterval from <CODE>start</code> to <CODE>end</code>
   * that belongs to character class <CODE>charClass</code>.
   *
   * @param start         The first character of the interval
   * @param end           The last character of the interval  
   * @param charClass     The code of the class all characters of this interval belong to.
   */
  public CharClassInterval(int start, int end, int charClass) {
    this.start = start;
    this.end = end;
    this.charClass = charClass;
  }

  /**
   * returns string representation of this class interval
   */
  public override String ToString() {
    return "["+start+"-"+end+"="+charClass+"]";
  }
}
}