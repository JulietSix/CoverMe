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

namespace CoverMe
{
	/// <summary>
	/// Pretty simple Logger. Probably needs some more work so you don't have to create a log
	/// By calling through a BAT-file
	/// </summary>
	public class Logger
	{
		/// <summary>
		/// Simply used to have an object referenced by the GC-list do be finalized when the
		/// program is terminated, presenting a way to close the log when the program is
		/// terminated through Environment.Exit()
		/// </summary>
		private class FinalizerDummy {
			~FinalizerDummy() {
				Logger.FinishLogging();
			}
		}
		
		private Logger() {}
		
		static int startTime = 0;
		private static FinalizerDummy mFinalizer = new FinalizerDummy();
		
		/// <summary>
		/// Logs an Exception
		/// </summary>
		/// <param name="additionalMessage">An additional message that should be logged explaining when or why the exception was raised</param>
		/// <param name="e">The actual exception</param>
		public static void logException(String additionalMessage, Exception e) {
			log(additionalMessage);
			log(e.GetType().ToString() + " - " + e.Message);
			log(e.StackTrace);
		}
				
		/// <summary>
		/// Logs a message
		/// </summary>
		/// <param name="message">The message</param>
		public static void log(String message) {
			
			// If we have not logged anything, set the current time as the start time of the log
			if (startTime == 0)
				startTime = Environment.TickCount;
			
			// Remove NewLine and LineFeed
			String s = message.Replace('\n', ' ');
			s = message.Replace("\r", "");
			
			// Calculate time
			int time = Environment.TickCount - startTime;
			int timeSecs = time / 1000;
			int timeMilis = time % 1000;
			
			// Format time
			string sTimeMilis = timeMilis.ToString().PadLeft(4, '0');
			string sTimeSecs = timeSecs.ToString().PadLeft(5, '0');
			
			// Print
			Console.Write(sTimeSecs);
			Console.Write('.');
			Console.Write(sTimeMilis);
			Console.Write(' ');
			Console.WriteLine(s);
		}
		
		/// <summary>
		/// Stops logging on program termination, flushing IO streams
		/// </summary>
		private static void FinishLogging() {
			Console.Out.Flush();
		}
	}
}
