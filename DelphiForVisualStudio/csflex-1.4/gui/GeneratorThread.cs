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
using System.Threading;

namespace CSFlex.gui
{
  /**
   * Low priority thread for code generation (low priority so
   * that gui has time for screen updates)
   *
   * @author Gerwin Klein
   * @version JFlex 1.4, $Revision: 2.5 $, $Date: 2004/04/12 10:07:48 $
   * @author Jonathan Gilbert
   * @version CSFlex 1.4
   */
  public class GeneratorThread
  {

    /** there must be at most one instance of this Thread running */
    private static volatile bool running = false;

    /** input file setting from GUI */
    String inputFile;

    /** output directory */
    String outputDir;
  
    /** main UI component, likes to be notified when generator finishes */
    MainFrame parent;

    /**
     * Create a new GeneratorThread, but do not run it yet.
     * 
     * @param parent      the frame, main UI component
     * @param inputFile   input file from UI settings
     * @param messages    where generator messages should appear
     * @param outputDir   output directory from UI settings
     */
    public GeneratorThread(MainFrame parent, String inputFile, 
      String outputDir) 
    {
      this.parent    = parent;
      this.inputFile = inputFile;
      this.outputDir = outputDir;
    }


    /**
     * Run the generator thread. Only one instance of it can run at any time.
     */
    Thread thread;

    public void start()
    {
      lock (this)
      {
        if (running)
        {
          Out.error(ErrorMessages.ALREADY_RUNNING);
          parent.generationFinished(false);
        }
        else
        {
          running = true;

          thread = new Thread(new ThreadStart(run));

          //thread.Priority = ThreadPriority.BelowNormal;
          thread.IsBackground = true;
          thread.Start();
        }
      }
    }

    public void run()
    {
      try 
      {
        if (outputDir != "")
          Options.setDir(outputDir);

        MainClass.generate(new File(inputFile));
        Out.statistics();
        parent.generationFinished(true);
      }
      catch (GeneratorException) 
      {
        Out.statistics();
        parent.generationFinished(false);
      }
      catch (ThreadInterruptedException) { }
      catch (ThreadAbortException) { }
      finally 
      {
        lock (this)
        {
          running = false;
          thread = null;
        }
      }
    }

    public void stop()
    {
      lock (this)
      {
        if (running && (thread != null))
        {
          thread.Interrupt();
          thread.Abort();
        }
      }
    }
  }
}
