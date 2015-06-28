/* 
 * CoverMe! - Copyright (C) 2015 Juliet_Six
 * 
 * This file is part of "CoverMe!". "CoverMe!" is licensed under
 * the Microsoft Public License. You should have received a copy
 * of this license together with this file. Otherwise, see 
 * http://www.microsoft.com/en-us/openness/licenses.aspx
 *
 */

using System;
using System.Windows.Forms;

namespace CoverMe
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			try {
				Logger.log("Running CoverMe! by J6");
				Logger.log("Version is " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
				
				// Perform the actual running in another method. This is safer since the classes will not be loaded at this
				// point, so we can actually put out an error message if a linked library is not present.
				run();
			}
			catch (Exception e) {
				MessageBox.Show(e.Message, "Error - " + e.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
				Logger.logException("Unexpected Exception, program is terminating", e);
			}
			finally {
				Console.Out.Flush();
			}
		}
		
		private static void run()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
