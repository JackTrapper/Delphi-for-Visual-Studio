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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace CSFlex.gui
{
  /**
   * Grid layout manager like GridLayout but with predefinable
   * grid size.
   *
   * @author Gerwin Klein
   * @version JFlex 1.4, $Revision: 2.1 $, $Date: 2004/04/12 10:07:48 $
   * @author Jonathan Gilbert
   * @version CSFlex 1.4
   */
  public class GridPanel: Control 
  {
    private int cols;
    private int rows;

    private int hgap;
    private int vgap;
 
    private ArrayList constraints = new ArrayList();
    private Insets insets = new Insets(0, 0, 0, 0);

    public GridPanel(int cols, int rows)
      : this(cols, rows, 0, 0)
    {
      Resize += new EventHandler(GridPanel_Resize);
    }

    public GridPanel(int cols, int rows, int hgap, int vgap) 
    {
      this.cols = cols;
      this.rows = rows;
      this.hgap = hgap;
      this.vgap = vgap;
    }

    public void doLayout() 
    {
      Size size = Size;
      size.Height -= insets.top+insets.bottom;
      size.Width  -= insets.left+insets.right;

      float cellWidth  = size.Width/cols;
      float cellHeight = size.Height/rows;

      for (int i = 0; i < constraints.Count; i++) 
      {
        GridPanelConstraint c = (GridPanelConstraint) constraints[i];

        float x = cellWidth * c.x + insets.left + hgap/2;
        float y = cellHeight * c.y + insets.right + vgap/2;

        float width, height;

        if (c.handle == Handles.FILL)
        {
          width  = (cellWidth-hgap) * c.width;
          height = (cellHeight-vgap) * c.height;
        }
        else 
        {
          Size d = c.component.Size;
          width  = d.Width;
          height = d.Height;
        }

        switch (c.handle) 
        {
          case Handles.TOP_CENTER: 
            x+= (cellWidth+width)/2; 
            break;
          case Handles.TOP_RIGHT:
            x+= cellWidth-width;
            break;
          case Handles.CENTER_LEFT:
            y+= (cellHeight+height)/2;
            break;
          case Handles.CENTER:
            x+= (cellWidth+width)/2; 
            y+= (cellHeight+height)/2;
            break;
          case Handles.CENTER_RIGHT:
            y+= (cellHeight+height)/2;
            x+= cellWidth-width;
            break;
          case Handles.BOTTOM:
            y+= cellHeight-height;
            break;
          case Handles.BOTTOM_CENTER:
            x+= (cellWidth+width)/2; 
            y+= cellHeight-height;
            break;
          case Handles.BOTTOM_RIGHT:        
            y+= cellHeight-height;
            x+= cellWidth-width;
            break;
        }

        c.component.Bounds = new Rectangle((int)x, (int)y, (int)width, (int)height);
      }
    }

    public Size getPreferredSize() 
    {
      float dy = 0;
      float dx = 0;
   
      for (int i = 0; i < constraints.Count; i++) 
      {
        GridPanelConstraint c = (GridPanelConstraint) constraints[i];

        Size d = c.component.Size;

        dx = Math.Max(dx, d.Width/c.width);
        dy = Math.Max(dy, d.Height/c.height);
      }

      dx+= hgap;
      dy+= vgap;

      dx*= cols;
      dy*= rows;

      dx+= insets.left+insets.right;
      dy+= insets.top+insets.bottom;

      return new Size((int)dx,(int)dy);
    }

    public void setInsets(Insets insets) 
    {
      this.insets = insets;
    }

    public void add(int x, int y, Control c) 
    {
      add(x,y,1,1,Handles.FILL,c);
    }

    public void add(int x, int y, Handles handle, Control c) 
    {
      add(x,y,1,1,handle,c);
    }

    public void add(int x, int y, int dx, int dy, Control c) 
    {
      add(x,y,dx,dy,Handles.FILL,c);
    }

    public void add(int x, int y, int dx, int dy, Handles handle, Control c) 
    {
      Controls.Add(c);
      constraints.Add(new GridPanelConstraint(x,y,dx,dy,handle,c));
    }

    private void GridPanel_Resize(object sender, EventArgs e)
    {
      doLayout();
    }
  }

  public struct Insets
  {
    public int left, right, top, bottom;

    public Insets(int l, int r, int t, int b)
    {
      left = l;
      right = r;
      top = t;
      bottom = b;
    }
  }
}
