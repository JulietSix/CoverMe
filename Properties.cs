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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CoverMe;

namespace WT_Logger
{
	/// <summary>
	/// A very simple implementation to read Properties from a text file.
	/// </summary>
	public class Properties
	{
		public const string settingsPath = "Settings.txt";
		
		// Contains the loaded Settings
		private static Dictionary<string, string> settings = new Dictionary<string, string>();
		
		private static string getSettingsPath()
		{				
			return settingsPath;
		}
		
		/// <summary>
		/// Loads the settings from the disk
		/// </summary>
		public static void LoadSettings()
		{
			char[] separators =  { '=' };
			
			try {
				string[] lines = File.ReadAllLines(getSettingsPath());
				
				foreach (string line in lines)
				{
					if (line.StartsWith("#"))
						continue;
					
					string[] separated = line.Split(separators, 2);
					
					if (separated.Length != 2)
						continue;
					
					settings.Add(separated[0], separated[1]);
				}
				
				CoverMe.Logger.log("Imported Settings successfully");
			}
			catch (Exception e) {
				Logger.logException("Error importing settings, using defaults", e);
			}
		}
		
		/// <summary>
		/// Retrieves a setting from the dictionary.
		/// </summary>
		/// <param name="name">The name of the setting</param>
		/// <param name="defaultValue">The default value of the setting</param>
		/// <returns>The value of the setting if it was present, otherwise returns the default value</returns>
		public static string GetSetting(string name, string defaultValue)
		{
			string s;
			
			if (!settings.TryGetValue(name, out s))
				s = defaultValue;
			
			return s;
		}
	}
}
