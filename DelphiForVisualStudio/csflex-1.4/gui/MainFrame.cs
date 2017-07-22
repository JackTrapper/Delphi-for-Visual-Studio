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
using System.IO;
using System.Windows.Forms;

namespace CSFlex.gui
{
  /**
   * C# Flex main application frame (GUI mode only)
   *
   * @author Gerwin Klein
   * @version JFlex 1.4, $Revision: 2.6 $, $Date: 2004/04/12 10:07:48 $
   * @author Jonathan Gilbert
   * @version CSFlex 1.4
   */
  sealed public class MainFrame: Form 
  {

    private volatile bool choosing;

    private String fileName = "";
    private String dirName = "";
  
    private Button quit; 
    private Button options;
    private Button generate;
    private Button stop;
    private Button specChoose; 
    private Button dirChoose;

    private TextBox spec;
    private TextBox dir;

    private RichTextBox messages;

    private GeneratorThread thread;

    private OptionsDialog dialog;

  
    public MainFrame() 
    {
      this.Text = "JFlex " + MainClass.version;

      buildContent();

      Closed += new EventHandler(MainFrame_Closed);
    
      Show();
    }

    private void MainFrame_Closed(object sender, EventArgs e)
    {
      do_quit();
    }

    int calculate_char_width(int num_chars, TextBox tb)
    {
      // first, actually average all the letters & numbers,
      // and eliminate that silly extra few pixels that
      // System.Drawing likes to add.

      Graphics g = Graphics.FromHwnd(tb.Handle);

      SizeF one_a = g.MeasureString("a", tb.Font);
      SizeF two_a = g.MeasureString("aa", tb.Font);

      SizeF seventy_characters = g.MeasureString(
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.!?'\"#-(", tb.Font);

      float width_of_an_a_glyph = two_a.Width - one_a.Width;
      float measurement_error = one_a.Width - width_of_an_a_glyph;

      float width_of_seventy_character_glyphs = seventy_characters.Width - measurement_error;
      float width_of_one_character = width_of_seventy_character_glyphs / 70.0f;

      g.Dispose();

      return (int)(width_of_one_character * num_chars);
    }

    int calculate_char_width(int num_chars, RichTextBox tb)
    {
      // first, actually average all the letters & numbers,
      // and eliminate that silly extra few pixels that
      // System.Drawing likes to add.

      Graphics g = Graphics.FromHwnd(tb.Handle);

      SizeF one_a = g.MeasureString("a", tb.Font);
      SizeF two_a = g.MeasureString("aa", tb.Font);

      SizeF seventy_characters = g.MeasureString(
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.!?'\"#-(", tb.Font);

      float width_of_an_a_glyph = two_a.Width - one_a.Width;
      float measurement_error = one_a.Width - width_of_an_a_glyph;

      float width_of_seventy_character_glyphs = seventy_characters.Width - measurement_error;
      float width_of_one_character = width_of_seventy_character_glyphs / 70.0f;

      g.Dispose();

      return (int)(width_of_one_character * num_chars);
    }

    int calculate_lines_height(int num_lines, TextBox tb)
    {
      return (int)(num_lines * tb.Font.Height);
    }

    int calculate_lines_height(int num_lines, RichTextBox tb)
    {
      return (int)(num_lines * tb.Font.Height);
    }

    private void buildContent() 
    {
      BackColor = SystemColors.Control;

      generate   = new Button();
      generate.Text = "Generate";

      quit       = new Button();
      quit.Text = "Quit";

      options    = new Button();
      options.Text = "Options";

      stop       = new Button();
      stop.Text = "Stop";

      dirChoose  = new Button();
      dirChoose.Text = "Browse";

      dir        = new TextBox();
      dir.Width = calculate_char_width(10, dir);

      specChoose = new Button();
      specChoose.Text = "Browse";

      spec       = new TextBox();
      spec.Width = calculate_char_width(10, spec);

      messages   = new RichTextBox();
      messages.Multiline = true;
      messages.ScrollBars = RichTextBoxScrollBars.Both;
      messages.Font = new Font(FontFamily.GenericMonospace, messages.Font.Size, messages.Font.Style, messages.Font.Unit);
      messages.Width = calculate_char_width(80, messages);
      messages.Height = calculate_lines_height(10, messages);
      messages.ReadOnly = true;
      messages.MaxLength = int.MaxValue;

      generate.Click += new EventHandler(generate_Click);
      options.Click += new EventHandler(options_Click);
      quit.Click += new EventHandler(quit_Click);
      stop.Click += new EventHandler(stop_Click);
      specChoose.Click += new EventHandler(specChoose_Click);
      dirChoose.Click += new EventHandler(dirChoose_Click);
      spec.KeyPress += new KeyPressEventHandler(spec_KeyPress);
      spec.TextChanged += new EventHandler(spec_TextChanged);
      dir.KeyPress += new KeyPressEventHandler(dir_KeyPress);
      dir.TextChanged += new EventHandler(dir_TextChanged);

      GridPanel north = new GridPanel(5,4,10,10);
      north.setInsets( new Insets(10,5,5,10) );

      Label lblLexicalSpecification = new Label();
      lblLexicalSpecification.AutoSize = true;
      lblLexicalSpecification.Text = "Lexical specification:";

      Label lblOutputDirectory = new Label();
      lblOutputDirectory.AutoSize = true;
      lblOutputDirectory.Text = "Output directory:";

      north.add( 4,0, quit);
      north.add( 4,1, generate);
      north.add( 4,2, options);
      north.add( 4,3, stop);

      north.add( 0,0, Handles.BOTTOM, lblLexicalSpecification);
      north.add( 0,1, 2,1, spec);
      north.add( 2,1, specChoose);

      north.add( 0,2, Handles.BOTTOM, lblOutputDirectory);
      north.add( 0,3, 2,1, dir);
      north.add( 2,3, dirChoose);

      Panel true_north = new Panel();

      true_north.Controls.Add(north);

      Label lblMessages = new Label();
      lblMessages.TextAlign = ContentAlignment.MiddleCenter;
      lblMessages.Text = "Messages:";
      lblMessages.Dock = DockStyle.Top;

      Panel center = new Panel();

      center.Controls.Add(lblMessages);
      center.Controls.Add(messages);

      north.Size = north.getPreferredSize();
      north.doLayout();

      true_north.Size = north.Size;

      north.Anchor = AnchorStyles.Top | AnchorStyles.Left;

      true_north.Dock = DockStyle.Top;

      center.Anchor = (AnchorStyles)15;
      center.Location = new Point(0, north.Height);
      center.Size = new Size(ClientSize.Width, ClientSize.Height - north.Height);

      SuspendLayout();
      Controls.Add(true_north);
      Controls.Add(center);
      ResumeLayout(false);

      messages.Top = lblMessages.Bottom;
      messages.Left = 4;
      messages.Width = center.Width - 8;
      messages.Height = center.Height - lblMessages.Height - 4;
      messages.Anchor = (AnchorStyles)15;

      this.ClientSize = new Size(north.Width + 8, ClientSize.Height);
      this.MinimumSize = this.ClientSize;

      setEnabledAll(false);

      Out.setGUIMode(messages);
    }

    void showOptions() 
    {
      if (dialog == null) 
      {
        dialog = new OptionsDialog(this);
      }
      dialog.ShowDialog();
    }

    private void setEnabledAll(bool generating) 
    {
      stop.Enabled = generating;
      quit.Enabled = !generating;
      generate.Enabled = !generating;
      dirChoose.Enabled = !generating;
      dir.Enabled = !generating;
      specChoose.Enabled = !generating;
      spec.Enabled = !generating;
    }

    private void do_generate() 
    {
      // workaround for a weird AWT bug
      if (choosing) return;
   
      setEnabledAll(true);

      thread = new GeneratorThread(this, fileName, dirName);
      thread.start();

      messages.Focus();
    } 

    public void generationFinished(bool success) 
    {
      setEnabledAll(false);

      messages.Focus();
    
      if (success) 
        messages.AppendText(Out.NL+"Generation finished successfully."+Out.NL);
      else
        messages.AppendText(Out.NL+"Generation aborted."+Out.NL);
    }

    private void do_stop() 
    {
      if (thread != null) 
      {
        /* stop ok here despite deprecation (?)
           I don't know any good way to abort generation without changing the
           generator code */ 
        thread.stop();
        thread = null;
      }
      generationFinished(false);
    }
 
    private void do_quit() 
    {
      Hide();
      Application.Exit();
    }
  
    private void do_dirChoose() 
    {
      choosing = true;

      OpenFileDialog d = new OpenFileDialog();

      d.Title = "Choose directory";

      DialogResult result = d.ShowDialog();

      if (result != DialogResult.Cancel)
        dir.Text = Path.GetDirectoryName(Path.GetFullPath(d.FileName));

      d.Dispose();
    
      choosing = false;    
    }

    private void do_specChoose() 
    {
      choosing = true;
    
      OpenFileDialog d = new OpenFileDialog();

      d.Title = "Choose file";
      d.Filter = "JFlex and CSFlex Specifications (*.flex)|*.flex|All files (*.*)|*.*";
      d.FilterIndex = 1;

      DialogResult result = d.ShowDialog();

      if (result != DialogResult.Cancel)
      {
        fileName = d.FileName;
        dir.Text = Path.GetDirectoryName(Path.GetFullPath(fileName));
        spec.Text = fileName;
      }

      d.Dispose();

      choosing = false;    
    }

    private void generate_Click(object sender, EventArgs e)
    {
      do_generate();
    }

    private void options_Click(object sender, EventArgs e)
    {
      showOptions();
    }

    private void quit_Click(object sender, EventArgs e)
    {
      do_quit();
    }

    private void stop_Click(object sender, EventArgs e)
    {
      do_stop();
    }

    private void specChoose_Click(object sender, EventArgs e)
    {
      do_specChoose();
    }

    private void dirChoose_Click(object sender, EventArgs e)
    {
      do_dirChoose();
    }

    private void spec_TextChanged(object sender, EventArgs e)
    {
      fileName = spec.Text;
    }

    private void spec_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == '\n')
      {
        fileName = spec.Text;
        do_generate();
      }
    }

    private void dir_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == '\n')
      {
        dirName = dir.Text;
        do_generate();
      }
    }

    private void dir_TextChanged(object sender, EventArgs e)
    {
      dirName = dir.Text;
    }
  }
}