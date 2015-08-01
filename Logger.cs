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
using System.Diagnostics;
using System.IO;

namespace CoverMe
{
	/// <summary>
	/// Pretty simple Logger. Probably needs some more work so you don't have to create a log
	/// By calling through a BAT-file
	/// </summary>
	public class Logger
	{
		// Write to memory stream until told to write to a file
		private static MemoryStream tempStream;
		private static StreamWriter logWriter = new StreamWriter(tempStream = new MemoryStream());
		private static bool loggingToFile = false;
		
		private static Stopwatch loggerStopwatch = Stopwatch.StartNew();
		private static FinalizerDummy mFinalizer = new FinalizerDummy();
		
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

		private static void LogToNewFile() {
			DateTime now = DateTime.Now;
			
			// Flush to old stream
			logWriter.Flush();
			
			// Create a new file to which the logger writes instead
			logWriter = new StreamWriter("log_" 
			                               + now.Day.ToString() + "_"
			                               + now.Month.ToString() + "_"
			                               + now.Year.ToString() + "__"
			                               + now.Hour.ToString() + "_"
			                               + now.Minute.ToString() + ".txt"
			                              );
			
			if (!loggingToFile)
			{
				// Write any content previously in the memory stream to the new stream
				StreamReader tempReader = new StreamReader(tempStream);
				tempReader.BaseStream.Seek(0, SeekOrigin.Begin);
				logWriter.Write(tempReader.ReadToEnd());
				
				// Clear the memory stream
				tempStream.SetLength(0);
				
				loggingToFile = true;
			}
		}
		
		public static void EnableLogging() {
			if (!loggingToFile)
				LogToNewFile();
		}
		
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
			
			// Remove NewLine and LineFeed
			String s = message.Replace('\n', ' ');
			s = message.Replace("\r", "");
			
			// Calculate time
			long time = loggerStopwatch.ElapsedMilliseconds;
			long timeSecs = time / 1000;
			long timeMilis = time % 1000;
			
			// Format time
			string sTimeMilis = timeMilis.ToString().PadLeft(4, '0');
			string sTimeSecs = timeSecs.ToString().PadLeft(5, '0');
			
			// Print
			logWriter.Write(sTimeSecs);
			logWriter.Write('.');
			logWriter.Write(sTimeMilis);
			logWriter.Write(' ');
			logWriter.WriteLine(s);
		}
		
		/// <summary>
		/// Stops logging on program termination, flushing IO streams
		/// </summary>
		private static void FinishLogging() {
			logWriter.Flush();
			logWriter.Close();
		}
	}
}
