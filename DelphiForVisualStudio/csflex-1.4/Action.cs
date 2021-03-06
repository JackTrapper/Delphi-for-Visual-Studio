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
 * Encapsulates an action in the specification.
 *
 * It stores the Java code as String together with a priority (line number in the specification).
 *
 * @author Gerwin Klein
 * @version JFlex 1.4, $Revision: 2.5 $, $Date: 2004/04/12 10:07:48 $
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
sealed public class Action {


  /**
   * The Java code this Action represents
   */
  internal String content;
  string content_trimmed;

  /**
   * The priority (i.e. line number in the specification) of this Action. 
   */
  internal int priority;

  /**
   * True iff the action belongs to an lookahead expresstion 
   * (<code>a/b</code> or <code>r$</code>)
   */
  private bool _isLookAction;


  /**
   * Creates a new Action object with specified content and line number.
   * 
   * @param content    java code
   * @param priority   line number
   */
  public Action(String content, int priority) {
    this.content = content;
    content_trimmed = content.Trim();
    this.priority = priority;
  }  


  /**
   * Compares the priority value of this Action with the specified action.
   *
   * @param other  the other Action to compare this Action with.
   *
   * @return this Action if it has higher priority - the specified one, if not.
   */
  public Action getHigherPriority(Action other) {
    if (other == null) return this;

    // the smaller the number the higher the priority
    if (other.priority > this.priority) 
      return this;
    else
      return other;
  }


  /**
   * Returns the String representation of this object.
   * 
   * @return string representation of the action
   */
  public override String ToString() {
    return "Action (priority "+priority+", lookahead "+_isLookAction.ToString().ToLower()+") :"+Out.NL+content; //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
  }


  /**
   * Returns <code>true</code> iff the parameter is an
   * Action with the same content as this one.
   *
   * @param a   the object to compare this Action with
   * @return    true if the action strings are equal
   */
  public bool isEquiv(Action a) {
    return this == a || ((Options.emit_csharp == false) && this.content_trimmed.Equals(a.content_trimmed));
  }


  /**
   * Calculate hash value.
   * 
   * @return a hash value for this Action
   */
  public override int GetHashCode() {
    return content_trimmed.GetHashCode();
  }


  /**
   * Test for equality to another object.
   * 
   * This action equals another object if the other 
   * object is an equivalent action. 
   * 
   * @param o  the other object.
   * 
   * @see Action#isEquiv(Action)
   */
  public override bool Equals(Object o) {
    if (o is Action) 
      return isEquiv((Action) o);
    else
      return false;    
  }
  
  /**
   * Return look ahead flag.
   * 
   * @return true if this actions belongs to a lookahead rule
   */
  public bool isLookAction() {
    return _isLookAction;
  }

  /**
   * Sets the look ahead flag for this action
   * 
   * @param b  set to true if this action belongs to a look ahead rule  
   */
  public void setLookAction(bool b) {
    _isLookAction = b;
  }
  
}
}