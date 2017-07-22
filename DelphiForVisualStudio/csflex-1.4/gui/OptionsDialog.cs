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

using CSFlex;

using System;
using System.Drawing;
using System.Windows.Forms;

namespace CSFlex.gui
{
  /**
   * A dialog for setting C# Flex options
   * 
   * @author Gerwin Klein
   * @version $Revision: 1.6 $, $Date: 2004/04/12 10:07:48 $
   * @author Jonathan Gilbert
   * @version CSFlex 1.4
   */
  public class OptionsDialog: Form 
  {

    private Form owner;

    private Button skelBrowse;
    private TextBox skelFile;

    private Button ok;
    private Button defaults;

    private CheckBox dump;
    private CheckBox verbose;
    private CheckBox jlex;
    private CheckBox no_minimize; 
    private CheckBox no_backup; 
    private CheckBox time;
    private CheckBox dot;
    private CheckBox csharp;

    private RadioButton tableG;
    private RadioButton switchG;
    private RadioButton packG; 
  

    /**
     * Create a new options dialog
     * 
     * @param owner
     */
    public OptionsDialog(Form owner) 
    {
      this.Text = "Options";

      this.owner = owner;
    
      setup();
    }

    public void setup() 
    {
      // create components
      ok = new Button();
      ok.Text = "Ok";

      defaults = new Button();
      defaults.Text = "Defaults";

      skelBrowse = new Button();
      skelBrowse.Text = " Browse";

      skelFile = new TextBox();
      skelFile.ReadOnly = true;

      dump = new CheckBox();
      dump.Text = " dump";

      verbose = new CheckBox();
      verbose.Text = " verbose";

      jlex = new CheckBox();
      jlex.Text = " JLex compatibility";

      no_minimize = new CheckBox();
      no_minimize.Text = " skip minimization";

      no_backup = new CheckBox();
      no_backup.Text = " no backup file";

      time = new CheckBox();
      time.Text = " time statistics";

      dot = new CheckBox();
      dot.Text = " dot graph files";

      csharp = new CheckBox();
      csharp.Text = " C# output";

      tableG = new RadioButton();
      tableG.Text = " table";

      switchG = new RadioButton();
      switchG.Text = " switch";

      packG = new RadioButton();
      packG.Text = " pack";

      switch (Options.gen_method)
      {
        case Options.TABLE:  tableG.Checked = true;   break;
        case Options.SWITCH: switchG.Checked = true;  break;
        case Options.PACK:   packG.Checked = true;    break;
      }

      // setup interaction
      ok.Click += new EventHandler(ok_Click);
      defaults.Click += new EventHandler(defaults_Click);
      skelBrowse.Click += new EventHandler(skelBrowse_Click);
      tableG.CheckedChanged += new EventHandler(tableG_CheckedChanged);
      switchG.CheckedChanged += new EventHandler(switchG_CheckedChanged);
      packG.CheckedChanged += new EventHandler(packG_CheckedChanged);
      verbose.CheckedChanged += new EventHandler(verbose_CheckedChanged);
      dump.CheckedChanged += new EventHandler(dump_CheckedChanged);
      jlex.CheckedChanged += new EventHandler(jlex_CheckedChanged);
      no_minimize.CheckedChanged += new EventHandler(no_minimize_CheckedChanged);
      no_backup.CheckedChanged += new EventHandler(no_backup_CheckedChanged);
      dot.CheckedChanged += new EventHandler(dot_CheckedChanged);
      csharp.CheckedChanged += new EventHandler(csharp_CheckedChanged);
      time.CheckedChanged += new EventHandler(time_CheckedChanged);

      // setup layout
      GridPanel panel = new GridPanel(4,7,10,10);
      panel.setInsets( new Insets(10,5,5,10) );
    
      panel.add(3,0,ok);
      panel.add(3,1,defaults);

      Label lblSkeletonFile = new Label();
      lblSkeletonFile.AutoSize = true;
      lblSkeletonFile.Text = "skeleton file:";

      Label lblCode = new Label();
      lblCode.AutoSize = true;
      lblCode.Text = "code:";
     
      panel.add(0,0,2,1,Handles.BOTTOM, lblSkeletonFile);
      panel.add(0,1,2,1,skelFile);
      panel.add(2,1,1,1,Handles.TOP, skelBrowse);
     
      panel.add(0,2,1,1,Handles.BOTTOM, lblCode);
      panel.add(0,3,1,1,tableG);
      panel.add(0,4,1,1,switchG);
      panel.add(0,5,1,1,packG);

      panel.add(1,3,1,1,dump);
      panel.add(1,4,1,1,verbose);
      panel.add(1,5,1,1,time);
    
      panel.add(2,3,1,1,no_minimize);
      panel.add(2,4,1,1,no_backup);
      panel.add(2,5,1,1,csharp);

      panel.add(3,3,1,1,jlex);
      panel.add(3,4,1,1,dot);

      panel.Size = panel.getPreferredSize();
      panel.doLayout();

      Size panel_size = panel.Size;
      Size client_area_size = this.ClientSize;
      Size left_over = new Size(client_area_size.Width - panel_size.Width, client_area_size.Height - panel_size.Height);

      Controls.Add(panel);
    
      panel.Location = new Point(0, 8);
      this.ClientSize = new Size(panel.Width + 8, panel.Height + 8);
      this.MaximumSize = this.MinimumSize = this.ClientSize;

      updateState();
    }
  
