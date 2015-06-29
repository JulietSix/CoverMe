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
using System.Collections.Generic;
using WindowsInput;

namespace CoverMe
{
	/// <summary>
	/// Collection of general helper-functions
	/// </summary>
	public class Helpers
	{
		private static readonly char[] ARRAY_SEPARATORS = { ',' };
		
		public const double METER_PER_FEET = 0.3048;
		
		private Helpers() {}
		
		/// <summary>
		/// Rounds an Integer to a pre-decimal position
		/// </summary>
		/// <param name="Number">The number to round</param>
		/// <param name="ZeroesPreDecimal">Amount of Zeroes that should preceed the decimal</param>
		/// <returns>The rounded number</returns>
		public static int RoundPreDecimal(int Number, int ZeroesPreDecimal) {
			double d = Number;
			double divisor = Math.Pow(10, ZeroesPreDecimal);
			
			d /= divisor;
			d = Math.Round(d);
			d *= divisor;
			
			return (int)d;
		}
		
		/// <summary>
		/// Parses a string containing multiple enumerators of the same type seperated by ','
		/// and returns a collection of those enumerators.
		/// </summary>
		/// <param name="s">String to be parsed</param>
		/// <returns>Collection of found enumerators</returns>
		public static List<T> ParseEnumArray<T>(string s) {			
			string[] elements = s.Split(Helpers.ARRAY_SEPARATORS);
			
			List<T> enums = new List<T>();
			
			foreach(string element in elements)
				enums.Add((T)Enum.Parse(typeof(T), element));
			
			return enums;
		}
		
		/// <summary>
		/// Checks wether one of the keys in the given list is pressed
		/// </summary>
		/// <param name="keys">A list of virtual keys</param>
		/// <returns>True if at least one key is pressed, otherwise false</returns>
		public static bool IsOneKeyDown(List<VirtualKeyCode> keys) {
			foreach (VirtualKeyCode key in keys)
				if (InputSimulator.IsKeyDown(key))
					return true;
			return false;
		}
	}
}