    private void do_skelBrowse() 
    {
      OpenFileDialog d = new OpenFileDialog();

      d.Title = "Choose file";

      DialogResult result = d.ShowDialog();

      if (result != DialogResult.Cancel) 
      {
        string skel = d.FileName;
        try 
        {
          Skeleton.readSkelFile(skel);
          skelFile.Text = skel;
        }
        catch (GeneratorException)
        {
          // do nothing
        }
      }

      d.Dispose();
    }

    private void setGenMethod() 
    {
      if ( tableG.Checked ) 
      {
        Options.gen_method = Options.TABLE;
        return;
      }
    
      if ( switchG.Checked ) 
      {
        Options.gen_method = Options.SWITCH;
        return;
      }
    
      if ( packG.Checked ) 
      {
        Options.gen_method = Options.PACK;
        return;
      }
    }

    private void updateState() 
    {
      dump.Checked = Options.dump;
      verbose.Checked = Options.verbose;
      jlex.Checked = Options.jlex;
      no_minimize.Checked = Options.no_minimize; 
      no_backup.Checked = Options.no_backup;
      time.Checked = Options.time;
      dot.Checked = Options.dot;

      switch (Options.gen_method)
      {
        case Options.TABLE:  tableG.Checked = true;   break;
        case Options.SWITCH: switchG.Checked = true;  break;
        case Options.PACK:   packG.Checked = true;    break;
      }
    }

    private void setDefaults() 
    {
      Options.setDefaults();
      Skeleton.readDefault();
      skelFile.Text = "";
      updateState();
    }

    private void ok_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
    }

    private void defaults_Click(object sender, EventArgs e)
    {
      setDefaults();
    }

    private void skelBrowse_Click(object sender, EventArgs e)
    {
      do_skelBrowse();
    }

    private void tableG_CheckedChanged(object sender, EventArgs e)
    {
      if (tableG.Checked)
        setGenMethod();
    }

    private void switchG_CheckedChanged(object sender, EventArgs e)
    {
      if (switchG.Checked)
        setGenMethod();
    }

    private void packG_CheckedChanged(object sender, EventArgs e)
    {
      if (packG.Checked)
        setGenMethod();
    }

    private void verbose_CheckedChanged(object sender, EventArgs e)
    {
      Options.verbose = verbose.Checked;
    }

    private void dump_CheckedChanged(object sender, EventArgs e)
    {
      Options.dump = dump.Checked;
    }

    private void jlex_CheckedChanged(object sender, EventArgs e)
    {
      Options.jlex = jlex.Checked;
    }

    private void no_minimize_CheckedChanged(object sender, EventArgs e)
    {
      Options.no_minimize = no_minimize.Checked;
    }

    private void no_backup_CheckedChanged(object sender, EventArgs e)
    {
      Options.no_backup = no_backup.Checked;
    }

    private void dot_CheckedChanged(object sender, EventArgs e)
    {
      Options.dot = dot.Checked;
    }

    private void time_CheckedChanged(object sender, EventArgs e)
    {
      Options.time = time.Checked;
    }

    private void csharp_CheckedChanged(object sender, EventArgs e)
    {
      Options.emit_csharp = csharp.Checked;
    }
  }
}
